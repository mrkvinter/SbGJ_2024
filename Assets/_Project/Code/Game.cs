using Code.Buddies;
using Code.Dices;
using Code.States;
using Code.UI;
using Code.Utilities;
using Code.Visual;
using Cysharp.Threading.Tasks;
using RG.ContentSystem.UnityAdapter;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code
{
    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; }

        [SerializeField] public DiceHandHolder handDiceHolder;
        [SerializeField] public DiceHandHolder attackDiceHolder;
        [SerializeField] public TMP_Text damageText;
        [SerializeField] public Transform attackPoint;
        [SerializeField] public GameObject attackFx;
        [SerializeField] public Transform BuddyPoint;

        [SerializeField] public Transform FrontEnemyPoint;
        [SerializeField] public Transform LeftEnemyPoint;
        [SerializeField] public Transform RightEnemyPoint;
        [Space]
        public ShopItem[] ShopSlots;

        public DialoguePanel DialoguePanel;

        [Space]
        [SerializeField] public Button AttackButton;
        [SerializeField] public Transform AttackHolder;
        [SerializeField] public Tooltip Tooltip;
        [SerializeField] public Image BlackScreen;
        [SerializeField] public Transform GameUIRoot;
        [SerializeField] public TMP_Text GoldCountText;

        [Space] 
        public Transform BuddySelectorRoot;
        public BuddySelector TutorialBuddySelector;
        public BuddySelector FirstBuddySelector;
        public BuddySelector SecondBuddySelector;
        public BuddySelector LastBuddySelector;
        
        public TMP_Text DiceInBagText;

        public TooltipService TooltipService { get; } = new();
        public GameFlow GameFlow => gameFlow;

        private GameFlow gameFlow;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

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
        
        public void SetTextToButton(string text)
        {
            AttackButton.GetComponentInChildren<TMP_Text>().text = text;
        }
    }
}