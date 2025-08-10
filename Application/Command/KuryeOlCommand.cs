using MediatR;
using Domain.Entities;

namespace Application.Command
{
    public class KuryeOlCommand : IRequest<Unit>
    {
        public Kuryeci Kuryeci { get; set; }

        public KuryeOlCommand(Kuryeci kuryeci)
        {
            Kuryeci = kuryeci;
        }
    }
}
