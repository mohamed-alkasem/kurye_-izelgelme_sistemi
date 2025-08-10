using MediatR;
using Newtonsoft.Json;
using Domain.Entities;
using Application.DTOs;
using Application.Query;
using Domain.Interfaces;
using Infrastructure.Data;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Kurye.Web.Controllers
{
    [Authorize(Roles = "Kullanici")]
    public class KullaniciController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IKuryeService _kuryeService;
        private readonly ISikayetService _sikayetService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IKullaniciService _kullaniciService;
        private readonly SignInManager<AppUser> _signInManager;

        public KullaniciController(IKuryeService kuryeService, ISikayetService sikayetService,
            SignInManager<AppUser> signInManager, IKullaniciService kullaniciService,
            UserManager<AppUser> userManager, IMediator mediator)
        {
            _mediator = mediator;
            _userManager = userManager;
            _kuryeService = kuryeService;
            _signInManager = signInManager;
            _sikayetService = sikayetService;
            _kullaniciService = kullaniciService;
        }

        //Anasayfa--------------------------------------------
        public IActionResult Anasayfa()
        {
            return View();
        }
        //----------------------------------------------------

        //Chat------------------------------------------------
        public async Task<IActionResult> Chat(int kuryeId, string kuryeciTC)
        {
            var kuryeci = await _mediator.Send(new GetKuryeciByTCQuery(kuryeciTC));
            if (kuryeci == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Kurye = kuryeci;
            ViewBag.CurrentUserTC = user.TC;
            ViewBag.ToUserTC = kuryeci.TC;
            ViewBag.CurrentUserName = user.AdSoyad;
            return View();
        }
        //----------------------------------------------------

        //Profil----------------------------------------------
        public async Task<IActionResult> Profil(string kullaniciTC)
        {
            var kullanici = await _kullaniciService.GetKullaniciByTC(kullaniciTC);
            if (kullanici == null) return NotFound();
            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> Profil(Kullanici model)
        {
            ModelState.Remove("clientFile");
            ModelState.Remove("Image");

            if (ModelState.IsValid)
            {
                if (model.clientFile != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await model.clientFile.CopyToAsync(stream);
                        model.Image = stream.ToArray();
                    }

                    if (model.Image != null)
                    {
                        var kullanici = await _kullaniciService.GetKullaniciByTC(model.TC);
                        if (kullanici is not null)
                        {
                            if (kullanici.Eposta == model.Eposta && kullanici.Tel == model.Tel && kullanici.Adres == model.Adres && kullanici.Image == model.Image)
                            {
                                ModelState.AddModelError("", "Girilen bilgiler zaten mevcut. Lütfen değişiklik yapın.");
                                return View(model);
                            }
                            else
                            {
                                await _kullaniciService.KullaniciGuncelle(model);
                                return RedirectToAction("Profil", new { kullaniciTC = model.TC });
                            }
                        }
                    }
                }
                if (model.clientFile == null)
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

                    if (resim != null)
                    {
                        var kullanici = await _kullaniciService.GetKullaniciByTC(model.TC);
                        if (kullanici is not null)
                        {
                            if (kullanici.Eposta == model.Eposta && kullanici.Tel == model.Tel && kullanici.Adres == model.Adres)
                            {
                                ModelState.AddModelError("", "Girilen bilgiler zaten mevcut. Lütfen değişiklik yapın.");
                                return View(model);
                            }
                            else
                            {
                                model.Image = resim;
                                await _kullaniciService.KullaniciGuncelle(model);
                                return RedirectToAction("Profil", new { kullaniciTC = model.TC });
                            }
                        }
                    }
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //SiparislerimKullanici-------------------------------
        public async Task<IActionResult> SiparislerimKullanici()
        {
            var user = await _userManager.GetUserAsync(User);

            var kuryeler = await _kuryeService.GetKuryelerByKullaniciTC(user.TC);
            if (kuryeler is not null)
            {
                var kuryeListesi = new List<SiparislerKullaniciListDto>();
                foreach (var item in kuryeler)
                {
                    var kuryeci = await _mediator.Send(new GetKuryeciByTCQuery(item.KuryeciTC));
                    var kurye = new SiparislerKullaniciListDto()
                    {
                        KuryeId = item.Id,
                        KuryeciTC = item.KuryeciTC,
                        KullaniciTC = item.KullaniciTC,
                        KuryeciTel = kuryeci.Tel,
                        KuryeDurum = item.Durum,
                        KuryeTeslimDurumu = item.TeslimDurumu,
                        SiparisTarihi = item.SiparisTarihi
                    };
                    kuryeListesi.Add(kurye);
                }
                return View(kuryeListesi);
            }
            return View(new List<SiparislerKullaniciListDto>());
        }
        //----------------------------------------------------

        //SiparisDetayKullanici-------------------------------
        public async Task<IActionResult> SiparisDetayKullanici(int kuryeId)
        {
            var kurye = await _kuryeService.GetKuryeById(kuryeId);
            return View(kurye);
        }
        //----------------------------------------------------

        //SikayetlerKullanici---------------------------------
        public IActionResult SikayetlerKullanici()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SikayetlerKullanici(Sikayet sikayet)
        {
            if (ModelState.IsValid)
            {
                sikayet.SikayetTarihi = DateTime.Now;
                await _sikayetService.SikayetGonder(sikayet);
                ModelState.Clear();
                TempData["SuccessMessage"] = "Şikayetiniz başarıyla gönderilmiştir";
                return View();
            }
            return View(sikayet);
        }
        //----------------------------------------------------

        //KuryeCagir------------------------------------------
        public IActionResult KuryeCagir()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KuryeCagir(Domain.Entities.Kurye model)
        {
            ModelState.Remove("Durum");
            ModelState.Remove("KuryeciTc");
            ModelState.Remove("KullaniciTc");
            ModelState.Remove("TeslimDurumu");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                model.SiparisTarihi = DateTime.Now;
                model.Durum = "Pasif";
                model.TeslimDurumu = "Teslim Edilmedi";
                model.KullaniciTC = user.TC;
                return RedirectToAction("AktifKurye", model);
            }
            return View(model);
        }
        //----------------------------------------------------

        //AktifKurye------------------------------------------
        public async Task<IActionResult> AktifKurye(Domain.Entities.Kurye kurye)
        {
            var kuryeciler = await _mediator.Send(new GetAcceptedKuryecilerQuery());
            TempData["kurye"] = JsonConvert.SerializeObject(kurye);
            return View(kuryeciler);
        }
        //----------------------------------------------------

        //Cagir-----------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Cagir(string kuryeciTC)
        {
            var kuryeJson = TempData["kurye"] as string;
            if (kuryeJson == null)
            {
                return RedirectToAction("AktifKurye");
            }

            var kurye = JsonConvert.DeserializeObject<Domain.Entities.Kurye>(kuryeJson);
            if (kurye != null)
            {
                kurye.KuryeciTC = kuryeciTC;

                var manager = new KuryeManager(_kuryeService);
                manager.Attach(new KuryeNotificationObserver(_mediator));
                manager.Attach(new KuryeLogObserver());

                await manager.GonderKuryeAsync(kurye);
                TempData["SuccessMessage"] = "Talebiniz başarıyla alınmıştır";
                return RedirectToAction("Anasayfa");
            }

            return RedirectToAction("AktifKurye");
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

        //CikisYap--------------------------------------------
        public async Task<IActionResult> CikisYap()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Anasayfa", "Giris", null);
        }
        //----------------------------------------------------
    }
}
