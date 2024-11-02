using System;
using RG.ContentSystem.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RG.ContentSystem.UnityAdapter
{
    /// <summary>
    /// For source that represents a list of content objects
    /// </summary>
    /// <typeparam name="T">The type of the content object</typeparam>
    public interface IScriptableSourceContentObjects<out T> where T : ContentEntry
    {
        Type ContentType { get; }
        T[] ContentObjects { get; }
    }

    public abstract class BaseScriptableTableContentObject : BaseScriptableSourceContentObject
    {
    }

    public abstract class BaseScriptableTableContentObject<T> : BaseScriptableTableContentObject,
        IScriptableSourceContentObjects<T>
        where T : ContentEntry
    {
        public override Type ContentType => typeof(T);

        [field: SerializeField, TableList] public T[] ContentObjects { get; private set; }
    }
}