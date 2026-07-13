using UnityEngine;
using System.Collections.Generic;
using EventHandler; // 플레이어 코드에 있던 이벤트 핸들러 네임스페이스
using Enemy.Move;   // EnemyMove 네임스페이스

public class PlatformResetManager : MonoBehaviour
{
    [SerializeField] private EnemyMove[] _platforms;

    private void OnEnable()
    {
        // 플레이어 사망 이벤트 구독 
        // (※ 사용 중이신 GameEventHandler의 정적 이벤트 이름(예: OnPlayerDead)에 맞게 매칭해 주세요)
        GameEventHandler.OnPlayerDead += ResetAllPlatforms;
    }

    private void OnDisable()
    {
        // 이벤트 해제 (메모리 누수 방지)
        GameEventHandler.OnPlayerDead -= ResetAllPlatforms;
    }

    /// <summary>
    /// 플레이어 사망 시 호출되어 모든 플랫폼을 초기화합니다.
    /// </summary>
    private void ResetAllPlatforms()
    {
        Debug.Log("<color=orange>[PlatformResetManager]</color> 플레이어 사망 감지. 모든 플랫폼을 초기화합니다.");
        
        foreach (var platform in _platforms)
        {
            if (platform != null)
            {
                platform.ResetToInitialState();
            }
        }
    }
}