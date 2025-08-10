using MediatR;
using Domain.Entities;

namespace Application.Query
{
    public class GetAllKuryecilerQuery : IRequest<List<Kuryeci>> { }
}
