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
    public class HousesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Houses
        public ActionResult Index()
        {
            return View(db.Houses.ToList());
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
                .Include(h => h.Rooms).Where(h => h.Id == id).SingleOrDefault();
            var itemInHouseList = db.ItemInHouses
                .Include(i => i.Medias)
                .Include(i => i.Item)
                .Include(i => i.House).Where(i => i.HouseId == id).ToList();
            house.Items = itemInHouseList;
            HouseViewModel viewModel = new HouseViewModel(house);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoom([Bind(Include = "Id,RoomName,RoomType")] HouseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //List<Media> medias = new List<Media>();
                //for (int i = 0; i < Request.Files.Count; i++)
                //{
                //    var file = Request.Files[i];

                //    if (file != null && file.ContentLength > 0)
                //    {
                //        var fileName = Path.GetFileName(file.FileName);
                //        Media fileDetail = new Media()
                //        {
                //            Media_Name = fileName,
                //            Media_Extension = Path.GetExtension(fileName),
                //            Id = Guid.NewGuid()
                //        };
                //        medias.Add(fileDetail);

                //        var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Media_Extension);
                //        file.SaveAs(path);
                //    }
                //}

                Room room = new Room
                {
                    Name = viewModel.RoomName,
                    Type = viewModel.RoomType
                };

                db.Rooms.Add(room);
                db.SaveChanges();
                //house.Medias = medias;
                //house.Room_Count = 0;

                //db.Houses.Add(house);
                //db.SaveChanges();
                return RedirectToAction("Detail");
            }
            return RedirectToAction("Detail");
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
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Find(id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

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
