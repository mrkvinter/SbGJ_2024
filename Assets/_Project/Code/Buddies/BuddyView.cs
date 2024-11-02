using Code.Dices;
using UnityEngine;

namespace Code.Buddies
{
    public class BuddyView : MonoBehaviour
    {
        public DiceHandHolder HpDiceHandHolder;
        public DiceHandHolder ShieldDiceHandHolder;

        public Buddies.Buddy Buddy { get; private set; }

        public void Initialize(Buddy buddy)
        {
            Buddy = buddy;
            
            HpDiceHandHolder.maxDiceCount = buddy.BuddyEntry.HealthDiceCount;
            ShieldDiceHandHolder.maxDiceCount = buddy.BuddyEntry.ShieldDiceCount;
        }
    }
}