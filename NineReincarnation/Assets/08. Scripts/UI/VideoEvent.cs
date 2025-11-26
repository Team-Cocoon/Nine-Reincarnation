using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoEvent : MonoBehaviour
{
    public VideoPlayer _targetVideo;
    public UnityEvent _event;
    void Start()
    {
        _targetVideo.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        _event.Invoke();
    }
}
