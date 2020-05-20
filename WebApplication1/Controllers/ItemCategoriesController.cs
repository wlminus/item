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
    public class ItemCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ItemCategories
        public ActionResult Index()
        {
            return View(db.ItemCategories.ToList());
        }

        [HttpPost]
        public ActionResult Index(string query)
        {
            ViewBag.SearchKey = query;
            var data = db.ItemCategories.Where(it => (it.Name.Contains(query) || it.Description.Contains(query))).ToList();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FormCollection formCollection)
        {
            if (String.IsNullOrEmpty(formCollection["Id"]))
            {
                ItemCategory itemCategory = new ItemCategory()
                {
                    Name = formCollection["Name"],
                    Description = formCollection["Description"]
                };

                db.ItemCategories.Add(itemCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                long id = long.Parse(formCollection["Id"]);
                var dataEdit = db.ItemCategories.Find(id);
                dataEdit.Name = formCollection["Name"];
                dataEdit.Description = formCollection["Description"];
                db.SaveChanges();

                return RedirectToAction("Index");
            }

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemCategory itemCategory = db.ItemCategories.Find(id);
            db.ItemCategories.Remove(itemCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //public ActionResult Index()
        //{
        //    return View(db.ItemCategories.ToList());
        //}

        //// GET: ItemCategories/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ItemCategory itemCategory = db.ItemCategories.Find(id);
        //    if (itemCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(itemCategory);
        //}

        //// GET: ItemCategories/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: ItemCategories/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,Description")] ItemCategory itemCategory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.ItemCategories.Add(itemCategory);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(itemCategory);
        //}

        //// GET: ItemCategories/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ItemCategory itemCategory = db.ItemCategories.Find(id);
        //    if (itemCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(itemCategory);
        //}

        //// POST: ItemCategories/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,Description")] ItemCategory itemCategory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(itemCategory).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(itemCategory);
        //}

        //// GET: ItemCategories/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ItemCategory itemCategory = db.ItemCategories.Find(id);
        //    if (itemCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(itemCategory);
        //}

        //// POST: ItemCategories/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    ItemCategory itemCategory = db.ItemCategories.Find(id);
        //    db.ItemCategories.Remove(itemCategory);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
