using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeGameManager : MonoBehaviour
{
    [Header("Настройки сетки")]
    public int width = 20, height = 20;
    public float stepDelay = 0.2f;
    public TMP_Text gridText;
    public int fruitsToWin = 10;

    [Header("Звуки движения")]
    public AudioSource audioSource;       // ссылка на AudioSource
    public List<AudioClip> moveSounds;    // список клипов для ходьбы

    private enum Cell { Empty, Snake, Fruit }
    private Cell[,] grid;
    private List<Vector2Int> snake;
    private Vector2Int dir;
    private Vector2Int fruitPos;
    private int eatenCount;
    private Coroutine gameLoop;

    // New Input System
    private InputActions _actions;
    private Vector2 _moveInput;

    private Action onWin, onLose;

    public void Init(InputActions actions, Action onWinCallback, Action onLoseCallback)
    {
        _actions = actions;
        onWin   = onWinCallback;
        onLose  = onLoseCallback;

        // Input
        _actions.Snake.Move.performed += OnMove;
        _actions.Snake.Move.canceled  += OnMove;
        _actions.Snake.Enable();

        InitGame();
        gameLoop = StartCoroutine(GameLoop());
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        // Сначала читаем ввод
        Vector2 newInput = ctx.ReadValue<Vector2>();

        // Только по событию performed и если игрок реально нажимает в каком-то направлении
        if (ctx.performed && (Mathf.Abs(newInput.x) > 0.5f || Mathf.Abs(newInput.y) > 0.5f))
        {
            PlayRandomMoveSound();
        }

        // Обновляем направление
        _moveInput = newInput;
        HandleInput();
    }

    private void PlayRandomMoveSound()
    {
        if (audioSource == null || moveSounds == null || moveSounds.Count == 0) 
            return;

        int idx = UnityEngine.Random.Range(0, moveSounds.Count);
        AudioClip clip = moveSounds[idx];
        audioSource.PlayOneShot(clip);
    }

    private void InitGame()
    {
        if (gameLoop != null) StopCoroutine(gameLoop);

        grid       = new Cell[width, height];
        snake      = new List<Vector2Int> { new Vector2Int(width/2, height/2) };
        dir        = Vector2Int.right;
        eatenCount = 0;
        SpawnFruit();
        DrawGrid();
    }

    public void RestartGame()
    {
        StopCoroutine(gameLoop);
        InitGame();
        gameLoop = StartCoroutine(GameLoop());
    }

    private void SpawnFruit()
    {
        var empties = new List<Vector2Int>();
        for(int x=0; x<width; x++)
            for(int y=0; y<height; y++)
                if (!snake.Contains(new Vector2Int(x,y)))
                    empties.Add(new Vector2Int(x,y));
        fruitPos = empties[UnityEngine.Random.Range(0, empties.Count)];
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepDelay);
            if (!Step()) yield break;
            DrawGrid();
            if (eatenCount >= fruitsToWin)
            {
                CleanUp();
                onWin?.Invoke();
                yield break;
            }
        }
    }

    private void HandleInput()
    {
        if (_moveInput.y > 0.5f && dir != Vector2Int.down)       dir = Vector2Int.up;
        else if (_moveInput.y < -0.5f && dir != Vector2Int.up)   dir = Vector2Int.down;
        else if (_moveInput.x < -0.5f && dir != Vector2Int.right)dir = Vector2Int.left;
        else if (_moveInput.x > 0.5f && dir != Vector2Int.left)  dir = Vector2Int.right;
    }

    private bool Step()
    {
        var newHead = snake[0] + dir;

        if (newHead.x < 0 || newHead.x >= width ||
            newHead.y < 0 || newHead.y >= height ||
            snake.Contains(newHead))
        {
            CleanUp();
            onLose?.Invoke();
            return false;
        }

        snake.Insert(0, newHead);
        if (newHead == fruitPos)
        {
            eatenCount++;
            SpawnFruit();
        }
        else
        {
            snake.RemoveAt(snake.Count - 1);
        }
        return true;
    }

    private void DrawGrid()
    {
        var sb = new System.Text.StringBuilder();
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                var pos = new Vector2Int(x, y);
                if      (pos == snake[0])    sb.Append('С');
                else if (snake.Contains(pos)) sb.Append('+');
                else if (pos == fruitPos)     sb.Append('#');
                else                          sb.Append('.');
            }
            sb.AppendLine();
        }
        gridText.text = sb.ToString();
    }

    private void CleanUp()
    {
        _actions.Snake.Move.performed -= OnMove;
        _actions.Snake.Move.canceled  -= OnMove;
        _actions.Snake.Disable();
    }
}
