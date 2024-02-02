using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    //播放走路音效
    public void FootstepSound() 
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }
}
