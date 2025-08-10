using MediatR;
using Domain.Entities;
using Domain.Interfaces;
using Application.Query;
using Application.Command;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Kurye.Web.Controllers
{
    [Authorize(Roles = "Kuryeci")]
    public class KuryeciController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IKuryeService _kuryeService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public KuryeciController(IKuryeService kuryeService, SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager, IMediator mediator)
        {
            _mediator = mediator;
            _userManager = userManager;
            _kuryeService = kuryeService;
            _signInManager = signInManager;
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
            var kurye = await _kuryeService.GetKuryeById(kuryeId);
            ViewBag.Kurye = kuryeci;
            ViewBag.ToUserTC = kurye.KullaniciTC;
            ViewBag.CurrentUserTC = kuryeci.TC;
            ViewBag.CurrentUserName = kuryeci.AdSoyad;
            return View();
        }
        //----------------------------------------------------

        //PasifSiparislerKuryeci------------------------------
        public async Task<IActionResult> PasifSiparislerKuryeci()
        {
            var user = await _userManager.GetUserAsync(User);

            var kuryeler = await _kuryeService.GetPasifKuryelerByKuryeciTC(user.TC);
            return View(kuryeler);
        }
        //----------------------------------------------------

        //SiparisDetay----------------------------------------
        public async Task<IActionResult> SiparisDetay(int kuryeId)
        {
            var kurye = await _kuryeService.GetKuryeById(kuryeId);
            return View(kurye);
        }

        [HttpPost]
        public async Task<IActionResult> SiparisDetay(Domain.Entities.Kurye model)
        {
            var user = await _userManager.GetUserAsync(User);
            model.KuryeciTC = user.TC;
            await _kuryeService.KuryeOnayla(model.Id);
            return RedirectToAction("PasifSiparislerKuryeci");
        }
        //----------------------------------------------------

        //SiparislerimKuryeci---------------------------------
        public async Task<IActionResult> SiparislerimKuryeci()
        {
            var user = await _userManager.GetUserAsync(User);
            var kuryeler = await _kuryeService.GetAktifKuryelerByKuryeciTC(user.TC);
            return View(kuryeler);
        }
        //----------------------------------------------------

        //SiparisTeslimEt-------------------------------------
        public async Task<IActionResult> SiparisTeslimEt(int kuryeId)
        {
            await _kuryeService.KuryeTeslimEt(kuryeId);
            return RedirectToAction("SiparislerimKuryeci");
        }
        //----------------------------------------------------

        //TeslimEdilenSiparisDetay----------------------------
        public async Task<IActionResult> TeslimEdilenSiparisDetay(int kuryeId)
        {
            var kurye = await _kuryeService.GetKuryeById(kuryeId);
            return View(kurye);
        }
        //----------------------------------------------------

        //Profil----------------------------------------------
        public async Task<IActionResult> Profil(string kuryeciTC)
        {
            var kuryeci = await _mediator.Send(new GetKuryeciByTCQuery(kuryeciTC));
            if (kuryeci == null) return NotFound();
            return View(kuryeci);
        }

        [HttpPost]
        public async Task<IActionResult> Profil(Kuryeci model)
        {
            ModelState.Remove("clientFile");
            ModelState.Remove("Image");
            ModelState.Remove("Cinsiyet");
            ModelState.Remove("IkametIl");
            ModelState.Remove("IkametIlce");
            ModelState.Remove("IkametMahalle");
            ModelState.Remove("IkametAdres");
            ModelState.Remove("CalisacakIlce");
            ModelState.Remove("EhliyetTipi");
            ModelState.Remove("KabulDurumu");
            ModelState.Remove("IzinGunu");

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
                        var kuryeci = await _mediator.Send(new GetKuryeciByTCQuery(model.TC));
                        if (kuryeci is not null)
                        {
                            if (kuryeci.Yas == model.Yas && kuryeci.Tel == model.Tel && kuryeci.Image == model.Image)
                            {
                                ModelState.AddModelError("", "Girilen bilgiler zaten mevcut. Lütfen değişiklik yapın.");
                                return View(model);
                            }
                            else
                            {
                                await _mediator.Send(new GuncelleKuryeciCommand(model));
                                return RedirectToAction("Profil", "Kuryeci", new { kuryeciTC = model.TC });
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
                        var kuryeci = await _mediator.Send(new GetKuryeciByTCQuery(model.TC));
                        if (kuryeci is not null)
                        {
                            if (kuryeci.Yas == model.Yas && kuryeci.Tel == model.Tel)
                            {
                                ModelState.AddModelError("", "Girilen bilgiler zaten mevcut. Lütfen değişiklik yapın.");
                                return View(model);
                            }
                            else
                            {
                                model.Image = resim;
                                await _mediator.Send(new GuncelleKuryeciCommand(model));
                                return RedirectToAction("Profil", "Kuryeci", new { kuryeciTC = model.TC });
                            }
                        }
                    }
                }
            }
            return View(model);
        }
        //----------------------------------------------------

        //Hakkimizda------------------------------------------
        public IActionResult Hakkimizda()
        {
            return View();
        }
        //----------------------------------------------------

        //Hakedis---------------------------------------------
        public IActionResult Hakedis()
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
