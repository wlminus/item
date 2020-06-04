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
    public class DistrictsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Districts
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

            var district = from s in db.Districts join sup in db.Provinces on s.ProvinceId equals sup.Id select s;
            var provinces = db.Provinces.ToList();
            ViewBag.Province = new MultiSelectList(provinces, "Id", "Name");

            if (!String.IsNullOrEmpty(searchString))
            {
                district = district.Where(s => s.Name.Contains(searchString)
                                       || s.Type.Contains(searchString)
                                       || s.Province.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    district = district.OrderByDescending(s => s.Name);
                    break;
                case "type":
                    district = district.OrderBy(s => s.Type);
                    break;
                case "date_desc":
                    district = district.OrderByDescending(s => s.Type);
                    break;
                default:  // Name ascending 
                    district = district.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(district.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["Id"]))
            {
                District district = new District()
                {
                    Name = formCollection["Name"],
                    Type = formCollection["Type"],
                    ProvinceId = long.Parse(formCollection["Province"])
                };

                db.Districts.Add(district);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                long id = long.Parse(formCollection["Id"]);
                var dataEdit = db.Districts.Find(id);
                dataEdit.Name = formCollection["Name"];
                dataEdit.Type = formCollection["Type"];
                dataEdit.ProvinceId = long.Parse(formCollection["Province"]);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

        }
















        // GET: Districts/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.Districts.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        // GET: Districts/Create
        public ActionResult Create()
        {
            ViewBag.ProvinceId = new SelectList(db.Provinces, "Id", "Name");
            return View();
        }

        // POST: Districts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Type,ProvinceId")] District district)
        {
            if (ModelState.IsValid)
            {
                db.Districts.Add(district);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProvinceId = new SelectList(db.Provinces, "Id", "Name", district.ProvinceId);
            return View(district);
        }

        // GET: Districts/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.Districts.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProvinceId = new SelectList(db.Provinces, "Id", "Name", district.ProvinceId);
            return View(district);
        }

        // POST: Districts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Type,ProvinceId")] District district)
        {
            if (ModelState.IsValid)
            {
                db.Entry(district).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProvinceId = new SelectList(db.Provinces, "Id", "Name", district.ProvinceId);
            return View(district);
        }

        // GET: Districts/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.Districts.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        // POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            District district = db.Districts.Find(id);
            db.Districts.Remove(district);
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
