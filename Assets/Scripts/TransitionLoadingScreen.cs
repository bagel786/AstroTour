using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionLoadingScreen : MonoBehaviour
{
    private static TransitionLoadingScreen instance;
    public static TransitionLoadingScreen Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("TransitionLoadingScreen");
                instance = go.AddComponent<TransitionLoadingScreen>();
                DontDestroyOnLoad(go);
                instance.Initialize();
            }
            return instance;
        }
    }

    private Canvas canvas;
    private Image fadeImage;
    private GameObject loadingIndicator;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Initialize()
    {
        // Create canvas
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        gameObject.AddComponent<GraphicRaycaster>();

        // Create fade panel that covers entire screen
        GameObject fadePanel = new GameObject("FadePanel");
        fadePanel.transform.SetParent(transform, false);
        fadeImage = fadePanel.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        
        RectTransform fadeRect = fadeImage.GetComponent<RectTransform>();
        fadeRect.anchorMin = Vector2.zero;
        fadeRect.anchorMax = Vector2.one;
        fadeRect.offsetMin = new Vector2(-200, -200);
        fadeRect.offsetMax = new Vector2(200, 200);
        fadeRect.pivot = new Vector2(0.5f, 0.5f);
        fadeRect.anchoredPosition = Vector2.zero;

        // Create loading indicator
        loadingIndicator = new GameObject("LoadingIndicator");
        loadingIndicator.transform.SetParent(transform, false);
        
        Text loadingText = loadingIndicator.AddComponent<Text>();
        loadingText.text = "Loading...";
        loadingText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        loadingText.fontSize = 36;
        loadingText.color = Color.white;
        loadingText.alignment = TextAnchor.MiddleRight;

        RectTransform loadingRect = loadingIndicator.GetComponent<RectTransform>();
        loadingRect.anchorMin = new Vector2(1f, 0f);
        loadingRect.anchorMax = new Vector2(1f, 0f);
        loadingRect.pivot = new Vector2(1f, 0f);
        loadingRect.sizeDelta = new Vector2(300, 80);
        loadingRect.anchoredPosition = new Vector2(-30, 30);

        loadingIndicator.SetActive(false);
        canvas.enabled = false;
    }

    public void StartTransition(System.Action onTransitionMiddle, float duration = 2.5f)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(onTransitionMiddle, duration));
        }
    }

    private IEnumerator TransitionCoroutine(System.Action onTransitionMiddle, float duration)
    {
        isTransitioning = true;
        canvas.enabled = true;

        // Fade to black
        float fadeInDuration = duration * 0.3f;
        yield return StartCoroutine(FadeToBlack(fadeInDuration));

        // Show loading indicator
        loadingIndicator.SetActive(true);

        // Wait a moment
        float waitDuration = duration * 0.4f;
        yield return new WaitForSeconds(waitDuration);

        // Execute the transition action (move player, etc.)
        onTransitionMiddle?.Invoke();

        // Wait a bit more
        yield return new WaitForSeconds(waitDuration);

        // Hide loading indicator
        loadingIndicator.SetActive(false);

        // Fade back to clear
        float fadeOutDuration = duration * 0.3f;
        yield return StartCoroutine(FadeToClear(fadeOutDuration));

        canvas.enabled = false;
        isTransitioning = false;
    }

    private IEnumerator FadeToBlack(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeToClear(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }
}
