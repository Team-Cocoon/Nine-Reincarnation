using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using TMPro;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Navigate", story: "[Enemy] Navigate From [ChaseStopPosition]", category: "Action", id: "6cfa77aadcc624886a56ccc0f642cb3f")]
public partial class NavigateAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform>  Enemy;
    [SerializeReference] public BlackboardVariable<Vector2>    ChaseStopPosition;
    [SerializeReference] public BlackboardVariable<float>      Duration = new(5f); // 이동시간
    [SerializeReference] public BlackboardVariable<Vector2>    Rectangle  = new(Vector2.zero);

    private Vector2 _currentVelocity = Vector2.zero;
    private Vector2 _targetPosition  = Vector2.zero;
    protected override Status OnStart()
    {
        _targetPosition = ChaseStopPosition.Value + GetRandomPosition();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Enemy.Value.position = Vector2.SmoothDamp((Vector2)Enemy.Value.position, _targetPosition, ref _currentVelocity, Duration);

        float distance = Vector2.Distance(Enemy.Value.position, _targetPosition);

        if (distance <= 0.01f)
        {
            _targetPosition = ChaseStopPosition.Value + GetRandomPosition();
        }

        return Status.Running;
    }

    private Vector2 GetRandomPosition()
    {
        float x = Rectangle.Value.x;
        float y = Rectangle.Value.y; 

        return new Vector2(UnityEngine.Random.Range(-x, x), UnityEngine.Random.Range(-y, y));
    }
}

