using MediatR;
using Application.Command;
using Domain.Interfaces;

namespace Application.Handle
{
    public class KuryeOlCommandHandler : IRequestHandler<KuryeOlCommand, Unit>
    {
        private readonly IKuryeciRepository _repository;

        public KuryeOlCommandHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(KuryeOlCommand request, CancellationToken cancellationToken)
        {
            await _repository.Add(request.Kuryeci);
            return Unit.Value;
        }
    }
}
