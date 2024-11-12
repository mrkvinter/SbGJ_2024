using System.Text;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace Code.Enemies
{
    public class EnemyView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly int Fade = Shader.PropertyToID("_FadeAmount");

        [SerializeField] private GameObject selectionIndicator;
        [SerializeField] private GameObject hoverIndicator;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Light2D light2D;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private SpriteRenderer shadowSpriteRenderer;
        [SerializeField] private SpriteRenderer damageFlashSpriteRenderer;
        [SerializeField] private SpriteRenderer modifierIconSpriteRenderer;

        private Material dissolveMaterial;
        private float dissolveAmount = 1.6f;

        public Enemy Enemy { get; private set; }

        public event System.Action<Enemy> OnEnemyClicked;

        private void Start()
        {
            dissolveMaterial = spriteRenderer.material;
            damageFlashSpriteRenderer.sprite = spriteRenderer.sprite;
        }

        public void Init(Enemy enemy)
        {
            Enemy = enemy;
            nameText.text = enemy.EnemyEntry.NameLocalized;
            if (!enemy.EnemyEntry.Modifier.IsEmpty)
            {
                modifierIconSpriteRenderer.gameObject.SetActive(true);
                modifierIconSpriteRenderer.sprite = enemy.EnemyEntry.Modifier.Unwrap().Icon;
            }
            else
            {
                modifierIconSpriteRenderer.gameObject.SetActive(false);
            }

            UpdateHealth(enemy.EnemyEntry.HealthCount);
            Deselect();
        }

        public void UpdateHealth(int health) => healthText.text = health.ToString("0");

        public void OnPointerClick(PointerEventData eventData) => OnEnemyClicked?.Invoke(Enemy);

        public void Deselect() => selectionIndicator.SetActive(false);
        
        public void Select() => selectionIndicator.SetActive(true);
        
        public void OnDamage()
        {
            visualRoot.DOKill();
            visualRoot.DOShakePosition(0.2f, 0.1f, 10, 90, false);
            damageFlashSpriteRenderer.DOFade(1, 0.1f).OnComplete(() => damageFlashSpriteRenderer.DOFade(0, 0.1f));
        }

        public async UniTask Die()
        {
            DOTween.To(() => dissolveAmount, SetDissolveAmount, 0, 2f);
            shadowSpriteRenderer.DOFade(0, 2f);
            modifierIconSpriteRenderer.DOFade(0, 2f);
            nameText.DOFade(0, 1f);
            healthText.DOFade(0, 1f);
            await DOTween.Sequence()
                .Append(DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 1, 0.4f))
                .Append(DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 0, 1.6f))
                .ToUniTask();
        }

        private void SetDissolveAmount(float amount)
        {
            dissolveAmount = amount;
            dissolveMaterial.SetFloat(Fade, dissolveAmount);
        }

        public void OnPointerEnter(PointerEventData eventData) 
        {
            hoverIndicator.SetActive(true);

            var sb = new StringBuilder();
            sb.AppendLine($"<size=+15>{Enemy.EnemyEntry.NameLocalized}</size>");
            if (LanguageController.Current == Language.Russian)
            {
                sb.AppendLine($"Урон: {Enemy.EnemyEntry.DamageCount}");
            }
            else
            {
                sb.AppendLine($"Damage: {Enemy.EnemyEntry.DamageCount}");
            }

            if (Enemy.EnemyEntry.MakeDiceHot)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine($"Одна лучайная кость становится: {Texts.Hot}");
                }
                else
                {
                    sb.AppendLine($"Makes a random die {Texts.Hot}");
                }
            }

            Game.Instance.TooltipService.ShowTooltip(sb.ToString(), transform, new Vector2(50, 50));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverIndicator.SetActive(false);
            Game.Instance.TooltipService.HideTooltip(transform);
        }

        private void OnDisable()
        {
            Game.Instance.TooltipService.HideTooltip(transform);
        }
    }
}