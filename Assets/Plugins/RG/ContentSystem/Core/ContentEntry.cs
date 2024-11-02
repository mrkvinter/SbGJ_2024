using System;
using UnityEngine;

namespace RG.ContentSystem.Core
{
    [Serializable]
    public abstract class ContentEntry
    {
        [field: SerializeField]
        public virtual string Id { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is ContentEntry contentObject && contentObject.Id == Id && contentObject.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [{Id}]";
        }
    }
}