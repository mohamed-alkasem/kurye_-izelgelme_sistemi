using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Sikayet
    {
        private int _id;
        private string _adSoyad;
        private string _tel;
        private string _sikayetMesaji;
        private DateTime _sikayetTarihi;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Required(ErrorMessage = "Ad ve Soyad Boş Bırakılamaz!")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "karakter sayısı 6-20 arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-ZğüşöçıİĞÜŞÖÇ\s]+$", ErrorMessage = "Ad Soyad alanında sadece harf ve boşluk kullanılabilir.")]
        public string AdSoyad
        {
            get => _adSoyad;
            set => _adSoyad = value;
        }

        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Telefon numarası 11 haneli olmalıdır.")]
        [RegularExpression(@"^0\d+$", ErrorMessage = "Telefon numarası 0 ile başlamalı ve sadece rakamlardan oluşmalıdır.")]
        public string Tel
        {
            get => _tel;
            set => _tel = value;
        }

        [Required(ErrorMessage = "Mesaj Boş Bırakılamaz!")]
        public string SikayetMesaji
        {
            get => _sikayetMesaji;
            set => _sikayetMesaji = value;
        }

        public DateTime SikayetTarihi
        {
            get => _sikayetTarihi;
            set => _sikayetTarihi = value;
        }
    }
}
