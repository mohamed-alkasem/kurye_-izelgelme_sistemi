using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class KuryeLogObserver : IKuryeObserver  // Observer Pattern kullandik
    {
        public async Task Update(Domain.Entities.Kurye kurye)
        {
            Console.WriteLine($"📄 Talep {kurye.KuryeciTC} numaralı kuryeye {DateTime.Now} tarihinde kaydedildi.");
        }
    }
}
