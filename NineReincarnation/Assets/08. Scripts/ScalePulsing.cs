using UnityEngine;

public class ScalePulsing : MonoBehaviour
{
    public float speed = 1.0f;
    public float scaleAmount = 0.02f;
    private Vector3 localScale;
    void Awake()
    {
        localScale = transform.localScale;
    }
    void Update()
    {
        transform.localScale = localScale * (1f + Mathf.Sin(Time.time * speed) * scaleAmount);
    }
}