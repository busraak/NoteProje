using Note.BusinessLayer;
using Note.BusinessLayer.Results;
using Note.Domain;
using Note.Domain.Messages;
using Note.Domain.ValueObjects;
using Note.WebApp.Filters;
using Note.WebApp.Models;
using Note.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Note.WebApp.Controllers
{
    [Exc]
    public class HomeController : Controller
    {
        private NotManager notManager = new NotManager();
        private CategoryManager categoryManager = new CategoryManager();
        private UserManager userManager = new UserManager();
        // GET: Home
        public ActionResult Index()
        {
            return View(notManager.ListQueryable().Where(x=>x.IsDraft==false).OrderByDescending(x=>x.ModifiedOn).ToList());
            //return View(nm.GetAllNotQueryable().OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Category cat = categoryManager.Find(x=>x.Id== id.Value);

            //if (cat == null)
            //{
            //    return HttpNotFound();

            //}
            //List<Not> nots= cat.Nots.Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList();
            List<Not> nots=notManager.ListQueryable()
                .Where(x => x.IsDraft == false && x.CategoryId == id)
                .OrderByDescending(x => x.ModifiedOn).ToList();
            return View("Index",nots);
        }
    
        public ActionResult MostLiked()
        {
            
            return View("Index",notManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            
           
            BusinessLayerResult<NotUser> res = userManager.GetUserById(CurrentSession.User.Id);
            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyobj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };

                return View("Error", errorNotifyobj);
            }
            return View(res.Result);
        }

        [Auth]
        public ActionResult EditProfile()
        {

            UserManager userManager = new UserManager();
            BusinessLayerResult<NotUser> res = userManager.GetUserById(CurrentSession.User.Id);
            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyobj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };

                return View("Error", errorNotifyobj);
            }
            return View(res.Result);
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(NotUser model,HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                if (ProfileImage != null && (ProfileImage.ContentType == "Image/jpeg" || ProfileImage.ContentType == "Image/jpg" || ProfileImage.ContentType == "Image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));
                    model.ProfileImageFilename = filename;
                }

                BusinessLayerResult<NotUser> res = userManager.UpdateProfile(model);
                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Items = res.Errors,
                        Title = "Profil Güncellenemedi.",
                        RedirectingUrl = "/Home/EditProfile"
                    };

                    return View("Error", errorNotifyObj);
                }
                CurrentSession.Set<NotUser>("login",res.Result);
                return RedirectToAction("ShowProfile");
            }
            return View(model);
           
        }
        
        [Auth]
        public ActionResult DeleteProfile()
        {
            

            BusinessLayerResult<NotUser> res = userManager.RemoveUserById(CurrentSession.User.Id);
            
            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title = "Profil Silinemedi.",
                    RedirectingUrl = "/Home/ShowProfile"
                };

                return View("Error", errorNotifyObj);
            }
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserManager userManager = new UserManager();
                BusinessLayerResult<NotUser> res = userManager.LoginUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(model);
                }

                CurrentSession.Set<NotUser>("login",res.Result);
                return RedirectToAction("Index");
            }

            return View(model);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                BusinessLayerResult<NotUser> res = userManager.RegisterUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);

                }
                OkViewModel notifyObj = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl="/Home/Login",
                };

                notifyObj.Items.Add(" Lütfen e-posta adresinize gönderdiğiniz aktivasyon linkine tıklayarak hesabınızı aktive ediniz.");
                return View("Ok",notifyObj);
            }
            return View(model);
        }
     
        public ActionResult UserActivate(Guid id)
        {
          
            BusinessLayerResult<NotUser> res= userManager.ActivateUser(id);

            if(res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyobj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };
                
                return View("Error", errorNotifyobj);
            }
            OkViewModel okNotifyobj = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi",
                RedirectingUrl = "/Home/Login"
            };
            okNotifyobj.Items.Add("Hesap aktifleştirildi. Artık paylaşım yapabilirsiniz.");
            return View("Ok",okNotifyobj);
        }

    
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}