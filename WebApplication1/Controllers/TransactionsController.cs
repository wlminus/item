using PagedList;
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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        //public ActionResult Index()
        //{
        //    return View(db.Transactions.ToList());
        //}



        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.TypeSortParm = sortOrder == "type" ? "date_desc" : "type";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var listHistory = db.Transactions.ToList();
            List<HistoryVM> vmData = new List<HistoryVM>();
            var listItem = db.ItemInHouses.ToList();
            var listStatus = db.ItemStatuses.ToList();
            var listHouse = db.Houses.ToList();
            var listRoom = db.Rooms.ToList();
            foreach (var trasn in listHistory)
            {
                HistoryVM tmp = new HistoryVM()
                {
                    Id = trasn.Id,
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

            if (!String.IsNullOrEmpty(searchString))
            {
                vmData = vmData.Where(s => s.FromHouse.Contains(searchString)
                                       || s.FromRoom.Contains(searchString)
                                       || s.FromStatus.Contains(searchString)
                                       || s.ToHouse.Contains(searchString)
                                       || s.ToRoom.Contains(searchString)
                                       || s.ToStatus.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "from_house_desc":
                    vmData = vmData.OrderByDescending(s => s.FromHouse).ToList();
                    break;
                case "from_house":
                    vmData = vmData.OrderBy(s => s.FromHouse).ToList();
                    break;
                case "date_desc":
                    vmData = vmData.OrderByDescending(s => s.Date).ToList();
                    break;
                default:  // Name ascending 
                    vmData = vmData.OrderBy(s => s.Date).ToList();
                    break;
            }

            int pageSize = 40;
            int pageNumber = (page ?? 1);
            return View(vmData.ToPagedList(pageNumber, pageSize));
        }










        // GET: Transactions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,ItemId,FromHouseId,FromRoomId,FromStatusId,ToHouseId,ToRoomId,ToStatusId,MediaId,IsVerified,VerifiedById")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,ItemId,FromHouseId,FromRoomId,FromStatusId,ToHouseId,ToRoomId,ToStatusId,MediaId,IsVerified,VerifiedById")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
