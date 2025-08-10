using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SqlDatabaseService : IDatabaseService // Adapter pattern burada uygulanmıştır.
    {
        private readonly DatabaseContext _context;

        public SqlDatabaseService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Kullanici>> GetKullanicilar()
        {
            var kullanicilar = await _context.Kullanicilar.ToListAsync();

            return kullanicilar;
        }

        public async Task<Kullanici> GetKullaniciByTC(string tc)
        {
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(x => x.TC == tc);

            if (kullanici == null)
            {
                return null;
            }
            else
            {
                return kullanici;
            }
        }

        public async Task KullaniciEkle(Kullanici kullanici)
        {
            await _context.Kullanicilar.AddAsync(kullanici);
            await _context.SaveChangesAsync();
        }

        public async Task KullaniciGuncelle(Kullanici kullanici)
        {
            var guncellenecek = await _context.Kullanicilar.FirstOrDefaultAsync(x => x.Id == kullanici.Id);
            if (guncellenecek != null)
            {
                guncellenecek.Tel = kullanici.Tel;
                guncellenecek.Eposta = kullanici.Eposta;
                guncellenecek.Adres = kullanici.Adres;
                guncellenecek.Image = kullanici.Image;
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<bool> TcVarMi(string tc)
        {
            var kuryeciVar = await _context.Kuryeciler.AnyAsync(k => k.TC == tc && k.KabulDurumu != "Red");
            var kullaniciVar = await _context.Kullanicilar.AnyAsync(k => k.TC == tc);

            return kuryeciVar || kullaniciVar;
        }
    }
}
