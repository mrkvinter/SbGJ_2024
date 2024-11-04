using System;
using Code.Utilities;
using DG.Tweening;
using KvinterGames;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Code.Dices
{
    public class Dice : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public TMP_Text diceCountText;
        public SpriteRenderer DiceSpriteRenderer;
        public SpriteRenderer LeftIndicator;
        public SpriteRenderer Hot;
        private DiceState diceState;

        private Vector3 shift;
        private Collider2D collider2D;
        private RaycastHit2D[] hits = new RaycastHit2D[30];
        private SortingGroup sortingGroup;
        private int sortingOrder;

        private DiceHandHolder diceHolderParent;
        private DiceHandHolder previewDiceHolder;
        
        public DiceHandHolder DiceHolderParent => diceHolderParent;
        public DiceState DiceState => diceState;
        public bool IsDraggable { get; set; } = true;
        

        public event Action OnClick;

        private void Awake()
        {
            sortingGroup = GetComponent<SortingGroup>();
            sortingOrder = sortingGroup.sortingOrder;

            collider2D = GetComponent<Collider2D>();
            LeftIndicator.color = LeftIndicator.color.WithAlpha(0);
            LeftIndicator.gameObject.SetActive(false);
            
            Hot.color = Hot.color.WithAlpha(0);
        }
        
        public void Init(DiceState diceState)
        {
            this.diceState = diceState;
            DiceSpriteRenderer.sprite = diceState.DiceEntry.DiceSprite;
            DiceSpriteRenderer.color = diceState.DiceEntry.DiceColor;
            diceCountText.fontSizeMax = diceState.DiceEntry.FontSize;
            diceCountText.color = diceState.DiceEntry.DiceTextColor;
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
            if (diceHolderParent?.IsLocked == true || !IsDraggable)
            {
                return;
            }

            sortingGroup.sortingOrder = 1000;
            if (diceState.HaveToShowLeftIndicator)
            {
                LeftIndicator.DOKill();
                LeftIndicator.gameObject.SetActive(true);
                LeftIndicator.DOFade(1, 0.2f);
            }

            var pos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            shift = transform.position - pos;
            
            diceHolderParent?.DeOccupy(this);
            transform.position = pos + shift;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (diceHolderParent?.IsLocked == true || !IsDraggable)
            {
                return;
            }

            diceState.OnPointerEnter();
            var pos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            transform.position = pos + shift;
            
            var diceHolder = FindDiceHolderParent();
            // if (diceHolder == diceHolderParent)
            // {
            //     diceHolder = null;
            // }

            if (diceHolder != previewDiceHolder)
            {
                previewDiceHolder?.OccupyPreview(null);
                previewDiceHolder = diceHolder;
            }
            
            previewDiceHolder?.OccupyPreview(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (diceHolderParent?.IsLocked == true || !IsDraggable)
            {
                return;
            }

            sortingGroup.sortingOrder = sortingOrder;
            if (diceState.HaveToShowLeftIndicator)
            {
                LeftIndicator.DOKill();
                LeftIndicator.DOFade(0, 0.2f)
                    .OnComplete(() => LeftIndicator.gameObject.SetActive(false));
            }
            
            previewDiceHolder?.OccupyPreview(null);
            previewDiceHolder = null;

            var count = collider2D.Cast(Vector2.zero, hits);
            if (count > 0)
            {
                var diceHolder = FindDiceHolderParent();
                if (diceHolder != null && diceHolder.Occupy(this))
                {
                    SoundController.Instance.PlaySound("dice_deal", 0.1f, 0.7f);
                    return;
                }
            }
            
            diceHolderParent?.Occupy(this);//????
            SoundController.Instance.PlaySound("dice_deal", 0.1f, 0.35f);
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
            diceCountText.text = value <= 0 ? "?" : value.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            diceState.OnPointerEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            diceState.OnPointerExit();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
    }
}