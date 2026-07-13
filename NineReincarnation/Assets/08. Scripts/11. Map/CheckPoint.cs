using DG.Tweening;
using Player.Controller;
using UnityEngine;

public class CheckPoint : MonoBehaviour, ICollidable
{
    [SerializeField] private bool _playSFX = true;
    [SerializeField] private Transform _lamp;
    [SerializeField] private GameObject _light;
    [SerializeField] private BoxCollider2D _collider;

    [Header("Pendulum Settings")]
    [SerializeField] private float startAngle = 45f;      // 처음 튕겨나갈 각도
    [SerializeField] private float swingTime = 0.4f;      // 한 번 흔들리는 데 걸리는 시간 (작을수록 빠름)
    [SerializeField] private int swingCount = 6;          // 흔들리는 횟수
    [SerializeField, Range(0f, 1f)] private float damping = 0.6f; // 갈수록 흔들림이 줄어드는 비율

    private PlayerController player;

    public void Enter(GameObject go = null)
    {
        if (_playSFX)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.SavePoint);
        }
        player = go.GetComponent<PlayerController>();
        player?.SetCheckPoint(transform.position);
        if (player != null)
        {
            _collider.enabled = false;
            _light.SetActive(true);
            Sequence swingSequence = DOTween.Sequence();
            float currentAngle = startAngle;

            for (int i = 0; i < swingCount; i++)
            {
                // Ease.InOutSine: 양 끝에서 속도가 부드럽게 줄어드는 자연스러운 진자 운동 형태
                swingSequence.Append(_lamp.DOLocalRotate(new Vector3(0, 0, currentAngle), swingTime)
                             .SetEase(Ease.InOutSine));
                
                // 다음번엔 반대 방향으로, 각도는 감쇠(damping) 비율만큼 줄어듦
                currentAngle = -(currentAngle * damping); 
            }

            // 마지막에는 다시 0도(원래 위치)로 돌아오게 함
            swingSequence.Append(_lamp.DOLocalRotate(Vector3.zero, swingTime)
                         .SetEase(Ease.InOutSine));
        }
    }

    public void Exit(GameObject go = null)
    {
        return;
    }
}
