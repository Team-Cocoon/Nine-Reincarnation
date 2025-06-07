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
        }
    }
}
