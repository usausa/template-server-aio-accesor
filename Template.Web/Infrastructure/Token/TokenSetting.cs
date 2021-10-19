namespace Template.Web.Infrastructure.Token
{
    using System.Diagnostics.CodeAnalysis;

    public class TokenSetting
    {
        [AllowNull]
        public string Token { get; set; }
    }
}
