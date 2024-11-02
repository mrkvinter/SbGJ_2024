using System;
using UnityEngine;

namespace RG.ContentSystem.UnityAdapter
{
    public abstract class BaseScriptableSourceContentObject : ScriptableObject
    {
        public abstract Type ContentType { get; }
    }
}