namespace AppCore.Business.Models.JsonWebToken.Bases
{
    public abstract class JwtOptionsBase
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int JwtExpirationMinutes { get; set; }
        public string SecurityKey { get; set; }
    }
}
