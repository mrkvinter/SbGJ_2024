using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using RG.ContentSystem.Core;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Code.Enemies
{
    public class Enemy
    {
        public EnemyEntry EnemyEntry { get; }
        public EnemyView View { get; private set; }
        public bool IsDead => health <= 0;

        private int health;
        
        public Enemy(EnemyEntry enemyEntry, EnemyView view)
        {
            EnemyEntry = enemyEntry;
            View = view;
            
            health = enemyEntry.HealthCount;
            view.Init(this);
        }

        public void TakeDamage(int attackAmount)
        {
            health -= attackAmount;
            health = Mathf.Max(0, health);
            
            View.UpdateHealth(health);
        }
        
        public async UniTask Die()
        {
            await View.Die();
            Object.Destroy(View.gameObject);
            View = null;
        }
        
        public void OnRoundStart()
        {
            if (EnemyEntry.MakeDiceHot)
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
    
    [Serializable]
    public class EnemyEntry : ContentEntry
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string NameRus { get; private set; }
        [field: SerializeField] public int HealthCount { get; private set; }
        [field: SerializeField] public int DamageCount { get; private set; }
        [field: SerializeField] public EnemyView Prefab { get; private set; }
        [field: SerializeField] public bool MakeDiceHot { get; private set; }

        public string NameLocalized => LanguageController.Current switch
        {
            Language.Russian => NameRus,
            _ => Name
        };
    }
    
    [Serializable]
    public class ChallengeEntry : ContentEntry
    {
        [field: SerializeField] public ContentRef<EnemyEntry> FrontEnemy { get; private set; }
        [field: SerializeField] public ContentRef<EnemyEntry> LeftEnemy { get; private set; }
        [field: SerializeField] public ContentRef<EnemyEntry> RightEnemy { get; private set; }
        [field: SerializeField] public int CoinsReward { get; private set; }
    }
}