using UnityEngine;
using UnityEngine.UI;

public class ButtonLimit : MonoBehaviour
{
    [SerializeField] private int maxCount = 3;
    int count = 0;

    private void Awake()
    {
        Button button = GetComponent<Button>();

        button.onClick.AddListener(
            () =>
            {
                count++;
                if(count >= maxCount)
                {
                    button.gameObject.SetActive(false);
                }
            }
        );
    }
}
