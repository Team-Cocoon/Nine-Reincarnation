using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class StoryObstacleH : MonoBehaviour, IEventInterface
{
    [SerializeField] private float _targetAlpha = 0.15f;
    [SerializeField] private List<SpriteRenderer> _spriteRenderers;

    [SerializeField] private Collider2D _collider2D;

    private List<UniTask> _spriteRenderFade = new List<UniTask>();

    public async UniTask ExecuteEvent(int index)
    {
        await ExecutePhaseSequence();
    }

    private async UniTask ExecutePhaseSequence()
    {
        foreach (var spriteRenderer in _spriteRenderers)
        {
            if (spriteRenderer == null) continue;

            _spriteRenderFade.Add(
                spriteRenderer.DOFade(_targetAlpha, 0.5f).SetLink(gameObject).ToUniTask()
            );
        }

        await UniTask.WhenAll(_spriteRenderFade);

        if (_collider2D != null)
        {
            _collider2D.enabled = false;
        }
    }
}
