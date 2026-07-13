using UnityEngine;

public class SpriteFlipbookEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite[] frames;
    private float frameRate;
    private int currentFrame;
    private float frameTimer;

    public static GameObject Play(Sprite[] frames, Vector3 position, float frameRate = 24f, float scale = 1f, int sortingOrder = 25)
    {
        if (frames == null || frames.Length == 0)
        {
            return null;
        }

        GameObject effectObject = new GameObject("SpriteFlipbookEffect");
        effectObject.transform.position = position;
        effectObject.transform.localScale = Vector3.one * scale;

        SpriteRenderer renderer = effectObject.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = sortingOrder;

        SpriteFlipbookEffect flipbook = effectObject.AddComponent<SpriteFlipbookEffect>();
        flipbook.Initialize(renderer, frames, frameRate);

        return effectObject;
    }

    private void Initialize(SpriteRenderer renderer, Sprite[] effectFrames, float fps)
    {
        spriteRenderer = renderer;
        frames = effectFrames;
        frameRate = Mathf.Max(1f, fps);
        spriteRenderer.sprite = frames[0];
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0)
        {
            return;
        }

        frameTimer += Time.deltaTime;

        if (frameTimer < 1f / frameRate)
        {
            return;
        }

        frameTimer = 0f;
        currentFrame++;

        if (currentFrame >= frames.Length)
        {
            Destroy(gameObject);
            return;
        }

        spriteRenderer.sprite = frames[currentFrame];
    }
}
