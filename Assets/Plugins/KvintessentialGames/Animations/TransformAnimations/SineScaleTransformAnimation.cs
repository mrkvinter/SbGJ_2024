using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.KvintessentialGames.Animations.TransformAnimations
{
    public class SineScaleTransformAnimation : BaseSineTransformAnimation
    {
        [Title("Sine Scale Settings")]
        [SerializeField] private Vector3 scaleOffset;
        [SerializeField] private Vector3 magnitude;
        [SerializeField] private bool usePosition;

        protected override void ApplyTransform()
        {
            var position = transform.position;
            var x = GetSinValue(usePosition ? position.y : 0) * magnitude.x;
            var y = GetSinValue(usePosition ? position.x : 0) * magnitude.y;
            var z = GetSinValue(usePosition ? position.z : 0) * magnitude.z;

            transform.localScale = originalScale + new Vector3(x, y, z) + scaleOffset;
        }
    }
}