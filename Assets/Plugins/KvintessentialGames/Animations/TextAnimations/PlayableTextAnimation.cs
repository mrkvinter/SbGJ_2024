using Sirenix.OdinInspector;
using UnityEngine;

namespace KvintessentialGames.TextAnimations
{
    public abstract class PlayableTextAnimation : BaseTextAnimation
    {
        [Title("Playable Settings")]
        [SerializeField] private float duration = 1;
        
        private bool isPlaying;
        private float progress;
        
        public bool IsPlaying => isPlaying;
        [Button]
        public void Play()
        {
            isPlaying = true;
            progress = 0;
        }

        public override void OnStartAnimation()
        {
            if (isPlaying)
            {
                progress += Time.deltaTime;
            }
        }

        public override void ApplyCharTransform(ref CharTransformation charTransformation)
        {
            if (isPlaying)
            {
                if (progress >= duration)
                {
                    progress = duration;
                    isPlaying = false;
                    ApplyCharTransform(ref charTransformation, 1);
                    return;
                }

                ApplyCharTransform(ref charTransformation, progress / duration);
            }
        }
        
        protected abstract void ApplyCharTransform(ref CharTransformation charTransformation, float progress);
    }
    
}