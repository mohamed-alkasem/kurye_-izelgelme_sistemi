using MediatR;
using Domain.Interfaces;
using Domain.Entities;
using Application.Query;

namespace Application.Handle
{
    public class GetAllKuryecilerQueryHandler : IRequestHandler<GetAllKuryecilerQuery, List<Kuryeci>>
    {
        private readonly IKuryeciRepository _repository;

        public GetAllKuryecilerQueryHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Kuryeci>> Handle(GetAllKuryecilerQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllOrderedByIstekTarihi();
        }
    }
}


