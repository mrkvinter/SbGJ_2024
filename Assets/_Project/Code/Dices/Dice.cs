using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Code.Dices
{
    public class Dice : MonoBehaviour, IDragAndDropEvent, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public TMP_Text diceCountText;
        public SpriteRenderer DiceSpriteRenderer;

        private DiceState diceState;

        private Vector3 shift;
        private Collider2D collider2D;
        private RaycastHit2D[] hits = new RaycastHit2D[30];

        private DiceHandHolder diceHolderParent;
        private DiceHandHolder previewDiceHolder;
        
        public DiceHandHolder DiceHolderParent => diceHolderParent;
        public DiceState DiceState => diceState;
        
        private void Awake()
        {
            collider2D = GetComponent<Collider2D>();
        }
        
        public void Init(DiceState diceState)
        {
            this.diceState = diceState;
            DiceSpriteRenderer.sprite = diceState.DiceEntry.DiceSprite;
            DiceSpriteRenderer.color = diceState.DiceEntry.DiceColor;
            diceCountText.fontSizeMax = diceState.DiceEntry.FontSize;
        }

        public void SetDiceHolderParent(DiceHandHolder diceHolder)
        {
            if (diceHolderParent != null)
            {
                diceHolderParent.DeOccupy(this);
            }

            diceHolderParent = diceHolder;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (diceHolderParent?.IsLocked == true)
            {
                return;
            }

            var pos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            shift = transform.position - pos;
            
            diceHolderParent?.DeOccupy(this);
            transform.position = pos + shift;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (diceHolderParent?.IsLocked == true)
            {
                return;
            }

            var pos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            transform.position = pos + shift;
            
            var diceHolder = FindDiceHolderParent();
            if (diceHolder == diceHolderParent)
            {
                diceHolder = null;
            }

            if (diceHolder != previewDiceHolder)
            {
                previewDiceHolder?.OccupyPreview(null);
                previewDiceHolder = diceHolder;
            }
            
            previewDiceHolder?.OccupyPreview(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (diceHolderParent?.IsLocked == true)
            {
                return;
            }

            previewDiceHolder?.OccupyPreview(null);
            previewDiceHolder = null;

            var count = collider2D.Cast(Vector2.zero, hits);
            if (count > 0)
            {
                var diceHolder = FindDiceHolderParent();
                if (diceHolder != null && diceHolder.Occupy(this))
                {
                    diceHolderParent = diceHolder;
                    return;
                }
            }
            
            diceHolderParent?.Occupy(this);
        }

        private DiceHandHolder FindDiceHolderParent()
        {
            var count = collider2D.Cast(Vector2.zero, hits);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (hits[i].collider.TryGetComponent(out DiceHandHolder diceHolder))
                    {
                        return diceHolder;
                    }
                }
            }

            return null;
        }

        public void SetValue(int value)
        {
            diceCountText.text = value.ToString();
        }
    }
}