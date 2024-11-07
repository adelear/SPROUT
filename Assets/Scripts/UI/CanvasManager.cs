using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] AudioManager asm;  
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pauseSound;
    [SerializeField] AudioClip gameMusic;
    [SerializeField] AudioClip titleMusic;
    [SerializeField] AudioClip buttonSound; 

    [SerializeField] AudioMixer audioMixer;

    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button backButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button returnToMenuButton;
    [SerializeField] Button backToPauseMenu;
    [SerializeField] Button resumeGame;

    [Header("Menus")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;

    [Header("Text")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text deathText;

    [Header("Sliders")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [SerializeField] private Button[] menuButtons; 
    [SerializeField] int currentButtonIndex = 0; 

    void Start()
    {
        asm = GetComponent<AudioManager>();

        if (masterSlider)
        {
            masterSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(value, "MasterVol"));
        }

        if (musicSlider)
        {
            musicSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(value, "MusicVol"));
        }

        if (sfxSlider)
        {
            sfxSlider.onValueChanged.AddListener((value) => OnSliderValueChanged(value, "SFXVol")); 
        }

        if (menuButtons.Length > 0)
        {
            SelectButton(menuButtons[currentButtonIndex]);
        }

        InitializeButton(startButton, StartGame);
        InitializeButton(settingsButton, ShowSettingsMenu);
        InitializeButton(backButton, ShowMainMenu);
        InitializeButton(backToPauseMenu, UnpauseGame);
        InitializeButton(quitButton, Quit);
        InitializeButton(resumeGame, UnpauseGame);
        InitializeButton(returnToMenuButton, LoadTitle);

        //Add listener to update death and score for arcade machine update
    }

    void InitializeButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
            EventTrigger buttonTrigger = button.gameObject.AddComponent<EventTrigger>();
            AddPointerEnterEvent(buttonTrigger, PlayButtonSound);
        }
    }

    void LoadTitle()
    {
        SceneTransitionManager.Instance.LoadScene("MainMenu");
    }
    void UnpauseGame()
    {
        Time.timeScale = 1.0f;
        GameManager.Instance.SwitchState(GameManager.GameState.GAME);
        pauseMenu.SetActive(false);
        if (settingsMenu) settingsMenu.SetActive(false);
    }

    void UpdateScoreText(int value)
    {
        scoreText.text = value.ToString();
    }

    void UpdateDeathText(int value)
    {
        deathText.text = value.ToString();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                NavigateButtons(-1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                NavigateButtons(1);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuButtons[currentButtonIndex].onClick.Invoke();
            }
        }

        if (!pauseMenu) return;
        if (pauseMenu.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                NavigateButtons(-1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                NavigateButtons(1);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuButtons[currentButtonIndex].onClick.Invoke();
            }
        } 

        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            if (settingsMenu) settingsMenu.SetActive(false); 


            if (pauseMenu.activeSelf)
            {
                Time.timeScale = 0f; 
                GameManager.Instance.SwitchState(GameManager.GameState.PAUSE);   
                asm.PlayOneShot(pauseSound, false);
                pauseMenu.SetActive(true);
            }

            else
            {
                UnpauseGame();
            }

        }
    }

    void NavigateButtons(int direction)
    {
        DeselectButton(menuButtons[currentButtonIndex]);
        currentButtonIndex = (currentButtonIndex + direction + menuButtons.Length) % menuButtons.Length;
        SelectButton(menuButtons[currentButtonIndex]);
    }


    void SelectButton(Button button)
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        var colors = button.colors;
        button.image.color = colors.highlightedColor;
    }

    void DeselectButton(Button button)
    {
        menuButtons[currentButtonIndex].OnDeselect(null); 
        var colors = button.colors;
        button.image.color = colors.normalColor;
    }

    void ShowSettingsMenu()
    {
        if (mainMenu) mainMenu.SetActive(false);
        if (pauseMenu) pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        if (masterSlider) 
        {
            float value;
            audioMixer.GetFloat("MasterVol", out value);
            masterSlider.value = value + 80;
        }
        else
        {
            Debug.LogError("Audio Mixer is not assigned.");
        }

        if (musicSlider)
        {
            float value;
            audioMixer.GetFloat("MusicVol", out value);
            musicSlider.value = value + 80; 
        }

        if (sfxSlider)
        {
            float value;
            audioMixer.GetFloat("SFXVol", out value);
            sfxSlider.value = value + 80;
        }
    }

    void ShowMainMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    void StartGame()
    {
        SceneTransitionManager.Instance.LoadScene("Alaska's Scene"); ;
        Time.timeScale = 1.0f;
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = gameMusic;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Audio Source is not assigned.");
        }
    }

    void OnSliderValueChanged(float value, string volume)
    {
        audioMixer.SetFloat(volume, value - 80);
    }

    private void AddPointerEnterEvent(EventTrigger trigger, UnityEngine.Events.UnityAction action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => action.Invoke());
        trigger.triggers.Add(entry);
    } 

    void PlayButtonSound()
    {
        asm.PlayOneShot(buttonSound, false);
    }

    void Quit() 
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}