using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DemoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Demoes
        public ActionResult Index()
        {
            return View(db.Demos.ToList());
        }

        // GET: Demoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Demo demo = db.Demos.Find(id);
            if (demo == null)
            {
                return HttpNotFound();
            }
            return View(demo);
        }

        // GET: Demoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Demoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Age")] Demo demo)
        {
            if (ModelState.IsValid)
            {
                db.Demos.Add(demo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(demo);
        }

        // GET: Demoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Demo demo = db.Demos.Find(id);
            if (demo == null)
            {
                return HttpNotFound();
            }
            return View(demo);
        }

        // POST: Demoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Age")] Demo demo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(demo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(demo);
        }

        // GET: Demoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Demo demo = db.Demos.Find(id);
            if (demo == null)
            {
                return HttpNotFound();
            }
            return View(demo);
        }

        // POST: Demoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Demo demo = db.Demos.Find(id);
            db.Demos.Remove(demo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
