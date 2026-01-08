using System.Collections;
using UnityEngine;
using TMPro;

public class CanvasGameController : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject PauseMenu;
    public GameObject GameOverMenu;
    public GameObject Background;

    [Header("Game Over")]
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI bestScoreText;

    [Header("Animation")]
    public float fadeDuration = 0.3f;

    private CanvasGroup pauseGroup;
    private CanvasGroup gameOverGroup;

    void Awake()
    {
        pauseGroup = PauseMenu.GetComponent<CanvasGroup>();
        gameOverGroup = GameOverMenu.GetComponent<CanvasGroup>();

        HideAllInstant();
    }

    // =========================
    // PAUSE
    // =========================
    public void ShowPause()
    {
        Time.timeScale = 0f;

        Background.SetActive(true);
        GameOverMenu.SetActive(false);

        PauseMenu.SetActive(true);
        StartCoroutine(FadeIn(pauseGroup));
    }

    public void HidePause()
    {
        Time.timeScale = 1f;

        StartCoroutine(FadeOut(pauseGroup, () =>
        {
            PauseMenu.SetActive(false);
            Background.SetActive(false);
        }));
    }

    // =========================
    // GAME OVER
    // =========================
    public void ShowGameOver(int score)
    {
        Time.timeScale = 0f;

        // ===== SCORE =====
        if (gameOverScoreText != null)
            gameOverScoreText.text = "Score: " + score + " m";

        // ===== BEST SCORE =====
        int bestScore = PlayerData.Instance.BestScore;

        if (score > bestScore)
        {
            bestScore = score;
            PlayerData.Instance.BestScore = bestScore;
            PlayerPrefs.Save();
        }

        if (bestScoreText != null)
            bestScoreText.text = "Best score: " + bestScore + " m";

        PauseMenu.SetActive(false);
        Background.SetActive(true);

        GameOverMenu.SetActive(true);
        StartCoroutine(FadeIn(gameOverGroup));
    }

    // =========================
    // FADE LOGIC
    // =========================
    IEnumerator FadeIn(CanvasGroup group)
    {
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    IEnumerator FadeOut(CanvasGroup group, System.Action onComplete)
    {
        group.interactable = false;
        group.blocksRaycasts = false;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        group.alpha = 0f;
        onComplete?.Invoke();
    }

    // =========================
    // UTILS
    // =========================
    void HideAllInstant()
    {
        Time.timeScale = 1f;

        Background.SetActive(false);

        PauseMenu.SetActive(false);
        GameOverMenu.SetActive(false);

        pauseGroup.alpha = 0f;
        gameOverGroup.alpha = 0f;
    }
}
