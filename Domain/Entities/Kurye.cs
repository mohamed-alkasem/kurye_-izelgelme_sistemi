using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Kurye
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad ve Soyad Boş Bırakılamaz!")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "karakter sayısı 6-20 arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$", ErrorMessage = "Ad Soyad alanında sadece harf ve boşluk kullanılabilir.")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Telefon numarası 11 haneli olmalıdır.")]
        [RegularExpression(@"^0\d+$", ErrorMessage = "Telefon numarası 0 ile başlamalı ve sadece rakamlardan oluşmalıdır.")]
        public string Tel { get; set; }

        [Required(ErrorMessage = "İl Boş Bırakılamaz!")]
        public string Il { get; set; }

        [Required(ErrorMessage = "İlçe Boş Bırakılamaz!")]
        public string Ilce { get; set; }

        [Required(ErrorMessage = "Mahalle Boş Bırakılamaz!")]
        public string Mahalle { get; set; }

        [Required(ErrorMessage = "Adres Boş Bırakılamaz!")]
        public string Adres { get; set; }

        [Required(ErrorMessage = "Market Boş Bırakılamaz!")]
        public string Market { get; set; }

        [Required(ErrorMessage = "Bu Alan Boş Bırakılamaz!")]
        public string UrunAdi { get; set; }

        public string Not { get; set; }

        public string Durum { get; set; }

        public string TeslimDurumu { get; set; }

        public string KuryeciTC { get; set; }

        public string KullaniciTC { get; set; }

        public DateTime SiparisTarihi { get; set; } = DateTime.Now;

        public DateTime TeslimTarihi { get; set; } = DateTime.Now;
    }
}
