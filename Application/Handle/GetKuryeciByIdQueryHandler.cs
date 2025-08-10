using MediatR;
using Application.Query;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Handle
{
    public class GetKuryeciByIdQueryHandler : IRequestHandler<GetKuryeciByIdQuery, Kuryeci>
    {
        private readonly IKuryeciRepository _repository;

        public GetKuryeciByIdQueryHandler(IKuryeciRepository repository)
        {
            _repository = repository;
        }

        public async Task<Kuryeci> Handle(GetKuryeciByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetById(request.Id);
        }
    }
}
