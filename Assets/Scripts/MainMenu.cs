using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject settingsMenu;
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenSettings()
    {

        settingsMenu.SetActive(true);
        // gameObject.SetActive(false);
    }
    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        // gameObject.SetActive(true);
    }

}
