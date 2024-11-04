using Code.Buddies;
using Code.Dices;
using Code.States;
using Code.UI;
using Code.Utilities;
using Code.Visual;
using Cysharp.Threading.Tasks;
using RG.ContentSystem.UnityAdapter;
using Sirenix.OdinInspector;
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

        [SerializeField] public Transform LangButtons;

        [Space] 
        public Transform BuddySelectorRoot;
        public BuddySelector TutorialBuddySelector;
        public BuddySelector FirstBuddySelector;
        public BuddySelector SecondBuddySelector;
        public BuddySelector LastBuddySelector;
        
        public TMP_Text DiceInBagText;

        public RealWorldServiceSettings RealWorldServiceSettings;

        public TooltipService TooltipService { get; } = new();
        public RealWorldService RealWorldService { get; } = new();
        public GameFlow GameFlow => gameFlow;

        private GameFlow gameFlow;

        [Button]
        public void ShowRealWorld() => RealWorldService.ShowRealWorld().Forget();

        public void SelectRussianLanguage() => LanguageController.SetLanguage(Language.Russian);
        
        public void SelectEnglishLanguage() => LanguageController.SetLanguage(Language.English);

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
            gameFlow?.Tick();

            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }

            // if (Input.GetKeyDown(KeyCode.F))
            // {
            //     gameFlow.RollDice().Forget();
            // }
            
            //если щелчоек мыши или нажали на экран
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                DialoguePanel.Skip();
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