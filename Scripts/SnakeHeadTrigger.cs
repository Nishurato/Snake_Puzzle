using UnityEngine;

public class SnakeHeadTrigger : MonoBehaviour
{
    public int snakeIndex; // индекс змейки в SnakeManager
    public SnakeManager manager; // ссылка на SnakeManager

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            manager.SnakeReachedFinish(snakeIndex);
        }
        else if (collision.CompareTag("Apple"))
        {
            manager.GrowSnake(snakeIndex); // вызываем рост
            Destroy(collision.gameObject); // удал€ем €блоко
        }
    }
}