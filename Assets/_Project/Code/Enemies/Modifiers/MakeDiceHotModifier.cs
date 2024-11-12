using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace Code.Enemies.Modifiers
{
    [Serializable]
    public class MakeDiceHotModifier : BaseEnemyModifier
    {
    }

    public class MakeDiceHotModifierHandler : BaseEnemyModifierHandler<MakeDiceHotModifier>
    {
        public override void Apply(MakeDiceHotModifier modifier)
        {
            var hand = Game.Instance.GameFlow.GameState.Hand.Where(d => !d.IsHot).ToList();
            if (hand.Count == 0)
            {
                return;
            }
            var dice = hand[Random.Range(0, hand.Count)];
            dice.SetIsHot(true);
        }
    }
}