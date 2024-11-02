using System;
using System.Collections.Generic;

namespace RG.ContentSystem.Core
{
    public interface IContentMap
    {
        Type ContentType { get; }
    }

    public interface IContentMap<out T> : IContentMap
    {
        IReadOnlyList<T> ContentEntries { get; }
    }

    public sealed class ContentMap<T> : IContentMap<T>
        where T : ContentEntry
    {
        private readonly Dictionary<string, T> contentsById = new();
        private readonly List<T> contents;

        public IReadOnlyList<T> ContentEntries => contents;
        public Type ContentType => typeof(T);

        public ContentMap(IReadOnlyCollection<T> contents)
        {
            this.contents = new List<T>(contents);
            foreach (var contentEntry in contents)
            {
                contentsById[contentEntry.Id] = contentEntry;
            }
        }

        public T GetContent(string id)
        {
            return contentsById[id];
        }

        public bool TryGetContent(string id, out T content)
        {
            return contentsById.TryGetValue(id, out content);
        }
    }
}