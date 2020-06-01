using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
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


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Include(h => h.Medias)
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


            if (house == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }







        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        //public ActionResult Admin()
        //{
        //    return View();
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