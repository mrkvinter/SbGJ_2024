using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RG.ContentSystem.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RG.ContentSystem.UnityAdapter
{
    /// <summary>
    /// For source that represents a single content object
    /// </summary>
    /// <typeparam name="T">The type of the content object</typeparam>
    public interface IScriptableContentObject<out T> where T : ContentEntry
    {
        Type ContentType { get; }
        T ContentObject { get; }
    }

    public interface IProcessSelfAttributes
    {
        IEnumerable<Attribute> ProcessSelfAttributes(string property);
    }

    public abstract class BaseScriptableContentObject : BaseScriptableSourceContentObject
    {
        public abstract string ContentId { get; }
    }

    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public abstract class ScriptableContentObject<T> : BaseScriptableContentObject, IScriptableContentObject<T>, IProcessSelfAttributes,
        ISelfValidator
        where T : ContentEntry
    {
        [SerializeField] [InlineProperty, HideLabel]
        private T content;

        public T ContentObject => content;

        public override string ContentId => content.Id;
        public override Type ContentType => typeof(T);

        public virtual IEnumerable<Attribute> ProcessSelfAttributes(string property)
        {
            if (property == nameof(ContentEntry.Id) && ContentType.IsSubclassOf(typeof(SettingsEntry)))
            {
                yield return new HideInInspector();
                yield break;
            }

            foreach (var processAttribute in ProcessAttributes(property))
            {
                yield return processAttribute;
            }
        }

        protected virtual IEnumerable<Attribute> ProcessAttributes(string property)
        {
            yield break;
        }

        protected virtual void OnValidated(SelfValidationResult result)
        {
        }

        public void Validate(SelfValidationResult result)
        {
            if (!typeof(T).GetCustomAttributes().Any(e => e is SerializableAttribute))
            {
                result.AddError($"{typeof(T).Name} must be marked as [Serializable]");
            }

            OnValidated(result);
        }
    }
}