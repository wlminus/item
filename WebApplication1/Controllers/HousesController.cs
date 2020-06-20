using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "ROLE_ADMIN")]
    public class HousesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Houses
        public ActionResult Index()
        {
            return View(db.Houses.ToList());
        }

        [Authorize(Roles = "ROLE_USER")]
        public ActionResult Me()
        {
            var userName = User.Identity.GetUserName();
            List<House> lstHouseRented = new List<House>();

            var lstRoomRnt = db.Rooms.Where(r => r.RentUser == userName).ToList();
            foreach(var rm in lstRoomRnt)
            {
                var tmp = db.Houses.Find(rm.HouseId);
                lstHouseRented.Add(tmp);
            }
            return View(lstHouseRented);
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
            var hause = db.Houses.ToList();
            ViewBag.House = new MultiSelectList(hause, "Id", "Name");
            var roomInThatHouse = db.Rooms.Where(r => r.HouseId == id).ToList();
            var fakeHouse = new Room()
            {
                Id = 0,
                Name = "Thêm tài sản trong nhà"
            };
            roomInThatHouse.Add(fakeHouse);
            ViewBag.RoomInHouse = new MultiSelectList(roomInThatHouse, "Id", "Name");

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
            long rooomId = long.Parse(formCollection["Rum"]);

            if(rooomId == 0)
            {
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
            } else
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                long secondsSinceEpoch = (long)t.TotalSeconds;

                ItemInRoom item = new ItemInRoom
                {
                    Name = formCollection["ItemName"],
                    Description = formCollection["ItemDescription"],
                    RoomId = rooomId,
                    ItemCategoryId = long.Parse(formCollection["ItemCategoryId"]),
                    StatusId = long.Parse(formCollection["ItemStatusId"]),
                    Medias = medias,
                    AddedDate = secondsSinceEpoch
                };

                db.ItemInRooms.Add(item);
                db.SaveChanges();

                return RedirectToAction("Details", new { id = houseId });
            } 
        }

        public JsonResult GetRoomByHouse(int? id)
        {
            var data = db.Rooms.Where(r => r.HouseId == id).ToList();
            List<Room> vm = new List<Room>();
            foreach (var r in data)
            {
                Room tmp = new Room()
                {
                    Name = r.Name,
                    Id = r.Id
                };
                vm.Add(tmp);
            }
            return Json(vm, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHistory(int? id)
        {
            var listHistory = db.Transactions.Where(i => i.ItemId == id).OrderBy(k => k.Date).ToList();
            List<HistoryVM> vmData = new List<HistoryVM>();
            var listItem = db.ItemInHouses.ToList();
            var listStatus = db.ItemStatuses.ToList();
            var listHouse = db.Houses.ToList();
            var listRoom = db.Rooms.ToList();
            foreach(var trasn in listHistory)
            {
                HistoryVM tmp = new HistoryVM()
                {
                    Date = trasn.Date,
                    ItemId = trasn.ItemId,

                    Item = listItem.Where(i => i.Id == trasn.ItemId).Single().Name,

                    FromHouseId = trasn.FromHouseId,
                    FromRoomId = trasn.FromRoomId,
                    FromStatusId = trasn.FromStatusId,

                    FromHouse = trasn.FromHouseId == 0 ? "KTD" : listHouse.Where(h => h.Id == trasn.FromHouseId).Single().Name,
                    FromRoom = trasn.FromRoomId == 0 ? "KTD" : listRoom.Where(h => h.Id == trasn.FromRoomId).Single().Name,
                    FromStatus = trasn.FromStatusId == 0 ? "KTD" : listStatus.Where(s => s.Id == trasn.FromStatusId).Single().Status,


                    ToHouseId = trasn.ToHouseId,
                    ToRoomId = trasn.ToRoomId,
                    ToStatusId = trasn.ToStatusId,

                    ToHouse = trasn.ToHouseId == 0 ? "KTD" : listHouse.Where(h => h.Id == trasn.ToHouseId).Single().Name,
                    ToRoom = trasn.ToRoomId == 0 ? "KTD" : listRoom.Where(h => h.Id == trasn.ToRoomId).Single().Name,
                    ToStatus = trasn.ToStatusId == 0 ? "KTD" : listStatus.Where(s => s.Id == trasn.ToStatusId).Single().Status,

                    MediaId = trasn.MediaId,
                    Media = trasn.Media,

                    IsVerified = trasn.IsVerified,

                    Description = trasn.Description
                };
                vmData.Add(tmp);
            }

            return Json(vmData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeItemStatus(FormCollection formCollection)
        {
            //System.Text.StringBuilder st = new System.Text.StringBuilder();
            //foreach (string key in formCollection.Keys)
            //{
            //    st.AppendLine(String.Format("{0} - {1}", key, formCollection.GetValue(key).AttemptedValue));
            //}
            //string formValues = st.ToString();

            Media medias = new Media();
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
                    medias = fileDetail;

                    var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Media_Extension);
                    file.SaveAs(path);
                }
            }

            long houseId = long.Parse(formCollection["House-Id"]);
            long itemId = long.Parse(formCollection["Item-Id"]);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long secondsSinceEpoch = (long)t.TotalSeconds;

            Transaction transaction;
            if (medias.Id != Guid.Empty)
            {
                transaction = new Transaction
                {
                        Date = secondsSinceEpoch,
                    ItemId = itemId,
                    Description = formCollection["Description"],
                    FromHouseId = 0,
                    FromRoomId = 0,
                    ToHouseId = 0,
                    ToRoomId = 0,
                    FromStatusId = long.Parse(formCollection["Current-Status-Id"]),
                    ToStatusId = long.Parse(formCollection["NewStatus"]),
                    Media = medias,
                    IsVerified = true
                };
            } else
            {
                transaction = new Transaction
                {
                    Date = secondsSinceEpoch,
                    ItemId = itemId,
                    Description = formCollection["Description"],
                    FromHouseId = 0,
                    FromRoomId = 0,
                    ToHouseId = 0,
                    ToRoomId = 0,
                    FromStatusId = long.Parse(formCollection["Current-Status-Id"]),
                    ToStatusId = long.Parse(formCollection["NewStatus"]),
                    IsVerified = true
                };
            }
                    

            db.Transactions.Add(transaction);
            db.SaveChanges();

            var item = db.ItemInHouses.Where(it => it.Id == itemId).SingleOrDefault();
            item.StatusId = long.Parse(formCollection["NewStatus"]);

            db.SaveChanges();

            return RedirectToAction("Details", new { id = houseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeItemPlace(FormCollection formCollection)
        {
            //System.Text.StringBuilder st = new System.Text.StringBuilder();
            //foreach (string key in formCollection.Keys)
            //{
            //    st.AppendLine(String.Format("{0} - {1}", key, formCollection.GetValue(key).AttemptedValue));
            //}
            //string formValues = st.ToString();
            Media medias = new Media();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    Media fileDetail = new Media()
                    {
                        Media_Name = fileName,
                        Media_Extension = Path.GetExtension(fileName),
                        Id = Guid.NewGuid()
                    };
                    medias = fileDetail;

                    var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Media_Extension);
                    file.SaveAs(path);
                }
            }

            long houseId = long.Parse(formCollection["House-Id"]);
            long itemId = long.Parse(formCollection["Item-Id"]);

            long newHouseId = long.Parse(formCollection["NewHouse"]);
            long newRoomId = formCollection["NewRoom"] == null ? 0 : long.Parse(formCollection["NewRoom"]);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long secondsSinceEpoch = (long)t.TotalSeconds;
            
            Transaction transaction;
            if (medias.Id != Guid.Empty)
            {
                transaction = new Transaction
                {
                    Date = secondsSinceEpoch,
                    ItemId = itemId,
                    Description = formCollection["Description"],
                    FromHouseId = houseId,
                    FromRoomId = 0,
                    ToHouseId = newHouseId,
                    ToRoomId = newRoomId,
                    FromStatusId = 0,
                    ToStatusId = 0,
                    Media = medias,
                    IsVerified = true
                };
            } else
            {
                transaction = new Transaction
                {
                    Date = secondsSinceEpoch,
                    ItemId = itemId,
                    Description = formCollection["Description"],
                    FromHouseId = houseId,
                    FromRoomId = 0,
                    ToHouseId = newHouseId,
                    ToRoomId = newRoomId,
                    FromStatusId = 0,
                    ToStatusId = 0,
                    IsVerified = true
                };
            }

            db.Transactions.Add(transaction);
            db.SaveChanges();

            var item = db.ItemInHouses.Include(h => h.Medias).Where(it => it.Id == itemId).SingleOrDefault();
            if (newRoomId == 0)
            {
                item.HouseId = newHouseId;
            } else
            {
                ItemInRoom itemInRoom = new ItemInRoom()
                {
                    Name = item.Name,
                    Description = item.Description,
                    RoomId = newRoomId,
                    StatusId = item.StatusId,
                    ItemCategoryId = item.ItemCategoryId,
                    AddedDate = item.AddedDate,
                    Medias = item.Medias
                };
                db.ItemInRooms.Add(itemInRoom);
                db.ItemInHouses.Remove(item);
            }
            
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
            var listTinh = db.Provinces.ToList();
            ViewBag.ListProvince = new SelectList(listTinh, "Name", "Name");
            return View();
        }

        public JsonResult GetDistricByProvince(string name)
        {
            var data = db.Districts.Include(c => c.Province).Where(r => r.Province.Name == name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWardByDistric(string name)
        {
            var data = db.Wards.Include(c => c.District).Where(r => r.District.Name == name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
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
            return RedirectToAction("Details", new { id = house.Id });
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
