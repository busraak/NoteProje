using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Note.BusinessLayer;
using Note.BusinessLayer.Results;
using Note.Domain;
using Note.WebApp.Filters;

namespace Note.WebApp.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class NotUserController : Controller
    {
        private UserManager userManager = new UserManager();
        public ActionResult Index()
        {
            return View(userManager.List());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotUser notUser = userManager.Find(x => x.Id == id.Value);
            if (notUser == null)
            {
                return HttpNotFound();
            }
            return View(notUser);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NotUser notUser)
        {
            ModelState.Remove("CreateOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<NotUser> res = userManager.Insert(notUser);
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(notUser);
                }
                return RedirectToAction("Index");
            }

            return View(notUser);
        }

        public ActionResult Edit(int? id)
        {
        
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotUser notUser = userManager.Find(x => x.Id == id.Value);
            if (notUser == null)
            {
                return HttpNotFound();
            }
            return View(notUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NotUser notUser)
        {
            ModelState.Remove("CreateOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<NotUser> res = userManager.Update(notUser);
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(notUser);
                }
                return RedirectToAction("Index");
            }
            return View(notUser);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NotUser notUser = userManager.Find(x => x.Id == id.Value);
            if (notUser == null)
            {
                return HttpNotFound();
            }
            return View(notUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NotUser notUser = userManager.Find(x => x.Id == id);
            userManager.Delete(notUser);
            return RedirectToAction("Index");
        }

    }
}
