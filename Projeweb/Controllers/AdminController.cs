using Projeweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace Projeweb.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        GAMAOTOWEBEntities db = new GAMAOTOWEBEntities();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GirisYap(Tbl_Admin p)
        {
            var bilgiler = db.Tbl_Admin.FirstOrDefault(x => x.KullaniciAdi == p.KullaniciAdi && x.Sifre == p.Sifre);

            if (bilgiler != null)
            {
                FormsAuthentication.SetAuthCookie(bilgiler.KullaniciAdi, false);
                Session["AdminAdi"] = bilgiler.KullaniciAdi;
                return RedirectToAction("IndexAdmin", "Default");
            }
            else
            {
                TempData["Hata"] = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }
        }

        public ActionResult CikisYap()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("GirisYap", "Admin");
        }
    }
}