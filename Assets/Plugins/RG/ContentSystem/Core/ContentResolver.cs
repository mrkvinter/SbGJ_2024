using System;
using System.Collections.Generic;

namespace RG.ContentSystem.Core
{
    public sealed class ContentResolver
    {
        private readonly Dictionary<Type, IContentMap> maps;

        public ContentResolver(IEnumerable<IContentMap> contentMap)
        {
            maps = new Dictionary<Type, IContentMap>();
            foreach (var map in contentMap)
            {
                maps[map.ContentType] = map;
            }
        }

        public ContentMap<T> GetContentMap<T>() where T : ContentEntry
        {
            return (ContentMap<T>) maps[typeof(T)];
        }

        public T GetContent<T>(string id) where T : ContentEntry
        {
            return GetContentMap<T>().GetContent(id);
        }

        public T GetContent<T>(ContentRef<T> contentRef) where T : ContentEntry
        {
            return GetContentMap<T>().GetContent(contentRef.Id);
        }

        public T GetSettings<T>() where T : SettingsEntry
        {
            return (T) GetContentMap<SettingsEntry>().GetContent(typeof(T).Name);
        }
    }
}