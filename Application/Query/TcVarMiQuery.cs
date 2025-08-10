using MediatR;

namespace Application.Query
{
    public class TcVarMiQuery : IRequest<bool>
    {
        public string Tc { get; set; }

        public TcVarMiQuery(string tc)
        {
            Tc = tc;
        }
    }
}
