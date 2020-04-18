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
    public class TransactionCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TransactionCategories
        public ActionResult Index()
        {
            return View(db.TransactionCategories.ToList());
        }

        // GET: TransactionCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionCategory transactionCategory = db.TransactionCategories.Find(id);
            if (transactionCategory == null)
            {
                return HttpNotFound();
            }
            return View(transactionCategory);
        }

        // GET: TransactionCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TransactionCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Decription")] TransactionCategory transactionCategory)
        {
            if (ModelState.IsValid)
            {
                db.TransactionCategories.Add(transactionCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(transactionCategory);
        }

        // GET: TransactionCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionCategory transactionCategory = db.TransactionCategories.Find(id);
            if (transactionCategory == null)
            {
                return HttpNotFound();
            }
            return View(transactionCategory);
        }

        // POST: TransactionCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Decription")] TransactionCategory transactionCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transactionCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transactionCategory);
        }

        // GET: TransactionCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionCategory transactionCategory = db.TransactionCategories.Find(id);
            if (transactionCategory == null)
            {
                return HttpNotFound();
            }
            return View(transactionCategory);
        }

        // POST: TransactionCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TransactionCategory transactionCategory = db.TransactionCategories.Find(id);
            db.TransactionCategories.Remove(transactionCategory);
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
