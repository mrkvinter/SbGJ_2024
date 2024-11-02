using System.Collections.Generic;
using Code.Dices;
using Code.Entites;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RG.ContentSystem.UnityAdapter;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    public class Game : MonoBehaviour
    {
        [SerializeField] public Dice dicePrefab;
        [SerializeField] public DiceHandHolder handDiceHolder;
        [SerializeField] public DiceHandHolder attackDiceHolder;
        [SerializeField] public TMP_Text damageText;
        [SerializeField] public Enemy enemy;
        [SerializeField] public Transform attackPoint;
        [SerializeField] public GameObject attackFx;
        
        public TMP_Text DiceInBagText;

        private GameFlow gameFlow;

        private void Start()
        {
            InitContentSystem();

            gameFlow = new GameFlow(this);
            gameFlow.StartGame();
        }
        
        private void InitContentSystem()
        {
            var database = Resources.Load<ContentDatabase>("Data/ContentDatabase");
            ContentInstaller.Install(database);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                gameFlow.RollDice().Forget();
            }
        }

        private void Restart()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        public void Attack() => gameFlow.Attack().Forget();
    }
}