using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class KuryeService : IKuryeService
    {
        private readonly DatabaseContext _databaseContext;

        public KuryeService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        //----------------------------------------------------
        public async Task<List<Domain.Entities.Kurye>> GetKuryeler()
        {
            var kuryeler = await _databaseContext.Kuryeler
                .OrderByDescending(x => x.SiparisTarihi)
                .ToListAsync();

            return kuryeler;
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task<List<Domain.Entities.Kurye>> GetKuryelerByKullaniciTC(string kullaniciTC)
        {
            var kuryeler = await _databaseContext.Kuryeler
                .Where(x => x.KullaniciTC == kullaniciTC)
                .OrderBy(x => x.SiparisTarihi)
                .ToListAsync();

            return kuryeler;
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task<List<Domain.Entities.Kurye>> GetPasifKuryelerByKuryeciTC(string kuryeciTC)
        {
            var kuryeler = await _databaseContext.Kuryeler
                .Where(x => x.Durum == "Pasif" && x.KuryeciTC == kuryeciTC)
                .OrderByDescending(x => x.SiparisTarihi)
                .ToListAsync();

            return kuryeler;
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task<List<Domain.Entities.Kurye>> GetAktifKuryelerByKuryeciTC(string kuryeciTC)
        {
            var kuryeler = await _databaseContext.Kuryeler
                .Where(x => x.Durum == "Aktif" && x.KuryeciTC == kuryeciTC)
                .OrderByDescending(x => x.SiparisTarihi)
                .ToListAsync();

            return kuryeler;
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task<List<Domain.Entities.Kurye>> GetKuryelerByKuryeciTC(string kuryeciTC)
        {
            var kuryeler = await _databaseContext.Kuryeler
                .Where(x => x.KuryeciTC == kuryeciTC)
                .OrderByDescending(x => x.SiparisTarihi)
                .ToListAsync();

            return kuryeler;
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task<Domain.Entities.Kurye> GetKuryeById(int kuryeId)
        {
            var kurye = await _databaseContext.Kuryeler.FirstOrDefaultAsync(x => x.Id == kuryeId);

            if (kurye == null)
            {
                return null;
            }
            else
            {
                return kurye;
            }
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task KuryeGonder(Domain.Entities.Kurye kurye)
        {
            var gonderilecekKurye = await _databaseContext.Kuryeler.AddAsync(kurye);
            await _databaseContext.SaveChangesAsync();
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task KuryeOnayla(int kuryeId)
        {
            var onaylanacakKurye = await _databaseContext.Kuryeler.FirstOrDefaultAsync(x => x.Id == kuryeId);

            if (onaylanacakKurye is not null)
            {
                onaylanacakKurye.Durum = "Aktif";

            }
            await _databaseContext.SaveChangesAsync();
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task KuryeTeslimEt(int kuryeId)
        {
            var teslimEdilecekKurye = await _databaseContext.Kuryeler.FirstOrDefaultAsync(x => x.Id == kuryeId);

            if (teslimEdilecekKurye is not null)
            {
                teslimEdilecekKurye.TeslimTarihi = DateTime.Now;
                teslimEdilecekKurye.TeslimDurumu = "Teslim Edildi";
            }
            await _databaseContext.SaveChangesAsync();
        }
        //----------------------------------------------------
    }
}
