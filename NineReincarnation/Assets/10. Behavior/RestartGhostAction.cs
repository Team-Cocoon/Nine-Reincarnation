using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RestartGhost", story: "RestartGhost", category: "Action", id: "f5df05c896c9ffbb65ee06c7213d8747")]
public partial class RestartGhostAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Enemy;
    [SerializeReference] public BlackboardVariable<bool> IsTargetDetected;
    [SerializeReference] public BlackboardVariable<bool> IsTargetCatched;
    [SerializeReference] public BlackboardVariable<bool> IsClear;
    [SerializeReference] public BlackboardVariable<Vector2> ChaseStopPosition;
    [SerializeReference] public BlackboardVariable<ChaseState> currentState;
    [SerializeReference] public BlackboardVariable<ChaseState> prevState;


    protected override Status OnStart()
    {
        Reset();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    private void Reset()
    {
        Enemy.Value.GetComponent<ChaseGhost>().Restart();
        IsTargetDetected.Value = true;
        IsTargetCatched.Value = false;
        currentState.Value = ChaseState.Idle;
        prevState.Value = ChaseState.Idle;
        ChaseStopPosition.Value = Enemy.Value.position;
        IsClear.Value = false;

        Debug.Log("초기화 실행");
    }
}

