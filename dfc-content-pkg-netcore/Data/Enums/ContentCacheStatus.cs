namespace DFC.Content.Pkg.Netcore.Data.Enums
{
    public enum ContentCacheStatus
    {
        NotFound = 0,
        Content = 1,
        ContentItem = 2,
        Both = Content | ContentItem,
    }
}
