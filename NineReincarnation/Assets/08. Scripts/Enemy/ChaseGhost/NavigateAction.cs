using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Navigate", story: "[Enemy] Navigate From [ChaseStopPosition]", category: "Action", id: "6cfa77aadcc624886a56ccc0f642cb3f")]
public partial class NavigateAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Enemy;
    [SerializeReference] public BlackboardVariable<Vector2> ChaseStopPosition;
    [SerializeReference] public BlackboardVariable<float> Duration = new(5f); // 이동시간
    [SerializeReference] public BlackboardVariable<Vector2> XArea = new(Vector2.zero);

    private Vector2 _currentVelocity = Vector2.zero;
    private Vector2 _targetPosition = Vector2.zero;
    protected override Status OnStart()
    {
        _targetPosition = ChaseStopPosition.Value + GetRandomX();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Enemy.Value.position = Vector2.SmoothDamp((Vector2)Enemy.Value.position, _targetPosition, ref _currentVelocity, Duration);

        float distance = Vector2.Distance(Enemy.Value.position, _targetPosition);

        if (distance <= 0.1f)
        {
            _targetPosition = ChaseStopPosition.Value + GetRandomX();
        }

        return Status.Running;
    }

    private Vector2 GetRandomX()
    {
        int sign = UnityEngine.Random.Range(0, 2);

        float x = XArea.Value.x;
        float y = XArea.Value.y;

        if (sign == 1)
        {
            return new Vector2(UnityEngine.Random.Range(x, y), 0);
        }
        else
        {
            return new Vector2(UnityEngine.Random.Range(-y, -x),0);
        }
    }
}

