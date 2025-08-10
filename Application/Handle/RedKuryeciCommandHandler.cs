using MediatR;
using Application.Command;
using Domain.Interfaces;

namespace Application.Handle
{
    public class RedKuryeciCommandHandler : IRequestHandler<RedKuryeciCommand, Unit>
    {
        private readonly IKuryeciRepository _repository;

        public RedKuryeciCommandHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RedKuryeciCommand request, CancellationToken cancellationToken)
        {
            await _repository.MarkAsRejected(request.Id);
            return Unit.Value;
        }
    }
}
