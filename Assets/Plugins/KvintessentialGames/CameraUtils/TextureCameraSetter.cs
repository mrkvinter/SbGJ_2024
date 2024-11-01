using UnityEngine;
using UnityEngine.UI;

namespace KvinterGames.CameraUtils
{
    public class TextureCameraSetter : MonoBehaviour
    {
        public Camera textureCamera;
        public RawImage renderTexture;

        private void Awake()
        {
            var rectTransform = renderTexture.rectTransform.rect;
            var texture = new RenderTexture((int)rectTransform.width, (int)rectTransform.height, 16, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
            texture.filterMode = FilterMode.Point;
            texture.useMipMap = false;
            texture.Create();

            textureCamera.targetTexture = texture;
            renderTexture.texture = textureCamera.targetTexture;
        }
    }
}