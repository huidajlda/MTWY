using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Transition 
{
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string sceneToGo;//切换场景的名称
        public Vector3 positionToGo;//场景的目标位置
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) 
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}
