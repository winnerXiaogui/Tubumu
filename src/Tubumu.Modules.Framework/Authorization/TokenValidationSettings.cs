namespace Tubumu.Modules.Framework.Authorization
{
    public class TokenValidationSettings
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string IssuerSigningKey { get; set; }
    }
}
