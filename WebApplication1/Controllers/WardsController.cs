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
    public class WardsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Wards
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

            var wards = from s in db.Wards join sup in db.Districts on s.DistrictId equals sup.Id select s;
            var district = db.Districts.ToList();
            ViewBag.District = new MultiSelectList(district, "Id", "Name");

            if (!String.IsNullOrEmpty(searchString))
            {
                wards = wards.Where(s => s.Name.Contains(searchString)
                                       || s.Type.Contains(searchString) 
                                       || s.District.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    wards = wards.OrderByDescending(s => s.Name);
                    break;
                case "type":
                    wards = wards.OrderBy(s => s.Type);
                    break;
                case "date_desc":
                    wards = wards.OrderByDescending(s => s.Type);
                    break;
                default:  // Name ascending 
                    wards = wards.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 40;
            int pageNumber = (page ?? 1);
            return View(wards.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["Id"]))
            {
                Ward ward = new Ward()
                {
                    Name = formCollection["Name"],
                    Type = formCollection["Type"],
                    DistrictId = long.Parse(formCollection["District"])
                };

                db.Wards.Add(ward);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                long id = long.Parse(formCollection["Id"]);
                var dataEdit = db.Wards.Find(id);
                dataEdit.Name = formCollection["Name"];
                dataEdit.Type = formCollection["Type"];
                dataEdit.DistrictId = long.Parse(formCollection["District"]);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

        }












        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ward ward = db.Wards.Find(id);
            if (ward == null)
            {
                return HttpNotFound();
            }
            return View(ward);
        }

        // GET: Wards/Create
        public ActionResult Create()
        {
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name");
            return View();
        }

        // POST: Wards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Type,DistrictId")] Ward ward)
        {
            if (ModelState.IsValid)
            {
                db.Wards.Add(ward);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", ward.DistrictId);
            return View(ward);
        }

        // GET: Wards/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ward ward = db.Wards.Find(id);
            if (ward == null)
            {
                return HttpNotFound();
            }
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", ward.DistrictId);
            return View(ward);
        }

        // POST: Wards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Type,DistrictId")] Ward ward)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ward).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", ward.DistrictId);
            return View(ward);
        }

        // GET: Wards/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ward ward = db.Wards.Find(id);
            if (ward == null)
            {
                return HttpNotFound();
            }
            return View(ward);
        }

        // POST: Wards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Ward ward = db.Wards.Find(id);
            db.Wards.Remove(ward);
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
