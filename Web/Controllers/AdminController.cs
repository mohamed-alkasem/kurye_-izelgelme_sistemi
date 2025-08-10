using MediatR;
using Application.DTOs;
using Application.Query;
using Domain.Interfaces;
using Infrastructure.Data;
using Application.Command;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IEmailSender _emailSender;
        private readonly IKuryeService _kuryeService;
        private readonly ISikayetService _sikayetService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IKullaniciService _kullaniciService;
        private readonly SiparisPdfService _siparisPdfService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly KullaniciPdfService _kullaniciPdfService;
        private readonly SiparisPdfTumService _siparisPdfTumService;
        private readonly KullaniciPdfTumService _kullaniciPdfTumService;

        public AdminController(ISikayetService sikayetService, UserManager<AppUser> userManager, IEmailSender emailSender,
            SignInManager<AppUser> signInManager, IMediator mediator, IKullaniciService kullaniciService, IKuryeService kuryeService,
            KullaniciPdfService kullaniciPdfService, KullaniciPdfTumService kullaniciPdfTumService, SiparisPdfService siparisPdfService,
            SiparisPdfTumService siparisPdfTumService)
        {
            _mediator = mediator;
            _emailSender = emailSender;
            _userManager = userManager;
            _kuryeService = kuryeService;
            _signInManager = signInManager;
            _sikayetService = sikayetService;
            _kullaniciService = kullaniciService;
            _siparisPdfService = siparisPdfService;
            _kullaniciPdfService = kullaniciPdfService;
            _siparisPdfTumService = siparisPdfTumService;
            _kullaniciPdfTumService = kullaniciPdfTumService;
        }

        //Anasayfa--------------------------------------------
        public IActionResult Anasayfa()
        {
            return View();
        }
        //----------------------------------------------------

        //SikayetlerAdmin-------------------------------------
        public async Task<IActionResult> SikayetlerAdmin()
        {
            var sikayetler = await _sikayetService.GetSikayetler();
            return View(sikayetler);
        }
        //----------------------------------------------------

        //KabulKuryecilerAdmin--------------------------------
        public async Task<IActionResult> KabulKuryecilerAdmin()
        {
            var kuryeciler = await _mediator.Send(new GetAcceptedKuryecilerQuery());
            return View(kuryeciler);
        }
        //----------------------------------------------------

        //KabulKuryeciDetay-----------------------------------
        public async Task<IActionResult> KabulKuryeciDetay(int kuryeciId)
        {
            var kuryeci = await _mediator.Send(new GetKuryeciByIdQuery(kuryeciId));
            return View(kuryeci);
        }
        //----------------------------------------------------

        //IsteklerAdmin---------------------------------------
        public async Task<IActionResult> IsteklerAdmin()
        {
            var kuryeciler = await _mediator.Send(new GetAllKuryecilerQuery());
            return View(kuryeciler);
        }
        //----------------------------------------------------

        //IstekDetay------------------------------------------
        public async Task<IActionResult> IstekDetay(int kuryeciId)
        {
            var kuryeci = await _mediator.Send(new GetKuryeciByIdQuery(kuryeciId));
            return View(kuryeci);
        }
        //----------------------------------------------------

        //SifreUret-------------------------------------------
        [HttpGet]
        public async Task<IActionResult> SifreUret(int kuryeciId)
        {
            var kuryeci = await _mediator.Send(new GetKuryeciByIdQuery(kuryeciId));
            string sifre = SifreUreticiServisi.Instance.RastgeleSifreUret(8);

            AppUser user = new()
            {
                UserName = kuryeci.TC,
                TC = kuryeci.TC,
                AdSoyad = kuryeci.AdSoyad,
                Email = kuryeci.Eposta,
                KullaniciRolu = "Kuryeci"
            };

            var result = await _userManager.CreateAsync(user, sifre);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Kuryeci");
                await _emailSender.SendEmailAsync(
                    kuryeci.Eposta,
                    "Talep Durumu Hakkında",
                    $@"<p>Merhaba {kuryeci.AdSoyad},</p>
                    <p>🎉 <strong>Sistem tarafından kuryeci hesabınız başarıyla oluşturulmuştur!</strong> 🎉</p>
                    <p>🔑 <strong>Şifreniz:</strong> <code>{sifre}</code></p>
                    <p>Lütfen bu şifre ile sisteme giriş yapınız.</p>
                    <p>💼 İyi çalışmalar dileriz! 😊</p>
                    <p style='color: gray; font-size: 10px;'> Gönderim Zamanı: {DateTime.Now}</p>"
                );

                await _mediator.Send(new KabulKuryeciCommand(kuryeciId));
                return RedirectToAction("IsteklerAdmin");
            }

            return NotFound();
        }
        //----------------------------------------------------

        //IstekRed--------------------------------------------
        public async Task<IActionResult> IstekRed(int kuryeciId, string sebep)
        {
            await _mediator.Send(new RedKuryeciCommand(kuryeciId));
            var kuryeci = await _mediator.Send(new GetKuryeciByIdQuery(kuryeciId));

            kuryeci.ReddetmeSebebi = sebep;
            await _mediator.Send(new GuncelleKuryeciCommand(kuryeci));

            var mailContent = $@"
                <p>Merhaba {kuryeci.AdSoyad},</p>
                <p>Üzgünüz 😔, sistem tarafından yapılan inceleme sonucunda 
                <strong style='color:red;'>kuryeci hesabı oluşturma talebiniz <u>REDDEDİLMİŞTİR</u>.</strong></p>";

            if (!string.IsNullOrWhiteSpace(sebep))
            {
                mailContent += $@"<p><strong>Reddetme Sebebi:</strong> {System.Net.WebUtility.HtmlEncode(sebep)}</p>";
            }

            mailContent += $@"
                <p>Bu süreç hakkında daha fazla bilgi almak isterseniz, bizimle iletişime geçebilirsiniz. 📞✉️</p>
                <p>Anlayışınız için teşekkür ederiz, gelecekte tekrar başvuru yapmanızı bekleriz. 🙏</p>
                <p>İyi günler dileriz! 🌟</p>
                <p style='color: gray; font-size: 10px;'>Gönderim Zamanı: {DateTime.Now}</p>";

            await _emailSender.SendEmailAsync(kuryeci.Eposta, "Talep Durumu Hakkında", mailContent);

            return RedirectToAction("IsteklerAdmin");
        }
        //----------------------------------------------------

        //RedKuryeciDetay-------------------------------------
        public async Task<IActionResult> RedKuryeciDetay(int kuryeciId)
        {
            var kuryeci = await _mediator.Send(new GetKuryeciByIdQuery(kuryeciId));
            return View(kuryeci);
        }
        //----------------------------------------------------

        //KullaniciRaporu-------------------------------------
        public async Task<IActionResult> KullaniciRaporu()
        {
            var dtoList = new List<KullaniciDto>();
            var kullanicilar = await _kullaniciService.GetKullanicilar();

            foreach (var kullanici in kullanicilar)
            {
                dtoList.Add(new KullaniciDto
                {
                    Id = kullanici.Id,
                    TC = kullanici.TC,
                    AdSoyad = kullanici.AdSoyad,
                    Tel = kullanici.Tel,
                    Adres = kullanici.Adres,
                    Eposta = kullanici.Eposta,
                    KayitTarihi = kullanici.KayitTarihi
                });
            }
            var list = dtoList.OrderByDescending(x => x.KayitTarihi).ToList();
            return View(list);
        }
        //----------------------------------------------------

        //KullaniciPdf----------------------------------------
        public async Task<IActionResult> KullaniciPdf(string tc)
        {
            var pdfBytes = await _kullaniciPdfService.GenerateKullaniciPdfAsync(tc);
            if (pdfBytes == null || pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", $"Kullanici_{tc}.pdf");
        }
        //----------------------------------------------------

        //KullaniciPdfTum-------------------------------------
        public async Task<IActionResult> KullaniciPdfTum()
        {
            var pdfBytes = await _kullaniciPdfTumService.GenerateKullaniciPdfTumAsync();
            if (pdfBytes == null || pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", "TumKullanicilar.pdf");
        }
        //----------------------------------------------------

        //SiparisRaporu---------------------------------------
        public async Task<IActionResult> SiparisRaporu()
        {
            var dtoList = new List<SiparislerAdminListDto>();

            var siparisler = await _kuryeService.GetKuryeler();
            var kullanicilar = await _kullaniciService.GetKullanicilar();
            var kuryeciler = await _mediator.Send(new GetAllKuryecilerQuery());

            foreach (var siparis in siparisler)
            {
                var kuryeci = kuryeciler.FirstOrDefault(u => u.TC == siparis.KuryeciTC);
                var kullanici = kullanicilar.FirstOrDefault(u => u.TC == siparis.KullaniciTC);
                dtoList.Add(new SiparislerAdminListDto
                {
                    SiparisId = siparis.Id,
                    KullaniciAdSoyad = kullanici!.AdSoyad,
                    KuryeciAdSoyad = kuryeci!.AdSoyad,
                    UrunAdi = siparis.UrunAdi,
                    Durum = siparis.Durum,
                    TeslimDurumu = siparis.TeslimDurumu,
                    SiparisTarihi = siparis.SiparisTarihi,
                    TeslimTarihi = siparis.TeslimTarihi
                });
            }
            var list = dtoList.OrderBy(x => x.SiparisId).ToList();
            return View(list);
        }
        //----------------------------------------------------

        //SiparisPdf------------------------------------------
        public async Task<IActionResult> SiparisPdf(int id)
        {
            var pdfBytes = await _siparisPdfService.GenerateSiparisPdfAsync(id);
            if (pdfBytes == null || pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", $"Siparis_{id}.pdf");
        }
        //----------------------------------------------------

        //SiparisPdfTum---------------------------------------
        public async Task<IActionResult> SiparisPdfTum()
        {
            var pdfBytes = await _siparisPdfTumService.GenerateSiparisPdfTumAsync();
            if (pdfBytes == null || pdfBytes.Length == 0) return NotFound();
            return File(pdfBytes, "application/pdf", "TumSiparisler.pdf");
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

        //CikisYap--------------------------------------------
        public async Task<IActionResult> CikisYap()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Anasayfa", "Giris", null);
        }
        //----------------------------------------------------
    }
}
