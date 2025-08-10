using MediatR;

namespace Application.Command
{
    public class KabulKuryeciCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public KabulKuryeciCommand(int id)
        {
            Id = id;
        }
    }
}
