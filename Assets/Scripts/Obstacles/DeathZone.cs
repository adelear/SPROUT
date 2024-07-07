using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (!playerController.isDying) GameManager.Instance.Lives++;
            playerController.StartDying();  
        }
        
    }
}
