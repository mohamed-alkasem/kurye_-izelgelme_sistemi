using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class AppUser : IdentityUser
    {
        public string TC { get; set; }

        public string AdSoyad { get; set; }

        public string KullaniciRolu { get; set; }
    }
}
