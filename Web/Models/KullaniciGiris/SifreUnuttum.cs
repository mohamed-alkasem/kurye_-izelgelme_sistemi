using System.ComponentModel.DataAnnotations;

namespace Web.Models.KullaniciGiris
{
    public class SifreUnuttum
    {
        [Required(ErrorMessage = "TC Kimlik Numarası Boş Bırakılamaz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik Numarası 11 haneli olmalıdır.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "TC Kimlik Numarası sadece rakamlardan oluşmalıdır.")]
        public string TC { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni şifre boş bırakılamaz.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Şifre uzunluğu 8 ile 20 karakter arasında olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,20}$", ErrorMessage = "Şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir.")]
        public string YeniSifre { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre tekrarı gereklidir.")]
        [Compare("YeniSifre", ErrorMessage = "Şifreler birbiriyle eşleşmiyor.")]
        public string SifreTekrar { get; set; }
    }
}
