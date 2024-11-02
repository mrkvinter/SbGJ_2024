using System;
using UnityEngine;

namespace RG.ContentSystem.Core
{
    [Serializable]
    public struct ContentRef<T>
        where T : ContentEntry
    {
        [SerializeField] private string id;

        public string Id => id;
        public Type Type => typeof(T);
        public bool IsEmpty => string.IsNullOrEmpty(id);

        public ContentRef(string id)
        {
            this.id = id;
        }

        public bool Equals(ContentRef<T> other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is ContentRef<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (id != null ? id.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"[{Type.Name}] '{id}'";
        }

        public static implicit operator ContentRef<T>(string id) => new(id);

        public static bool operator ==(ContentRef<T> left, ContentRef<T> right)
        {
            return left.Id == right.Id;
        }

        public static bool operator !=(ContentRef<T> left, ContentRef<T> right)
        {
            return left.Id != right.Id;
        }
    }
}