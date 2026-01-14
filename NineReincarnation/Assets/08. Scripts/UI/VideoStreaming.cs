using UnityEngine;
using UnityEngine.Video;

public class VideoStreaming : MonoBehaviour
{
    [SerializeField] private VideoPlayer _vp;
    [SerializeField] private string _videoName;

    void Awake()
    {

#if UNITY_WEBGL && !UNITY_EDITOR //웹빌드시
        _vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, _videoName);
        _vp.Play();
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
