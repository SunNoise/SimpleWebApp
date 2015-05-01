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
            if (!users.Any())
            {
                var u = new User()
                {
                    Username = User.Identity.Name
                };
                _db.Users.Add(u);
                _db.SaveChanges();
            }

            return View();
        }
    }
}
