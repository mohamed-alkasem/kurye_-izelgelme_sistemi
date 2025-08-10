namespace Application.DTOs
{
    public class SiparislerAdminListDto
    {
        public int SiparisId { get; set; }
        public string KullaniciAdSoyad { get; set; }
        public string KuryeciAdSoyad { get; set; }
        public string UrunAdi { get; set; }
        public string Durum { get; set; }
        public string TeslimDurumu { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public DateTime TeslimTarihi { get; set; }
    }
}
