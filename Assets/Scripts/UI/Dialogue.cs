using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public Sprite[] images;
    public Image imgComponent; 
    public float textSpeed = 0.05f;
    public bool isOutro = false;
    public AudioClip dialogueSound;

    private int index;
    private Coroutine typingCoroutine;

    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    public void StartDialogue()
    {
        index = 0;
        StartTyping();
    }

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty; // Clear text before typing
        bool playSound = false; // Flag to determine whether to play sound for the current letter

        // Set the corresponding image for the current line
        if (index < images.Length)
        {
            imgComponent.sprite = images[index];
            imgComponent.enabled = true; // Enable the image component
        }
        else
        {
            imgComponent.enabled = false; // Disable the image component if no corresponding image
        }

        foreach (char c in lines[index].ToCharArray())
        {
            if (playSound)
            {
                AudioManager.Instance.PlayOneShot(dialogueSound, false); // Play dialogue sound for every other letter
            }
            textComponent.text += c;
            playSound = !playSound; // Toggle the flag
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void StartTyping()
    {
        GameManager.Instance.SwitchState(GameManager.GameState.PAUSE);
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartTyping();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        if (isOutro)
        {
            gameObject.SetActive(false);
            // Trigger Ending Received 
            // Move to gameOver
            GameManager.Instance.SwitchState(GameManager.GameState.DEFEAT);
            Debug.Log("Game Over");
            SceneTransitionManager.Instance.LoadScene("MainMenu");
        }
        else
        {
            gameObject.SetActive(false);
            GameManager.Instance.SwitchState(GameManager.GameState.GAME);
        }
    }

    private void Update() 
    {
        if (Input.anyKeyDown)  
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }
}