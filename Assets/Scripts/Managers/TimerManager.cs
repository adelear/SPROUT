using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; } 

    private float elapsedTime; 
    private bool timerRunning;
    [SerializeField] TMP_Text timerText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Optional: to keep the timer persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        elapsedTime = 0f;
        timerRunning = false;
    }

    private void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void EndTimer()
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay();
    } 

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
