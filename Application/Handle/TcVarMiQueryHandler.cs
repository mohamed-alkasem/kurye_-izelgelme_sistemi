using MediatR;
using Application.Query;
using Domain.Interfaces;

namespace Application.Handle
{
    public class TcVarMiQueryHandler : IRequestHandler<TcVarMiQuery, bool>
    {
        private readonly IKuryeciRepository _repository;

        public TcVarMiQueryHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(TcVarMiQuery request, CancellationToken cancellationToken)
        {
            return await _repository.TCExists(request.Tc);
        }
    }
}
