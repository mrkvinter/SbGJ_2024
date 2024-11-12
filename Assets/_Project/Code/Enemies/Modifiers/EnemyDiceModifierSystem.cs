using System;
using System.Collections.Generic;

namespace Code.Enemies.Modifiers
{
    public class EnemyDiceModifierSystem
    {
        private readonly Dictionary<Type, BaseEnemyModifierHandler> modifiers = new();

        public void ApplyModifier(BaseEnemyModifier modifier)
        {
            switch (modifier)
            {
                case MakeDiceHotModifier makeDiceHotModifier:
                    GetHandler<MakeDiceHotModifierHandler>().Apply(makeDiceHotModifier);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException($"Unknown modifier type: {modifier.GetType()}");
            }
        }

        private T GetHandler<T>() where T : BaseEnemyModifierHandler
        {
            if (modifiers.TryGetValue(typeof(T), out var handler))
            {
                return (T)handler;
            }

            var newHandler = Activator.CreateInstance<T>();
            modifiers.Add(typeof(T), newHandler);
            return newHandler;
        }
    }
}