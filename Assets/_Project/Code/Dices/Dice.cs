using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Code.Dices
{
    public class Dice : MonoBehaviour, IDragAndDropEvent, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private TMP_Text diceCountText;
        [SerializeField] private int maxDiceValue;

        private int diceValue;
        private Vector3 originalPosition;
        private Vector3 shift;
        private Collider2D collider2D;
        private RaycastHit2D[] hits = new RaycastHit2D[30];

        private DiceHandHolder diceHolderParent;
        private DiceHandHolder previewDiceHolder;
        
        public int DiceValue => diceValue;
        public DiceHandHolder DiceHolderParent => diceHolderParent;
        
        private void Awake()
        {
            collider2D = GetComponent<Collider2D>();
        }

        public void Roll()
        {
            diceValue = Random.Range(1, maxDiceValue + 1);
            diceCountText.text = diceValue.ToString();
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
            originalPosition = transform.position;
            var pos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            shift = transform.position - pos;
            
            diceHolderParent?.DeOccupy(this);
            transform.position = pos + shift;
        }

        public void OnDrag(PointerEventData eventData)
        {
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
    }
}