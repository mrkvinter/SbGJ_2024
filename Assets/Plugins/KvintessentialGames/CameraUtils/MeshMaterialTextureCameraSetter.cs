using Sirenix.OdinInspector;
using UnityEngine;

namespace KvinterGames.CameraUtils
{
    public class MeshMaterialTextureCameraSetter : BaseTextureCameraSetter
    {
        public MeshRenderer meshRenderer;
        public Material material;
        public string textureName = "_BaseMap";

        private Material instanceMaterial;

        protected override void SetTexture(RenderTexture texture)
        {
            if (instanceMaterial == null)
            {
                instanceMaterial = new Material(material);
                meshRenderer.material = instanceMaterial;
            }

            instanceMaterial.SetTexture(textureName, texture);
        }

        [Button]
        private void SetTexture()
        {
            if (textureCamera.gameObject.TryGetComponent(out cameraTextureSource))
                SetTexture(cameraTextureSource.renderTexture);
        }
    }
}