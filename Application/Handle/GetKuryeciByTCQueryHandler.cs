using MediatR;
using Application.Query;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Handle
{
    public class GetKuryeciByTCQueryHandler : IRequestHandler<GetKuryeciByTCQuery, Kuryeci>
    {
        private readonly IKuryeciRepository _repository;

        public GetKuryeciByTCQueryHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<Kuryeci> Handle(GetKuryeciByTCQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByTC(request.TC);
        }
    }
}


