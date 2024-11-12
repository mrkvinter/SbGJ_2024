using System;
using RG.ContentSystem.Core;
using UnityEngine;

namespace Code.Enemies.Modifiers
{
    [Serializable]
    public abstract class BaseEnemyModifier : ContentEntry
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
    }

    public abstract class BaseEnemyModifierHandler
    {
        public abstract Type ModifierType { get; }
    }
    public abstract class BaseEnemyModifierHandler<T> : BaseEnemyModifierHandler where T : BaseEnemyModifier
    {
        public override Type ModifierType => typeof(T);

        public abstract void Apply(T modifier);
    }
}