using System.Collections.Generic;

namespace Code.UI
{
    public class Dialogue
    {
        private readonly List<DialogueStage> stages = new();

        public IReadOnlyList<DialogueStage> Stages => stages;
        
        public string OverrideSpeaker { get; set; }
        
        public const int LongDelay = 1300;
        public const int MediumDelay = LongDelay / 2;
        public const int ShortDelay = LongDelay / 4;
        public const int NoDelay = 0;

        private Dialogue()
        {
        }

        public Dialogue SetSpeaker(string speaker)
        {
            OverrideSpeaker = speaker;
            return this;
        }

        public Dialogue Text(string text, DialogueSpeed speed = DialogueSpeed.Medium, bool withSpeaker = true)
        {
            stages.Add(new DialogueStage
            {
                Type = DialogueStageType.Text,
                Text = text,
                Speed = speed,
                WithSpeaker = withSpeaker,
            });
            return this;
        }
        
        public Dialogue Clear()
        {
            stages.Add(new DialogueStage
            {
                Type = DialogueStageType.Clear,
            });
            return this;
        }

        public Dialogue Delay(int delay)
        {
            stages.Add(new DialogueStage
            {
                Type = DialogueStageType.Delay,
                Delay = delay,
            });
            return this;
        }

        public static Dialogue Create()
        {
            return new Dialogue().Clear();
        }
    }

    public enum DialogueStageType
    {
        Text,
        Delay,
        Clear
    }
    
    public enum DialogueSpeed
    {
        Fast, //25ms
        Medium, //50ms
        Slow, //100ms
        Instant //0ms
    }

    public struct DialogueStage
    {
        public DialogueStageType Type;
        public string Text;
        public DialogueSpeed Speed;
        public int Delay;
        public bool WithSpeaker;
    }
}