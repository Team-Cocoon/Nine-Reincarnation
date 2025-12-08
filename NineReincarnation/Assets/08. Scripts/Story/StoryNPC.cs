using Player.Controller;
using UnityEngine;

public class StoryNPC : MonoBehaviour
{
    [SerializeField] private Animator       _npcAnimator;
    [SerializeField] private SpriteRenderer _npcSpriteRenderer;

    public Animator       NpcAnimator    => _npcAnimator;
    public SpriteRenderer SpriteRenderer => _npcSpriteRenderer;

    public void Flip(PlayerDirection playerDirection)
    {
        _npcSpriteRenderer.flipX = playerDirection == PlayerDirection.Left ? true : false;
    }
}
