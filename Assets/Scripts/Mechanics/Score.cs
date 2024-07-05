using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] int score = 10;
    [SerializeField] AudioClip collectSound; 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            GameManager.Instance.Score+=score;
            AudioManager.Instance.PlayOneShotWithRandomPitch(collectSound, false, 0.8f, 1.2f);
            Destroy(gameObject); 
        }
    }
}
 