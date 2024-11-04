using System;
using Animancer;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Effects;
using KvinterGames;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class RealWorldService
    {
        public async UniTask ShowRealWorld(int index = 0)
        {
            var settings = Game.Instance.RealWorldServiceSettings;
            
            foreach (var settingsTwist in settings.Twists)
            {
                settingsTwist.SetActive(false);
            }
            
            var clip = SoundController.Instance.PlaySound("glitch", 0.05f, 0.3f, fadeIn: true);
            settings.GlitchIncreaseScreenEffect.StartGlitch();
            await UniTask.Delay(TimeSpan.FromSeconds(settings.GlitchIncreaseScreenEffect.Duration));
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            
            settings.Twists[index].SetActive(true);
            await settings.RealWorldImage.DOFade(1, 0.1f).ToUniTask();
            await settings.RealWorldCamera.transform.DOLocalMove(settings.LookAtWorldCameraPosition, 3);
            await settings.RealWorldCamera.transform.DOLocalRotate(settings.LookAtWorldCameraRotation, 1).ToUniTask();
            
            var state = settings.AnimancerComponent.Play(settings.ShowTwistClip);
            SoundController.Instance.PlaySound("print", 0.1f);
            settings.ShowTwistClip.Events.SetCallback("list", () => SoundController.Instance.PlaySound("list"));
            state.Time = 0;
            state.Speed = 1;
            await UniTask.WaitUntil(() => state.NormalizedTime >= 1);
            await UniTask.WaitUntil(() => Input.anyKey || Input.touchCount > 0);
            settings.ShowTwistClip.Events.SetCallback("list", () => {});

            state = settings.AnimancerComponent.Play(settings.ShowTwistClip);
            state.Speed = -1;
            await UniTask.Delay(1000);
            settings.GlitchIncreaseScreenEffect.StartGlitch(true);
            settings.RealWorldCamera.transform.DOLocalMove(settings.LookAtPcCameraPosition, 1);
            await settings.RealWorldCamera.transform.DOLocalRotate(settings.LookAtPcCameraRotation, 1).ToUniTask();
            settings.RealWorldImage.DOFade(0, 0.1f);
        }
    }
    
    [Serializable]
    public class RealWorldServiceSettings
    {
        public Camera RealWorldCamera;
        public RawImage RealWorldImage; 
        
        public GlitchIncreaseScreenEffect GlitchIncreaseScreenEffect;
        
        public Vector3 LookAtPcCameraPosition;
        public Vector3 LookAtPcCameraRotation;
        
        public Vector3 LookAtWorldCameraPosition;
        public Vector3 LookAtWorldCameraRotation;

        [Space] public GameObject[] Twists;

        [Space] public AnimancerComponent AnimancerComponent;
        public ClipTransition ShowTwistClip;
    }
}