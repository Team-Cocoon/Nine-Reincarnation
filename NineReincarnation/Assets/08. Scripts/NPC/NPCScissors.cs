using Cysharp.Threading.Tasks;
using UnityEngine;

public class NPCScissors : NPC, IEventInterface
{
    [Header("이동 속도")]
    [SerializeField] private float _speed = 1f;
    [Header("도착 지점")]
    [SerializeField] private GameObject _checkPoint;

    [Header("Wave Movement Settings")]
    [SerializeField] private float _amplitude = 0.5f; // 위아래로 움직이는 높이 (진폭)
    [SerializeField] private float _frequency = 3f;

    public async UniTask ExecuteEvent()
    {
        await Move();
    }

    public async UniTask Move()
    {
        Vector3 position = transform.position;
        float startY = position.y;
        float elapsedTime = 0f;

        while (transform.position.x <= _checkPoint.transform.position.x)
        {
            elapsedTime += Time.deltaTime;

            position.x += _speed * Time.deltaTime;
            float wave = _amplitude * Mathf.Sin(elapsedTime * _frequency);
            position.y = startY + wave;
            transform.position = position;

            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        }

    }
}
