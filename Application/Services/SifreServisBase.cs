using System.Security.Cryptography;

namespace Application.Services
{
    public class SifreServisBase
    {
        protected char RastgeleKarakterSec(string karakterler, RandomNumberGenerator rng)
        {
            byte[] dizi = new byte[1];
            char secilen;
            do
            {
                rng.GetBytes(dizi);
                secilen = karakterler[dizi[0] % karakterler.Length];
            } while (!karakterler.Contains(secilen));
            return secilen;
        }

        protected int RastgeleSayi(int min, int max, RandomNumberGenerator rng)
        {
            byte[] dizi = new byte[4];
            rng.GetBytes(dizi);
            int sonuc = Math.Abs(BitConverter.ToInt32(dizi, 0));
            return min + (sonuc % (max - min));
        }
    }
}