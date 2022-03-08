using System;

namespace AppCore.Business.Models.JsonWebToken.Bases
{
    public abstract class JwtBase
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
