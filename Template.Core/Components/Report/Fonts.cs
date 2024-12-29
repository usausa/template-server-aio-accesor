namespace Template.Components.Report;

using PdfSharp.Drawing;

#pragma warning disable IDE0032
public static class Fonts
{
    [ThreadStatic]
    private static XFont? largeFontB;

    public static XFont LargeFontB => largeFontB ??= new XFont("Gothic", 28, XFontStyleEx.Bold);

    [ThreadStatic]
    private static XFont? normalFont;

    public static XFont NormalFont => normalFont ??= new XFont("Gothic", 11, XFontStyleEx.Regular);

    [ThreadStatic]
    private static XFont? smallFont;

    public static XFont SmallFont => smallFont ??= new XFont("Gothic", 10, XFontStyleEx.Regular);

    [ThreadStatic]
    private static XFont? minimumFont;

    public static XFont MinimumFont => minimumFont ??= new XFont("Gothic", 9, XFontStyleEx.Regular);
}
#pragma warning restore IDE0032
