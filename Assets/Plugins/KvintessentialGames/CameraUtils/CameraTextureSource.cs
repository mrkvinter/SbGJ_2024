using System;
using UnityEngine;

namespace KvinterGames.CameraUtils
{
    public class CameraTextureSource : MonoBehaviour
    {
        public RenderTexture renderTexture;
        public Vector2Int screenSize;
     
        private Camera camera;
        
        public event Action<RenderTexture> OnTextureCreated;

        private void Awake()
        {
            camera = GetComponent<Camera>(); 
            CreateTexture();
        }

        
        private void CreateTexture()
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                renderTexture = null;
            }

            renderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                filterMode = FilterMode.Point,
                useMipMap = false
            };
            renderTexture.useMipMap = false;
            renderTexture.Create();
            
            camera.targetTexture = renderTexture;
            
            screenSize = new Vector2Int(Screen.width, Screen.height);
            
            OnTextureCreated?.Invoke(renderTexture);
        }
        
        private void Update()
        {
            if (screenSize.x != Screen.width || screenSize.y != Screen.height)
            {
                CreateTexture();
            }
        }
    }
}