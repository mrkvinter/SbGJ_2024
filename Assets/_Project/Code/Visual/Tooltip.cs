using UnityEngine;

namespace Code.Visual
{
    public class Tooltip : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI TextComponent;
        
        public RectTransform RectTransform => (RectTransform) transform;
    }
    
    public class TooltipService
    {
        private Transform lastTarget;

        public void ShowTooltip(string text, Transform target, Vector2 offset)
        {
            lastTarget = target;
            var tooltip = Game.Instance.Tooltip;
            tooltip.TextComponent.text = text;
            tooltip.gameObject.SetActive(true);
            var canvas = tooltip.GetComponentInParent<Canvas>();
            var screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, target.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPoint);
            tooltip.RectTransform.localPosition = localPoint + offset;
            // tooltip.RectTransform.anchoredPosition = screenPoint;
        }
        
        
        public void HideTooltip(Transform target)
        {
            if (lastTarget == target)
            {
                Game.Instance.Tooltip.gameObject.SetActive(false);
            }
        }
    }
}