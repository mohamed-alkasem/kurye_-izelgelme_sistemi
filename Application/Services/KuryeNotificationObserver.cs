using MediatR;
using Application.Query;
using Domain.Interfaces;

namespace Application.Services
{
    public class KuryeNotificationObserver : IKuryeObserver
    {
        private readonly IMediator _mediator;

        public KuryeNotificationObserver(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Update(Domain.Entities.Kurye kurye)
        {
            var kuryeci = await _mediator.Send(new GetKuryeciByTCQuery(kurye.KuryeciTC));
            if (kuryeci != null)
            {
                Console.WriteLine($"� Kurye {kuryeci.AdSoyad}'a bildirim: Yeni bir sipariş verildi.");
            }
        }
    }
}
