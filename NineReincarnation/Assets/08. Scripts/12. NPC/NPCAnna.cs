public class NPCAnna : NPC, IEvent
{
    public override void StartAnim(string animName)
    {
        _currentAnimName = animName;
        switch (animName)
        {
            case "Anna_Down":
                _animator.SetTrigger("isDown");
                break;
            case "Anna_Wake":
                _animator.SetTrigger("isWake");
                break;
            case "Anna_Move":
                PlayMoveSound();
                _animator.SetTrigger("isMove");
                break;
            case "Anna_Idle":
                _animator.SetTrigger("isIdle");
                break;
        }
    }

    public void PlayMoveSound()
    {
        AudioManager.Instance.PlayLoopingSfx(AudioManager.LoopSfx.Walk);
    }
}
