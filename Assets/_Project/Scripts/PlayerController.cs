using System;
//using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private CharacterController characterController;
    //public CinemachineVirtualCamera playerCamera;
    public Camera playerCamera;
    private float xRotation = 0f;

    [HideInInspector]
    public InputActions inputActions;

    [Header("Диалог")]
    public bool isDialogueActive = false;
    public Transform dialogueTarget;
    public float dialogueRotationSpeed = 10f;
    public DialogueUI dialogueUI;
    public GameObject nextPartTrigger1;
    public GameObject invisibleStartWall;
    public GameObject invisibleStartWall2;

    [Header("Пауза")]
    [SerializeField] private GameObject pauseCanvas;
    private bool isPaused = false;

    private bool isInputBlocked = false;

    // Для взаимодействия с объектами
    private IInteractable currentInteractable;

    // Для хранения взятого модуля
    [HideInInspector]
    public ModuleController HeldModule { get; set; }

    private void Awake()
    {
        inputActions = new InputActions();
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
        mouseSensitivity = savedSensitivity;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        // Подписка на события для экшенов
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
        //inputActions.Player.Click.performed += OnClick;
        inputActions.Player.Interact.performed += OnInteract;
        inputActions.Player.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        // Отписка от событий
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;
        //inputActions.Player.Click.performed -= OnClick;
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Pause.performed -= OnPause;
    }

    private void Update()
    {
        if (isDialogueActive)
        {
            SetInputBlocked(true);
            RotateTowardsDialogueTarget();
            return;
        }

        if (isInputBlocked) return;

        // Движение персонажа
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // Обработка обзора (поворот камеры)
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void EndDialogue(int index)
    {
        isDialogueActive = false;
        SetInputBlocked(false);
        moveInput = Vector2.zero;

        if (index == 1)
        {
            invisibleStartWall.SetActive(false);
            //firstDialogueHappened = true; 
            //dialogueUI.ShowTutorialDialogue("Press F to throw the flare that drives away the darkness");
        }

        if (index == 2)
        {
            dialogueTarget = null;
            nextPartTrigger1.SetActive(true);
            //effectController.TriggerHitEffects();
            //AudioManager.Instance.PlayAmbient1();
        }

        if (index == 3)
        {
            dialogueTarget = null;
        }

        if (index == 4)
        {
            invisibleStartWall2.SetActive(false);
        }
    }

    public void RotateTowardsDialogueTarget()
    {
        if (dialogueTarget == null) return;

        Vector3 direction = dialogueTarget.position - transform.position;
        direction.y = 0f; // игнорируем вертикальную составляющую
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, dialogueRotationSpeed * Time.deltaTime);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (isInputBlocked) return;
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (isInputBlocked) return;
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;  // Останавливаем время
            SetInputBlocked(true); // Блокируем управление
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;  // Возвращаем время
            SetInputBlocked(false); // Возвращаем управление
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInputBlocked) return;
        //effectController.TriggerHitEffects();
        //Debug.Log("Interact");
        //dialogueUI.ShowPlayerDialogue("Interact pressed");

        // Вызываем взаимодействие с текущим объектом
        currentInteractable?.Interact(this);
    }

    public void SetInputBlocked(bool blocked)
    {
        isInputBlocked = blocked;
        if (blocked)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
        }
    }


    public void SetInputBlocked2(bool blocked)
    {
        isInputBlocked = blocked;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, реализует ли объект IInteractable
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            currentInteractable = interactable;
    }

    private void OnTriggerExit(Collider other)
    {
        // Очищаем ссылку, когда игрок уходит из зоны
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null && currentInteractable == interactable)
            currentInteractable = null;
    }
}
