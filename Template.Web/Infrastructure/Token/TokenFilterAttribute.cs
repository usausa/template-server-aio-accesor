namespace Template.Web.Infrastructure.Token;

using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class TokenFilterAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => true;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return new TokenFilter((TokenSetting)serviceProvider.GetService(typeof(TokenSetting))!);
    }

    public sealed class TokenFilter : IAuthorizationFilter
    {
        private readonly TokenSetting setting;

        public TokenFilter(TokenSetting setting)
        {
            this.setting = setting;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("token", out var value) ||
                (value != setting.Token))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
