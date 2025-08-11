using System;
using System.Collections;
using UnityEngine;

public class NPCScissors : NPC
{
    [Header("이동 속도")]
    [SerializeField] private float _speed = 1f;
    [Header("도착 지점")]
    [SerializeField] private GameObject _checkPoint;

    public override void TriggerEvent(string eventName, Action triggerAction)
    {
        if (triggerAction != null)
        {
            _triggerAction = triggerAction;
        }
        switch (eventName)
        {
            case "Move":
                StartCoroutine(Move());
                break;
        }
    }

    private IEnumerator Move()
    {
        Vector3 position = transform.position;

        while (transform.position.x <= _checkPoint.transform.position.x)
        {
            position.x += _speed * Time.deltaTime;
            transform.position = position;

            yield return null;
        }
        _triggerAction?.Invoke();
        _triggerAction = null;
    }
}
