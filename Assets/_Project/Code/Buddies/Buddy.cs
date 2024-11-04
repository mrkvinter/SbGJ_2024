using System;
using System.Collections.Generic;
using System.Linq;
using Code.Dices;
using Code.Enemies;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RG.ContentSystem.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Buddies
{
    public class Buddy
    {
        public BuddyEntry BuddyEntry { get; }
        public BuddyView BuddyView { get; }

        private DiceHandHolder healthDiceHandHolder;
        private DiceHandHolder shieldDiceHandHolder;
        public int Health => healthDiceHandHolder.Dices.Sum(dice => dice.DiceState.Value);
        
        private List<DiceState> usedShieldDices = new();

        public Buddy(BuddyEntry buddyEntry, BuddyView buddyView)
        {
            BuddyEntry = buddyEntry;
            BuddyView = buddyView;
            
            healthDiceHandHolder = buddyView.HpDiceHandHolder;
            shieldDiceHandHolder = buddyView.ShieldDiceHandHolder;

            BuddyView.Initialize(this);
        }

        public async UniTask TakeDamage(int damage)
        {
            while (damage > 0 && Health > 0)
            {
                if (shieldDiceHandHolder.Dices.Count > 0)
                {
                    var shieldDice = shieldDiceHandHolder.Dices[^1];
                    var shieldDiceValue = shieldDice.DiceState.Value;
                    var damageToTake = Mathf.Min(shieldDiceValue, damage);
                    damage -= damageToTake;
                    shieldDice.DiceState.SetValue(shieldDiceValue - damageToTake);
                    await shieldDice.transform.DOShakePosition(0.5f, 0.1f, 10, 90f, false).AsyncWaitForCompletion();
                    if (shieldDice.DiceState.Value == 0)
                    {
                        usedShieldDices.Add(shieldDice.DiceState);
                        await shieldDice.DiceState.DestroyDice();
                        await UniTask.Delay(100);
                    }
                }
                else if (healthDiceHandHolder.Dices.Count > 0)
                {
                    var healthDice = healthDiceHandHolder.Dices[^1];
                    var healthDiceValue = healthDice.DiceState.Value;
                    var damageToTake = Mathf.Min(healthDiceValue, damage);
                    damage -= damageToTake;
                    healthDice.DiceState.SetValue(healthDiceValue - damageToTake);
                    await healthDice.transform.DOShakePosition(0.5f, 0.1f, 10, 90f, false).AsyncWaitForCompletion();
                    if (healthDice.DiceState.Value == 0)
                    {
                        await healthDice.DiceState.DestroyDice();
                        await UniTask.Delay(100);
                    }
                }
                
                await UniTask.Yield();
            }
        }

        public DiceState GetNextDice()
        {
            if (shieldDiceHandHolder.Dices.Count > 0)
            {
                return shieldDiceHandHolder.Dices[^1].DiceState;
            }

            if (healthDiceHandHolder.Dices.Count > 0)
            {
                return healthDiceHandHolder.Dices[^1].DiceState;
            }

            return null;
        }
        
        public void OnFightEnd()
        {
            shieldDiceHandHolder.Unlock();
            foreach (var usedShieldDice in usedShieldDices)
            {
                usedShieldDice.SetView(Object.Instantiate(usedShieldDice.DiceEntry.DicePrefab));
                shieldDiceHandHolder.Occupy(usedShieldDice.DiceView);
            }

            foreach (var dice in shieldDiceHandHolder.Dices)
            {
                dice.SetValue(dice.DiceState.DiceEntry.MaxDiceValue);
            }
            shieldDiceHandHolder.Lock();
        }
    }
    
    [Serializable]
    public class BuddyEntry : ContentEntry
    {
        [field: SerializeField] public int HealthDiceCount { get; private set; }
        [field: SerializeField] public int ShieldDiceCount { get; private set; }
        [field: SerializeField] public int AttackDiceCount { get; private set; }
        [field: SerializeField] public BuddyView Prefab { get; private set; }
        [field: SerializeField] public bool IsTutorialBuddy { get; private set; }
        [field:SerializeField] public int Index { get; set; }
        [field: SerializeField] public bool HasTwist { get; private set; }
        public int TwistIndex;
        public ContentRef<ChallengeEntry>[] Challenges;

    }
}