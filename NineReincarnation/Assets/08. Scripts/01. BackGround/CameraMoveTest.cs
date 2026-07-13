using UnityEngine;

public class CameraMoveTest : MonoBehaviour
{
    void Update()
    {
        transform.position += Vector3.right * 0.5f * Time.deltaTime;
    }
}
