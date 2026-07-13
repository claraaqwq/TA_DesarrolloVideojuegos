using System.Collections;
using UnityEngine;

public class Seal : MonoBehaviour
{
    [SerializeField] private Collider2D sealCollider;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private float fadeDuration = 0.4f;

    private bool isActive = true;

    private void Awake()
    {
        if (sealCollider == null)
        {
            sealCollider = GetComponent<Collider2D>();
        }

        if (visual == null)
        {
            visual = GetComponent<SpriteRenderer>();
        }
    }

    public void Deactivate()
    {
        if (!isActive)
        {
            return;
        }

        isActive = false;

        if (sealCollider != null)
        {
            sealCollider.enabled = false;
        }

        if (visual != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        Color start = visual.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(start.a, 0f, elapsed / fadeDuration);
            visual.color = new Color(start.r, start.g, start.b, alpha);
            yield return null;
        }

        visual.enabled = false;
    }
}
