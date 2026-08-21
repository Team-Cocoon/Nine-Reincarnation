using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineEvent : MonoBehaviour, IEventInterface
{
    [SerializeField] private PlayableDirector _director;

    public async UniTask ExecuteEvent(int index)
    {
        await PlayTimelineTask();
    }

    private async UniTask PlayTimelineTask()
    {
        _director.Play();

        await UniTask.WaitForSeconds((float)_director.duration);
    }
}
