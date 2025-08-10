namespace Application.DTOs
{
    public class SiparislerKullaniciListDto
    {
        public string KuryeciTel { get; set; }

        public int KuryeId { get; set; }

        public string KuryeDurum { get; set; }

        public string KuryeTeslimDurumu { get; set; }

        public DateTime SiparisTarihi { get; set; }

        public string KuryeciTC { get; set; }

        public string KullaniciTC { get; set; }
    }
}
