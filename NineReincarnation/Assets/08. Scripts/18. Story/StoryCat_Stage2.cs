using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryCat_Stage2 : StoryCat
{
    public override async UniTask ExecuteEvent(int index)
    {
        if (index == 0)
        {
            await MoveToTarget(false);
        }
    }
}
