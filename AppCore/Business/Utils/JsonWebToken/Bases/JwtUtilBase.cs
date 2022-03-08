using AppCore.Business.Models.JsonWebToken;
using AppCore.Business.Models.Results;
using AppCore.MvcWebUI.Utils.Bases;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppCore.Business.Utils.JsonWebToken.Bases
{
    public abstract class JwtUtilBase
    {
        private readonly AppSettingsUtilBase _appSettingsUtil;

        protected JwtUtilBase(AppSettingsUtilBase appSettingsUtil)
        {
            _appSettingsUtil = appSettingsUtil;
        }

        public virtual Result<Jwt> CreateJwt(string userName, string roleName, string userId, string appSettingsSectionKey = "JwtOptions")
        {
            try
            {
                var jwtOptions = _appSettingsUtil.Bind<JwtOptions>(appSettingsSectionKey);
                var symmetricSecurityKey = CreateSecurityKey(jwtOptions.SecurityKey);
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
                var claimList = new List<Claim>()
                {
                    new Claim(ClaimTypes.Sid, userId),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, roleName)
                };
                var expiration = DateTime.Now.AddMinutes(jwtOptions.JwtExpirationMinutes);
                var jwtSecurityToken = new JwtSecurityToken(jwtOptions.Issuer, jwtOptions.Audience, claimList, DateTime.Now, expiration, signingCredentials);
                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
                var jwt = new Jwt()
                {
                    Token = token,
                    Expiration = expiration
                };
                return new SuccessResult<Jwt>("JwtCreated", jwt);
            }
            catch (Exception exc)
            {
                return new ExceptionResult<Jwt>(exc);
            }
        }

        public virtual SecurityKey CreateSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }
    }
}
