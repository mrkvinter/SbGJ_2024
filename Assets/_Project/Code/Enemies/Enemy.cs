using System;
using Cysharp.Threading.Tasks;
using RG.ContentSystem.Core;
using UnityEngine;
using Object = UnityEngine.Object;

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
    }
    
    [Serializable]
    public class EnemyEntry : ContentEntry
    {
        [field: SerializeField] public int HealthCount { get; private set; }
        [field: SerializeField] public int DamageCount { get; private set; }
        [field: SerializeField] public EnemyView Prefab { get; private set; }
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