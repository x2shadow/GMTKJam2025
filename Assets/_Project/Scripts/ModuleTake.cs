using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class ModuleTake : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Ваша UI-подсказка 'Взять модуль'")]
    public GameObject promptUI;

    [Tooltip("Тег игрока или компонент для идентификации")]
    public string playerTag = "Player";

    [Tooltip("Слот в камере, в который нужно поместить модуль")]
    public Transform slot;

    [Tooltip("Сам объект модуля, который лежит рядом и должен быть взят")]
    public Transform module;

    private InputActions inputActions;
    private InputAction interactAction;
    private bool inRange = false;
    private bool hasModule = false;

    private void Awake()
    {
        // 1) Инициализируем свой InputActions-ассет
        inputActions = new InputActions(); 
        interactAction = inputActions.Player.Interact;

        // 2) Подписываемся на нажатие
        interactAction.performed += ctx => OnInteract();

        // 3) Включаем все экшены
        inputActions.Player.Enable();

        // 4) Прячем UI изначально
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void OnDisable()
    {
        // Отписываемся, чтобы не было утечек памяти
        interactAction.performed -= ctx => OnInteract();
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        // Отписываемся, чтобы не было утечек памяти
        interactAction.performed -= ctx => OnInteract();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasModule) return;

        // По тегу или по компоненту
        if (other.CompareTag(playerTag))
        {
            inRange = true;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            inRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    private void OnInteract()
    {
        if (!inRange || hasModule) 
            return;

        hasModule = true;
        Debug.Log("Модуль взят!");
        
        // Скрываем подсказку
        if (promptUI != null)
            promptUI.SetActive(false);


        // Делаем модуль дочерним к слоту и сбрасываем локальные преобразования
        module.SetParent(slot);
        module.localPosition = Vector3.zero;
        module.localRotation = Quaternion.identity;
    }
}
