using UnityEngine;

namespace Code.Utilities
{
    public static class ContentUtility
    {
        public static Sprite LoadSprite(string path)
        {
            return Resources.Load<Sprite>(path);
        }
        
        public static Sprite LoadSpriteFromSpritesheet(string imageName, string spriteName)
        {
            var all = Resources.LoadAll<Sprite>(imageName);
 
            foreach(var s in all)
            {
                if (s.name == spriteName)
                {
                    return s;
                }
            }
            return null;
        }
    }
}