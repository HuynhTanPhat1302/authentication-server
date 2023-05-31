
using System.Security.Cryptography;


namespace Authentication_Authorization_1._0.Helpers
{

    public static class SecretKey
    {
        public static string GenerateRandomSecretKey(int length)
        {
            var randomNumber = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }
    }
}
    
