using Application.Strategies;

namespace Application.Services
{
    public class RedirectStrategyContext
    {
        private readonly Dictionary<string, IRedirectStrategy> _strategies;

        public RedirectStrategyContext()
        {
            _strategies = new Dictionary<string, IRedirectStrategy>(StringComparer.OrdinalIgnoreCase)
            {
                { "Admin", new AdminRedirectStrategy() },
                { "Kuryeci", new KuryeciRedirectStrategy() },
                { "Kullanici", new KullaniciRedirectStrategy() }
            };
        }

        public IRedirectStrategy GetStrategy(string? role)
        {
            if (role != null && _strategies.TryGetValue(role, out var strategy))
                return strategy;

            return new GirisRedirectStrategy();
        }
    }
}
