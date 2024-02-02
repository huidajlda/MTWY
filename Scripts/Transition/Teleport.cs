using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Transition 
{
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string sceneToGo;//�л�����������
        public Vector3 positionToGo;//������Ŀ��λ��
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) 
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}
