using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UsersController : Controller
    {
        ApplicationDbContext context;

        public UsersController()
        {
            context = new ApplicationDbContext();
        }

        // GET: Roles
        public ActionResult Index()
        {
            var Users = context.Users.ToList();
            return View(Users);
        }
        public ActionResult Create()
        {
            var NewUser = new IdentityUser();
            return View(NewUser);
        }

        //[HttpPost]
        //public ActionResult Create(IdentityUser User)
        //{
        //    context.Users.Add(User);
        //    context.SaveChanges();
        //    return RedirectToAction("Index");
        //}
    }
}