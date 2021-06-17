using rest_api_blueprint.DbContexts;
using rest_api_blueprint.Entities.Authentication;
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
    public class ExternalLoginTokenRepository : IExternalLoginTokenRepository
    {
        private readonly MainDbContext _dbContext;
        private readonly DbSet<ExternalLoginToken> _externalLoginTokens;

        private const uint TTL = 30; // minutes;


        public ExternalLoginTokenRepository(MainDbContext context, IOptions<JwtTokenOptions> options)
        {
            _dbContext = context;
            _externalLoginTokens = context.Authentication_ExternalLoginTokens;
        }


        public async Task<ExternalLoginToken> GetNewExternalLoginToken(string loginProvider, string ip, string? userId)
        {
            await DeleteExpiredExternalLoginTokens(userId, false);

            string tokenString = TextService.GenerateToken();

            ExternalLoginToken externalLoginToken = new ExternalLoginToken
            {
                Token = tokenString,
                UserId = userId,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(TTL),
                IpAddress = ip,
                LoginProvider = loginProvider
            };

            await _externalLoginTokens.AddAsync(externalLoginToken);
            await _dbContext.SaveChangesAsync();

            return externalLoginToken;
        }

        /// <summary>
        /// Returns null if no valid token is with the given token string exists.
        /// </summary>
        public async Task<ExternalLoginToken?> GetExternalLoginToken(string loginProvider, string token, string? userId)
        {
            await DeleteExpiredExternalLoginTokens(userId, true);

            IQueryable<ExternalLoginToken> collection = _externalLoginTokens as IQueryable<ExternalLoginToken>;
            ExternalLoginToken? externalLoginToken = await collection.FirstOrDefaultAsync(rt => rt.Token == token && rt.LoginProvider == loginProvider && rt.UserId == userId);

            return externalLoginToken;
        }


        public async Task DeleteExpiredExternalLoginTokens(string? userId, bool saveChanges)
        {
            IQueryable<ExternalLoginToken> collection = _externalLoginTokens as IQueryable<ExternalLoginToken>;
            List<ExternalLoginToken> expiredExternalLoginTokens = await collection.Where(rt => rt.UserId == userId && rt.ExpiresAt <= DateTimeOffset.UtcNow.Date).ToListAsync();
            if (expiredExternalLoginTokens.Count > 0)
            {
                _externalLoginTokens.RemoveRange(expiredExternalLoginTokens);
                if (saveChanges)
                    await _dbContext.SaveChangesAsync();
            }
        }
    }
}
