using TMPro;
using UnityEngine;

namespace Code
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField, TextArea] private string russianText;
        [SerializeField, TextArea] private string englishText;
        
        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            LanguageController.OnLanguageChanged += UpdateText;
            
            UpdateText(LanguageController.Current);
        }
        
        private void UpdateText(Language language)
        {
            text.text = language == Language.English ? englishText : russianText;
        }
    }
}