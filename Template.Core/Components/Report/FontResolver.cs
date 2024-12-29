namespace Template.Components.Report;

using PdfSharp.Fonts;

public sealed class FontResolver : IFontResolver
{
    private readonly string path;

    private readonly IDictionary<string, string> fontFiles;

    public string DefaultFontName { get; }

    public FontResolver(string path, string defaultFontName, IDictionary<string, string> fontFiles)
    {
        this.path = path;
        this.fontFiles = fontFiles;
        DefaultFontName = defaultFontName;
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic)
    {
        return fontFiles.TryGetValue(familyName, out var fileName) ? new FontResolverInfo(fileName) : null!;
    }

    public byte[] GetFont(string faceName) => File.ReadAllBytes(Path.Combine(path, faceName));
}
