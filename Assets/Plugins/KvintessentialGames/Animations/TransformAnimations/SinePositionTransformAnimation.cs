using Plugins.KvintessentialGames.Animations.TransformAnimations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KvinterGames.Animations.TransformAnimations
{
    public class SinePositionTransformAnimation : BaseSineTransformAnimation
    {
        [Title("Sine Position Settings")]
        [SerializeField] private Vector3 magnitude;

        protected override void ApplyTransform()
        {
            var x = GetSinValue(originalPosition.y) * magnitude.x;
            var y = GetSinValue(originalPosition.x) * magnitude.y;
            var z = GetSinValue(originalPosition.z) * magnitude.z;
            
            transform.localPosition = new Vector3(x, y, z);
        }
    }
}