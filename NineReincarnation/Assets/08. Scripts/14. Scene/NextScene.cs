using UnityEngine;
using VContainer;

public class NextScene : MonoBehaviour
{
    [Inject] private StageManager _stageManager;
    private bool isExecuting = false; 

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !isExecuting)
        {
            isExecuting = true;
            _stageManager.GoToNextMap().Forget();
        }
    }
}
