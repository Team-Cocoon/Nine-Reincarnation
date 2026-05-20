using UnityEngine;

public enum BackgroundType
{
    Wall,
    Ground
}

public class ParallaxInfiniteGround : MonoBehaviour
{
    [SerializeField] private BackgroundType _type;
    [Header("카메라 설정")]
    public Transform cam;
    
    [Header("페럴렉스 설정")]
    public float parallaxSpeedX = 0.5f; 
    public float parallaxSpeedY = 0.5f; 
    public float topLimitY = 0f;
    
    [Header("크기 설정")]
    public float quadWidth = 30f; 
    public float quadHeight = 20f; 
    [SerializeField] private Transform _transform;
    private Material mat;
    private string texturePropertyName = "_BaseMap";

    void Start()
    {
        if (cam == null) cam = Camera.main.transform;
        
        // 런타임 인스턴스 생성을 통한 원본 에셋 보호
        mat = GetComponent<Renderer>().material;

        if (mat.GetTexture(texturePropertyName) != null)
            mat.GetTexture(texturePropertyName).wrapMode = TextureWrapMode.Repeat;
    }

    void LateUpdate()
    {
        ParallaxGround();
    }

    private void ParallaxGround()
    {
        Vector2 tiling = mat.GetTextureScale(texturePropertyName);

        // --- X축 오프셋 계산 (음수 방지 및 지터링 처리) ---
        // (값 % 1.0f + 1.0f) % 1.0f 공식을 사용하면 음수에서도 항상 0~1 사이의 양수 값이 나옵니다.
        float rawOffsetX = (cam.position.x * parallaxSpeedX * tiling.x) / quadWidth;
        float offsetX = (rawOffsetX % 1f + 1f) % 1f;
        
        float offsetY = 0f;
        float targetY = Mathf.Min(cam.position.y, topLimitY);

        if(_type == BackgroundType.Ground)
        {
            _transform.position = new Vector3(cam.position.x, targetY, _transform.position.z);
            
            if (cam.position.y <= topLimitY)
            {
                // 평상시 Y축 오프셋
                float rawOffsetY = (cam.position.y * parallaxSpeedY * tiling.y) / quadHeight;
                offsetY = (rawOffsetY % 1f + 1f) % 1f;
            }
            else
            {
                // 한계선 도달 후 계산
                float baseOffset = topLimitY * parallaxSpeedY;
                float overDistance = cam.position.y - topLimitY;
                float correctedValue = baseOffset - (overDistance * (1f - parallaxSpeedY));
                
                float rawOffsetY = (correctedValue * tiling.y) / quadHeight;
                offsetY = (rawOffsetY % 1f + 1f) % 1f;
            }
        }
        else
        {
            // Wall 타입일 경우 X축만 따라감
            _transform.position = new Vector3(cam.position.x, _transform.position.y, _transform.position.z);
        }

        // 최종 적용
        mat.SetTextureOffset(texturePropertyName, new Vector2(offsetX, offsetY));
    }
}