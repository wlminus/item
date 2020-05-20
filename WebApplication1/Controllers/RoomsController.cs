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
    public class RoomsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rooms
        public ActionResult Index()
        {
            return View(db.Rooms.ToList());
        }

        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.
                Include(r => r.Medias).Include(r => r.Items).Include(r => r.House)
                .Where(r => r.Id == id).SingleOrDefault();

            var status = db.ItemStatuses.ToList();
            ViewBag.Status = new MultiSelectList(status, "Id", "Status");
            var cat = db.ItemCategories.ToList();
            ViewBag.ItemCategories = new MultiSelectList(cat, "Id", "Name");

            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem(FormCollection formCollection)
        {
            List<Media> medias = new List<Media>();
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
                    medias.Add(fileDetail);

                    var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Media_Extension);
                    file.SaveAs(path);
                }
            }

            long roomId = long.Parse(formCollection["Room-Id"]);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long secondsSinceEpoch = (long)t.TotalSeconds;

            ItemInRoom item = new ItemInRoom
            {
                Name = formCollection["ItemName"],
                Description = formCollection["ItemDescription"],
                RoomId = roomId,
                ItemCategoryId = long.Parse(formCollection["ItemCategoryId"]),
                StatusId = long.Parse(formCollection["ItemStatusId"]),
                Medias = medias,
                AddedDate = secondsSinceEpoch
            };

            db.ItemInRooms.Add(item);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = roomId });
        }

        [ValidateAntiForgeryToken]
        public ActionResult DeleteItem(int? id)
        {

            ItemInRoom itemToDel = db.ItemInRooms
                .Include(h => h.Room).Where(h => h.Id == id).SingleOrDefault();

            long roomId = itemToDel.Room.Id;

            db.ItemInRooms.Remove(itemToDel);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = roomId });
        }







        // GET: Rooms/Create
        public ActionResult Create()
        {
            return View();
        }







        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Type,Rent_User_Id")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(room);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Type,Rent_User_Id")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
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
