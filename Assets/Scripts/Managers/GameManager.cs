using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Bloqueia o cursor no centro da tela
        Cursor.visible = false;  // Torna o cursor invisível
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
