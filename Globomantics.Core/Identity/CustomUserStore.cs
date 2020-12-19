using Dapper;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Globomantics.Core.Identity
{
    public class CustomUserStore : IUserPasswordStore<CustomUser>, IUserEmailStore<CustomUser>
    {
        private readonly IDbConnection db;

        public CustomUserStore(IDbConnection db)
        {
            this.db = db;
        }
        public Task<IdentityResult> CreateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(CustomUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<CustomUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return FindByNameAsync(normalizedEmail, cancellationToken);
        }

        public async Task<CustomUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await db.QueryAsync<CustomUser>(
                "Select * From GlobomanticsUser Where UserId =@userID"
                , new { userId });
            return user.SingleOrDefault();

        }

        public async Task<CustomUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await db.QueryAsync<CustomUser>(
                "Select * From GlobomanticsUser Where LoginName = @LoginName"
                , new { LoginName = normalizedUserName });
            return user.SingleOrDefault();
        }

        public Task<string> GetEmailAsync(CustomUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<string> GetNormalizedEmailAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LoginName.ToUpper());
        }

        public Task<string> GetPasswordHashAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LoginName);
        }

        public Task<bool> HasPasswordAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(CustomUser user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(CustomUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(CustomUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(CustomUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(CustomUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            user.PasswordSalt = null;
            user.PasswordModifiedDate = DateTime.Now;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(CustomUser user, string userName, CancellationToken cancellationToken)
        {
            user.LoginName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            IdentityResult result;
            try
            {
                db.ExecuteAsync(@"
Update GlobomanticsUser
Set PasswordHash = @PasswordHash
,PasswordSalt = @PasswordSalt
,LoginName = @LoginName
,PasswordModifiedDate = @PasswordModifiedDate
,CreateDate = @CreateDate
,Status =@Status
Where UserId = @UserId", user);
                result = IdentityResult.Success;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating identity");
                result = IdentityResult.Failed();
            }
            return Task.FromResult(result);
        }
    }
}
