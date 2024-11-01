using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.KvintessentialGames.Animations.TransformAnimations
{
    public class SineRotationTransformAnimation : BaseSineTransformAnimation
    {
        [Title("Sine Rotation Settings")]
        [SerializeField] private Vector3 magnitude;

        protected override void ApplyTransform()
        {
            var rotation = GetSinValue() * magnitude;

            transform.rotation = originalRotation * Quaternion.Euler(rotation);
        }
    }
}