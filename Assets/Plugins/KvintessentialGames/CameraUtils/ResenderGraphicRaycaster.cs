using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KvinterGames.CameraUtils
{
    public class ResenderGraphicRaycaster : PhysicsRaycaster
    {
        public BaseRaycaster[] graphicRaycaster;
        
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            
            foreach (var raycaster in graphicRaycaster)
            {
                raycaster.Raycast(eventData, resultAppendList);
            }
            
            DrawRaycast(eventData);
        }
        
        private void DrawRaycast(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.isValid)
            {
                Debug.DrawLine(eventData.pointerCurrentRaycast.worldPosition,
                    eventData.pointerCurrentRaycast.worldPosition + Vector3.up * 10, Color.red);
            }
        }

        public override Camera eventCamera => GetComponent<Camera>();
    }
}