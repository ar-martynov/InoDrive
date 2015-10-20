using System.Threading.Tasks;
using System.Collections.Generic;

using Domain.Entities;

namespace Domain.Abstract.EntityRepositories
{
    public interface IAuthRepository
    {
        Task<bool> AddRefreshToken(RefreshToken token);
        Task<bool> RemoveRefreshToken(string refreshTokenId);
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        List<RefreshToken> GetAllRefreshTokens();
        Client FindClient(string clientId);
    }
}
