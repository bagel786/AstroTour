using UnityEngine;

public class SimpleTerminalInteract : MonoBehaviour, IInteractable
{
    [Header("Console Reference")]
    public SimpleConsolePassword consolePassword;

    public bool canInteract()
    {
        return consolePassword != null;
    }

    public void Interact()
    {
        if (canInteract())
        {
            consolePassword.OpenConsole();
        }
    }
}