using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HousesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Houses
        public ActionResult Index()
        {
            return View(db.Houses.ToList());
        }

        [HttpPost]
        public ActionResult Index(string query)
        {
            ViewBag.SearchKey = query;
            var data = db.Houses.Where(it => (it.Name.Contains(query) || it.Location.Contains(query) || it.District.Contains(query) || it.Province.Contains(query) || it.Description.Contains(query))).ToList();
            return View(data);
        }

        // GET: Houses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses
                .Include(h => h.Medias)
                .Include(h => h.Rooms)
                .Include(h => h.Items).Where(h => h.Id == id).SingleOrDefault();
            //var itemInHouseList = db.ItemInHouses
            //    .Include(i => i.Medias)
            //    .Include(i => i.House).Where(i => i.HouseId == id).ToList();

            //house.Items = itemInHouseList;
            HouseViewModel viewModel = new HouseViewModel(house);

            var status = db.ItemStatuses.ToList();
            ViewBag.Status = new MultiSelectList(status, "Id", "Status");
            var cat = db.ItemCategories.ToList();
            ViewBag.ItemCategories = new MultiSelectList(cat, "Id", "Name");

            if (house == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoom(FormCollection formCollection)
        {
            long houseId = long.Parse(formCollection["House-Id"]);

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

            Room room = new Room
            {
                Name = formCollection["RoomName"],
                Type = formCollection["RoomType"],
                Medias = medias
            };
            db.Rooms.Add(room);

            House house = db.Houses.Where(h => h.Id == houseId).SingleOrDefault();

            house.Rooms.Add(room);
            house.Room_Count++;

            db.SaveChanges();

            return RedirectToAction("Details", new { id = houseId });
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

            long houseId = long.Parse(formCollection["House-Id"]);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long secondsSinceEpoch = (long)t.TotalSeconds;

            ItemInHouse item = new ItemInHouse
            {
                Name = formCollection["ItemName"],
                Description = formCollection["ItemDescription"],
                HouseId = houseId,
                ItemCategoryId = long.Parse(formCollection["ItemCategoryId"]),
                StatusId = long.Parse(formCollection["ItemStatusId"]),
                Medias = medias,
                AddedDate = secondsSinceEpoch
            };

            db.ItemInHouses.Add(item);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = houseId });
        }


        [ValidateAntiForgeryToken]
        public ActionResult DeleteHouse(int? id)
        {
            House house = db.Houses.Find(id);
            db.Houses.Remove(house);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoom(int? id)
        {

            Room roomToDel = db.Rooms
                .Include(h => h.House).Where(h => h.Id == id).SingleOrDefault();

            long houseId = roomToDel.House.Id;
            House house = db.Houses.Where(h => h.Id == houseId).SingleOrDefault();
            house.Room_Count--;

            db.Rooms.Remove(roomToDel);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = houseId });
        }

        [ValidateAntiForgeryToken]
        public ActionResult DeleteItem(int? id)
        {

            ItemInHouse itemToDel = db.ItemInHouses
                .Include(h => h.House).Where(h => h.Id == id).SingleOrDefault();

            long houseId = itemToDel.House.Id;

            db.ItemInHouses.Remove(itemToDel);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = houseId });
        }




        // GET: Houses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Houses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Location,District,Province,Description,Room_Count,Status,Rent_User_Id")] House house)
        {
            if (ModelState.IsValid)
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

                house.Medias = medias;
                house.Room_Count = 0;

                db.Houses.Add(house);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(house);
        }

        // GET: Houses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Include(s => s.Medias).SingleOrDefault(x => x.Id == id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        // POST: Houses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Location,District,Province,Description,Room_Count,Status,Rent_User_Id")] House house)
        {
            if (ModelState.IsValid)
            {
                db.Entry(house).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(house);
        }

        // GET: Houses/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    House house = db.Houses.Find(id);
        //    if (house == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(house);
        //}

        

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            House house = db.Houses.Find(id);
            db.Houses.Remove(house);
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
