using UnityEngine;

public class ListPanel : MonoBehaviour
{
    [Header("--- 페이드 시킬 오브젝트 ---")]
    [SerializeField] private GameObject _fadeImage;

    public void OpenProfile()
    {
        _fadeImage.SetActive(true);
        //UIEventHandler.OnOpenProfileUI();
    }
}
