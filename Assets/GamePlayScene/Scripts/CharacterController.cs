using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Transform arm; // Рука с клюшкой
    public float liftPerTap = 5f; // Сколько градусов поднимаем за один тап
    public float returnSpeed = 20f; // Скорость возврата клюшки к исходному положению
    public GameManager gameManager;

    private float currentRotation = 0f;
    private float initialRotationZ;
    private bool isPreparingSwing = false;
    private const float minSwingAngle = -10f; // Минимальный замах

    void Start()
    {
        if (arm != null)
            initialRotationZ = arm.localEulerAngles.z > 180 ? arm.localEulerAngles.z - 360 : arm.localEulerAngles.z;
    }

    public void StartSwingPreparation()
    {
        isPreparingSwing = true;
        currentRotation = 0f;
        UpdateArmRotation();
    }

    public void OnTap()
    {
        if (isPreparingSwing)
        {
            currentRotation -= liftPerTap;
            currentRotation = Mathf.Clamp(currentRotation, -90f, 0f); // Не даём перевернуть
            UpdateArmRotation();
        }
    }

    void Update()
    {
        if (isPreparingSwing && currentRotation < 0f)
        {
            // Клюшка пытается вернуться назад
            currentRotation += returnSpeed * Time.deltaTime;
            currentRotation = Mathf.Clamp(currentRotation, -90f, 0f);
            UpdateArmRotation();
        }
    }

    private void UpdateArmRotation()
    {
        if (arm == null) return;

        Vector3 rot = arm.localEulerAngles;
        rot.z = initialRotationZ + currentRotation;
        arm.localEulerAngles = rot;
    }

    public void PerformSwing()
    {
        isPreparingSwing = false;

        float swingFrom = currentRotation;

        // Если игрок плохо тапал — минимальный замах
        if (swingFrom > minSwingAngle) // > потому что значения отрицательные
        {
            swingFrom = minSwingAngle;
            currentRotation = minSwingAngle;
            UpdateArmRotation();
        }

        float swingTo = -swingFrom; // Симметричный удар вперёд

        StartCoroutine(AnimateSwing(swingFrom, swingTo));
    }

    IEnumerator AnimateSwing(float from, float to)
    {
        float duration = 0.4f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            // Используем EaseOut для резкого удара
            t = 1f - Mathf.Pow(1f - t, 3f);

            currentRotation = Mathf.Lerp(from, to, t);
            UpdateArmRotation();
            yield return null;
        }

        // Удар завершён — передаём управление GameManager'у
        gameManager.LaunchBallFromSwing();
    }
}