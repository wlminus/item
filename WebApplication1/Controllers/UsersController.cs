using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UsersController : Controller
    {
        ApplicationDbContext context;

        UserManager<ApplicationUser> UserManager { get; set; }
        RoleManager<IdentityRole> RoleManager { get; set; }

        public UsersController()
        {
            context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        public async Task<ActionResult> Migrate()
        {
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);

            // RoleTypes is a class containing constant string values for different roles
            List<IdentityRole> identityRoles = new List<IdentityRole>();
            identityRoles.Add(new IdentityRole() { Name = RolesType.ROLE_ADMIN });
            identityRoles.Add(new IdentityRole() { Name = RolesType.ROLE_MANAGER });
            identityRoles.Add(new IdentityRole() { Name = RolesType.ROLE_USER });

            foreach (IdentityRole role in identityRoles)
            {
                manager.Create(role);
            }
            var UserManager2 = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            ApplicationUser admin = new ApplicationUser
            {
                Email = "admin@admin.com",
                UserName = "admin@admin.com",
                FirstName = "admin",
                LastName = "admin"
            };

            var result = await UserManager2.CreateAsync(admin, "AAssdd12@123");

            if (result.Succeeded)
                result = UserManager2.AddToRole(admin.Id, RolesType.ROLE_ADMIN);

            return View();
        }
        public ActionResult Index()
        {
            //var users = UserManager.Users;
            //List<ListUserViewModel> listModel = new List<ListUserViewModel>();
            //foreach (var user in users)
            //{
            //    var listRole = UserManager.GetRoles(user.Id);
            //    var model = new ListUserViewModel()
            //    {
            //        user = user,
            //        roles = listRole
            //    };
            //    listModel.Add(model);
            //}
            
            //return View(listModel);

            var roles = RoleManager.Roles.ToList();
            var userList = context.Users.ToList();
            List<ListUserViewModel> listModel = new List<ListUserViewModel>();

            userList.ForEach((x) =>
            {
                ListUserViewModel model = new ListUserViewModel(x);
                if (x.Roles.Any())
                {
                    foreach(var userRoles in x.Roles)
                    {
                        var roleDb = roles.FirstOrDefault(r => r.Id == userRoles.RoleId);
                        if (roleDb != null)
                        {
                            model.listRoles.Add(roleDb.Name);
                        }
                    }
                }
                listModel.Add(model);
            });
            return View(listModel);
        }

        public ActionResult Create()
        {
            var NewUser = new RegisterModel();
            return View(NewUser);
        }

        [HttpPost]
        public ActionResult Create(RegisterModel createUser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newUser = createUser.GetUser();

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

                        var path = Path.Combine(Server.MapPath("~/App_Data/Upload/"), fileDetail.Id + fileDetail.Media_Extension);
                        file.SaveAs(path);
                        newUser.Avatar = fileDetail;
                    }
                    
                }

                var result = UserManager.Create(newUser, createUser.Password);
                if (result.Succeeded)
                    result = UserManager.AddToRole(newUser.Id, RolesType.ROLE_USER);
                
                return RedirectToAction("Index");
            }
            return View(createUser);
        }
    }
}