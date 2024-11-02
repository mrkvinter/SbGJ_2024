using RG.ContentSystem.Core;

namespace Code.Utilities
{
    public static class ContentExtensions
    {
        public static T As<T>(this string id) where T : ContentEntry
        {
            return ContentManager.GetContent<T>(id);
        }
        
        public static T Unwrap<T>(this ContentRef<T> contentRef) where T : ContentEntry
        {
            return ContentManager.GetContent(contentRef);
        }
    }
}