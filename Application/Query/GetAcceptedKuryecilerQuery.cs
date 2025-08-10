using MediatR;
using Domain.Entities;

namespace Application.Query
{
    public class GetAcceptedKuryecilerQuery : IRequest<List<Kuryeci>> { }
}
