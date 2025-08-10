namespace Application.Strategies
{
    public interface IRedirectStrategy
    {
        string GetControllerName();
        string GetActionName();
    }
}
