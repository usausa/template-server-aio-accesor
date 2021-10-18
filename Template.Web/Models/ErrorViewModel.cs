namespace Template.Web.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class ErrorViewModel
    {
        [AllowNull]
        public string RequestId { get; set; }

        public bool ShowRequestId => !String.IsNullOrEmpty(RequestId);
    }
}
