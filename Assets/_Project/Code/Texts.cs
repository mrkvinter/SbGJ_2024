namespace Code
{
    public static class Texts
    {
        public static string MaxValue => LanguageController.Current switch
        {
            Language.Russian => "<color=#58800a><b>Максимальное значение<b></color>",
            _ => "<color=#58800a><b>Max Value<b></color>"
        };
        
        public static string Name(string name) => $"<color=#4765b5><b>{name}<b></color>";
        public static string Hot 
            => LanguageController.Current switch
        {
            Language.Russian => "<color=#e7431b><b>Горячий<b></color>",
            _ => "<color=#e7431b><b>Hot<b></color>"
        };

        public static string BattleButton => LanguageController.Current switch
        {
            Language.Russian => "В бой",
            _ => "Battle"
        };
        
        public static string EndRoundButton => LanguageController.Current switch
        {
            Language.Russian => "Закончить ход",
            _ => "End round"
        };
    }
}