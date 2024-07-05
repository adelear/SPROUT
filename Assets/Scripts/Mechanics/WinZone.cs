using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    [SerializeField] GameObject outroDialogue;
    [SerializeField] AudioClip winSound; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            AudioManager.Instance.PlayOneShot(winSound, false); 
            PlayerController playerController = collision.GetComponent<PlayerController>();
            GameManager.Instance.SwitchState(GameManager.GameState.WIN); 
            playerController.rb.velocity = Vector3.zero; 
            TimerManager.Instance.EndTimer();
            outroDialogue.SetActive(true);
        }
    }
}
