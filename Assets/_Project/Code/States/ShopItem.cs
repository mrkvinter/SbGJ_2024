using Code.DiceSets;
using RG.ContentSystem.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.States
{
    public class ShopItem : MonoBehaviour, IPointerClickHandler
    {
        public SpriteRenderer Icon;

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
        }
    }
}