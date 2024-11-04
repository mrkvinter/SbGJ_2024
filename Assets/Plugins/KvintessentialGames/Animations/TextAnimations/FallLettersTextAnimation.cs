using KvinterGames.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public class FallLettersTextAnimation : PlayableTextAnimation
    {
        [Title("Fall Settings")]
        [SerializeField] private float height = 1;
        [SerializeField] private float fallSpeed = 1;
        
        protected override void ApplyCharTransform(ref CharTransformation charTransformation, float progress)
        {
            var sin = Mathf.Sin(charTransformation.Position.x);
            charTransformation.PositionAdditionalData[this] = Vector3.down * (height * progress * (sin * 0.5f + 0.6f));
            charTransformation.RotationAdditionalData[this] = 45 * progress * sin;
            charTransformation.ColorAdditionalData[this] = charTransformation.Color.WithAlpha((byte)(255 * (1f - progress)));
            
            charTransformation.Position += charTransformation.PositionAdditionalData[this];
            charTransformation.Rotation += charTransformation.RotationAdditionalData[this];
            charTransformation.Color = charTransformation.ColorAdditionalData[this];
        }
    }
}