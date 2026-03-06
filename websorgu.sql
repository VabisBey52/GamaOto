CREATE DATABASE GAMAOTOWEB
GO
USE GAMAOTOWEB
GO

CREATE TABLE Tbl_Admin (
    ID INT PRIMARY KEY IDENTITY(1,1),
    KullaniciAdi NVARCHAR(50),
    Sifre NVARCHAR(50)
)

INSERT INTO Tbl_Admin (KullaniciAdi, Sifre) VALUES ('GAMAOTOWEB', '101230')

CREATE TABLE Randevular (
    RandevuID INT PRIMARY KEY IDENTITY(1,1),
    MusteriAdSoyad NVARCHAR(100),
    MusteriEmail NVARCHAR(100),
    HizmetTuru NVARCHAR(50),
    RandevuTarihi DATETIME,
    OzelTalep NVARCHAR(MAX),
    Durum NVARCHAR(20) DEFAULT 'Beklemede' 
)

CREATE TABLE Tbl_Teknisyenler (
    TeknisyenID INT PRIMARY KEY IDENTITY(1,1),
    AdSoyad NVARCHAR(100),
    Gorev NVARCHAR(100), 
    ResimYolu NVARCHAR(250), 
    FacebookUrl NVARCHAR(250),
    TwitterUrl NVARCHAR(250),
    InstagramUrl NVARCHAR(250)
)

CREATE TABLE Tbl_Hizmetler (
    HizmetID INT PRIMARY KEY IDENTITY(1,1),
    Baslik NVARCHAR(100),
    Aciklama NVARCHAR(MAX),
    IkonClass NVARCHAR(50), 
    ResimYolu NVARCHAR(250)
)

CREATE TABLE Tbl_Yorumlar (
    YorumID INT PRIMARY KEY IDENTITY(1,1),
    MusteriAdSoyad NVARCHAR(100),
    Meslek NVARCHAR(50),
    YorumMetni NVARCHAR(MAX),
    MusteriResim NVARCHAR(250),
    Durum BIT 
)

CREATE TABLE Tbl_Mesajlar
(
MesajID INT IDENTITY,
AdSoyad NVARCHAR(50),
Email NVARCHAR(70),
Konu NVARCHAR(50),
Mesaj NVARCHAR(MAX),
Tarih DATETIME
)

CREATE TABLE Tbl_Resimler
(
	SliderID int IDENTITY  PRIMARY KEY NOT NULL,
	UstBaslik nvarchar(150) NULL,
	Baslik nvarchar(250) NULL,
	ArkaPlanResim nvarchar(250) NULL,
	OnResim nvarchar(250) NULL,
	Durum bit  DEFAULT 1 NULl  
)

CREATE TABLE Tbl_Ayarlar(
	ID int IDENTITY NOT NULL PRIMARY KEY,
	Adres nvarchar(250) NULL,
	Telefon nvarchar(20) NULL,
	Email nvarchar(100) NULL,
	CalismaSaatleriIci nvarchar(100) NULL, 
	CalismaSaatleriSonu nvarchar(100) NULL,
	Facebook nvarchar(150) NULL,
	Twitter nvarchar(150) NULL,
	Instagram nvarchar(150) NULL,
	Linkedin nvarchar(150) NULL
)

CREATE TABLE Tbl_Uyeler(
	UyeID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	AdSoyad nvarchar(100) NULL,
	Email nvarchar(100) NULL,
	Sifre nvarchar(50) NULL,
	UyeResim nvarchar(250) NULL,
	KayitTarihi datetime NULL DEFAULT GETDATE()
)

CREATE TABLE Tbl_Duyurular (
    DuyuruID INT PRIMARY KEY IDENTITY(1,1),
    Baslik NVARCHAR(200),
    DuyuruMetni NVARCHAR(MAX),
    ResimYolu NVARCHAR(250),
    Tarih DATETIME DEFAULT GETDATE(),
    Goruntulenme INT DEFAULT 0,
    BegeniSayisi INT DEFAULT 0,
    Durum BIT DEFAULT 1
)

CREATE TABLE Tbl_DuyuruYorumlar (
    YorumID INT PRIMARY KEY IDENTITY(1,1),
    DuyuruID INT FOREIGN KEY REFERENCES Tbl_Duyurular(DuyuruID),
    AdSoyad NVARCHAR(100),
    YorumMetni NVARCHAR(MAX),
    Tarih DATETIME DEFAULT GETDATE(),
    Durum BIT DEFAULT 0 
)

CREATE TABLE Tbl_SSS (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Soru NVARCHAR(MAX),
    Cevap NVARCHAR(MAX),
    Sira INT, 
    Durum BIT DEFAULT 1 
)