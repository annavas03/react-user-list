using app.Server.Data.Entities.Identity;


namespace app.Server.Identity
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(UserEntity user);
    }
}
