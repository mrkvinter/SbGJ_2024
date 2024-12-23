using Code.DiceSets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.States
{
    public class ShopItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public SpriteRenderer Icon;
        public TMP_Text Title;
        public TMP_Text Price;

        public DiceSetShopEntry DiceSetShopEntry { get; private set; }

        public event System.Action<ShopItem> OnSelected;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSelected?.Invoke(this);
        }

        public void SetContent(DiceSetShopEntry shopEntry)
        {
            DiceSetShopEntry = shopEntry;
            Icon.sprite = shopEntry.Icon;
            Title.text = shopEntry.GetName();
            Price.text = shopEntry.Price.ToString();
            UpdateColorPrice();
        }

        public void UpdateColorPrice()
        {
            if (Game.Instance.GameFlow.GameState.Coins >= DiceSetShopEntry.Price)
            {
                Price.color = Color.white;
            }
            else
            {
                Price.color = Color.red;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<size=+10><b>{DiceSetShopEntry.GetName()}</b></size>");
            sb.AppendLine(DiceSetShopEntry.GetDescription());
            Game.Instance.TooltipService.ShowTooltip(sb.ToString(), transform, new Vector2(-50, 150));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Game.Instance.TooltipService.HideTooltip(transform);
        }

        private void OnDisable()
        {
            Game.Instance.TooltipService.HideTooltip(transform);
        }
    }
}