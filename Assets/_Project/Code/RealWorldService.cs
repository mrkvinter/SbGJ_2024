using System;
using Animancer;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            
            settings.Twists[index].SetActive(true);
            await settings.RealWorldImage.DOFade(1, 0.1f).ToUniTask();
            settings.RealWorldCamera.transform.DOLocalMove(settings.LookAtWorldCameraPosition, 1);
            await settings.RealWorldCamera.transform.DOLocalRotate(settings.LookAtWorldCameraRotation, 1).ToUniTask();
            
            var state = settings.AnimancerComponent.Play(settings.ShowTwistClip);
            state.Time = 0;
            state.Speed = 1;
            await UniTask.WaitUntil(() => state.NormalizedTime >= 1);
            await UniTask.WaitUntil(() => Input.anyKey || Input.touchCount > 0);
            
            state = settings.AnimancerComponent.Play(settings.ShowTwistClip);
            state.Speed = -1;
            await UniTask.Delay(1000);
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
        
        public Vector3 LookAtPcCameraPosition;
        public Vector3 LookAtPcCameraRotation;
        
        public Vector3 LookAtWorldCameraPosition;
        public Vector3 LookAtWorldCameraRotation;

        [Space] public GameObject[] Twists;

        [Space] public AnimancerComponent AnimancerComponent;
        public AnimationClip IdleTwistClip;
        public AnimationClip ShowTwistClip;
    }
}