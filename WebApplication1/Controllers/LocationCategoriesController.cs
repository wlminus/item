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
    public class LocationCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LocationCategories
        public ActionResult Index()
        {
            return View(db.LocationCategorys.ToList());
        }

        // GET: LocationCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationCategory locationCategory = db.LocationCategorys.Find(id);
            if (locationCategory == null)
            {
                return HttpNotFound();
            }
            return View(locationCategory);
        }

        // GET: LocationCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LocationCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] LocationCategory locationCategory)
        {
            if (ModelState.IsValid)
            {
                db.LocationCategorys.Add(locationCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(locationCategory);
        }

        // GET: LocationCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationCategory locationCategory = db.LocationCategorys.Find(id);
            if (locationCategory == null)
            {
                return HttpNotFound();
            }
            return View(locationCategory);
        }

        // POST: LocationCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] LocationCategory locationCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(locationCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(locationCategory);
        }

        // GET: LocationCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationCategory locationCategory = db.LocationCategorys.Find(id);
            if (locationCategory == null)
            {
                return HttpNotFound();
            }
            return View(locationCategory);
        }

        // POST: LocationCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocationCategory locationCategory = db.LocationCategorys.Find(id);
            db.LocationCategorys.Remove(locationCategory);
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
