using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiPIA.DTOs;

namespace WebApiPIA.Servicios
{
    public class HashService
    {
        public ResultadoHash Hash(string textoPlano)
        {
            var seed = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(seed);
            }

            return Hash(textoPlano, seed);
        }

        public ResultadoHash Hash(string textoPlano, byte[] seed)
        {
            var llaveDerivada = KeyDerivation.Pbkdf2(password: textoPlano,
                salt: seed, prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);

            var hash = Convert.ToBase64String(llaveDerivada);

            return new ResultadoHash()
            {
                Hash = hash,
                Seed = seed
            };
        }
    }
}
