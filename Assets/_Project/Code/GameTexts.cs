using Code.UI;

namespace Code
{
    public static class GameTexts
    {
        public static Dialogue tutor_first => LanguageController.Current switch
        {
            Language.Russian => Dialogue.Create()
                .Text("Отлично. Вы выбрали своего бади!").Delay(Dialogue.LongDelay).Clear()
                .Text("Перед тем как продолдить, давай купим дайсы. В начале игры, у вас всегда будет немного монет.")
                .Delay(Dialogue.LongDelay)
                .Text("Когда закончите, нажмите на кнопку \"В бой\".").Delay(Dialogue.LongDelay),
            
            _ => Dialogue.Create()
                .Text("Great. You have chosen your buddy!").Delay(Dialogue.LongDelay).Clear()
                .Text("Before proceeding, let's buy some dice. At the start of the game, you will always have some coins.")
                .Delay(Dialogue.LongDelay)
                .Text("When you're done, press the \"Battle\" button.").Delay(Dialogue.LongDelay),
        };
        
        public static Dialogue tutor_second => LanguageController.Current switch
        {
            Language.Russian => Dialogue.Create()
                .Text("Ваши кубики - это ваша защита").Delay(Dialogue.LongDelay).Clear()
                .Text("Распределите кубики между здоровьем и щитом.").Delay(Dialogue.LongDelay),
            
            _ => Dialogue.Create()
                .Text("Your dice are your defense").Delay(Dialogue.LongDelay).Clear()
                .Text("Distribute the dice between health and shield.").Delay(Dialogue.LongDelay),
        };
        
        public static Dialogue tutor_third => LanguageController.Current switch
        {
            Language.Russian => Dialogue.Create()
                .Text("Враг напал на вас!").Delay(Dialogue.LongDelay)
                .Text("Выберите кубик и бросьте его на поле боя.").Delay(Dialogue.LongDelay),
            
            _ => Dialogue.Create()
                .Text("Your dice are your defense").Delay(Dialogue.LongDelay).Clear()
                .Text("Distribute the dice between health and shield.").Delay(Dialogue.LongDelay),
        };
    }
}