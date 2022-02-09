namespace Template.Web.Infrastructure.State;

using System;

using Microsoft.AspNetCore.Http;

public static class StateExtensions
{
    public static string? RestoreState(this IUrlHelper urlHelper, string action)
    {
        var state = urlHelper.ActionContext.HttpContext.Request.Query["state"];
        if (String.IsNullOrEmpty(state))
        {
            return urlHelper.Action(action);
        }

        return urlHelper.Action(action) + StateHelper.Decode(state);
    }

    public static string State(this HttpContext context) =>
        context.Request.QueryString.Value is not null ? StateHelper.Encode(context.Request.QueryString.Value) : string.Empty;
}
