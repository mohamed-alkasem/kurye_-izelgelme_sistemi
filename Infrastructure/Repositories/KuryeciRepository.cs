using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class KuryeciRepository : IKuryeciRepository
    {
        private readonly DatabaseContext _context;

        public KuryeciRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Kuryeci>> GetAllOrderedByIstekTarihi() 
            => await _context.Kuryeciler.OrderByDescending(x => x.IstekTarihi).ToListAsync();

        public async Task<Kuryeci> GetByTC(string tc) 
            => await _context.Kuryeciler.FirstOrDefaultAsync(x => x.TC == tc);

        public async Task<List<Kuryeci>> GetAccepted() 
            => await _context.Kuryeciler.Where(x => x.KabulDurumu == "Kabul").OrderByDescending(x => x.AdSoyad).ToListAsync();

        public async Task<Kuryeci> GetById(int id) 
            => await _context.Kuryeciler.FirstOrDefaultAsync(x => x.Id == id);

        public async Task Add(Kuryeci entity) 
        { 
            await _context.Kuryeciler.AddAsync(entity); await _context.SaveChangesAsync(); 
        }

        public async Task MarkAsRejected(int id) 
        { 
            var k = await GetById(id); if (k != null) 
            { 
                k.KabulDurumu = "Red"; k.RedTarihi = DateTime.Now; await _context.SaveChangesAsync(); 
            } 
        }

        public async Task MarkAsAccepted(int id) 
        { 
            var k = await GetById(id); if (k != null) 
            { k.KabulDurumu = "Kabul"; k.KabulTarihi = DateTime.Now; await _context.SaveChangesAsync(); 
            } 
        }

        public async Task UpdateInfo(Kuryeci model) 
        { 
            var k = await GetById(model.Id); if (k != null) 
            { 
                k.Tel = model.Tel; k.Yas = model.Yas; k.Image = model.Image; await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailExists(string email) 
            => await _context.Kuryeciler.AnyAsync(k => k.Eposta.ToLower() == email.ToLower() && k.KabulDurumu != "Red");

        public async Task<bool> TCExists(string tc) 
            => await _context.Kuryeciler.AnyAsync(k => k.TC == tc && k.KabulDurumu != "Red") || await _context.Kullanicilar.AnyAsync(k => k.TC == tc);
    }
}
