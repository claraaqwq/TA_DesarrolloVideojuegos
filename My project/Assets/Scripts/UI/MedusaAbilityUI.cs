using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MedusaAbilityUI : MonoBehaviour
{
    private static MedusaAbilityUI instance;

    private Text messageText;
    private Coroutine messageRoutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        EnsureUi();
    }

    public static void ShowUnlockMessage()
    {
        ShowTemporaryMessage("MIRADA DE PIEDRA DESBLOQUEADA\nPresiona F para petrificar enemigos");
    }

    public static void ShowTemporaryMessage(string message)
    {
        EnsureInstance();

        if (instance.messageText == null)
        {
            return;
        }

        if (instance.messageRoutine != null)
        {
            instance.StopCoroutine(instance.messageRoutine);
        }

        instance.messageRoutine = instance.StartCoroutine(instance.ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        yield return new WaitForSeconds(3f);
        messageText.gameObject.SetActive(false);
        messageRoutine = null;
    }

    private static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        MedusaAbilityUI existing = FindFirstObjectByType<MedusaAbilityUI>();
        if (existing != null)
        {
            instance = existing;
            instance.EnsureUi();
            return;
        }

        GameObject uiObject = new GameObject("MedusaAbilityUI");
        instance = uiObject.AddComponent<MedusaAbilityUI>();
    }

    private void EnsureUi()
    {
        if (messageText != null)
        {
            return;
        }

        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("MedusaAbilityCanvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        messageText = CreateText("MedusaUnlockMessage", canvas.transform, new Vector2(0f, -145f), TextAnchor.MiddleCenter, 48);
        messageText.gameObject.SetActive(false);
    }

    private static Text CreateText(string name, Transform parent, Vector2 anchoredPosition, TextAnchor alignment, int fontSize)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = alignment == TextAnchor.MiddleLeft ? new Vector2(0f, 1f) : new Vector2(0.5f, 1f);
        rectTransform.anchorMax = rectTransform.anchorMin;
        rectTransform.pivot = alignment == TextAnchor.MiddleLeft ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(620f, 90f);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;

        Outline outline = textObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return text;
    }
}
