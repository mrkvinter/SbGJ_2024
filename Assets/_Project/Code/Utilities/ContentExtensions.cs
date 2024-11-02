using RG.ContentSystem.Core;

namespace Code.Utilities
{
    public static class ContentExtensions
    {
        public static T As<T>(this string id) where T : ContentEntry
        {
            return ContentManager.GetContent<T>(id);
        }
    }
}