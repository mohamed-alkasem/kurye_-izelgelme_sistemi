using MediatR;
using Domain.Entities;

namespace Application.Command
{
    public class GuncelleKuryeciCommand : IRequest<Unit>
    {
        public Kuryeci Kuryeci { get; set; }

        public GuncelleKuryeciCommand(Kuryeci kuryeci)
        {
            Kuryeci = kuryeci;
        }
    }
}
