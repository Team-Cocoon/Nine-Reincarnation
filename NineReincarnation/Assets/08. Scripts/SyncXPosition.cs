using UnityEngine;

public class SyncXPosition : MonoBehaviour
{
    [Header("따라갈 타겟 오브젝트")]
    [SerializeField] 
    private Transform targetTransform; 

    void LateUpdate()
    {
        // 타겟이 정상적으로 할당되어 있는지 확인
        if (targetTransform != null)
        {
            // 현재 오브젝트의 위치 값을 가져옴
            Vector3 newPosition = transform.position;

            // X 좌표만 타겟의 X 좌표 값으로 덮어씌움
            newPosition.x = targetTransform.position.x;

            // 변경된 위치 값을 현재 오브젝트에 다시 적용
            transform.position = newPosition;
        }
    }
}