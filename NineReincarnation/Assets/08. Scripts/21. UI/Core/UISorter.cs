using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class UISorter : MonoBehaviour
{
    [SerializeField] private Canvas[] _uiList;

    private void OnValidate() 
    {
        if(_uiList != null)
        {
            SortUIOrder();
        }
    }

    //Order 내림차순 정렬
    private void SortUIOrder()
    {
        int size = _uiList.Length;
        for (int i = 0; i < size; i++)
        {
            Canvas canvas = _uiList[i];
            if (canvas != null)
            {
                canvas.sortingOrder = size - i;
            }
        }
    }
}
