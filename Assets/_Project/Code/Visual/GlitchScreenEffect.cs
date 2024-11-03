using System.Linq;
using Code.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Visual
{
    public class GlitchScreenEffect : MonoBehaviour
    {
        [SerializeField] private Transform[] glitchObjects;
        [SerializeField] private float minGlitchInterval = 0.1f;
        [SerializeField] private float maxGlitchInterval = 0.5f;
        [SerializeField] private int minGlitchCount = 1;
        [SerializeField] private int maxGlitchCount = 5;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float probability = 0.5f;
        [SerializeField] private bool invertGlitch = false;

        private int[] indexes;
        private float timer;
        private float nextGlitchTime;
        private bool isGlitching;

        private void Awake()
        {
            indexes = Enumerable.Range(0, glitchObjects.Length).ToArray();
        }

        [Button]
        private void CollectGlitchObjects()
        {
            glitchObjects = transform.GetComponentsInChildren<Transform>(true).Where(t => t != transform).ToArray();
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (isGlitching)
            {
                if (timer >= nextGlitchTime)
                {
                    for (var i = 0; i < glitchObjects.Length; i++)
                    {
                        glitchObjects[i].gameObject.SetActive(invertGlitch);
                    }

                    isGlitching = false;
                    timer = 0;
                }
            }
            else
            {
                if (timer >= frequency)
                {
                    timer = 0;
                    if (Random.value <= probability)
                    {
                        isGlitching = true;
                        nextGlitchTime = Random.Range(minGlitchInterval, maxGlitchInterval);
                        var glitchCount = Random.Range(minGlitchCount, maxGlitchCount);
                        indexes.Shuffle();
                        for (var i = 0; i < glitchCount; i++)
                        {
                            var index = indexes[i];
                            glitchObjects[index].gameObject.SetActive(!invertGlitch);
                        }
                    }
                }
            }
        }
    }
}