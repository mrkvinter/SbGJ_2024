using TMPro;
using UnityEngine;

namespace Code.Visual
{
    public class MatrixEffect : MonoBehaviour
    {
        private TMP_Text[] texts;
        
        [SerializeField] private float frequency = 0.1f;

        private float timer;
        
        private void Awake()
        {
            texts = GetComponentsInChildren<TMP_Text>();
        }
        
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= frequency)
            {
                timer = 0;
                foreach (var text in texts)
                {
                    // var chars = text.text.ToCharArray();
                    // for (var i = 0; i < chars.Length; i++)
                    // {
                    //     if (Random.value < 0.1f)
                    //     {
                    //         chars[i] = (char) Random.Range(33, 126);
                    //     }
                    // }
                    text.text = new string((char) Random.Range(33, 126), 1);
                }
            }
        }
    }
}