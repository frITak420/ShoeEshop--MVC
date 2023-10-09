using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using Web.Models;

namespace Web.Controllers
{
    public class LoginController : BaseController
    {
       public MyDbContext db = new MyDbContext();


        public IActionResult BackToMain()
        {
            return RedirectToAction("Index","Home");
        }
        public IActionResult Index()
        {
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();
            return View();
        }

        public IActionResult TryLogin(Administrator admin, string returnUrl)
        {
            foreach (var item in db.Administrators)
            {
                if (item.Username == admin.Username && item.Password ==admin.Password)
                {
                    this.HttpContext.Session.SetString("login", item.Username);
                    ViewBag.IsAdmin = true;
                    return Redirect(returnUrl);
                }
            }
            ViewBag.IsAdmin = false;
            return RedirectToAction("Index");
        }

       public IActionResult LogOut()
       {
            string backurl = Request.Headers["Referer"].ToString();
            this.HttpContext.Session.Remove("login");
            return Redirect(backurl);
       }
    }
}
