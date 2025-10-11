// Yeni C# dosyası: RangeGoster.cs
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class RangeGoster : MonoBehaviour
{

    [SerializeField] float radius ;
    [SerializeField] int segments = 64;
    [SerializeField] float lineWidth = 0.03f;
    [SerializeField] Color color = new Color(0f, 1f, 0f, 0.85f);
    [SerializeField] float yOffset = 0.02f;     // zeminden hafif yukarı
    [SerializeField] bool autoHide = true;
    [SerializeField] float hideAfter = 2f;

    LineRenderer lr;

    void Awake()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = segments + 1;
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.startWidth = lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;
        Ciz();
        if (autoHide) Invoke(nameof(Kapat), hideAfter);
    }

    void Ciz()
    {
        float step = 2f * Mathf.PI / segments;
        for (int i = 0; i <= segments; i++)
        {
            float a = i * step;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, yOffset, Mathf.Sin(a) * radius));
        }
    }

    public void SetRadius(float r) { radius = 2*r; Ciz(); }
    public void SetVisible(bool v) { if (lr) lr.enabled = v; }
    void Kapat() { if (lr) lr.enabled = false; }
}
