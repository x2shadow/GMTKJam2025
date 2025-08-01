using UnityEngine;

public class ModuleTake : MonoBehaviour, IInteractable
{
    [Header("Ссылки")]
    [Tooltip("UI-подсказка 'Взять модуль'")]
    public GameObject promptUI;

    [Tooltip("Слот в камере, в который нужно поместить модуль")]
    public Transform slot;

    [Tooltip("Сам объект модуля, который лежит рядом и должен быть взят")]
    public Transform module;

    private bool inRange = false;
    private bool hasModule = false;

    private void Awake()
    {
        // Прячем подсказку сразу
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ждём, что PlayerInteraction или PlayerController пометит его как игрока
        if (hasModule) return;
        if (other.GetComponent<PlayerController>() != null)
        {
            inRange = true;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    // Этот метод будет вызван извне, когда игрок нажмёт E
    public void Interact(PlayerController player)
    {
        if (!inRange || hasModule)
            return;

        hasModule = true;
        promptUI?.SetActive(false);
        Debug.Log("Модуль взят!");

        // Прикрепляем к слоту
        var desiredSize = Vector3.one * 0.2f;
        module.transform.SetParentWithWorldScale(slot, desiredSize, worldPositionStays: true);
        module.localPosition = Vector3.zero;
        module.localRotation = Quaternion.identity;

        // Сообщаем игроку, что теперь он держит модуль
        player.HeldModule = module.GetComponent<ModuleController>();
    }

    public void Reset()
    {
        hasModule = false;   
    }
}
