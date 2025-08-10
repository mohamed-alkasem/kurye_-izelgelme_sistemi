using MediatR;
using Domain.Entities;
using Domain.Interfaces;
using Application.Query;
using Infrastructure.Data;
using Application.Command;
using Application.Services;
using Web.Models.AdminGiris;
using Microsoft.AspNetCore.Mvc;
using Web.Models.KullaniciGiris;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers
{
    public class GirisController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly IKullaniciService _kullaniciService;
        private readonly SignInManager<AppUser> _signInManager;

        public GirisController(IKuryeService kuryeService, ISikayetService sikayetService,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IKullaniciService kullaniciService,
            IEmailSender emailSender, IMediator mediator)
        {
            _mediator = mediator;
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _kullaniciService = kullaniciService;
        }

        //Anasayfa--------------------------------------------
        public IActionResult Anasayfa()
        {
            return View();
        }
        //----------------------------------------------------

        //KullaniciGiris--------------------------------------
        public IActionResult KullaniciGiris()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciGiris(UserLogin model)
        {
            if (model.TC == "12345678900")
            {
                ModelState.AddModelError("TC", "Kullanıcı TC bulunamadı.");
                return View(model);
            }
            else if (ModelState.IsValid)
            {
                // Kullanıcıyı TC no ile arama
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.TC == model.TC);

                // Eğer kullanıcı bulunmazsa    
                if (user == null)
                {
                    ModelState.AddModelError("TC", "Kullanıcı TC bulunamadı.");
                    return View(model);
                }

                // Kullanıcı bulunduysa, şifreyi kontrol et
                var result = await _signInManager.PasswordSignInAsync(
                    model.TC!, model.Sifre!, true, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Anasayfa", "Kullanici");
                }
                else
                {
                    ModelState.AddModelError("Sifre", "Şifre hatalı.");
                    return View(model);
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //KullaniciUye-----------------------------------------------
        public IActionResult KullaniciUye()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciUye(UserRegister model)
        {
            if (model.TC == "12345678900")
            {
                ModelState.AddModelError("TC", "Bu TC numarası zaten kayıtlı.");
                return View(model);
            }
            else if (ModelState.IsValid)
            {
                var TCZatenKayitliMi = await _kullaniciService.TcVarMi(model.TC);

                if (TCZatenKayitliMi)
                {
                    ModelState.AddModelError("TC", "Bu TC numarası zaten kayıtlı.");
                    return View(model);
                }

                AppUser user = new()
                {
                    UserName = model.TC, // Kullanıcı adı olarak TC kullan
                    TC = model.TC,
                    AdSoyad = model.AdSoyad,
                    KullaniciRolu = "Kullanici"
                };

                var result = await _userManager.CreateAsync(user, model.Sifre!);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Kullanici");
                }
                if (result.Succeeded)
                {
                    byte[] resim;

                    using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "person.jpg"), FileMode.Open))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            resim = memoryStream.ToArray();
                        }
                    }

                    //Kullaniciler tablosuna kullanici ekleme---------------------
                    var eklenecekKullanici = new Kullanici
                    {
                        TC = model.TC!,
                        AdSoyad = model.AdSoyad!,
                        Tel = "Eklenmedi",
                        Eposta = "Eklenmedi",
                        Adres = "Eklenmedi",
                        Image = resim,
                        KayitTarihi = DateTime.Now
                    };
                    await _kullaniciService.KullaniciEkle(eklenecekKullanici);
                    //---------------------------------------------------------
                    ModelState.Clear();
                    return RedirectToAction("KullaniciGiris");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //KullaniciSifreUnuttum-------------------------------
        public IActionResult KullaniciSifreUnuttum()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciSifreUnuttum(SifreUnuttum model)
        {
            if (model.TC == "12345678900")
            {
                ModelState.AddModelError("TC", "Kullanıcı TC bulunamadı.");
                return View(model);
            }
            else if (ModelState.IsValid)
            {
                // TC No ile kullanıcıyı bul
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.TC == model.TC);
                if (user == null)
                {
                    ModelState.AddModelError("TC", "TC bulunamadı.");
                    return View(model);
                }

                // Şifre kontrolü
                var isPasswordMatch = await _userManager.CheckPasswordAsync(user, model.YeniSifre);
                if (isPasswordMatch)
                {
                    ModelState.AddModelError("", "Yeni şifre, mevcut şifre ile aynı olamaz!");
                    return View(model);
                }

                // Şifre sıfırlama işlemi
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, model.YeniSifre);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    return RedirectToAction("KullaniciGiris");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //AdminGiris------------------------------------------
        public IActionResult AdminGiris()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminGiris(AdminLogin model)
        {
            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.Users.FirstOrDefaultAsync(u => u.TC == model.TC);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Sifre!, true, false);
                    if (result.Succeeded && await _signInManager.UserManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Anasayfa", "Admin");
                    }
                    ModelState.AddModelError("Sifre", "Şifre hatalı.");
                }
                else
                {
                    ModelState.AddModelError("", "TC No veya Şifre hatalı.");
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //KuryeciGiris----------------------------------------
        public IActionResult KuryeciGiris()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KuryeciGiris(Models.KuryeciGiris.UserLogin model)
        {
            if (ModelState.IsValid)
            {
                // Kuryeci eposta ile arama
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Eposta);

                if (user == null)
                {
                    ModelState.AddModelError("", "Eposta bulunamadı.");
                    return View(model);
                }

                // Kuryeci bulunduysa, şifreyi kontrol et
                var result = await _signInManager.PasswordSignInAsync(
                    user.TC!, model.Sifre!, true, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Anasayfa", "Kuryeci");
                }
                else
                {
                    ModelState.AddModelError("Sifre", "Şifre hatalı.");
                    return View(model);
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //KuryeOl---------------------------------------------
        public IActionResult KuryeOl()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KuryeOl(Kuryeci model)
        {
            ModelState.Remove("clientFile");
            ModelState.Remove("KabulDurumu");
            ModelState.Remove("Image");

            if (ModelState.IsValid)
            {
                var epostaZatenKayitliMi = await _mediator.Send(new EpostaVarMiQuery(model.Eposta));
                var TCZatenKayitliMi = await _mediator.Send(new TcVarMiQuery(model.TC));

                if (epostaZatenKayitliMi)
                {
                    ModelState.AddModelError("Eposta", "Bu e-posta adresi zaten kullanılıyor.");
                    return View(model);
                }
                else if (TCZatenKayitliMi)
                {
                    ModelState.AddModelError("TC", "Bu TC numarası zaten kayıtlı.");
                    return View(model);
                }
                else
                {
                    byte[] resim;
                    using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "person.jpg"), FileMode.Open))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            resim = memoryStream.ToArray();
                        }
                    }

                    model.Image = resim;
                    model.KabulDurumu = "Beklemede";
                    model.IstekTarihi = DateTime.Now;
                    await _mediator.Send(new KuryeOlCommand(model));

                    TempData["SuccessMessage"] = "Talebiniz başarıyla alınmıştır";

                    return RedirectToAction("KuryeOl");
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //KuryeciSifreUnuttum---------------------------------
        public IActionResult KuryeciSifreUnuttum()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KuryeciSifreUnuttum(Models.KuryeciGiris.SifreUnuttum model)
        {
            if (ModelState.IsValid)
            {
                // Kuryeci eposta ile arama
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Eposta);

                if (user == null)
                {
                    ModelState.AddModelError("", "Eposta bulunamadı.");
                    return View(model);
                }

                string sifre = SifreUreticiServisi.Instance.RastgeleSifreUret(8);

                await _emailSender.SendEmailAsync(
                    model.Eposta,
                    "Yeni Şifre Bilgisi",
                    $@"<p>Merhaba {user.AdSoyad},</p>
                    <p>🎉 <strong>Sistem tarafından kuryeci hesabınız için yeni bir şifre oluşturulmuştur.</strong> 🎉</p>
                    <p>🔑 <strong>Yeni Şifreniz:</strong> <code>{sifre}</code></p>
                    <p>Lütfen bu şifre ile sisteme giriş yapınız. 🚀</p>
                    <p>İyi çalışmalar dileriz! 😊</p>
                    <p style = 'color: gray; font-size: 10px;'> Gönderim Zamanı: {DateTime.Now}</p>"
                );

                // Şifre sıfırlama işlemi
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, sifre);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    return RedirectToAction("KuryeciGiris");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //SiparislerimGiris-----------------------------------
        public IActionResult SiparislerimGiris()
        {
            return View();
        }
        //----------------------------------------------------

        //SikayetlerGiris-------------------------------------
        public IActionResult SikayetlerGiris()
        {
            return View();
        }
        //----------------------------------------------------

        //Hakkimizda------------------------------------------
        public IActionResult Hakkimizda()
        {
            return View();
        }
        //----------------------------------------------------

        //SSS-------------------------------------------------
        public IActionResult SSS()
        {
            return View();
        }
        //----------------------------------------------------

        //Yardim----------------------------------------------
        public IActionResult Yardim()
        {
            return View();
        }
        //----------------------------------------------------

        //GizlilikPolitikasi----------------------------------
        public IActionResult GizlilikPolitikasi()
        {
            return View();
        }
        //----------------------------------------------------

        //KullanimKosullari-----------------------------------
        public IActionResult KullanimKosullari()
        {
            return View();
        }
        //----------------------------------------------------
    }
}
