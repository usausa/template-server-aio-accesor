namespace Template.Web.Infrastructure.State
{
    using System;
    using System.Text;

    public static class StateHelper
    {
        public static string Encode(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        public static string Decode(string value) => Encoding.UTF8.GetString(Convert.FromBase64String(value));
    }
}
