namespace Backend_CRUD.Application.Services
{
    public interface ITokenBlacklistService
    {
        void RevokeToken(string tokenId);
        bool IsTokenRevoked(string tokenId);
    }

    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly HashSet<string> _revokedTokens = new();
        private readonly object _lock = new();

        public void RevokeToken(string tokenId)
        {
            lock (_lock)
            {
                _revokedTokens.Add(tokenId);
            }
        }

        public bool IsTokenRevoked(string tokenId)
        {
            lock (_lock)
            {
                return _revokedTokens.Contains(tokenId);
            }
        }
    }
}