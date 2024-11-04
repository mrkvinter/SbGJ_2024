using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace KvinterGames.CameraUtils
{
    [ExecuteAlways]
    public class TexturesCameraSetter : BaseTextureCameraSetter
    {
        public RawImage[] renderTexture;

        protected override void SetTexture(RenderTexture texture)
        {
            foreach (var rawImage in renderTexture)
            {
                rawImage.texture = texture;
            }
        }
        
        [Button]
        private void GetTextures()
        {
            renderTexture = GetComponentsInChildren<RawImage>(true);
        }
    }
}