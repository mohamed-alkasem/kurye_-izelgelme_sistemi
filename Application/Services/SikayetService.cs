using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class SikayetService : ISikayetService
    {
        private readonly DatabaseContext _databaseContext;

        public SikayetService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        //----------------------------------------------------
        public async Task<List<Sikayet>> GetSikayetler()
        {
            var sikayetler = await _databaseContext.Sikayetler
                .OrderByDescending(x => x.SikayetTarihi)
                .ToListAsync();

            return sikayetler;
        }
        //----------------------------------------------------

        //----------------------------------------------------
        public async Task SikayetGonder(Sikayet sikayet)
        {
            var gonderilecekSikayet = await _databaseContext.Sikayetler.AddAsync(sikayet);
            await _databaseContext.SaveChangesAsync();
        }
        //----------------------------------------------------
    }
}
