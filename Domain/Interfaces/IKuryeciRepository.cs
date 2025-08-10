using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IKuryeciRepository
    {
        Task<List<Kuryeci>> GetAllOrderedByIstekTarihi();

        Task<Kuryeci> GetByTC(string tc);

        Task<List<Kuryeci>> GetAccepted();

        Task<Kuryeci> GetById(int id);

        Task Add(Kuryeci kuryeci);

        Task MarkAsRejected(int id);

        Task MarkAsAccepted(int id);

        Task UpdateInfo(Kuryeci kuryeci);

        Task<bool> EmailExists(string email);

        Task<bool> TCExists(string tc);
    }
}
