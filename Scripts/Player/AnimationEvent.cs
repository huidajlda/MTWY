using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    //������·��Ч
    public void FootstepSound() 
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }
}
