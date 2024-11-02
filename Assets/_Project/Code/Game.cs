using System.Collections.Generic;
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
        
        [SerializeField] private Transform attackPoint;
        [SerializeField] private GameObject attackFx;

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
                await dice.transform.DOLocalMoveY(.25f, 0.1f).ToUniTask();
                await dice.transform.DOLocalMoveY(0, 0.05f).ToUniTask();
                await UniTask.Delay(500);
            }
            var tasks = new List<UniTask>();
            var countFX = attackAmount/4f;
            for (var i = 0; i < countFX; i++)
            {
                var fx = Instantiate(attackFx, enemy.transform);
                fx.transform.localPosition = attackPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                var task = fx.transform.DOMove(enemy.transform.position, 0.25f).SetEase(Ease.InSine).SetDelay(Random.Range(0, 0.2f)).ToUniTask()
                    .ContinueWith(() =>
                    {
                        Destroy(fx);
                    });
                tasks.Add(task);
            }
            await UniTask.WhenAll(tasks);
            enemy.TakeDamage(attackAmount);
            damageText.text = string.Empty;
            
            for (var index = attackDiceHolder.Dices.Count - 1; index >= 0; index--)
            {
                var dice = attackDiceHolder.Dices[index];
                attackDiceHolder.DeOccupy(dice);
                dice.transform.DOKill();
                dice.transform.DOScale(Vector3.zero, 0.2f)
                    .OnComplete(() => Destroy(dice.gameObject));
            }
        });
    }
}