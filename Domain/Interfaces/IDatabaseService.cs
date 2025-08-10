using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDatabaseService // Adapter pattern burada uygulanmıştır.
    {
        Task<List<Kullanici>> GetKullanicilar();
        Task<Kullanici> GetKullaniciByTC(string tc);
        Task KullaniciEkle(Kullanici kullanici);
        Task KullaniciGuncelle(Kullanici kullanici);
        Task<bool> TcVarMi(string tc);
    }
}
