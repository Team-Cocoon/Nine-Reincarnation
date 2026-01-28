using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Enemy] Chase [Target] With [isTargetDetected]", category: "Action", id: "b4312b99ee1b375ba96e55ed65f69edc")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Enemy;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<bool> IsTargetDetected;
    [SerializeReference] public BlackboardVariable<bool> IsTargetCatched;

    [SerializeReference] public BlackboardVariable<float> TurnSpeed = new(5f);
    [SerializeReference] public BlackboardVariable<float> MaxSpeed = new(5f); // 최대 이동 속도
    [SerializeReference] public BlackboardVariable<Vector2> ChaseStopPotion = new(Vector2.zero);
    [SerializeReference] public BlackboardVariable<Vector2> EndVectorYRange = new(Vector2.zero);

    private Vector2 _currentVelocity = Vector2.zero;
    protected override Status OnStart()
    {
        _currentVelocity = Vector2.zero;

        Enemy.Value.GetComponent<ChaseGhost>().SoundPlay();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsTargetDetected.Value)
        {
            Vector2 directionToTarget = (Target.Value.position - Enemy.Value.position).normalized;

            Vector2 desiredVelocity = directionToTarget * MaxSpeed.Value;

            _currentVelocity = Vector2.MoveTowards(_currentVelocity, desiredVelocity, TurnSpeed.Value * Time.deltaTime);

            Enemy.Value.position += (Vector3)_currentVelocity * Time.deltaTime;
        }
        else
        {
            _currentVelocity = Vector2.MoveTowards(_currentVelocity, Vector2.zero, 1.0f);
            Enemy.Value.position += (Vector3)_currentVelocity * Time.deltaTime;

            if (_currentVelocity.magnitude < 0.01f)
            {
                ChaseStopPotion.Value = Enemy.Value.position;

                Vector2 newVector = Enemy.Value.position;
                newVector.y = UnityEngine.Random.Range(EndVectorYRange.Value.x, EndVectorYRange.Value.y);
                ChaseStopPotion.Value = newVector;

                return Status.Success;
            }
        }
        return Status.Running;
    }

}

