using System.Linq;
using Code.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class GlitchIncreaseScreenEffect : MonoBehaviour
    {
        [SerializeField] private RectTransform[] glitchObjects;
        [SerializeField] private float minGlitchInterval = 0.1f;
        [SerializeField] private float maxGlitchInterval = 0.5f;
        [SerializeField] private AnimationCurve glitchIntervalCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float glitchDuration = 1f;
        [SerializeField] private int minGlitchCount = 1;
        [SerializeField] private int maxGlitchCount = 50;

        public float Duration => glitchDuration;

        [Button]
        private void CollectGlitchObjects()
        {
            glitchObjects = transform.GetComponentsInChildren<RectTransform>(true);
        }
        
        [Button]
        public void StartGlitch(bool invert = false)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            StartCoroutine(DoGlitch(invert));
        }
        
        [Button]
        public void HideAll()
        {
            foreach (var glitchObject in glitchObjects)
            {
                glitchObject.gameObject.SetActive(false);
            }
        }
        
        private System.Collections.IEnumerator DoGlitch(bool invert = false)
        {
            var timer = 0f;
            var indexes = Enumerable.Range(0, glitchObjects.Length).ToList();
            while (timer < glitchDuration)
            {
                var t = timer / glitchDuration;
                var glitchCount = Mathf.RoundToInt(Mathf.Lerp(minGlitchCount, maxGlitchCount, t));
                var glitchInterval = Mathf.Lerp(minGlitchInterval, maxGlitchInterval, glitchIntervalCurve.Evaluate(t));
                for (var i = 0; i < glitchObjects.Length; i++)
                {
                    glitchObjects[i].gameObject.SetActive(invert);
                }
                
                indexes.Shuffle();
                for (var i = 0; i < glitchCount; i++)
                {
                    var randomIndex = indexes[i];
                    glitchObjects[randomIndex].gameObject.SetActive(!invert);
                }
                yield return new WaitForSeconds(glitchInterval);
                timer += glitchInterval;
            }
            for (var i = 0; i < glitchObjects.Length; i++)
            {
                glitchObjects[i].gameObject.SetActive(!invert);
            }
        }
    }
}