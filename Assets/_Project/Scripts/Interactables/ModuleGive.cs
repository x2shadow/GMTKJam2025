using UnityEngine;

public class ModuleGive : MonoBehaviour, IInteractable
{
    [Header("Ссылки")]
    [Tooltip("UI-подсказка 'Взять модуль'")]
    public GameObject promptUI;
    public AudioSource audioSource;

    [Tooltip("Слот в камере, в который нужно поместить модуль")]
    public Transform moduleTakeSlot;
    public Transform chipTakeSlot;

    [Tooltip("Сам объект модуля, который лежит рядом и должен быть взят")]
    public Transform module;
    public Transform chip;

    private bool inRange = false;
    private bool hasModule = false;

    public ModuleTake moduleTake;
    public Table table;
    public ChipTake chipTake;

    private void Awake()
    {
        // Прячем подсказку сразу
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (promptUI == null) return;
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        var module = player.HeldModule;
        if (module == null) return;             // Нет модуля — не показываем
        if (module.CurrentState == ModuleController.State.WiringDone)
        {
            promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    // Этот метод будет вызван извне, когда игрок нажмёт E
    public void Interact(PlayerController player)
    {
        if (module.GetComponent<ModuleController>().CurrentState
            != ModuleController.State.WiringDone) return;

        promptUI?.SetActive(false);
        Debug.Log("Модуль сдан!");

        // Вернуть модуль на место в ModuleTake
        module.SetParent(moduleTakeSlot, worldPositionStays: false);
        module.localPosition = Vector3.zero;
        module.localRotation = Quaternion.identity;

        // Вернуть чип на место в ChipTake
        chip.SetParent(chipTakeSlot, worldPositionStays: true);
        chip.localPosition = Vector3.zero;
        chip.localRotation = Quaternion.identity;

        // Сообщаем игроку, что теперь он не держит модуль
        player.HeldModule.CurrentState = ModuleController.State.InHand;
        player.HeldModule = null;

        // Триггерим событие сдачи
        ShiftManager.NotifyModuleDelivered();

        // Проигрываем звук двери?
        audioSource.Play();

        // Делаем ресет модулей
        moduleTake.Reset();
        table.Reset();
        chipTake.Reset();
    }

    public void Reset()
    {
        hasModule = false;
    }
}
