using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueRunner : MonoBehaviour
{
    public DialogueScriptUI playerUI;
    public DialogueScriptUI oldOneUI;
    public DialogueScriptUI noteUI;

    public PlayerController player;

    public AudioSource audioSource;
    public AudioClip oldOneVoiceClip;
    
    bool skipPressed = false;

    public void StartDialogue(DialogueScript script, PlayerController player, int index)
    {
        player.isDialogueActive = true;
        // Подписываемся на событие Skip
        player.inputActions.Player.Click.performed += OnClick;
        StartCoroutine(RunDialogue2(script, player, index));
    }

    public void StartDialogue(DialogueScript script, int index)
    {
        player.isDialogueActive = true;
        // Подписываемся на событие Skip
        player.inputActions.Player.Click.performed += OnClick;
        StartCoroutine(RunDialogue2(script, player, index));
    }

    public IEnumerator StartDialogueCoroutine(DialogueScript script, int index)
    {
        player.isDialogueActive = true;
        player.inputActions.Player.Click.performed += OnClick;
        yield return RunDialogue2(script, player, index);
    }

    private IEnumerator RunDialogue(DialogueScript script, PlayerController player, int index)
    {
        foreach (var line in script.lines)
        {
            if (line.speaker == DialogueLine.Speaker.Player)
                playerUI.Show(line.text);
            else
                oldOneUI.Show(line.text);

            yield return new WaitForSeconds(line.duration);

            playerUI.Hide();
            oldOneUI.Hide();
        }

        player.EndDialogue(index);
    }

    private IEnumerator RunDialogue2(DialogueScript script, PlayerController player, int index)
    {
        foreach (var line in script.lines)
        {
            // Сброс флага перед каждой репликой
            skipPressed = false;

            if (line.speaker == DialogueLine.Speaker.Player)
                playerUI.Show(line.text);
            else if (line.speaker == DialogueLine.Speaker.Note)
                noteUI.Show(line.text);
            else
            {
                oldOneUI.Show(line.text);
                audioSource.Play();
            }

            // Вместо ожидания по времени ждём нажатия ЛКМ (Skip)
            yield return new WaitUntil(() => skipPressed);
            
            playerUI.Hide();
            oldOneUI.Hide();
            noteUI.Hide();
        }

        // Отписываемся от события Click после завершения диалога
        player.inputActions.Player.Click.performed -= OnClick;

        player.EndDialogue(index);
    }


    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            skipPressed = true;
    }

}
