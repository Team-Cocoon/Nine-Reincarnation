using UnityEngine;

public class TutorialBox : MonoBehaviour
{
    [Header("튜토리얼 창")]
    [SerializeField] private GameObject _tutorialObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);
        if (layerName == "Player")
        {
            _tutorialObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);
        if (layerName == "Player")
        {
            _tutorialObject.SetActive(false);
        }
    }
}