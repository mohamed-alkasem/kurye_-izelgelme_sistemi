using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class DatabaseServiceAdapter : IKullaniciService 
    {
        private readonly IDatabaseService _databaseService;

        public DatabaseServiceAdapter(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Task<List<Kullanici>> GetKullanicilar()
            => _databaseService.GetKullanicilar();

        public Task<Kullanici> GetKullaniciByTC(string tc)
            => _databaseService.GetKullaniciByTC(tc);

        public Task KullaniciEkle(Kullanici kullanici)
            => _databaseService.KullaniciEkle(kullanici);

        public Task KullaniciGuncelle(Kullanici kullanici)
            => _databaseService.KullaniciGuncelle(kullanici);

        public Task<bool> TcVarMi(string tc)
            => _databaseService.TcVarMi(tc);
    }
}
