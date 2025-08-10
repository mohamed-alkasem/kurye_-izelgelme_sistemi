using System.ComponentModel.DataAnnotations;

namespace Web.Models.KullaniciGiris
{
    public class UserLogin
    {
        [Required(ErrorMessage = "TC Kimlik Numarası Boş Bırakılamaz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik Numarası 11 haneli olmalıdır.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "TC Kimlik Numarası sadece rakamlardan oluşmalıdır.")]
        public string TC { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz!")]
        public string Sifre { get; set; }
    }
}
