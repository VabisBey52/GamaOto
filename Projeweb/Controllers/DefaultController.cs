using Projeweb.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Projeweb.Controllers
{
    public class AnasayfaViewModel
    {
        public List<Tbl_Hizmetler> Hizmetler { get; set; }
        public List<Tbl_Yorumlar> Yorumlar { get; set; }
        public List<Tbl_SSS> SSS { get; set; }
        public List<Tbl_Teknisyenler> Teknisyenler { get; set; }
        public List<Tbl_Resimler> Resimler { get; set; }

        public List<Tbl_Duyurular> Duyurular { get; set; }
    }
    [Authorize]
    public class DefaultController : Controller
    {
        GAMAOTOWEBEntities db = new GAMAOTOWEBEntities();

        [AllowAnonymous]
        public ActionResult  Index()
        {
            ViewBag.TeknisyenSayisi = db.Tbl_Teknisyenler.Count();
            ViewBag.Uyesayisi = db.Tbl_Uyeler.Count();
            

            string[] kelimeler = { "GAMA", "OTO", "SERVIS", "BAKIM", "TEKNIK", "GUVEN" };
            Random rnd = new Random();
            string secilenKelime = kelimeler[rnd.Next(kelimeler.Length)];


            Session["DogrulamaKodu"] = secilenKelime;

            ViewBag.GosterilenKod = secilenKelime;

            var model = new AnasayfaViewModel
            {
                Hizmetler = db.Tbl_Hizmetler.ToList(),
                Yorumlar = db.Tbl_Yorumlar.ToList(),
                Resimler = db.Tbl_Resimler.Where(x => x.Durum == true).ToList(),
                SSS = db.Tbl_SSS.Where(x => x.Durum == true).OrderBy(x => x.Sira).ToList(),
               Teknisyenler = db.Tbl_Teknisyenler.ToList(),
               Duyurular = db.Tbl_Duyurular.OrderByDescending(x => x.Tarih).ToList()
            };

            return View("~/Views/Default/Index.cshtml", model);
        }


        public ActionResult IndexAdmin()
        {
            var randevular = db.Randevular.ToList();
            return View(randevular);
        }

        // --- RANDEVU İŞLEMLERİ -------------------------------------------------------

        public ActionResult IndexRandevu()
        {
            var randevular = db.Randevular.ToList();
            
            return View(randevular);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RandevuAl(Randevular p)
        {
            if (Session["UyeID"] != null)
            {
                p.UyeID = (int)Session["UyeID"];
                p.MusteriAdSoyad = Session["UyeAd"].ToString();
                p.MusteriEmail = Session["UyeEmail"].ToString();
            }

            p.Durum = "Beklemede";
            db.Randevular.Add(p);
            db.SaveChanges();

            TempData["Mesaj"] = "Randevu talebiniz başarıyla iletildi!";
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Randevularim()
        {
            if (Session["UyeID"] == null)
            {
                return RedirectToAction("GirisYap", "Default"); 
            }

            int suAnkiUyeID = (int)Session["UyeID"];

            var kullaniciRandevulari = db.Randevular.Where(x => x.UyeID == suAnkiUyeID).ToList();

            return View(kullaniciRandevulari);
        }

        public ActionResult RandevuOnayla(int id)
        {
            var randevu = db.Randevular.Find(id);

            if (randevu != null)
            {
                randevu.Durum = "Onaylandı";

                db.SaveChanges();

                string konu = "Randevunuz Onaylandı - CarServ";
                string mesaj = $"Sayın {randevu.MusteriAdSoyad}, {randevu.RandevuTarihi.Value.ToShortDateString()} tarihindeki randevu talebiniz onaylanmıştır. Sizi servisimizde bekliyoruz.";
                MailGonder(randevu.MusteriEmail, konu, mesaj);
            }

            return RedirectToAction("IndexRandevu");
        }

        public ActionResult RandevuSil(int id)
        {
            var randevu = db.Randevular.Find(id);
            if (randevu != null)
            {
                string konu = "Randevu Talebiniz Hakkında - CarServ";
                string mesaj = $"Sayın {randevu.MusteriAdSoyad}, randevu talebiniz şu anki yoğunluk nedeniyle maalesef onaylanamamıştır. Başka bir tarih için tekrar randevu alabilirsiniz.";
                MailGonder(randevu.MusteriEmail, konu, mesaj);

                db.Randevular.Remove(randevu);
                db.SaveChanges();
            }

            return RedirectToAction("IndexRandevu");
        }

        // --- TEKNİSYEN İŞLEMLERİ -------------------------------------------

        [AllowAnonymous]
        public ActionResult IndexTeknisyen()
        {
            var teknisyenler = db.Tbl_Teknisyenler.ToList();
            return View(teknisyenler);
        }

        public ActionResult TeknisyenEkle()
        {
            var teknisyenler = db.Tbl_Teknisyenler.ToList();
            return View(teknisyenler);
        }

        [HttpPost]
        public ActionResult TeknisyenEkle(Tbl_Teknisyenler gelenVeri, HttpPostedFileBase ResimDosyasi)
        {
            Tbl_Teknisyenler yeni = new Tbl_Teknisyenler();
            yeni.AdSoyad = gelenVeri.AdSoyad;
            yeni.Gorev = gelenVeri.Gorev;

            if (ResimDosyasi != null && ResimDosyasi.ContentLength > 0)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(ResimDosyasi.FileName);
                string yol = System.IO.Path.Combine(Server.MapPath("~/tema/img/"), dosyaAdi);
                ResimDosyasi.SaveAs(yol);
                yeni.ResimYolu = "/tema/img/" + dosyaAdi;
            }

            db.Tbl_Teknisyenler.Add(yeni);
            db.SaveChanges();

            return RedirectToAction("TeknisyenEkle");
        }

        public ActionResult TeknisyenSil(int id)
        {
            var teknisyen = db.Tbl_Teknisyenler.Find(id);
            db.Tbl_Teknisyenler.Remove(teknisyen);
            db.SaveChanges();
            return RedirectToAction("IndexTeknisyen");
        }

        // --- HİZMET YÖNETİMİ -------------------------------------------------------------

        public ActionResult IndexHizmetler()
        {
            var hizmetler = db.Tbl_Hizmetler.ToList();
            return View(hizmetler);
        }

        [HttpGet]
        public ActionResult HizmetEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HizmetEkle(Tbl_Hizmetler h, HttpPostedFileBase ResimDosyasi)
        {
            if (ResimDosyasi != null && ResimDosyasi.ContentLength > 0)
            {
                string dosyaAdi = Path.GetFileName(ResimDosyasi.FileName);
                string yol = "~/tema/img/" + dosyaAdi;

                ResimDosyasi.SaveAs(Server.MapPath(yol));

                h.ResimYolu = "/tema/img/" + dosyaAdi;
            }

            db.Tbl_Hizmetler.Add(h);
            db.SaveChanges();
            return RedirectToAction("IndexHizmetler");
        }

        public ActionResult HizmetSil(int id)
        {
            var hizmet = db.Tbl_Hizmetler.Find(id);
            if (hizmet != null)
            {
                db.Tbl_Hizmetler.Remove(hizmet);
                db.SaveChanges();
            }
            return RedirectToAction("IndexHizmetler");
        }

        public ActionResult HizmetGetir(int id)
        {
            var hizmet = db.Tbl_Hizmetler.Find(id);
            return View(hizmet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult HizmetGuncelle(Tbl_Hizmetler h, HttpPostedFileBase ResimDosyasi)
        {
            var deger = db.Tbl_Hizmetler.Find(h.HizmetID);

            if (ResimDosyasi != null && ResimDosyasi.ContentLength > 0)
            {
                string dosyaAdi = Path.GetFileName(ResimDosyasi.FileName);
                string yol = "~/tema/img/" + dosyaAdi;
                ResimDosyasi.SaveAs(Server.MapPath(yol));

                deger.ResimYolu = "/tema/img/" + dosyaAdi;
            }

            deger.Baslik = h.Baslik;
            deger.Aciklama = h.Aciklama;
            deger.IkonClass = h.IkonClass;

            db.SaveChanges(); 
            return RedirectToAction("IndexHizmetler");
        }

        [AllowAnonymous]
        public ActionResult TumHizmetler()
        {
            var hizmetler = db.Tbl_Hizmetler.ToList();
            return View(hizmetler);
        }

        // Y O R U M L A R ---------------------------------------------------------------------

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult YorumEkle(Tbl_Yorumlar p, string KullaniciKodu, int Puan)
        {
            string dogruKod = Session["DogrulamaKodu"] as string;

            if (dogruKod != null && dogruKod == KullaniciKodu)
            {
                p.YorumTarihi = DateTime.Now;
                p.Puan = Puan;
                p.Durum = false;

                if (Session["UyeID"] != null)
                {
                    p.UyeID = (int)Session["UyeID"];
                    p.MusteriAdSoyad = Session["UyeAd"]?.ToString();
                    p.MusteriResim = Session["UyeResim"]?.ToString();
                }
                else
                {
                    p.MusteriResim = "/tema/img/uyeler/user-profile.png";
                }

                db.Tbl_Yorumlar.Add(p);
                db.SaveChanges();

                TempData["Mesajyorum"] = "Değerlendirmeniz başarıyla alındı. Teşekkür ederiz!";
                TempData["Durumyorum"] = "success";

                return RedirectToAction("Index");
            }
            else
            {
                TempData["Mesaj"] = "Güvenlik kodunu yanlış girdiniz. Lütfen tekrar deneyin.";
                TempData["Durum"] = "danger";

                return RedirectToAction("Index");
            }
        }

        [AllowAnonymous]
        public ActionResult TumYorumlar()
        {
            var yorumlar = db.Tbl_Yorumlar.Where(x => x.Durum == true).OrderByDescending(x => x.YorumTarihi).ToList();
            return View(yorumlar);
        }

        [AllowAnonymous]
        public ActionResult Yorumlarim()
        {
            if (Session["UyeID"] == null) return RedirectToAction("GirisYap");

            int id = (int)Session["UyeID"];

            var genelYorumlar = db.Tbl_Yorumlar.Where(x => x.UyeID == id).OrderByDescending(x => x.YorumTarihi).ToList();

            ViewBag.DuyuruYorumlari = db.Tbl_DuyuruYorumlar.Where(x => x.UyeID == id).OrderByDescending(x => x.Tarih).ToList();

            return View(genelYorumlar);
        }


        public ActionResult AdminYorumlar()
        {
            var yorumlar = db.Tbl_Yorumlar.ToList();
            return View(yorumlar);
        }

        public ActionResult YorumOnayla(int id)
        {
            var yorum = db.Tbl_Yorumlar.Find(id);
            yorum.Durum = true;
            db.SaveChanges();
            return RedirectToAction("AdminYorumlar");
        }

        [AllowAnonymous]
        public ActionResult YorumSil(int id)
        {
            var yorum = db.Tbl_Yorumlar.Find(id);
            db.Tbl_Yorumlar.Remove(yorum);
            db.SaveChanges();
            return RedirectToAction("AdminYorumlar");
        }

        public ActionResult YorumGizle(int id)
        {
            var yorum = db.Tbl_Yorumlar.Find(id);

            yorum.Durum = false;
            db.SaveChanges();
            return RedirectToAction("AdminYorumlar");
        }

        //M E S A J G Ö N D E R M E ---------------------------------------------------------------------

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MesajGonder(Tbl_Mesajlar m)
        {
            if (Session["UyeID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                m.UyeID = (int)Session["UyeID"];
                m.Tarih = DateTime.Now;
                m.Okundu = false;

                db.Tbl_Mesajlar.Add(m);
                db.SaveChanges();

                TempData["Mesaj"] = "Mesajınız başarıyla iletildi.";
            }
            catch (Exception)
            {
                TempData["HataMesaji"] = "Mesaj gönderilirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        public ActionResult GelenMesajlar()
        {
            var mesajlar = db.Tbl_Mesajlar.OrderByDescending(x => x.Tarih).ToList();
            return View(mesajlar);
        }

        [AllowAnonymous]
        public ActionResult Mesajlarim()
        {
            if (Session["UyeID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            int uyeId = (int)Session["UyeID"];
            var mesajlar = db.Tbl_Mesajlar.Where(x => x.UyeID == uyeId)
                                         .OrderByDescending(x => x.Tarih)
                                         .ToList();
            return View(mesajlar);
        }

        [HttpPost]
        public ActionResult MesajCevapla(int MesajID, string CevapMetni)
        {
            var mesaj = db.Tbl_Mesajlar.Find(MesajID);
            if (mesaj != null)
            {
                mesaj.Cevap = CevapMetni;
                mesaj.CevapTarihi = DateTime.Now;

                db.SaveChanges();
            }

            return RedirectToAction("MesajDetay", new { id = MesajID });
        }

        public ActionResult MesajDetay(int id)
        {
            var mesaj = db.Tbl_Mesajlar.Find(id);
            mesaj.Okundu = true;
            db.SaveChanges();

            return View(mesaj);
        }

        public ActionResult MesajSil(int id)
        {
            var mesaj = db.Tbl_Mesajlar.Find(id);

            if (mesaj != null)
            {
                db.Tbl_Mesajlar.Remove(mesaj);
                db.SaveChanges();

                TempData["Mesaj"] = "Mesaj başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Mesaj bulunamadı.";
            }

            // Tekrar listeye (GelenMesajlar sayfasına) dönüyoruz
            return RedirectToAction("GelenMesajlar");
        }

        // RESİMLER---------------------------------------------
        public ActionResult AdminResimler()
        {
            var liste = db.Tbl_Resimler.ToList();
            return View(liste);
        }


        [HttpGet]
        public ActionResult ResimlerEkle()
        {
            var liste = db.Tbl_Resimler.ToList();

            return View(liste);
        }

        [HttpGet]
        public ActionResult ResimGetir(int id)
        {
            var resim = db.Tbl_Resimler.Find(id);

            return View(resim);
        }

        [HttpPost]
        public ActionResult ResimlerEkle(Tbl_Resimler p, HttpPostedFileBase ArkaPlan, HttpPostedFileBase OnResim)
        {

            if (ArkaPlan != null)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ArkaPlan.FileName);
                string yol = "~/tema/img/" + dosyaAdi;
                ArkaPlan.SaveAs(Server.MapPath(yol));
                p.ArkaPlanResim = "/tema/img/" + dosyaAdi;
            }

            if (OnResim != null)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(OnResim.FileName);
                string yol = "~/tema/img/" + dosyaAdi;
                OnResim.SaveAs(Server.MapPath(yol));
                p.OnResim = "/tema/img/" + dosyaAdi;
            }

            p.Durum = true; 
            db.Tbl_Resimler.Add(p);
            db.SaveChanges();
            return RedirectToAction("ResimlerEkle");
        }

        [HttpPost]
        public ActionResult ResimGuncelle(Tbl_Resimler p, HttpPostedFileBase ArkaPlan, HttpPostedFileBase OnResim)
        {
            var r = db.Tbl_Resimler.Find(p.SliderID);

            if (r != null)
            {
                r.Baslik = p.Baslik;
                r.UstBaslik = p.UstBaslik;

                if (ArkaPlan != null)
                {
                    string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ArkaPlan.FileName);
                    ArkaPlan.SaveAs(Server.MapPath("~/tema/img/" + dosyaAdi));
                    r.ArkaPlanResim = "/tema/img/" + dosyaAdi;
                }

                if (OnResim != null)
                {
                    string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(OnResim.FileName);
                    OnResim.SaveAs(Server.MapPath("~/tema/img/" + dosyaAdi));
                    r.OnResim = "/tema/img/" + dosyaAdi;
                }

                db.SaveChanges();
            }
            return RedirectToAction("ResimlerEkle");
        }

        public ActionResult ResimlerSil(int id)
        {
            var slider = db.Tbl_Resimler.Find(id);
            db.Tbl_Resimler.Remove(slider);
            db.SaveChanges();
            return RedirectToAction("ResimlerEkle");
        }

        //S İ T E A Y A R L A R I----------------------------------------------------------------------

        [HttpGet]
        public ActionResult SiteAyarlari()
        {
            var ayarlar = db.Tbl_Ayarlar.FirstOrDefault(); // İlk ve tek satırı getir
            return View(ayarlar);
        }

        [HttpPost]
        public ActionResult SiteAyarlari(Tbl_Ayarlar p)
        {
            var a = db.Tbl_Ayarlar.Find(p.ID);
            a.Adres = p.Adres;
            a.Telefon = p.Telefon;
            a.Email = p.Email;
            a.CalismaSaatleriIci = p.CalismaSaatleriIci;
            a.CalismaSaatleriSonu = p.CalismaSaatleriSonu;
            a.Facebook = p.Facebook;
            a.Twitter = p.Twitter;
            a.Instagram = p.Instagram;
            a.Linkedin = p.Linkedin;

            db.SaveChanges();
            TempData["Mesaj"] = "Ayarlar başarıyla güncellendi.";
            return RedirectToAction("SiteAyarlari");
        }

        [AllowAnonymous]
        public PartialViewResult PartialNavbar()
        {
            var ayarlar = db.Tbl_Ayarlar.FirstOrDefault();
            return PartialView(ayarlar);
        }

        [AllowAnonymous]
        public PartialViewResult PartialFooter()
        {
            var ayarlar = db.Tbl_Ayarlar.FirstOrDefault();
            return PartialView(ayarlar);
        }

        // K A Y I T Y E R İ-----------------------------------------------------------------

        [HttpGet]
        [AllowAnonymous]
        public ActionResult KayitOl()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult KayitOl(Tbl_Uyeler p, HttpPostedFileBase ResimDosyasi)
        {
            p.KayitTarihi = DateTime.Now;

            // 1. Resim Yükleme İşlemi
            if (ResimDosyasi != null)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ResimDosyasi.FileName);
                string yol = "~/tema/img/uyeler/" + dosyaAdi;
                ResimDosyasi.SaveAs(Server.MapPath(yol));
                p.UyeResim = "/tema/img/uyeler/" + dosyaAdi;
            }
            else
            {
                p.UyeResim = "/tema/img/uyeler/default-user.png";
            }

            // 2. Kaydetme İşlemi
            db.Tbl_Uyeler.Add(p);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Girisyap(Tbl_Uyeler p)
        {
            var bilgiler = db.Tbl_Uyeler.FirstOrDefault(x => x.Email == p.Email && x.Sifre == p.Sifre);

            if (bilgiler != null)
            {
                Session["UyeID"] = bilgiler.UyeID;
                Session["UyeAd"] = bilgiler.AdSoyad;
                Session["UyeResim"] = bilgiler.UyeResim;
                Session["UyeEmail"] = bilgiler.Email;
                
                return RedirectToAction("Index", "Default");
            }
            else
            {
                ViewBag.Hata = "E-posta veya şifre hatalı!";
                return View("GirisYap"); 
            }
        }

        [AllowAnonymous]
        public ActionResult CikisYap()
        {
            Session.Abandon(); 
            return RedirectToAction("Index", "Default");
        }

        // P R O F İ L ----------------------------------------------------------------------------------------------------

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Profilim()
        {
            if (Session["UyeID"] == null) return RedirectToAction("GirisYap");

            int id = (int)Session["UyeID"];
            var uye = db.Tbl_Uyeler.Find(id);
            return View(uye);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Profilim(Tbl_Uyeler p, HttpPostedFileBase ResimDosyasi)
        {
            int id = (int)Session["UyeID"];
            var guncellenecekUye = db.Tbl_Uyeler.Find(id);

            guncellenecekUye.AdSoyad = p.AdSoyad;
            guncellenecekUye.Email = p.Email;

            if (ResimDosyasi != null)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ResimDosyasi.FileName);
                string yol = "~/tema/img/uyeler/" + dosyaAdi;
                ResimDosyasi.SaveAs(Server.MapPath(yol));
                guncellenecekUye.UyeResim = "/tema/img/uyeler/" + dosyaAdi;
            }

            db.SaveChanges();

            Session["UyeAd"] = guncellenecekUye.AdSoyad;
            Session["UyeEmail"] = guncellenecekUye.Email;
            Session["UyeResim"] = guncellenecekUye.UyeResim;

            TempData["Mesaj"] = "Profil bilgileriniz başarıyla güncellendi!";
            return RedirectToAction("Profilim");
        }

        //SIKCA SORULAN SORULAR

        [AllowAnonymous]
        public ActionResult SSS()
        {
            var sorular = db.Tbl_SSS.OrderBy(x => x.Sira).ToList();
            return View(sorular);
        }

        [HttpGet]
        public ActionResult SSSEkle()
        {
            var sorular = db.Tbl_SSS.OrderBy(x => x.Sira).ToList();
            return View(sorular);
        }

        [HttpPost]
        public ActionResult SSSEkle(Tbl_SSS p)
        {
            p.Durum = true;
            db.Tbl_SSS.Add(p);
            db.SaveChanges();
            return RedirectToAction("SSSEkle");
        }

        public ActionResult SSSSil(int id)
        {
            var veri = db.Tbl_SSS.Find(id);
            db.Tbl_SSS.Remove(veri);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SSSGuncelle(int id)
        {
            var veri = db.Tbl_SSS.Find(id);
            return View(veri);
        }

        [HttpPost]
        public ActionResult SSSGuncelle(Tbl_SSS p)
        {
            var veri = db.Tbl_SSS.Find(p.ID);
            veri.Soru = p.Soru;
            veri.Cevap = p.Cevap;
            veri.Sira = p.Sira;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //DUYURU -------------------------------------------------------------------------------------

        [AllowAnonymous]
        public ActionResult DuyuruDetay(int id)
        {
            var duyuru = db.Tbl_Duyurular.Find(id);
                duyuru.Goruntulenme++; ;
                db.SaveChanges();

            ViewBag.DigerDuyurular = db.Tbl_Duyurular.Where(x => x.DuyuruID != id && x.Durum == true).OrderByDescending(x => x.Tarih).Take(5).ToList();

            ViewBag.OnayliYorumlar = db.Tbl_DuyuruYorumlar.Where(x => x.DuyuruID == id && x.Durum == true).ToList();

            Random rnd = new Random();
            string kod = rnd.Next(10000, 99999).ToString();

            Session["GuvenlikKodu"] = kod;
            ViewBag.GosterilenKod = kod;

            return View(duyuru);
        }

        [HttpPost]
        public JsonResult BegeniArtir(int id)
        {
            var duyuru = db.Tbl_Duyurular.Find(id);
            if (duyuru != null)
            {
                duyuru.BegeniSayisi = (duyuru.BegeniSayisi ?? 0) + 1;
                db.SaveChanges();
                return Json(new { success = true, newCount = duyuru.BegeniSayisi });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DuyuruYorumEkle(int DuyuruID, string YorumMetni, string AdSoyad, string KullaniciKodu)
        {
            string sunucudakiKod = Session["GuvenlikKodu"] as string;

            if (sunucudakiKod == null || sunucudakiKod != KullaniciKodu)
            {
                TempData["Mesaj"] = "Güvenlik kodu hatalı! Lütfen tekrar deneyin.";
                TempData["Durum"] = "danger";
                return RedirectToAction("DuyuruDetay", new { id = DuyuruID });
            }

            Tbl_DuyuruYorumlar yeniYorum = new Tbl_DuyuruYorumlar();
            yeniYorum.DuyuruID = DuyuruID;
            yeniYorum.YorumMetni = YorumMetni;
            yeniYorum.Tarih = DateTime.Now;
            yeniYorum.Durum = false; 

            if (Session["UyeID"] != null)
            {
                yeniYorum.UyeID = (int)Session["UyeID"];
                yeniYorum.AdSoyad = Session["UyeAd"]?.ToString();
            }
            else
            {
                yeniYorum.AdSoyad = string.IsNullOrEmpty(AdSoyad) ? "Ziyaretçi" : AdSoyad;
            }

            db.Tbl_DuyuruYorumlar.Add(yeniYorum);
            db.SaveChanges();

            TempData["Mesaj"] = "Yorumunuz iletildi, onaylanınca gösterilecek.";
            TempData["Durum"] = "success";

            return RedirectToAction("DuyuruDetay", new { id = DuyuruID });
        }

        [HttpGet]
        public ActionResult DuyuruEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // Eğer metin içinde HTML kullanacaksan gereklidir güvenlikleri devre dısı bıreakıyor
        public ActionResult DuyuruEkle(Tbl_Duyurular p, HttpPostedFileBase ResimDosyasi)
        {
            if (ResimDosyasi != null && ResimDosyasi.ContentLength > 0)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ResimDosyasi.FileName);
                string yol = Path.Combine(Server.MapPath("~/tema/img/duyurular/"), dosyaAdi);
                ResimDosyasi.SaveAs(yol);

                p.ResimYolu = "/tema/img/duyurular/" + dosyaAdi;
            }

            if (Session["UyeID"] != null)
            {
                p.UyeID = Convert.ToInt32(Session["UyeID"]);
            }

            p.Tarih = DateTime.Now;
            p.Goruntulenme = 0;
            p.BegeniSayisi = 0;
            p.Durum = true;

            db.Tbl_Duyurular.Add(p);
            db.SaveChanges();

            return RedirectToAction("DuyuruListesi"); 
        }

        public ActionResult DuyuruListesi()
        {
            var liste = db.Tbl_Duyurular.Where(x => x.Durum == true).OrderByDescending(x => x.DuyuruID).ToList();

            ViewBag.TumYorumlar = db.Tbl_DuyuruYorumlar.OrderByDescending(x => x.Tarih).ToList();

            return View(liste);
        }

        public ActionResult DuyuruSil(int id)
        {
            var duyuru = db.Tbl_Duyurular.Find(id);
            if (duyuru != null)
            {
                duyuru.Durum = false;
                db.SaveChanges();
            }
            return RedirectToAction("DuyuruListesi");
        }

        [HttpGet]
        public ActionResult DuyuruGetir(int id)
        {
            var duyuru = db.Tbl_Duyurular.Find(id);
            return View(duyuru);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DuyuruGuncelle(Tbl_Duyurular p, HttpPostedFileBase ResimDosyasi)
        {
            var deger = db.Tbl_Duyurular.Find(p.DuyuruID);
            deger.Baslik = p.Baslik;
            deger.DuyuruMetni = p.DuyuruMetni;

            if (ResimDosyasi != null && ResimDosyasi.ContentLength > 0)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ResimDosyasi.FileName);
                string yol = Path.Combine(Server.MapPath("~/tema/img/duyurular/"), dosyaAdi);
                ResimDosyasi.SaveAs(yol);
                deger.ResimYolu = "/tema/img/duyurular/" + dosyaAdi;
            }

            db.SaveChanges();
            return RedirectToAction("DuyuruListesi");
        }

        public ActionResult DuyuruYorumOnayla(int id)
        {
            var yorum = db.Tbl_DuyuruYorumlar.Find(id);
                yorum.Durum = true; 
                db.SaveChanges();
            return RedirectToAction("DuyuruListesi");
        }

        public ActionResult DuyuruYorumSil(int id)
        {
            var yorum = db.Tbl_DuyuruYorumlar.Find(id);
            if (yorum != null)
            {
                db.Tbl_DuyuruYorumlar.Remove(yorum);
                db.SaveChanges();
            }
            return RedirectToAction("DuyuruListesi");
        }

        public ActionResult DuyuruYorumGizle(int id)
        {
            var yorum = db.Tbl_DuyuruYorumlar.Find(id);
            yorum.Durum = false; 
            db.SaveChanges(); 
            return RedirectToAction("DuyuruListesi");
        }

        [AllowAnonymous]
        public ActionResult TumDuyurular()
        {
            var liste = db.Tbl_Duyurular
                          .Where(x => x.Durum == true)
                          .OrderByDescending(x => x.Tarih)
                          .ToList();

            return View(liste);
        }

        // MAİL GONDERME YERİ

        [AllowAnonymous]
        public void MailGonder(string aliciMail, string konu, string mesaj)
        {
            try
            {
                string gonderici = ConfigurationManager.AppSettings["EmailUser"];
                string uygulamaSifresi = ConfigurationManager.AppSettings["EmailPassword"];

                // Mail adreslerini tanımlıyoruz
                var gondericiEmail = new MailAddress(gonderici, "OtoServis");
                var aliciEmail = new MailAddress(aliciMail);

                // Şifre değişkenini artık doğrudan ConfigurationManager'dan gelen değerle kullanıyoruz
                string sifre = uygulamaSifresi;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(gondericiEmail.Address, sifre)
                };

                using (var mess = new MailMessage(gondericiEmail, aliciEmail)
                {
                    Subject = konu,
                    Body = mesaj,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(mess);
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}