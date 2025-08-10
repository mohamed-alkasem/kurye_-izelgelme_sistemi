using MediatR;
using Application.Query;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Handle
{
    public class GetAcceptedKuryecilerQueryHandler : IRequestHandler<GetAcceptedKuryecilerQuery, List<Kuryeci>>
    {
        private readonly IKuryeciRepository _repository;

        public GetAcceptedKuryecilerQueryHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Kuryeci>> Handle(GetAcceptedKuryecilerQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAccepted();
        }
    }
}
