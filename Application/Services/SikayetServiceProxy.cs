using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class SikayetServiceProxy : ISikayetService // Proxy pattern burada uygulanmıştır.
    {       
        private readonly ISikayetService _realService;
        private readonly ILogger<SikayetServiceProxy> _logger;

        public SikayetServiceProxy(ISikayetService realService, ILogger<SikayetServiceProxy> logger)
        {
            _logger = logger;
            _realService = realService;
        }

        public  async Task<List<Sikayet>> GetSikayetler()
        {
            _logger.LogInformation("GetSikayetler çağrıldı.");
            var result = await _realService.GetSikayetler();
            _logger.LogInformation($"{result.Count} adet şikayet getirildi.");
            return result;
        }

        public  async Task SikayetGonder(Sikayet sikayet)
        {
            _logger.LogInformation($"Yeni şikayet gönderiliyor: {sikayet.AdSoyad} - {sikayet.Tel}");
            await _realService.SikayetGonder(sikayet);
            _logger.LogInformation("Şikayet başarıyla gönderildi.");
        }
    }
}
