using System;
using Code.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KvinterGames;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Code.Dices
{
    public class DiceState
    {
        public DiceEntry DiceEntry { get; }
        public Dice DiceView { get; private set; }
        public int Value { get; private set; }
        public bool IsHot { get; private set; }
        public bool HaveToShowLeftIndicator => DiceEntry.Duplicator || DiceEntry.Reroller;

        private GameSettings gameSettings;
        private bool wasRerolledOnThisTurn;
        private bool wasStrongCubeOnThisTurn;
        
        public int BreakLevel { get; set; }
        
        public bool WasPlayed { get; private set; }
        
        public bool CanBePlayed => !DiceEntry.IsGlassCube || !WasPlayed;
        public int MaxDiceValue => Mathf.Max(1, DiceEntry.MaxDiceValue - BreakLevel);
        
        public event Action<DiceState> OnClick; 
        

        public DiceState(DiceEntry diceEntry)
        {
            DiceEntry = diceEntry;
            
            gameSettings = ContentManager.GetSettings<GameSettings>();
        }
        
        public void SetValue(int value)
        {
            Value = value;
            if (Value > MaxDiceValue)
            {
                Value = MaxDiceValue;
            }
            DiceView?.SetValue(value);
        }
        
        public void SetView(Dice dice)
        {
            DiceView = dice;
            DiceView.Init(this);
            DiceView.SetValue(Value);

            DiceView.OnClick += HandleClick;
        }
        
        private void HandleClick() => OnClick?.Invoke(this);
        public void SetIsHot(bool isHot)
        {
            IsHot = isHot;
            if (DiceView != null)
            {
                DiceView.Hot.sprite = isHot ? GetHotSprite() : null;
                DiceView.Hot.DOFade(isHot ? 1 : 0, 0.2f)
                    .OnComplete(() => DiceView.Hot.gameObject.SetActive(isHot));
            }

            if (IsHot)
            {
                SoundController.Instance.PlaySound("hot");
            }
        }

        private Sprite GetHotSprite() => DiceEntry.MaxDiceValue switch
        {
            4 => gameSettings.D4HotModifierSprite,
            6 => gameSettings.D6HotModifierSprite,
            8 => gameSettings.D8HotModifierSprite,
            12 => gameSettings.D12HotModifierSprite,
            20 => gameSettings.D20HotModifierSprite,
            _ => null
        };

        public void ClearView()
        {
            DiceView.OnClick -= HandleClick;
            DiceView = null;
        }
        
        public void OnDiceHolderUpdated()
        {
            if (DiceEntry.Duplicator)
            {
                var index = DiceView?.DiceHolderParent?.Dices.IndexOf(DiceView) ?? -1;
                if (index >= 1)
                {
                    var leftDice = DiceView.DiceHolderParent.Dices[index - 1];
                    SetValue(leftDice.DiceState.Value);
                }
                else
                {
                    SetValue(0);
                }
            }
        }
        
        private void Reroll()
        {
            var value = Random.Range(1, MaxDiceValue + 1);
            SetValue(value);
        }


        public async UniTask DestroyDice()
        {
            var diceView = DiceView;
            ClearView();

            var diceHolder = diceView.DiceHolderParent;
            diceHolder.DeOccupy(diceView);
            diceView.transform.DOKill();
            await diceView.transform.DOScale(Vector3.zero, 0.15f).ToUniTask(); 
            Object.Destroy(diceView.gameObject);
        }
        
        public string GetTooltip()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<size=+15><b>d{DiceEntry.MaxDiceValue}</b></size>");
            sb.AppendLine($"<size=-5>{Texts.MaxValue}: {MaxDiceValue}</size>\n");
            
            if (DiceEntry.LoneWolf)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine(
                        $"{Texts.Name("Одинокий Волк")} - Этот кубик покажет своё {Texts.MaxValue}, если он один на поле.\n");
                }
                else
                {
                    sb.AppendLine(
                        $"{Texts.Name("Lone Wolf")} - This dice will show its {Texts.MaxValue} if it's the only dice on the field.\n");
                }
            }
            
            if (DiceEntry.Duplicator)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine(
                        $"{Texts.Name("Дубликатор")} - Этот кубик примет значение кубика слева.");
                }
                else
                {
                    sb.AppendLine(
                        $"{Texts.Name("Duplicator")} - This dice will show the value of the dice to the left.");
                }
            }
            
            if (DiceEntry.Reroller)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine($"{Texts.Name("Перекидыватель")} - Этот кубик перекинет кубик слева.");
                }
                else
                {
                    sb.AppendLine($"{Texts.Name("Reroller")} - This dice will reroll the dice to the left.");
                }
            }
            
            if (DiceEntry.GhostDice)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine($"{Texts.Name("Призрачный Кубик")} - Этот кубик не будет занимать слот на поле.");
                }
                else
                {
                    sb.AppendLine($"{Texts.Name("Ghost Dice")} - This dice will not occupy a slot on the field.");
                }
            }
            
            if (DiceEntry.IsEpicCube)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine($"{Texts.Name("Эпический Кубик")} - Все кубики на поле перекинутся и выбросят своё {Texts.MaxValue}.");
                }
                else
                {
                    sb.AppendLine($"{Texts.Name("Epic Cube")} - All dice on the field will reroll and show their {Texts.MaxValue}.");
                }
            }
            
            if (DiceEntry.IsStrongCube)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine($"{Texts.Name("Сильный Кубик")} - Все кубики на поле увеличат своё значение на 1.");
                }
                else
                {
                    sb.AppendLine(
                        $"{Texts.Name("Strong Cube")} - All dice on the field will increase their value by 1.");
                }
            }
            
            if (IsHot)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine($"{Texts.Name("Горячий")} - Этот кубик нанесёт 1 урон вашему бадди.");
                }
                else
                {
                    sb.AppendLine($"{Texts.Hot} - This dice will deal 1 damage to your buddy.");
                }
            }

            if (DiceEntry.IsGlassCube)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine($"{Texts.Name("Стеклянный Кубик")} - Этот кубик не может быть перекинут. Идет в сброс до конца боя."); 
                }
                else
                {
                    sb.AppendLine($"{Texts.Name("Glass Cube")} - This dice cannot be rerolled. Goes to the discard pile until the end of the battle.");
                }
            }

            if (BreakLevel > 0)
            {
                if (LanguageController.Current == Language.Russian)
                {
                    sb.AppendLine($"{Texts.Name("Кубик Сломан")} - Этот кубик сломан, его {Texts.MaxValue} уменьшено.");
                }
                else
                {
                    sb.AppendLine($"{Texts.Name("Broken Dice")} - This dice is broken, its {Texts.MaxValue} is reduced.");
                }
            }

            return sb.ToString();
        }
        
        public void DeOccupy()
        {
            DiceView.DiceHolderParent.DeOccupy(DiceView);
            DiceView.SetDiceHolderParent(null);
        }
        
        public void Occupy(DiceHandHolder diceHandHolder)
        {
            DiceView.SetDiceHolderParent(diceHandHolder);
            diceHandHolder.Occupy(DiceView);
        }
        
        public async UniTask CalculateValue(Action updater)
        {
            if (DiceView == null || DiceView.DiceHolderParent == null)
            {
                return;
            }

            await DiceView.transform.DOLocalMoveY(.25f, 0.1f).ToUniTask();

            WasPlayed = true;
            if (DiceEntry.LoneWolf)
            {
                if (DiceView.DiceHolderParent.Dices.Count == 1)
                {
                    var tween = ShakeDice();
                    await UniTask.Delay(200);
                    SetValue(MaxDiceValue);
                    await tween;
                }
            }

            if (DiceEntry.IsEpicCube)
            {
                var tween = ShakeDice();
                await UniTask.Delay(200);
                SetValue(MaxDiceValue);
                await tween;

            }
            
            if (IsHot)
            {
                var tween = ShakeDice();
                await UniTask.Delay(200);
                SoundController.Instance.PlaySound("hot");
                await Game.Instance.GameFlow.GameState.Buddy.TakeDamage(1);
                SetIsHot(false);
                await tween;
            }
            
            updater?.Invoke();
            await UniTask.Delay(500);

            if (DiceEntry.Reroller && !wasRerolledOnThisTurn)
            {
                wasRerolledOnThisTurn = true;
                var index = DiceView.DiceHolderParent.Dices.IndexOf(DiceView);
                if (index >= 1)
                {
                    var leftDice = DiceView.DiceHolderParent.Dices[index - 1];
                    var tween = leftDice.DiceState.ShakeDice();
                    await UniTask.Delay(200);
                    leftDice.DiceState.Reroll();
                    await tween;
                    await leftDice.DiceState.CalculateValue(updater);
                }
            }

            if (DiceEntry.IsEpicCube)
            {
                foreach (var dice in DiceView.DiceHolderParent.Dices)
                {
                    if (dice.DiceState.DiceEntry.IsEpicCube)
                    {
                        continue;
                    }

                    var tween = dice.DiceState.ShakeDice();
                    await UniTask.Delay(200);
                    dice.DiceState.SetValue(dice.DiceState.MaxDiceValue);
                    await tween;
                    await dice.DiceState.CalculateValue(updater);
                }
            }
            
            if (DiceEntry.IsStrongCube && !wasStrongCubeOnThisTurn)
            {
                wasStrongCubeOnThisTurn = true;
                foreach (var dice in DiceView.DiceHolderParent.Dices)
                {
                    // if (dice.DiceState == this)
                    // {
                    //     continue;
                    // }

                    var tween = dice.DiceState.ShakeDice();
                    await UniTask.Delay(200);
                    dice.DiceState.SetValue(dice.DiceState.Value + 1);
                    await tween;
                    await dice.DiceState.CalculateValue(updater);
                }
            }
            
            await DiceView.transform.DOLocalMoveY(0, 0.1f).ToUniTask();
        }
        
        public void OnEndTurn()
        {
            wasRerolledOnThisTurn = false;
            wasStrongCubeOnThisTurn = false;
            WasPlayed = false;
        }

        private UniTask ShakeDice()
        {
            return DOTween.Sequence()
                .Append(DiceView.transform.DOLocalRotate(new Vector3(0, 0, 10), 0.1f))
                .Append(DiceView.transform.DOLocalRotate(new Vector3(0, 0, -10), 0.1f))
                .Append(DiceView.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.1f))
                .SetLoops(2).ToUniTask();

        }
        public void OnPointerEnter()
        {
            if (DiceView == null)
            {
                return;
            }
            Game.Instance.TooltipService.ShowTooltip(GetTooltip(), DiceView.transform, new Vector2(-10f, 20f));
        }

        public void OnPointerExit()
        {
            Game.Instance.TooltipService.HideTooltip();
        }
    }
    
    public enum DiceRank
    {
        Poor,
        Common,
        Rare,
        Epic
    }
}