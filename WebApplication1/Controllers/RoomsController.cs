using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rooms
        public ActionResult Index()
        {
            return View(db.Rooms.Include(r => r.House).ToList());
        }

        public ActionResult Uncheck()
        {
            var data = db.Rooms.Include(r => r.House).Where(r => r.RentUser != null && r.Verified == false).ToList();
            return View(data);
        }


        public ActionResult Decline(FormCollection formCollection)
        {
            long roomId = long.Parse(formCollection["Id"]);

            var rm = db.Rooms.Find(roomId);
            rm.RentUser = null;

            db.SaveChanges();

            return RedirectToAction("Uncheck");
        }

        public async Task<ActionResult> Accept(FormCollection formCollection)
        {
            long roomId = long.Parse(formCollection["Id"]);

            var rm = db.Rooms.Find(roomId);
            rm.Verified = true;

            
            var message = new MailMessage();
            message.To.Add(new MailAddress(rm.RentUser));
            message.From = new MailAddress(Constant.EMAIL_ADDRESS);
            message.Subject = "Yêu cầu thuê được phê duyệt";
            var itemBody = "<p>Yêu cầu thuê phòng: {0} đã được chấp thuận </p>";
            itemBody += @"<table class='table'>
                                <tr>
                                    <th>Tài sản</th>
                                    <th>Tên</th>
                                    <th>Loại</th>
                                    <th>Trạng thái</th>
                                    <th>Ngày thêm
                                    </th>
                                </tr>";

            var itemlst = db.ItemInRooms.Include(t => t.Status).Where(it => it.RoomId == roomId).ToList();
            foreach(var item in itemlst)
            {
                itemBody += @"<tr><td></td>
                            <td>" + item.Name + @"</td>
                            <td>" + item.ItemCategory.Name + @"</td>
                            <td>" + item.Status.Status + @"</td>
                            <td>" + item.AddedDate + @"</td></tr>";
            }
            itemBody += "</table>";

            message.Body = string.Format(itemBody, rm.Name);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = Constant.EMAIL_ADDRESS,
                    Password = Constant.EMAIL_PASSWORD
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }

            db.SaveChanges();

            return RedirectToAction("Uncheck");
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
            var hause = db.Houses.ToList();
            ViewBag.House = new MultiSelectList(hause, "Id", "Name");

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



        public JsonResult GetHistory(int? id)
        {
            var listHistory = db.Transactions.Where(i => i.ItemId == id).OrderBy(k => k.Date).ToList();
            List<HistoryVM> vmData = new List<HistoryVM>();
            var listItem = db.ItemInRooms.ToList();
            var listStatus = db.ItemStatuses.ToList();
            var listHouse = db.Houses.ToList();
            var listRoom = db.Rooms.ToList();
            foreach (var trasn in listHistory)
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

            long roomId = long.Parse(formCollection["Room-Id"]);
            long itemId = long.Parse(formCollection["Item-Id"]);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long secondsSinceEpoch = (long)t.TotalSeconds;

            Transaction transaction = new Transaction
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

            db.Transactions.Add(transaction);
            db.SaveChanges();

            var item = db.ItemInRooms.Where(it => it.Id == itemId).SingleOrDefault();
            item.StatusId = long.Parse(formCollection["NewStatus"]);

            db.SaveChanges();

            return RedirectToAction("Details", new { id = roomId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeItemPlace(FormCollection formCollection)
        {
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
            long roomId = long.Parse(formCollection["Room-Id"]);
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
                    FromRoomId = roomId,
                    ToHouseId = newHouseId,
                    ToRoomId = newRoomId,
                    FromStatusId = 0,
                    ToStatusId = 0,
                    Media = medias,
                    IsVerified = true
                };
            }
            else
            {
                transaction = new Transaction
                {
                    Date = secondsSinceEpoch,
                    ItemId = itemId,
                    Description = formCollection["Description"],
                    FromHouseId = houseId,
                    FromRoomId = roomId,
                    ToHouseId = newHouseId,
                    ToRoomId = newRoomId,
                    FromStatusId = 0,
                    ToStatusId = 0,
                    IsVerified = true
                };
            }

            db.Transactions.Add(transaction);
            db.SaveChanges();

            var item = db.ItemInRooms.Include(h => h.Medias).Where(it => it.Id == itemId).SingleOrDefault();
            if (newRoomId == 0)
            {
                ItemInHouse itemInHouse = new ItemInHouse()
                {
                    Name = item.Name,
                    Description = item.Description,
                    HouseId = newHouseId,
                    StatusId = item.StatusId,
                    ItemCategoryId = item.ItemCategoryId,
                    AddedDate = item.AddedDate,
                    Medias = item.Medias
                };
                db.ItemInHouses.Add(itemInHouse);
                db.ItemInRooms.Remove(item);
                
            }
            else
            {
                item.RoomId = newRoomId;
            }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoom(FormCollection formCollection)
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
            long roomId = long.Parse(formCollection["Room-Id"]);

            var data = db.Rooms.Find(roomId);

            data.Medias = medias;
            data.Name = formCollection["RoomName"];
            data.Type = formCollection["RoomType"];

            db.SaveChanges();

            return RedirectToAction("Details", new { id = roomId });
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
