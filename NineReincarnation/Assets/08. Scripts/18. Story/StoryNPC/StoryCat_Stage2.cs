using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryCat_Stage2 : StoryCat
{
    [SerializeField] private StoryAnna _anna;

    protected override void Awake()
    {
        base.Awake();

        _dialogueManager.DialogueEndAddListener(DisableSelf);
    }

    public override async UniTask ExecuteEvent(int index)
    {
        if (index == 0)
        {
            await MoveToTarget(true);
        }
        if(index == 1)
        {
            _speed = _runSpeed;
            await UniTask.WhenAll(
                _anna.ExecuteEvent(index),
                MoveToTarget(true)
                );
            //await _fadeUI.UIEvent_FadeOut();
        }
    }

    public override void FinishEvent(int index)
    {
        if (index == 0)
        {
            FinishMoveToTarget();
        }
        if (index == 1)
        {
            _speed = _runSpeed;
            _anna.FinishEvent(index);
            FinishMoveToTarget();
        }
    }
}
