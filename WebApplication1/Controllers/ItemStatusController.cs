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
    public class ItemStatusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ItemStatus
        public ActionResult Index()
        {
            return View(db.ItemStatuses.ToList());
        }

        // GET: ItemStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemStatus itemStatus = db.ItemStatuses.Find(id);
            if (itemStatus == null)
            {
                return HttpNotFound();
            }
            return View(itemStatus);
        }

        // GET: ItemStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Status,Decription,Code")] ItemStatus itemStatus)
        {
            if (ModelState.IsValid)
            {
                db.ItemStatuses.Add(itemStatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(itemStatus);
        }

        // GET: ItemStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemStatus itemStatus = db.ItemStatuses.Find(id);
            if (itemStatus == null)
            {
                return HttpNotFound();
            }
            return View(itemStatus);
        }

        // POST: ItemStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Status,Decription,Code")] ItemStatus itemStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemStatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(itemStatus);
        }

        // GET: ItemStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemStatus itemStatus = db.ItemStatuses.Find(id);
            if (itemStatus == null)
            {
                return HttpNotFound();
            }
            return View(itemStatus);
        }

        // POST: ItemStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemStatus itemStatus = db.ItemStatuses.Find(id);
            db.ItemStatuses.Remove(itemStatus);
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
