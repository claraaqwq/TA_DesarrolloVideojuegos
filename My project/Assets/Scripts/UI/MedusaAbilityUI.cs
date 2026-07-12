using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MedusaAbilityUI : MonoBehaviour
{
    private static MedusaAbilityUI instance;

    private Text indicatorText;
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

    public static void UpdateIndicator(bool unlocked, float cooldownRemaining)
    {
        EnsureInstance();

        if (instance.indicatorText == null)
        {
            return;
        }

        if (!unlocked)
        {
            instance.indicatorText.gameObject.SetActive(false);
            return;
        }

        instance.indicatorText.gameObject.SetActive(true);
        instance.indicatorText.text = cooldownRemaining > 0f
            ? $"F - Medusa ({Mathf.CeilToInt(cooldownRemaining)}s)"
            : "F - Medusa";
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
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        if (indicatorText == null)
        {
            indicatorText = CreateText("MedusaIndicator", canvas.transform, new Vector2(20f, -135f), TextAnchor.MiddleLeft, 24);
            indicatorText.gameObject.SetActive(false);
        }

        if (messageText == null)
        {
            messageText = CreateText("MedusaUnlockMessage", canvas.transform, new Vector2(0f, -145f), TextAnchor.MiddleCenter, 28);
            messageText.gameObject.SetActive(false);
        }
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
        text.color = new Color(0.5f, 1f, 0.55f, 1f);
        text.raycastTarget = false;
        return text;
    }
}
