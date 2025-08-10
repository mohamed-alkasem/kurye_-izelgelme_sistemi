using System.ComponentModel.DataAnnotations;

namespace Web.Models.KuryeciGiris
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Eposta Boş Bırakılamaz!")]
        [DataType(DataType.EmailAddress)]
        public string Eposta { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz!")]
        public string Sifre { get; set; }
    }
}
