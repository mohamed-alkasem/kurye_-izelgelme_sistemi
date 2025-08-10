using System.Text;
using System.Security.Cryptography;

namespace Application.Services
{
    public sealed class SifreUreticiServisi : SifreServisBase
    {
        private static readonly Lazy<SifreUreticiServisi> _instance =
            new Lazy<SifreUreticiServisi>(() => new SifreUreticiServisi());

        private SifreUreticiServisi() { }

        public static SifreUreticiServisi Instance => _instance.Value;

        public string RastgeleSifreUret(int uzunluk = 8)
        {
            const string kucuk = "abcdefghijklmnopqrstuvwxyz";
            const string buyuk = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string rakamlar = "1234567890";
            const string ozel = "!@#$%^&*";
            const string tum = kucuk + buyuk + rakamlar + ozel;

            var sonuc = new StringBuilder();
            using var rastgele = RandomNumberGenerator.Create();

            sonuc.Append(RastgeleKarakterSec(kucuk, rastgele));
            sonuc.Append(RastgeleKarakterSec(buyuk, rastgele));
            sonuc.Append(RastgeleKarakterSec(rakamlar, rastgele));
            sonuc.Append(RastgeleKarakterSec(ozel, rastgele));

            while (sonuc.Length < uzunluk)
            {
                sonuc.Append(RastgeleKarakterSec(tum, rastgele));
            }

            return new string(sonuc.ToString().OrderBy(_ => RastgeleSayi(0, 100, rastgele)).ToArray());
        }
    }
}
