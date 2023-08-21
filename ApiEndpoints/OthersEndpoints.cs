using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace CatalogoAPI.ApiEndpoints
{
    public static class OthersEndpoints
    {
        public static void GetStringForJWT(this WebApplication app)
        {
            app.MapGet("/generate-kay", async () =>
            {
                string getString = "";
                GenerateKay();

                ActionResult<string> GenerateKay()
                {
                    int keyLength = 32; // Tamanho da chave em bytes (256 bits)
                    byte[] keyBytes = new byte[keyLength];
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(keyBytes);
                    }
                    string jwtSecretKey = Convert.ToBase64String(keyBytes);
                    getString = jwtSecretKey;

                    return jwtSecretKey;
                }

                return Results.Ok(getString);
            });
        }
    }
}
