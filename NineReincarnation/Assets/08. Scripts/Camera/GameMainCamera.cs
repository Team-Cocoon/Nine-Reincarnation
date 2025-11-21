using Unity.Cinemachine;
using UnityEngine;
using static Unity.Cinemachine.CinemachineCore;

public class GameMainCamera : MonoBehaviour
{
    public void BlendCreatedEvent(BlendEventParams evt)
    {
        Time.timeScale = 0f; //시간 정지
        Debug.Log(Time.timeScale);
        Debug.Log("BlendCreatedEvent");
    }

    public void BlendFinishedEvent(ICinemachineMixer mixer, ICinemachineCamera Cam)
    {
        Time.timeScale = 1f; // 되돌림
        Debug.Log(Time.timeScale);
        Debug.Log("BlendFinishedEvent");
    }
}
