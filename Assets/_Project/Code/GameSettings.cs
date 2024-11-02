using System;
using Code.Enemies;
using RG.ContentSystem.Core;
using Sirenix.OdinInspector;

namespace Code
{
    [Serializable]
    public class GameSettings : SettingsEntry
    {
        public ContentRef<ChallengeEntry>[] Challenges;

        [Title("===TEST===")] 
        public bool AutoSelectHpAndShield;
    }
}