using MediatR;
using Application.Query;
using Domain.Interfaces;

namespace Application.Handle
{
    public class EpostaVarMiQueryHandler : IRequestHandler<EpostaVarMiQuery, bool>
    {
        private readonly IKuryeciRepository _repository;

        public EpostaVarMiQueryHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(EpostaVarMiQuery request, CancellationToken cancellationToken)
        {
            return await _repository.EmailExists(request.Eposta);
        }
    }
}
