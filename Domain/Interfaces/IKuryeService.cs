namespace Domain.Interfaces
{
    public interface IKuryeService
    {
        Task<List<Entities.Kurye>> GetKuryeler();

        Task<List<Entities.Kurye>> GetKuryelerByKullaniciTC(string kullaniciTC);

        Task<List<Entities.Kurye>> GetPasifKuryelerByKuryeciTC(string kuryeciTC);

        Task<List<Entities.Kurye>> GetAktifKuryelerByKuryeciTC(string kuryeciTC);

        Task<List<Entities.Kurye>> GetKuryelerByKuryeciTC(string kuryeciTC);      

        Task<Entities.Kurye> GetKuryeById(int kuryeId);

        Task KuryeGonder(Entities.Kurye kurye);

        Task KuryeOnayla(int kuryeId);

        Task KuryeTeslimEt(int kuryeId);
    }
}
