using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
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
        [SerializeField] private SpriteRenderer shadowSpriteRenderer;

        private Material dissolveMaterial;
        private float dissolveAmount = 1.6f;

        public Enemy Enemy { get; private set; }

        public event System.Action<Enemy> OnEnemyClicked;

        private void Start()
        {
            dissolveMaterial = spriteRenderer.material;
        }

        public void Init(Enemy enemy)
        {
            Enemy = enemy;
            nameText.text = enemy.EnemyEntry.NameLocalized;
            UpdateHealth(enemy.EnemyEntry.HealthCount);
            Deselect();
        }

        public void UpdateHealth(int health) => healthText.text = health.ToString("0");

        public void OnPointerClick(PointerEventData eventData) => OnEnemyClicked?.Invoke(Enemy);

        public void Deselect() => selectionIndicator.SetActive(false);
        
        public void Select() => selectionIndicator.SetActive(true);

        public async UniTask Die()
        {
            DOTween.To(() => dissolveAmount, SetDissolveAmount, 0, 2f);
            shadowSpriteRenderer.DOFade(0, 2f);
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
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverIndicator.SetActive(false);
        }
    }
}