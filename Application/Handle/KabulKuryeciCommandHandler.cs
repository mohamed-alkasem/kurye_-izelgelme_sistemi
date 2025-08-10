using MediatR;
using Application.Command;
using Domain.Interfaces;

namespace Application.Handle
{
    public class KabulKuryeciCommandHandler : IRequestHandler<KabulKuryeciCommand, Unit>
    {
        private readonly IKuryeciRepository _repository;

        public KabulKuryeciCommandHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(KabulKuryeciCommand request, CancellationToken cancellationToken)
        {
            await _repository.MarkAsAccepted(request.Id);
            return Unit.Value;
        }
    }
}
