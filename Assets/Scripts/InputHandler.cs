using UnityEngine;

public class InputHandle : MonoBehaviour
{
   
    public bool PausePressed { get; private set; }

    void Update()
    {
        PausePressed = Input.GetKeyDown(KeyCode.Escape);
    }
}
