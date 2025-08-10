using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ISikayetService
    {
        Task<List<Sikayet>> GetSikayetler();

        Task SikayetGonder(Sikayet sikayet);
    }
}
