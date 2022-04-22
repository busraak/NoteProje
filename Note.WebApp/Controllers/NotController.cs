using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Note.BusinessLayer;
using Note.Domain;
using Note.WebApp.Filters;
using Note.WebApp.Models;

namespace Note.WebApp.Controllers
{
    [Exc]
    public class NotController : Controller
    {
        private NotManager notManager = new NotManager();
        private LikedManager likedManager = new LikedManager();

        [Auth]
        public ActionResult Index()
        {
            var nots = notManager.ListQueryable()
                .Include("Category").Include("Owner")
                .Where(x => x.Owner.Id == CurrentSession.User.Id)
                .OrderByDescending(x => x.ModifiedOn);

            return View(nots.ToList());
        }

        [Auth]
        public ActionResult MyLikedNots()
        {
            var nots = likedManager.ListQueryable()
                .Include("LikedUser").Include("Nots")
                .Where(x => x.LikedUser.Id == CurrentSession.User.Id)
                .Select(x => x.Nots).Include("Category").Include("Owner")
                .OrderByDescending(x => x.ModifiedOn);

            return View("Index", nots.ToList());
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Not not = notManager.Find(x => x.Id == id);
            if (not == null)
            {
                return HttpNotFound();
            }
            return View(not);
        }

        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title");
            return View();
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(Not not)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                not.Owner = CurrentSession.User;
                notManager.Insert(not);
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", not.CategoryId);
            return View(not);
        }

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Not not = notManager.Find(x => x.Id == id);
            if (not == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", not.CategoryId);
            return View(not);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Not not)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                Not db_not = notManager.Find(x => x.Id == not.Id);
                db_not.IsDraft = not.IsDraft;
                db_not.CategoryId = not.CategoryId;
                db_not.Text = not.Text;
                db_not.Title = not.Title;

                notManager.Update(db_not);

                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", not.CategoryId);
            return View(not);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Not not = notManager.Find(x => x.Id == id);
            if (not == null)
            {
                return HttpNotFound();
            }
            return View(not);
        }


        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Not not = notManager.Find(x => x.Id == id);
            notManager.Delete(not);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            List<int> likedNotIds = likedManager
                .List(x => x.LikedUser.Id == CurrentSession.User.Id && ids
                .Contains(x.Nots.Id))
                .Select(x => x.Nots.Id).ToList();

            return Json(new { result = likedNotIds });
        }
        public ActionResult SetLikeState(int noteid, bool liked)
        {
            int res = 0;

            if (CurrentSession.User == null)
                return Json(new { hasError = true, errorMessage = "Beğenme işlemi için giriş yapmalısınız.", result = 0 });

            Liked like =
                likedManager.Find(x => x.Nots.Id == noteid && x.LikedUser.Id == CurrentSession.User.Id);

            Not not = notManager.Find(x => x.Id == noteid);

            if (like != null && liked == false)
            {
                res = likedManager.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Nots = not
                });
            }

            if (res > 0)
            {
                if (liked)
                {
                    not.LikeCount++;
                }
                else
                {
                    not.LikeCount--;
                }

                res = notManager.Update(not);

                return Json(new { hasError = false, errorMessage = string.Empty, result = not.LikeCount });
            }

            return Json(new { hasError = true, errorMessage = "Beğenme işlemi gerçekleştirilemedi.", result = not.LikeCount });
        }


        public ActionResult GetNotText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Not not = notManager.Find(x => x.Id == id);

            if (not == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialNotText", not);
        }
    }



}

