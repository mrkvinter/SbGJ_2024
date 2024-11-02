using Code.Dices;
using Code.Entites;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Dice dicePrefab;
        [SerializeField] private DiceHandHolder handDiceHolder;
        [SerializeField] private DiceHandHolder attackDiceHolder;
        [SerializeField] private TMP_Text damageText;
        
        [SerializeField] private Enemy enemy;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                RollDice();
            }
        }

        private void Restart()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        private void RollDice()
        {
            var dice = Instantiate(dicePrefab);
            dice.Roll();
            dice.SetDiceHolderParent(handDiceHolder);
            handDiceHolder.Occupy(dice);
        }

        public void Attack() => UniTask.Create(async () =>
        {
            var attackAmount = 0;
            for (var i = 0; i < attackDiceHolder.Dices.Count; i++)
            {
                var dice = attackDiceHolder.Dices[i];
                attackAmount += dice.DiceValue;
                damageText.text = attackAmount.ToString();
                await dice.transform.DOLocalMoveY(.25f, 0.1f).AsyncWaitForCompletion();
                await dice.transform.DOLocalMoveY(0, 0.05f).AsyncWaitForCompletion();
                await UniTask.Delay(500);
            }
            enemy.TakeDamage(attackAmount);
            await UniTask.Delay(1000);
            damageText.text = string.Empty;
            
            for (var index = attackDiceHolder.Dices.Count - 1; index >= 0; index--)
            {
                var dice = attackDiceHolder.Dices[index];
                attackDiceHolder.DeOccupy(dice);
                dice.transform.DOKill();
                Destroy(dice.gameObject);
            }
        });
    }
}