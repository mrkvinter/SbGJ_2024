using System;

namespace RG.ContentSystem.Core.Constants
{
    public static class EntryConstantsUtility
    {
        public static string GetEntryName(Type type)
        {
            var name = type.Name;
            if (name.EndsWith("Entry"))
            {
                name = name.Substring(0, name.Length - "Entry".Length);
            }

            return $"{name}Type";
        }
    }
}