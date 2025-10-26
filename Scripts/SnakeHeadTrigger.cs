using UnityEngine;

public class SnakeHeadTrigger : MonoBehaviour
{
    public int snakeIndex; // ������ ������ � SnakeManager
    public SnakeManager manager; // ������ �� SnakeManager

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            manager.SnakeReachedFinish(snakeIndex);
        }
        else if (collision.CompareTag("Apple"))
        {
            manager.GrowSnake(snakeIndex); // �������� ����
            Destroy(collision.gameObject); // ������� ������
        }
    }
}