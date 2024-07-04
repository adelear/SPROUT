using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkPointNum;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            if (collision.GetComponent<PlayerController>().GetCurrentSpawnNum() < checkPointNum) collision.GetComponent<PlayerController>().UpdateSpawnPoint(checkPointNum); 
        }
    }
}

