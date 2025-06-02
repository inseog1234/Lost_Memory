using UnityEngine;
using UnityEngine.UI;

public class Font_Said : MonoBehaviour
{
    public float Life_Time = 2f;
    public float delayTime = 0f;
    public float VibMin;
    public float VibMax;
    public float VibeCycle = 0.05f;

    public float timer = 0f;
    private float nextVibeTime = 0f;
    private bool started = false;

    private Vector3 baseScale;
    public Vector2 First_Pos;

    private Vector3 color;
    private float size;
    public Outline outline;
    public Outline outline2;
    public void SetColor(Vector3 color_)
    {
        color = color_;
    }

    public void SetSize(float size_)
    {
        size = size_;
    }

    void Start()
    {
        outline = gameObject.AddComponent<Outline>();
        outline2 = gameObject.AddComponent<Outline>();
        baseScale = transform.localScale;
        First_Pos = transform.position;
        transform.localScale = baseScale * 5f;
        VibMin = -0.02f;
        VibMin = -0.02f;
        SetAlpha(0f);

        
    }

    void Update()
    {
        if (!started)
        {
            if (Time.time >= delayTime)
            {
                started = true;
                timer = 0f;
            }
            else return;
        }

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / Life_Time);
        float speedUpT = Mathf.Pow(t, 0.08f);

        transform.localScale = Vector3.Lerp(baseScale * 5f, baseScale, speedUpT * 1.2f);

        float alpha;
        if (t < 0.05f)
        {
            alpha = Mathf.Clamp01(t / 0.05f);
        }
        else if (t > 1f - 0.05f)
        {
            alpha = Mathf.Clamp01((1f - t) / 0.05f);
        }
        else
        {
            alpha = 1f;
        }
        SetAlpha(alpha);

        if (timer >= Life_Time || Time.deltaTime >= Life_Time)
        {
            SetAlpha(0f);
            Destroy(gameObject);
            return;
        }

        if (VibMax > 0 && timer >= nextVibeTime)
        {
            float offsetX = Random.Range(VibMin, VibMax);
            float offsetY = Random.Range(VibMin, VibMax);
            transform.position = new Vector2(First_Pos.x + offsetX, First_Pos.y + offsetY);
            nextVibeTime = timer + VibeCycle;
        }
    }
    void SetAlpha(float alpha)
    {
        Color glow = new Color(0, 0, 0);
        glow.a = alpha;
        outline.effectColor = glow;
        outline.effectDistance = new Vector2(0.01f, -0.01f);
        outline2.effectColor = glow;
        outline2.effectDistance = new Vector2(0.03f, -0.03f);

        Image image = GetComponent<Image>();
        float r = Mathf.Lerp(1f, color.x, alpha);
        float g = Mathf.Lerp(1f, color.y, alpha);
        float b = Mathf.Lerp(1f, color.z, alpha);
        image.color = new Color(r, g, b, alpha);
    }
}
