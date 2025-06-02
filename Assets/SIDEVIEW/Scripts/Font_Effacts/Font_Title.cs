using UnityEngine;
using UnityEngine.UI;

public class Font_Title : MonoBehaviour
{
    public float duration = 6.0f;  // 전체 진행 시간
    private float timer = 0f;
    public Color glowColor = Color.black;

    void Start()
    {
        timer = 0f;
        SetAlpha(0f); // 시작할 때 완전히 투명하게
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

        float alpha = 1f;

        if (t <= 0.2f)
        {
            // 0 ~ 20% 구간: 페이드 인
            alpha = Mathf.Lerp(0f, 1f, t / 0.2f);
        }
        else if (t >= 0.8f)
        {
            // 80% ~ 100% 구간: 페이드 아웃
            alpha = Mathf.Lerp(1f, 0f, (t - 0.8f) / 0.2f);
        }
        else
        {
            alpha = 1f;
        }

        SetAlpha(alpha);
        UpdateGlow(alpha);
    }

    void SetAlpha(float alpha)
    {
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                Color baseColor = img.color;
                baseColor.a = alpha;
                img.color = baseColor;
            }
        }
    }

    void UpdateGlow(float alpha)
    {
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                Outline outline = img.GetComponent<Outline>();
                Outline outline2 = img.GetComponent<Outline>();
                if (outline == null && outline2 == null)
                {
                    outline = img.gameObject.AddComponent<Outline>();
                    outline2 = img.gameObject.AddComponent<Outline>();
                }
                
                Color glow = glowColor;
                glow.a = alpha * 0.6f;
                outline.effectColor = glow;
                outline.effectDistance = new Vector2(0.05f, -0.05f);
                outline2.effectDistance = new Vector2(0.05f, -0.05f);
            }
        }
    }
}
