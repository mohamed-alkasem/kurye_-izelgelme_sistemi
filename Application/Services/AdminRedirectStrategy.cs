using Application.Strategies;

namespace Application.Services
{
    public class AdminRedirectStrategy : IRedirectStrategy
    {
        public string GetControllerName() => "Admin";
        public string GetActionName() => "Anasayfa";
    }

    public class KuryeciRedirectStrategy : IRedirectStrategy
    {
        public string GetControllerName() => "Kuryeci";
        public string GetActionName() => "Anasayfa";
    }

    public class KullaniciRedirectStrategy : IRedirectStrategy
    {
        public string GetControllerName() => "Kullanici";
        public string GetActionName() => "Anasayfa";
    }

    public class GirisRedirectStrategy : IRedirectStrategy
    {
        public string GetControllerName() => "Giris";
        public string GetActionName() => "Anasayfa";
    }
}
