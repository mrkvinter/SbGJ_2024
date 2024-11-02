using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Entites
{
    public class Enemy : MonoBehaviour
    {
        [FormerlySerializedAs("health")] [SerializeField] private int maxHealth;
        [SerializeField] private int damage;
        [SerializeField] private TMP_Text healthText;
        
        private float currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
            healthText.text = maxHealth.ToString("0");
        }
        
        public void TakeDamage(float damageAmount)
        {
            currentHealth -= damageAmount;
            healthText.text = currentHealth.ToString("0");
        }
    }
}