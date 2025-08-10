using MediatR;
using Application.Command;
using Domain.Interfaces;

namespace Application.Handle
{
    public class GuncelleKuryeciCommandHandler : IRequestHandler<GuncelleKuryeciCommand, Unit>
    {
        private readonly IKuryeciRepository _repository;

        public GuncelleKuryeciCommandHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(GuncelleKuryeciCommand request, CancellationToken cancellationToken)
        {
            await _repository.UpdateInfo(request.Kuryeci);
            return Unit.Value;
        }
    }
}
