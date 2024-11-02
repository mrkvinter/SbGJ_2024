using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.Utilities
{
    public static class ContentManager
    {
        private static ContentResolver contentResolver;
        
        public static void Initialize(ContentResolver resolver)
        {
            if (contentResolver != null)
            {
                Debug.LogError("ContentManager already initialized. Are you calling Initialize twice?");
            }

            contentResolver = resolver;
        }
        
        public static ContentMap<T> GetContentMap<T>() where T : ContentEntry
        {
            return contentResolver.GetContentMap<T>();
        }

        public static T GetContent<T>(string id) where T : ContentEntry
        {
            return contentResolver.GetContent<T>(id);
        }

        public static T GetContent<T>(ContentRef<T> contentRef) where T : ContentEntry
        {
            return contentResolver.GetContent(contentRef);
        }

        public static T[] GetContent<T>(ContentRef<T>[] contentRefs) where T : ContentEntry
        {
            var result = new T[contentRefs.Length];
            for (var i = 0; i < contentRefs.Length; i++)
            {
                result[i] = contentResolver.GetContent(contentRefs[i]);
            }
            
            return result;
        }

        public static T GetSettings<T>() where T : SettingsEntry
        {
            return contentResolver.GetSettings<T>();
        }
    }
}