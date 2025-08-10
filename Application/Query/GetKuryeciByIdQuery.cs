using MediatR;
using Domain.Entities;
namespace Application.Query
{
    public class GetKuryeciByIdQuery : IRequest<Kuryeci>
    {
        public int Id { get; set; }

        public GetKuryeciByIdQuery(int id)
        {
            Id = id;
        }
    }
}


