using CatalogoAPI.Models;

namespace CatalogoAPI.Services
{
    public interface ITokenService
    {
        string GetToken(string key, string issuer, string audience, UserModel model);
    }
}
