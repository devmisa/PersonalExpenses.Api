namespace PersonalExpenses.Security.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public int ExpiryInHours { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
