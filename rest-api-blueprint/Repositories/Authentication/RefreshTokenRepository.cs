using rest_api_blueprint.DbContexts;
using rest_api_blueprint.Entities.Authentication;
using rest_api_blueprint.Entities.Identity;
using rest_api_blueprint.Models.Authentication;
using rest_api_blueprint.StaticServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api_blueprint.Repositories.Authentication
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly MainDbContext _dbContext;
        private readonly DbSet<RefreshToken> _refreshTokens;
        private readonly IOptions<JwtTokenOptions> _options;


        public RefreshTokenRepository(MainDbContext context, IOptions<JwtTokenOptions> options)
        {
            _dbContext = context;
            _options = options;
            _refreshTokens = context.Authentication_RefreshTokens;
        }


        public async Task<RefreshToken> GetNewRefreshToken(User user, string ip)
        {
            await DeleteExpiredRefreshTokens(user, false);

            RefreshToken refreshToken = GenerateRefreshToken(user, ip);
            await _refreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return refreshToken;
        }


        public async Task<RefreshToken?> GetRefreshToken(RefreshTokenRequest requestRefreshToken, User user)
        {
            IQueryable<RefreshToken> collection = _refreshTokens as IQueryable<RefreshToken>;
            RefreshToken? refreshToken = await collection.FirstOrDefaultAsync(rt => rt.Token == requestRefreshToken.RefreshToken && rt.UserId == user.Id);

            return refreshToken;
        }


        public async Task DeleteRefreshToken(RefreshToken refreshToken)
        {
            _refreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();
        }


        public async Task DeleteExpiredRefreshTokens(User user, bool saveChanges = true)
        {
            IQueryable<RefreshToken> collection = _refreshTokens as IQueryable<RefreshToken>;
            List<RefreshToken> expiredRefreshTokens = await collection.Where(rt => rt.UserId == user.Id && rt.ExpiresAt <= DateTimeOffset.UtcNow.Date).ToListAsync();
            if (expiredRefreshTokens.Count > 0)
            {
                _refreshTokens.RemoveRange(expiredRefreshTokens);
                if (saveChanges)
                    await _dbContext.SaveChangesAsync();
            }
        }

        private RefreshToken GenerateRefreshToken(User user, string ip)
        {
            string tokenString = TextService.GenerateToken();

            return new RefreshToken
            {
                Token = tokenString,
                UserId = user.Id,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(_options.Value.RefreshTTL),
                IpAddress = ip
            };
        }
    }
}
