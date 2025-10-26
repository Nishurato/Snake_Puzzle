using UnityEngine;

public class AppleScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Snake snake = collision.GetComponent<Snake>();
        if (snake == null)
            snake = collision.GetComponentInParent<Snake>();

        // Если это действительно змейка — увеличиваем её
        if (snake != null)
        {
            snake.Grow();
            AppleRemove();
        }
    }

    private void AppleRemove()
    {
        Destroy(this);
    }
}
