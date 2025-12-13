using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StateToTrigger", story: "[CurrentState] To [Trigger]", category: "Action", id: "3378dcac3f05ce788875586c9a1d6138")]
public partial class StateToTriggerAction : Action
{
    [SerializeReference] public BlackboardVariable<ChaseState> CurrentState;
    [SerializeReference] public BlackboardVariable<string> Trigger;

    private string _defaultString = "Is";

    protected override Status OnStart()
    {
        Trigger.Value = $"{_defaultString}{CurrentState.Value}";
        return Status.Success;
    }
}

