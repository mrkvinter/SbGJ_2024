using UnityEngine;
using UnityEngine.UI;

namespace KvinterGames.CameraUtils
{
    public class TextureCameraSetter : BaseTextureCameraSetter
    {
        public RawImage renderTexture;
        
        // protected override void SetTexture()
        // {
        //     if (cameraTextureSource.renderTexture != null)
        //     {
        //         renderTexture.texture = cameraTextureSource.renderTexture;
        //         return;
        //     }
        //     
        //     // screenSize = new Vector2Int(Screen.width, Screen.height);
        //     var texture = new RenderTexture(screenSize.x, screenSize.y, 16, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
        //     {
        //         filterMode = FilterMode.Point,
        //         useMipMap = false
        //     };
        //
        //     texture.filterMode = FilterMode.Point;
        //     texture.useMipMap = false;
        //     texture.Create();
        //
        //     textureCamera.targetTexture = texture;
        //     SetTexture(texture);
        //     cameraTextureSource.renderTexture = texture;
        // }

        protected override void SetTexture(RenderTexture texture)
        {
            renderTexture.texture = texture;
        }
    }
}