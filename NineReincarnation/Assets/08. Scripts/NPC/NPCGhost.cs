using UnityEngine;

public class NPCGhost : NPC
{
    public override void StartAnim(string animName)
    {
        _currentAnimName = animName;
        switch (animName) 
        {
            case "Ghost_Down":
                _animator.SetTrigger("isDown");
                break;
            case "Ghost_Wake":
                _animator.SetTrigger("isWake");
                break;
            case "Ghost_Stretch":
                _animator.SetTrigger("isStretch");
                break;
            case "Ghost_Surprised":
                _animator.SetTrigger("isSurprised");
                break;
            case "Ghost_Laughing":
                _animator.SetTrigger("isLaughing");
                break;
            case "Ghost_Finger":
                _animator.SetTrigger("isFinger");
                break;
            case "Ghost_LookAround":
                _animator.SetTrigger("isLookAround");
                break;
            case "Ghost_NoWake":
                _animator.SetTrigger("isNoWake");
                break;
            case "Ghost_Anger":
                _animator.SetTrigger("isAnger");
                break;
            case "Ghost_Whisper":
                _animator.SetTrigger("isWhisper");
                break;
        }
    }
}
