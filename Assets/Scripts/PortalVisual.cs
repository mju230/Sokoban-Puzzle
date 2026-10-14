using UnityEngine;

public class PortalVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer accentRenderer;

    // 한 쌍의 입구와 출구가 같은 색상
    private static readonly Color[] PairColors =
    {
        new Color(0.2f, 0.7f, 1f),        
        new Color(1f, 0.55f, 0.15f),     
        new Color(0.7f, 0.4f, 1f),       
        new Color(0.25f, 0.9f, 0.45f),   
        new Color(1f, 0.35f, 0.6f),      
        new Color(1f, 0.9f, 0.25f),      
        new Color(0.2f, 0.95f, 0.9f),    
        new Color(1f, 0.3f, 0.2f),       
        new Color(0.9f, 0.9f, 1f)        
    };

    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock properties;

    public void Initialize(int pairId)
    {
        if (accentRenderer == null || pairId < 0 || pairId >= PairColors.Length)
        {
            Debug.LogError("포탈장식 연결 오류.", this);
            return;
        }

        if (properties == null)
        {
            properties = new MaterialPropertyBlock();
        }

        accentRenderer.GetPropertyBlock(properties);
        properties.SetColor(ColorId, PairColors[pairId]);
        accentRenderer.SetPropertyBlock(properties);
    }
}
