namespace Template.Web.Areas.Default.Models;

public class ErrorViewModel
{
    public int StatusCode { get; set; }

    [AllowNull]
    public string RequestId { get; set; }

    public bool ShowRequestId => !String.IsNullOrEmpty(RequestId);
}
