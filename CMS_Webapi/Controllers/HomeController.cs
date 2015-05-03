using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS_Webapi.Models;

namespace CMS_Webapi.Controllers
{
    public class HomeController : Controller
    {
        private readonly Context _db = new Context();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            IEnumerable<User> users = _db.Users.Where(u => u.Username == User.Identity.Name);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                var u = new User()
                {
                    Username = User.Identity.Name,
                    Email = "please@update.me"
                };
                _db.Users.Add(u);
                _db.SaveChanges();
                user = u;
            }

            return View(user);
        }
    }
}
