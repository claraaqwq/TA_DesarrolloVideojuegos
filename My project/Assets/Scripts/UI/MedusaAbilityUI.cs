using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MedusaAbilityUI : MonoBehaviour
{
    [Header("Fuente")]
    [SerializeField] private TMP_FontAsset messageFont;

    private static MedusaAbilityUI instance;

    private TMP_Text messageText;
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

        messageText = CreateText("MedusaUnlockMessage", canvas.transform, new Vector2(0f, 120f), 48);
        messageText.gameObject.SetActive(false);
    }

    private TMP_Text CreateText(string name, Transform parent, Vector2 anchoredPosition, float fontSize)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(900f, 120f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        if (messageFont != null)
        {
            text.font = messageFont;
        }
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;
        text.outlineWidth = 0.2f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.85f);

        return text;
    }
}
