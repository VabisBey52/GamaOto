# 🚗 GamaOto - Profesyonel Oto Servis Yönetim Sistemi
GamaOto, bir otomobil servisinin tüm dijital operasyonlarını (randevu, ekip yönetimi, müşteri geri bildirimleri) tek bir merkezden yönetmesini sağlayan, ASP.NET MVC mimarisi ile geliştirilmiş dinamik bir web platformudur.

## ✨ Öne Çıkan Özellikler
📅 Akıllı Randevu Sistemi: Kullanıcılar online randevu oluşturabilir. Admin onayladığında müşteriye otomatik SMTP tabanlı e-posta bildirimi gönderilir.

🛠️ Dinamik Hizmet Yönetimi: Servis tarafından sunulan tüm hizmetler, ikonları ve açıklamalarıyla birlikte admin panelinden anlık olarak güncellenebilir.

👥 Ekip ve Teknisyen Paneli: Çalışanların uzmanlık alanları ve profilleri veritabanı üzerinden yönetilir.

⭐ Puanlamalı Yorum Sistemi: Müşteriler servis deneyimlerini yıldızlarla puanlayabilir. Bot saldırılarını önlemek için özel CAPTCHA (Doğrulama Kodu) mekanizması entegre edilmiştir.

📢 Duyuru ve Haber Modülü: Servis ile ilgili güncel haberler HTML destekli (Summernote) editör ile paylaşılabilir.

🔐 Güvenli Yönetim Paneli: Session tabanlı yetkilendirme ve [Authorize] kısıtlamaları ile korunan, şık bir admin kontrol paneli.



## 🚀 Kurulum ve Çalıştırma
Veritabanı: SQL Server üzerinde GAMAOTOWEB adında bir veritabanı oluşturun ve sağlanan .sql scriptini çalıştırın.

Bağlantı Ayarı: Web.config dosyasındaki connectionString alanını kendi yerel SQL sunucu bilgilerinizle güncelleyin.

Derleme: Projeyi Visual Studio ile açın, NuGet paketlerini geri yükleyin (Restore) ve F5 ile çalıştırın.


## 🛠️Alan,			Kullanılan Teknolojiler
Backend,		"C#, ASP.NET MVC 5".

Veritabanı,		MSSQL Server & Entity Framework.

Frontend,		"Bootstrap 5, jQuery, Font-Awesome".

Eklentiler,		"Toastr (Bildirimler), Summernote (Zengin Metin Editörü)".

Tünelleme,		Ngrok (Yerel testi dış dünyaya açmak için)



## 👥 Geliştirici Ekibi ve Rol Dağılımı

<img src="https://github.com/VabisBey52.png" width="150px;" style="border-radius:50%;"/><br /><sub><b>Adahan Karadeniz</b></sub>](https://github.com/VabisBey52)
### 👑 Adahan Karadeniz - Kurucu & Baş Geliştirici (Full-Stack)

Proje mimarisi, veritabanı tasarımı ve backend (C#) süreçlerinin yönetimi.
<img src="https://github.com/hüseyin6060.png" width="150px;" style="border-radius:50%;"/><br /><sub><b>Hüseyin Yüce</b></sub>](https://github.com/huseyin6060)
### 🛠️ Hüseyin Yüce - Proje Asistanı & Yazılım Geliştirici

Sistem geliştirme süreçlerinde teknik destek ve modül entegrasyonları.

### 🎨 Buğrahan Yılmaz - Tester & Fikir Lideri

Yazılım test süreçlerinde ve hata ayıklamada büyük pay sahibi olmasının yanı sıra, projenin konsept ve vizyoner fikirlerine yön vermiştir.

