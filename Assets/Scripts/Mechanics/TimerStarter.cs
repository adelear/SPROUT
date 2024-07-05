using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerStarter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            TimerManager.Instance.StartTimer(); 
        }
    }
}
