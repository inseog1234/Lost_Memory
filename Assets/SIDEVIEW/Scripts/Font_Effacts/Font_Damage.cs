using UnityEngine;
using UnityEngine.UI;

public class Font_Damage : MonoBehaviour
{
    public float duration = 0.5f;
    private float timer = 0f;
    private Vector3 baseScale;
    private Vector3 basePosition;
    private float offsetX;
    private Vector3 color;
    private float size;
    public void SetColor(Vector3 color_) {
        color = color_;
    }
    public void SetSize(float size_) {
        size = size_;
    }
    void Start()
    {
        baseScale = transform.localScale;
        basePosition = transform.position;
        transform.localScale = Vector3.zero;

        duration = 1f;
        timer = 0f;
        SetAlpha(0f);
        offsetX = Random.Range(-2.0f, 2.0f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        if (t > 1f)
        {
            Destroy(gameObject);
            return;
        }
        float value = Mathf.Sin(t * Mathf.PI);
        transform.transform.position = new Vector3(Vector2.Lerp(new Vector2(transform.transform.position.x, 0), new Vector2(basePosition.x + offsetX, 0), 0.005f).x, Vector2.Lerp(new Vector2(transform.transform.position.y, 0), new Vector2(basePosition.y + ((3 * size) * value), 0), value >= 0 ? 20f : 40f).x, 0);
        transform.localScale = baseScale * (value + 0.4f);
        SetAlpha(value);
    }
    void SetAlpha(float value)
    {
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                float alpha = value;

                float r = Mathf.Lerp(1f, color.x, value);
                float g = Mathf.Lerp(1f, color.y, value);
                float b = Mathf.Lerp(1f, color.z, value);

                Color c = new Color(r, g, b, alpha);
                img.color = c;
            }
        }
    }
}
