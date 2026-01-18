using UnityEngine;

public class MapHighlight : MonoBehaviour
{
    public Color highlightColor = Color.yellow;

    private SpriteRenderer renderer;
    private Material baseMaterial;

    [SerializeField]
    private Material lineMaterial;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
            baseMaterial = renderer.material;
    }

    public void HighlightOn()
    {
        if (renderer == null || lineMaterial == null) return;

        renderer.material = lineMaterial;

        Vector4 color = new Vector4(
            highlightColor.r,
            highlightColor.g,
            highlightColor.b,
            highlightColor.a
        );

        lineMaterial.SetVector("_LineColor", color);

        // 강조를 위해 살짝 앞으로
        transform.localPosition += new Vector3(0f, 0f, -1f);
    }

    public void HighlightOff()
    {
        if (renderer == null || baseMaterial == null) return;

        renderer.material = baseMaterial;
        transform.localPosition += new Vector3(0f, 0f, 1f);
    }
}
