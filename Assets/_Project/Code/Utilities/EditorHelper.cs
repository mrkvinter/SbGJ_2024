#if UNITY_EDITOR
namespace Code.Utilities
{
    public static class EditorHelper
    {
        
        [UnityEditor.MenuItem("Tools/Clear PlayerPrefs")]
        public static void ClearPlayerPrefs()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
        }
    }
}
#endif