using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ItemInHousesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ItemInHouses
        public ActionResult Index()
        {
            var itemInHouses = db.ItemInHouses.Include(i => i.House).Include(i => i.ItemCategory).Include(i => i.Status);
            return View(itemInHouses.ToList());
        }

        // GET: ItemInHouses/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemInHouse itemInHouse = db.ItemInHouses.Find(id);
            if (itemInHouse == null)
            {
                return HttpNotFound();
            }
            return View(itemInHouse);
        }

        // GET: ItemInHouses/Create
        public ActionResult Create()
        {
            ViewBag.HouseId = new SelectList(db.Houses, "Id", "Name");
            ViewBag.ItemCategoryId = new SelectList(db.ItemCategories, "Id", "Name");
            ViewBag.StatusId = new SelectList(db.ItemStatuses, "Id", "Status");
            return View();
        }

        // POST: ItemInHouses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,HouseId,StatusId,ItemCategoryId,AddedDate")] ItemInHouse itemInHouse)
        {
            if (ModelState.IsValid)
            {
                db.ItemInHouses.Add(itemInHouse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseId = new SelectList(db.Houses, "Id", "Name", itemInHouse.HouseId);
            ViewBag.ItemCategoryId = new SelectList(db.ItemCategories, "Id", "Name", itemInHouse.ItemCategoryId);
            ViewBag.StatusId = new SelectList(db.ItemStatuses, "Id", "Status", itemInHouse.StatusId);
            return View(itemInHouse);
        }

        // GET: ItemInHouses/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemInHouse itemInHouse = db.ItemInHouses.Include(i => i.House).Where(i => i.Id == id).SingleOrDefault();
            if (itemInHouse == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseId = new SelectList(db.Houses, "Id", "Name", itemInHouse.HouseId);
            ViewBag.ItemCategoryId = new SelectList(db.ItemCategories, "Id", "Name", itemInHouse.ItemCategoryId);
            ViewBag.StatusId = new SelectList(db.ItemStatuses, "Id", "Status", itemInHouse.StatusId);
            return View(itemInHouse);
        }

        // POST: ItemInHouses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,HouseId,StatusId,ItemCategoryId,AddedDate")] ItemInHouse itemInHouse)
        {
            if (ModelState.IsValid)
            {
                ItemInHouse oldData = db.ItemInHouses.Include(i => i.House).Where(i => i.Id == itemInHouse.Id).SingleOrDefault();
                oldData.Name = itemInHouse.Name;
                oldData.ItemCategoryId = itemInHouse.ItemCategoryId;
                oldData.Description = itemInHouse.Description;

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        Media fileDetail = new Media()
                        {
                            Media_Name = fileName,
                            Media_Extension = Path.GetExtension(fileName),
                            Id = Guid.NewGuid()
                        };
                        oldData.Medias.Add(fileDetail);

                        var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Media_Extension);
                        file.SaveAs(path);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Details", "Houses", new { id = oldData.HouseId });
            }
            return View(itemInHouse);
        }

        // GET: ItemInHouses/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemInHouse itemInHouse = db.ItemInHouses.Find(id);
            if (itemInHouse == null)
            {
                return HttpNotFound();
            }
            return View(itemInHouse);
        }

        // POST: ItemInHouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ItemInHouse itemInHouse = db.ItemInHouses.Find(id);
            db.ItemInHouses.Remove(itemInHouse);
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
