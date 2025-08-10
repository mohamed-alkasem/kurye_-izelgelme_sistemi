using MediatR;

namespace Application.Command
{
    public class RedKuryeciCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public RedKuryeciCommand(int id)
        {
            Id = id;
        }
    }
}
