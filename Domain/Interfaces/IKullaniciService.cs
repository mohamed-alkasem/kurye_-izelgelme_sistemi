using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IKullaniciService
    {
        Task<List<Kullanici>> GetKullanicilar();

        Task<Kullanici> GetKullaniciByTC(string kullaniciTC);

        Task KullaniciEkle(Kullanici kullanici);

        Task KullaniciGuncelle(Kullanici kullanici);

        Task<bool> TcVarMi(string tc);
    }
}
