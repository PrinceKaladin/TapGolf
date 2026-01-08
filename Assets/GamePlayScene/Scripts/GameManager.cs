using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI scoreText;

    public CharacterController characterController;
    public BallController ballController;
    public Transform pointMin;
    public Transform pointMax;
    public CameraFollow cameraFollow;

    private int tapCount = 0;
    private bool isTappingPhase = false;

    private bool isBallMoving = false;
    private float maxDistance;
    private float finalDistance = 0f;

    [SerializeField] private CanvasGameController canvasGameController;

    void Start()
    {
        maxDistance = Vector2.Distance(pointMin.position, pointMax.position);
        scoreText.text = "0 m";
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "";
        StartTappingPhase();
    }

    void StartTappingPhase()
    {
        isTappingPhase = true;
        tapCount = 0;
        characterController.StartSwingPreparation();
        StartCoroutine(EndTappingPhaseAfterDelay(2f));
    }

    IEnumerator EndTappingPhaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isTappingPhase = false;
        characterController.PerformSwing();
    }

    void Update()
    {
        if (isTappingPhase && Input.GetMouseButtonDown(0))
        {
            tapCount++;
            characterController.OnTap();
        }

        // 🔥 Подсчёт метров в реальном времени
        if (isBallMoving)
        {
            float distance = Vector2.Distance(pointMin.position, ballController.transform.position);
            distance = Mathf.Clamp(distance, 0f, maxDistance);

            finalDistance = distance;
            scoreText.text = Mathf.FloorToInt(distance) + " m";
        }
    }

    public void LaunchBallFromSwing()
    {
        if (tapCount == 0)
        {
            finalDistance = 0f;
            scoreText.text = "0 m";

            Debug.Log("GAME OVER — RESULT: 0 meters");
            canvasGameController.ShowGameOver(0);
            return;
        }

        float distance = tapCount * 1f;
        distance = Mathf.Clamp(distance, 0f, maxDistance);

        ballController.Launch(distance);
        cameraFollow.StartFollowing();

        finalDistance = 0f;
        isBallMoving = true;
    }


    public void OnBallStopped()
    {
        isBallMoving = false;

        int meters = Mathf.FloorToInt(finalDistance);
        Debug.Log($"GAME OVER — RESULT: {meters} meters");
        canvasGameController.ShowGameOver(meters);
    }
}
