using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SnakeManager : MonoBehaviour
{
    [Header("Snakes")]
    public Snake[] snakes; // 0 - первая, 1 - вторая

    [Header("Tilemaps")]
    public Tilemap groundMap;
    public Tilemap wallMap;

    [Header("Movement")]
    public float moveTime = 0.15f;

    private bool[] reachedFinish;
    private bool isMoving;

    private void Awake()
    {
        reachedFinish = new bool[snakes.Length];
    }

    private void Start()
    {
        foreach (var snake in snakes)
            snake.Init(groundMap, wallMap);
    }

    // Управление только первой змейкой
    public void OnMoveSnake1(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        if (dir == Vector2.zero || isMoving)
            return;

        StartCoroutine(MoveBoth(dir));
    }

    private IEnumerator MoveBoth(Vector2 dir)
    {
        if (isMoving)
            yield break;

        isMoving = true;

        var snake1 = snakes[0];
        var snake2 = snakes[1];

        // Проверяем, может ли двигаться первая
        if (!snake1.CanMove(dir))
        {
            Debug.Log("Первая змейка не может двигаться → блокируем обеих");
            isMoving = false;
            yield break;
        }

        // Вторая двигается в том же направлении
        Vector2 dir2 = dir;
        if (!snake2.CanMove(dir2))
        {
            Debug.Log("Вторая змейка не может двигаться → двигается только первая");
            StartCoroutine(MoveSnakeAsync(snake1, dir, () => isMoving = false));
            yield break;
        }

        bool snake1Moving = true;
        bool snake2Moving = true;

        // Запускаем обе корутины одновременно
        StartCoroutine(MoveSnakeAsync(snake1, dir, () => snake1Moving = false));
        StartCoroutine(MoveSnakeAsync(snake2, dir2, () => snake2Moving = false));

        // Ждём, пока обе завершат движение
        yield return new WaitUntil(() => !snake1Moving && !snake2Moving);

        isMoving = false;
    }

    private IEnumerator MoveSnakeAsync(Snake snake, Vector2 dir, System.Action onComplete)
    {
        snake.SetLastDirection(dir);
        yield return snake.MoveStep(dir, moveTime);
        onComplete?.Invoke();
    }

    public void SnakeReachedFinish(int index)
    {
        if (!reachedFinish[index])
        {
            reachedFinish[index] = true;
            Debug.Log($"Змейка {index + 1} достигла финиша!");
            CheckBothSnakesFinished();
        }
    }

    public void GrowSnake(int index)
    {
        snakes[index].Grow();
    }

    private void CheckBothSnakesFinished()
    {
        if (reachedFinish[0] && reachedFinish[1])
        {
            Debug.Log("Обе змейки достигли финиша! Уровень пройден!");
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1.5f); // короткая пауза перед переходом

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Проверяем, есть ли следующая сцена в Build Settings
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("Это последняя сцена — возвращаемся к первой!");
            SceneManager.LoadScene(0);
        }
    }
}