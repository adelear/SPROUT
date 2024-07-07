using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] AudioManager asm; 
    [SerializeField] AudioClip LossSound;
    
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        GAME,
        PAUSE,
        DEFEAT,
        WIN
    }
    [SerializeField] private GameState currentState; 

    public event Action OnGameStateChanged;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (Instance != this)
            Destroy(gameObject);
    }

    public int Score
    {
        get => score;
        set
        {
            score = value;

            if (OnScoreValueChanged != null)
                OnScoreValueChanged.Invoke(score); 
        }
    }
    private int score = 0;
    public UnityEvent<int> OnScoreValueChanged;

    public int Lives
    {
        get => lives;
        set
        {
            lives = value;

            if (OnLifeValueChanged != null)
                OnLifeValueChanged.Invoke(lives);
        }
    }

    public UnityEvent<int> OnLifeValueChanged;

    private int lives = 0;


    public GameState GetGameState()
    {
        return currentState;
    }

    public void SwitchState(GameState newState)
    {
        Debug.Log("New state has been set to " + newState); 
        currentState = newState;
        OnGameStateChanged?.Invoke(); 
    }
}

