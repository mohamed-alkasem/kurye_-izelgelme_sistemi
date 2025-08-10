using MediatR;

namespace Application.Query
{
    public class EpostaVarMiQuery : IRequest<bool>
    {
        public string Eposta { get; set; }

        public EpostaVarMiQuery(string eposta)
        {
            Eposta = eposta;
        }
    }
}
