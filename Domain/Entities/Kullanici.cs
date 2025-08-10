using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Kullanici
    {
        public int Id { get; set; }

        public string TC { get; set; }

        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Telefon numarası 11 haneli olmalıdır.")]
        [RegularExpression(@"^0\d+$", ErrorMessage = "Telefon numarası 0 ile başlamalı ve sadece rakamlardan oluşmalıdır.")]
        public string Tel { get; set; }

        public string Adres { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Eposta { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public byte[] Image { get; set; }

        [NotMapped]
        public IFormFile clientFile { get; set; }

        [NotMapped]
        public string ImageSrc
        {
            get
            {
                if (Image != null)
                {
                    string base64String = Convert.ToBase64String(Image, 0, Image.Length);
                    return "data:image/jpg;base64," + base64String;
                }
                else
                {
                    return "~/imgs/person.jpg";
                }
            }
        }

    }
}
