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


    //public override void TriggerEvent(string eventName, Action triggerAction)
    //{
    //    if (triggerAction != null)
    //    {
    //        _triggerAction = triggerAction;
    //    }
    //    switch (eventName)
    //    {
    //        case "Move":
    //            StartCoroutine(Move());
    //            break;
    //    }
    //}

    //private IEnumerator Move()
    //{
    //    //Vector3 position = transform.position;

    //    //while (transform.position.x <= _checkPoint.transform.position.x)
    //    //{
    //    //    position.x += _speed * Time.deltaTime;
    //    //    transform.position = position;

    //    //    yield return null;
    //    //}
    //    //_triggerAction?.Invoke();
    //    //_triggerAction = null;
    //    Vector3 position = transform.position;
    //    float startY = position.y; // 움직이기 전의 초기 Y 위치를 저장합니다.
    //    float elapsedTime = 0f;    // 시간을 누적할 변수입니다.

    //    while (transform.position.x <= _checkPoint.transform.position.x)
    //    {
    //        elapsedTime += Time.deltaTime;

    //        // 1. X축으로 전진하는 것은 기존과 동일합니다.
    //        position.x += _speed * Time.deltaTime;

    //        // 2. Y축 위치를 사인 함수를 이용해 계산합니다.
    //        // 초기 Y 위치를 기준으로, _amplitude 만큼의 높이로, _frequency 만큼의 빠르기로 움직입니다.
    //        float wave = _amplitude * Mathf.Sin(elapsedTime * _frequency);
    //        position.y = startY + wave;

    //        // 계산된 최종 위치를 적용합니다.
    //        transform.position = position;

    //        yield return null;
    //    }

    //    // 루프가 끝난 후 원래 Y 높이로 부드럽게 복원하고 싶다면 아래 코드를 사용할 수 있습니다.
    //    // transform.DOMoveY(startY, 0.2f); // (DOTween 라이브러리 필요)

    //    _triggerAction?.Invoke();
    //    _triggerAction = null;
    //}


    public async UniTask ExecuteEvent(int index)
    {
        await Move();
    }

    public void FinishEvent(int index)
    {
        return;
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

        gameObject.SetActive(false);
    }
}
