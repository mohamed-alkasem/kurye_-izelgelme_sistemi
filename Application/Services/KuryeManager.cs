using Domain.Interfaces;

namespace Application.Services
{
    public class KuryeManager : IKuryeSubject   // Observer Pattern kullandik
    {
        private readonly List<IKuryeObserver> _observers = new();
        private readonly IKuryeService _kuryeService;

        public KuryeManager(IKuryeService kuryeService)
        {
            _kuryeService = kuryeService;
        }

        public void Attach(IKuryeObserver observer) => _observers.Add(observer);
        public void Detach(IKuryeObserver observer) => _observers.Remove(observer);

        public void Notify(Domain.Entities.Kurye kurye)
        {
            foreach (var observer in _observers)
            {
                observer.Update(kurye);
            }
        }

        public async Task GonderKuryeAsync(Domain.Entities.Kurye kurye)
        {
            await _kuryeService.KuryeGonder(kurye);
            Notify(kurye);
        }
    }
}
