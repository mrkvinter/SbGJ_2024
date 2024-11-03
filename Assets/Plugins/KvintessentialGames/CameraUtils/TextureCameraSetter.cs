using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KvinterGames.CameraUtils
{
    public class TextureCameraSetter : MonoBehaviour
    {
        public Camera textureCamera;
        public RawImage renderTexture;

        private void Awake()
        {
            SetTexture();
        }

        private void SetTexture()
        {
            var rectTransform = renderTexture.rectTransform.rect;
            var texture = new RenderTexture((int)rectTransform.width, (int)rectTransform.height, 16, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
            texture.filterMode = FilterMode.Point;
            texture.useMipMap = false;
            texture.Create();

            textureCamera.targetTexture = texture;
            renderTexture.texture = textureCamera.targetTexture;
        }

        private void OnValidate()
        {
            if (textureCamera == null || renderTexture == null)
            {
                return;
            }

            SetTexture();
        }
    }
}