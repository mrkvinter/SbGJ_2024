using UnityEngine;

namespace KvinterGames.CameraUtils
{
    public abstract class BaseTextureCameraSetter : MonoBehaviour
    {
        public Camera textureCamera;

        protected CameraTextureSource cameraTextureSource;

        private void Start()
        {
            if (!textureCamera.gameObject.TryGetComponent(out cameraTextureSource))
            {
                cameraTextureSource = textureCamera.gameObject.AddComponent<CameraTextureSource>();
            }

            this.cameraTextureSource.OnTextureCreated += SetTexture;
            SetTexture(cameraTextureSource.renderTexture);
        }

        // protected abstract void SetTexture();

        protected abstract void SetTexture(RenderTexture texture);

        // private void LateUpdate()
        // {
        //     if (screenSize.x != Screen.width || screenSize.y != Screen.height)
        //     {
        //         cameraTextureSource.renderTexture.Release();
        //         cameraTextureSource.renderTexture = null;
        //
        //         cameraTextureSource.screenSize = new Vector2Int(Screen.width, Screen.height);
        //         SetTexture();
        //         
        //         cameraTextureSource.s
        //     }
        //
        //     if
        // }

        private void OnValidate()
        {
            if (textureCamera == null || cameraTextureSource == null)
            {
                return;
            }

            SetTexture(cameraTextureSource.renderTexture);
        }
    }
}