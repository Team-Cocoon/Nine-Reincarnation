using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExcelData;
using UnityEngine;

public class StoryAnimationManager : MonoBehaviour
{
    [Header("----Animator-----")]
    [SerializeField] private Animator[] _animators;

    private Dictionary<string, Animator> _animatorDict = new();

    private void Awake()
    {
        foreach (Animator animator in _animators)
        {
            string name = animator.gameObject.name;
            _animatorDict.Add(name, animator);
        }
    }

    public async UniTask ExcuteAnimation(AnimationClass animationData)
    {
        string name = animationData.Name;
        string animationName = animationData.AnimationName;

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(animationName))
        {
            Animator animator = _animatorDict[name];

            animator.Play(animationName);

            //애니메이션 상태 감지용
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            //Play()가 실제로 적용되려면 '다음 프레임'까지 최소 1프레임을 대기
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());

            //상태 진입 대기
            // Play()가 실패했거나 즉시 다른 상태로 전환되는 경우를 대비
            await UniTask.WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(0).IsName(animationName),
                cancellationToken: this.GetCancellationTokenOnDestroy());

            await UniTask.WaitUntil(() =>
            {
                var current = animator.GetCurrentAnimatorStateInfo(0);

                // (상태를 빠져나갔거나 || 상태가 끝났다면) 대기 종료
                return !current.IsName(animationName) || current.normalizedTime >= 1.0f;

            }, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
}
