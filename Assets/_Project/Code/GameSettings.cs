using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public static string Path => "GameSettings";
    }
}