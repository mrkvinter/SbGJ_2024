using UnityEngine;

namespace KvinterGames.CameraUtils
{
    public class MeshMaterialTextureCameraSetter : BaseTextureCameraSetter
    {
        public MeshRenderer meshRenderer;
        public Material material;
        public string textureName = "_BaseMap";

        private Material instanceMaterial;

        // protected override void SetTexture()
        // {
        //     if (textureCamera == null || meshRenderer == null || material == null)
        //     {
        //         return;
        //     }
        //     
        //     if (instanceMaterial == null)
        //     {
        //         instanceMaterial = new Material(material);
        //         meshRenderer.material = instanceMaterial;
        //     }
        //
        //     if (cameraTextureSource.renderTexture != null)
        //     {
        //         instanceMaterial.SetTexture(textureName, cameraTextureSource.renderTexture);
        //         return;
        //     }
        //
        //     var texture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
        //     {
        //         filterMode = FilterMode.Point,
        //         useMipMap = false
        //     };
        //     
        //     
        //     textureCamera.targetTexture = texture;
        //     SetTexture(texture);
        //     
        //     cameraTextureSource.renderTexture = texture;
        // }

        protected override void SetTexture(RenderTexture texture)
        {
            if (instanceMaterial == null)
            {
                instanceMaterial = new Material(material);
                meshRenderer.material = instanceMaterial;
            }

            instanceMaterial.SetTexture(textureName, texture);
        }
    }
}