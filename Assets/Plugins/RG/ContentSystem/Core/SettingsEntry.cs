using System;

namespace RG.ContentSystem.Core
{
    [Serializable]
    public abstract class SettingsEntry : ContentEntry
    {
        public override string Id => GetType().Name;
    }
}