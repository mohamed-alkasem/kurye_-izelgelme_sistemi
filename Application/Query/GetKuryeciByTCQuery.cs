using MediatR;
using Domain.Entities;

namespace Application.Query
{
    public class GetKuryeciByTCQuery : IRequest<Kuryeci>
    {
        public string TC { get; set; }

        public GetKuryeciByTCQuery(string tc)
        {
            TC = tc;
        }
    }
}
