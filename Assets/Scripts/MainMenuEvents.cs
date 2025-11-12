using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _button;
    private Button _quitButton;
    private AudioSource _audioSource;
    private List<Button> _menuButtons = new List<Button>();

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();

        // Get Start button
        _button = _document.rootVisualElement.Q<Button>("Start");
        _button.RegisterCallback<ClickEvent>(OnPlayGameClick);
        _quitButton = _document.rootVisualElement.Q<Button>("Quit");
        _quitButton.RegisterCallback<ClickEvent>(OnQuitGameClick);
            

        // Get all buttons
        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();

        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnPlayGameClick);

        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnPlayGameClick(ClickEvent evt)
    {
        Debug.Log("You pressed the start button");
        SceneManager.LoadSceneAsync(1);

    }
    private void OnQuitGameClick(ClickEvent evt)
    {
        Application.Quit();
    }

    private void OnAllButtonsClick(ClickEvent evt)
    {
        _audioSource.Play();
    }
}
