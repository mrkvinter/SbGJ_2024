using KvinterGames.Animations.TransformAnimations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.KvintessentialGames.Animations.TransformAnimations
{
    public abstract class BaseSineTransformAnimation : BaseTransformAnimation
    {
        [Title("Sine Settings")]
        [SerializeField] private float speed = 5;
        [SerializeField] private float sinFrequency = 0.1f;
        [SerializeField] private float timeOffset;
        [SerializeField] private bool useAnimationCurve;
        [SerializeField, ShowIf(nameof(useAnimationCurve))] private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        
        protected float GetSinValue(float additionalOffset = 0)
        {
            var offset = Time.time * speed * 1f / sinFrequency + timeOffset;
            var sin = Mathf.Sin((offset + additionalOffset) * sinFrequency);
            if (useAnimationCurve)
            {
                sin = animationCurve.Evaluate(sin);
            }
                
            return sin;
        }
    }
}