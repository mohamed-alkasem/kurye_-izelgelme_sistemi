namespace Domain.Interfaces
{
    public interface IKuryeObserver  // Observer Pattern kullandik
    {
        Task Update(Entities.Kurye kurye);
    }

    public interface IKuryeSubject
    {
        void Attach(IKuryeObserver observer);

        void Detach(IKuryeObserver observer);

        void Notify(Entities.Kurye kurye);
    }
}
