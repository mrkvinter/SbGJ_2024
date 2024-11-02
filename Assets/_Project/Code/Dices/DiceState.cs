namespace Code.Dices
{
    public class DiceState
    {
        
        public DiceEntry DiceEntry { get; }
        public Dice DiceView { get; private set; }
        public int Value { get; private set; }

        public DiceState(DiceEntry diceEntry)
        {
            DiceEntry = diceEntry;
        }
        
        public void SetValue(int value)
        {
            Value = value;
            DiceView?.SetValue(value);
        }
        
        public void SetView(Dice dice)
        {
            DiceView = dice;
            DiceView.Init(this);
            DiceView.SetValue(Value);
        }
        
        public void ClearView()
        {
            DiceView = null;
        }
    }
}