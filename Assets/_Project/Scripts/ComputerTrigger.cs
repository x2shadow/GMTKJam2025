using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerTrigger : MonoBehaviour
{
    [Header("Ссылки")]
    public SnakeGameManager snakeManager;
    public PlayerController player;
    public GameObject promptUI;
    public DialogueUI dialogueUI;
    public string afterWinText;
    public GameObject dialogueTrigger2;
    public GameObject dialogueTrigger4;
    public GameObject plate;

    [Header("Поворот камеры")]
    public float cameraRotationSpeed = 10f;
    public float restoreDuration = 0.5f;

    private InputActions inputActions;
    private bool inRange = false;
    private bool isWon = false;

    // Для плавного поворота
    private bool isRotating = false;
    private Quaternion targetRotation;

    // Сохраняем исходную ориентацию
    private Quaternion originalPlayerRotation;
    private Quaternion originalCameraLocalRotation;

    private void Awake()
    {
        inputActions = player.inputActions;
    }

    private void OnEnable()
    {
        inputActions.Player.Interact.performed += OnInteract;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteract;
    }

    private void Update()
    {
        if (isRotating)
            RotateStep();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isWon) return;
        if (other.GetComponent<PlayerController>() != null)
        {
            inRange = true;
            promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inRange = false;
            promptUI.SetActive(false);
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (isWon || !inRange) return;

        // Скрываем подсказку и блокируем ввод
        promptUI.SetActive(false);
        inputActions.Player.Disable();
        player.SetInputBlocked(true);

        // Сохраняем текущие повороты
        originalPlayerRotation      = player.transform.rotation;
        originalCameraLocalRotation = player.playerCamera.transform.localRotation;

        // Начинаем поворот
        PrepareRotation();
    }

    private void PrepareRotation()
    {
        // Вектор от игрока к компьютеру (учитывает вертикаль)
        Vector3 dir = transform.position - player.transform.position;
        if (dir.sqrMagnitude < 0.001f)
        {
            // если слишком близко, сразу запускаем змейку
            StartSnake();
            return;
        }

        // Целевой вращение «смотрит» прямо на компьютер
        targetRotation = Quaternion.LookRotation(dir);
        isRotating = true;
    }

    private void RotateStep()
    {
        // Плавно приближаем текущую ротацию к targetRotation
        player.transform.rotation = Quaternion.Slerp(
            player.transform.rotation,
            targetRotation,
            cameraRotationSpeed * Time.deltaTime
        );

        player.playerCamera.transform.rotation = Quaternion.Slerp(
            player.transform.rotation,
            targetRotation,
            cameraRotationSpeed * Time.deltaTime
        );

        // Если угол маленький — завершаем
        if (Quaternion.Angle(player.transform.rotation, targetRotation) < 0.5f)
        {
            isRotating = false;
            StartSnake();
        }
    }

    private void StartSnake()
    {
        // Показываем змейку и стартуем игру
        snakeManager.GetComponent<Canvas>().gameObject.SetActive(true);
        snakeManager.Init(inputActions, OnWin, OnLose);
    }

    private void OnWin()
    {
        EndSession();

        isWon = true;
        dialogueUI.ShowPlayerDialogue(afterWinText);
        if (dialogueTrigger2 != null)
        {
            dialogueTrigger2.SetActive(true);
            plate.SetActive(true);
            AudioManager.Instance.FadeOut();
        }

        if (dialogueTrigger4 != null)
        {
            dialogueTrigger4.SetActive(true);
        }
    }

    private void OnLose()
    {
        // При поражении просто рестартуем змейку без повтора поворота
        StartSnake();
    }

    private void EndSession()
    {
        // Плавно возвращаем ориентацию
        StartCoroutine(RestoreOrientation());

        snakeManager.GetComponent<Canvas>().gameObject.SetActive(false);
    }

    private IEnumerator RestoreOrientation()
    {
        Quaternion startPlayerRot = player.transform.rotation;
        Quaternion startCameraRot = player.playerCamera.transform.localRotation;
        float elapsed = 0f;

        while (elapsed < restoreDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / restoreDuration;

            player.transform.rotation = Quaternion.Slerp(startPlayerRot, originalPlayerRotation, t);
            player.playerCamera.transform.localRotation =
                Quaternion.Slerp(startCameraRot, originalCameraLocalRotation, t);

            yield return null;
        }

        // Гарантируем точное восстановление
        player.transform.rotation = originalPlayerRotation;
        player.playerCamera.transform.localRotation = originalCameraLocalRotation;

        inputActions.Player.Enable();
        player.SetInputBlocked(false);
    }
}
