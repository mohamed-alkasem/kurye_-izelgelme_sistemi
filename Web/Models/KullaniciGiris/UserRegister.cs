using System.ComponentModel.DataAnnotations;

namespace Web.Models.KullaniciGiris
{
    public class UserRegister
    {
        [Required(ErrorMessage = "TC Kimlik Numarası Boş Bırakılamaz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik Numarası 11 haneli olmalıdır.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "TC Kimlik Numarası sadece rakamlardan oluşmalıdır.")]
        public string TC { get; set; }

        [Required(ErrorMessage = "Ad ve Soyad Boş Bırakılamaz!")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "karakter sayısı 6-20 arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$", ErrorMessage = "Ad Soyad alanında sadece harf ve boşluk kullanılabilir.")]
        public string AdSoyad { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz!")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Şifre uzunluğu 8 ile 20 karakter arasında olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,20}$", ErrorMessage = "Şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir.")]
        public string Sifre { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Required(ErrorMessage = "Şifre Tekrarı Boş Bırakılamaz!")]
        [Compare("Sifre", ErrorMessage = "Şifreler birbiriyle eşleşmiyor.")]
        public string SifreTekrar { get; set; }
    }
}
