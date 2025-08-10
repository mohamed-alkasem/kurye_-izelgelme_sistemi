using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Kuryeci
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "TC Kimlik Numarası Boş Bırakılamaz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik Numarası 11 haneli olmalıdır.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "TC Kimlik Numarası sadece rakamlardan oluşmalıdır.")]
        public string TC { get; set; }

        [Required(ErrorMessage = "Ad ve Soyad Boş Bırakılamaz!")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "karakter sayısı 6-20 arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$", ErrorMessage = "Ad Soyad alanında sadece harf ve boşluk kullanılabilir.")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Yaş Boş Bırakılamaz!")]
        public int Yas { get; set; }

        [Required(ErrorMessage = "Cinsiyet Bırakılamaz!")]
        public string Cinsiyet { get; set; }

        [Required(ErrorMessage = "İkamet İl Boş Bırakılamaz!")]
        public string IkametIl { get; set; }

        [Required(ErrorMessage = "İkamet İlce Boş Bırakılamaz!")]
        public string IkametIlce { get; set; }

        [Required(ErrorMessage = "İkamet Mahalle Boş Bırakılamaz!")]
        public string IkametMahalle { get; set; }

        [Required(ErrorMessage = "İkamet Adresi Boş Bırakılamaz!")]
        public string IkametAdres { get; set; }

        [Required(ErrorMessage = "Çalişacak İlce Boş Bırakılamaz!")]
        public string CalisacakIlce { get; set; }

        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Telefon numarası 11 haneli olmalıdır.")]
        [RegularExpression(@"^0\d+$", ErrorMessage = "Telefon numarası 0 ile başlamalı ve sadece rakamlardan oluşmalıdır.")]
        public string Tel { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "E-posta Boş Bırakılamaz!")]
        public string Eposta { get; set; }

        [Required(ErrorMessage = "Ehliyet Boş Bırakılamaz!")]
        public string EhliyetTipi { get; set; }

        public string KabulDurumu { get; set; }

        public string ReddetmeSebebi { get; set; } = "Varsayılan Değer";

        [Required(ErrorMessage = "İzin Boş Bırakılamaz!")]
        public string IzinGunu { get; set; }

        public DateTime IstekTarihi { get; set; } = DateTime.Now;

        public DateTime KabulTarihi { get; set; } = DateTime.Now;

        public DateTime RedTarihi { get; set; } = DateTime.Now;

        public byte[] Image { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Resim Seçin!")]
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
                    return string.Empty;
                }
            }
        }
    }
}
