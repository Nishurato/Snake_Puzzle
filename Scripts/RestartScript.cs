using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour
{
    public void RestartGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log($"Restart pressed from {context.control.device.displayName}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
