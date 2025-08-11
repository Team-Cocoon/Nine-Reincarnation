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
        }
    }
}
