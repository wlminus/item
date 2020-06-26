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
        private ApplicationSignInManager _signInManager;

        public UsersController()
        {
            context = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        [Authorize(Roles = "ROLE_ADMIN")]
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

        [Authorize(Roles = "ROLE_ADMIN")]
        public ActionResult Index()
        {
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

        [Authorize(Roles = "ROLE_ADMIN")]
        public ActionResult Create()
        {
            var roles = context.Roles.ToList();
            ViewBag.ListRoles = new MultiSelectList(roles, "Name", "Name");
            var NewUser = new RegisterModel();
            return View(NewUser);
        }

        [Authorize(Roles = "ROLE_ADMIN")]
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
                {
                    foreach (var role in createUser.listRoles)
                    {
                        UserManager.AddToRole(newUser.Id, role);
                    }
                } 

                return RedirectToAction("Index");
            }
            return View(createUser);
        }

        [Authorize]
        public ActionResult Me()
        {
            var userName = User.Identity.GetUserName();
            var user = context.Users.Include(u => u.Avatar).First(u => u.UserName == userName);
            var model = new EditUserViewModel(user);

            return View(model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Me(EditUserViewModel model)
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

            if (ModelState.IsValid)
            {
                var user = context.Users.First(u => u.UserName == model.UserName);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                if (medias.Id != Guid.Empty)
                {
                    user.Avatar = medias;
                }

                context.SaveChanges();
                return RedirectToAction("Admin", "Home");
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Admin", "Home");
            }
            return View(model);
        }


        [Authorize]
        public ActionResult Edit(string id)
        {
            var realUserName = id.Replace("~", ".");
            var user = context.Users.Include(u => u.Avatar).Where(u => u.UserName == realUserName).FirstOrDefault();
            var model = new EditUserViewModel(user);

            return View(model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel model)
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

            if (ModelState.IsValid)
            {
                var user = context.Users.First(u => u.UserName == model.UserName);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                if (medias.Id != Guid.Empty)
                {
                    user.Avatar = medias;
                }

                context.SaveChanges();
                return RedirectToAction("Admin", "Home");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult UserRoles(string id)
        {
            var realUserName = id.Replace("~", ".");
            var Db = new ApplicationDbContext();

            ViewBag.lstRole = new SelectList(Db.Roles.ToList(), "Name", "Name");
            
            var user = Db.Users.First(u => u.UserName == realUserName);
            var model = new SelectUserRolesViewModel(user);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(string[] DDLTest, SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);

                foreach (var role in DDLTest)
                {
                     idManager.AddUserToRole(user.Id, role);
                }
                return RedirectToAction("index");
            }
            return View();
        }

        public ActionResult Delete(string id = null)
        {
            var realUserName = id.Replace("~", ".");
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == realUserName);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            var realUserName = id.Replace("~", ".");
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == realUserName);
            Db.Users.Remove(user);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}