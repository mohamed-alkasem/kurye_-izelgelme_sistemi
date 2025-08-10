using System.ComponentModel.DataAnnotations;

namespace Web.Models.KuryeciGiris
{
    public class SifreUnuttum
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "E-posta Boş Bırakılamaz!")]
        public string Eposta { get; set; }
    }
}
