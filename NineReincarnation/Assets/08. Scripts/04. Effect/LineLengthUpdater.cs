using UnityEngine;

public class LineLengthUpdater : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Material lineMaterial;
    private Vector3[] positions = new Vector3[0];

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // 런타임에 머티리얼 인스턴스를 가져옵니다.
        lineMaterial = lineRenderer.material;
    }

    void Update()
    {
        int pointCount = lineRenderer.positionCount;
        
        // 점이 2개 미만이면 선이 아니므로 계산 생략
        if (pointCount < 2) return;

        // 라인 렌더러의 점 개수만큼 배열 크기 맞춰주기
        if (positions.Length != pointCount)
        {
            positions = new Vector3[pointCount];
        }

        // 라인 렌더러의 모든 점 위치를 배열에 가져오기
        lineRenderer.GetPositions(positions);

        // 점과 점 사이의 거리를 모두 더해서 전체 길이(Total Length) 구하기
        float totalLength = 0f;
        for (int i = 0; i < pointCount - 1; i++)
        {
            totalLength += Vector3.Distance(positions[i], positions[i + 1]);
        }

        // 셰이더에 만든 변수명("_LineLength")으로 전체 길이 전달
        if (lineMaterial != null)
        {
            // 원하는 밀도에 맞춰 길이에 상수를 곱해줄 수도 있습니다. 
            // 예: totalLength * 0.5f 
            lineMaterial.SetFloat("_LineLength", totalLength);
        }
    }
}
