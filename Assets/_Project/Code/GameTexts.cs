using Code.UI;

namespace Code
{
    public static class GameTexts
    {
        public static Dialogue no_hp_dice => LanguageController.Current switch
        {
            Language.Russian => 
                Dialogue.Create().Text("Положите хотя бы один кубик в здоровье").Delay(Dialogue.LongDelay).Clear(),
            _ => Dialogue.Create().Text("Put at least one die in health").Delay(Dialogue.LongDelay).Clear()
        };
        //
        // You have chosen your buddy!
        // Before proceeding, let's buy some dice. At the start of the game, you will always have some coins.
        //     When you're done, press the "Battle" button.
        public static Dialogue tutor_first => LanguageController.Current switch
        {
            Language.Russian => Dialogue.Create()
                .Text("Отлично. Вы выбрали своего бади!").Delay(Dialogue.LongDelay).Clear()
                .Text("Перед тем как продолжить давай купим дайсы. В начале игры у вас всегда будет немного монет.")
                .Delay(Dialogue.LongDelay)
                .Text("Когда закончите, нажмите на кнопку \"В бой\".").Delay(Dialogue.LongDelay),

            _ => Dialogue.Create()
                .Text("Great. You have chosen your buddy!").Delay(Dialogue.LongDelay).Clear()
                .Text(
                    "Before proceeding, let's buy some dice. At the start of the game, you will always have some coins.")
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
                .Text("The enemy has attacked you!").Delay(Dialogue.LongDelay)
                .Text("Choose a die and roll it on the battlefield.").Delay(Dialogue.LongDelay),
        };

        public static Dialogue no_dices => LanguageController.Current switch
        {
            Language.Russian => Dialogue.Create()
                .Text("У тебя не осталось кубиков в чаше.").Delay(Dialogue.LongDelay).Clear()
                .Text("Я верну все кубики из сброса обратно в чашу.").Delay(Dialogue.LongDelay).Clear()
                .Text("Но теперь ты будешь брать на один кубик меньше.").Delay(Dialogue.LongDelay),

            _ => Dialogue.Create()
                .Text("You have no dice left in your cup.").Delay(Dialogue.LongDelay).Clear()
                .Text("I'll return all the dice from the discard pile back to the cup.").Delay(Dialogue.LongDelay).Clear()
                .Text("But now you will take one less die.").Delay(Dialogue.LongDelay),
        };

        public static Dialogue BuddyWinDialogue => LanguageController.Current switch
        {
            Language.Russian => Dialogue.Create()
                .Text("Победа!").Delay(Dialogue.LongDelay).Clear(),

            _ => Dialogue.Create()
                .Text("Victory!").Delay(Dialogue.LongDelay).Clear()
        };
    }
}