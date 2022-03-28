using System;
using Other;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        private Camera mainCamera;
        public Camera MainCamera => mainCamera;
        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public bool ScreenToWorldRay(Vector2 position,LayerMask mask,out RaycastHit hit,int maxDistance=100)
        {
            var ray = MainCamera.ScreenPointToRay(position);
            return Physics.Raycast(ray, out hit,maxDistance,mask);
        }
    }
}
