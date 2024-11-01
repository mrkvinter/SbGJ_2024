using TMPro;
using UnityEngine;

namespace Code.Entites
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private int health;
        [SerializeField] private int damage;
        [SerializeField] private TMP_Text healthText;
        
        private void Start()
        {
            healthText.text = health.ToString();
        }
        
        public void TakeDamage(int damage)
        {
            health -= damage;
            healthText.text = health.ToString();
        }
    }
}