using System.Collections;
using UnityEngine;

public class WaterSurface : MonoBehaviour
{
    [SerializeField] float buoyancyForce = 5f;
    private PlayerController playerController;
    [SerializeField] AudioClip waterIn;
    [SerializeField] AudioClip waterOut;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController player))
        {
            playerController = player;
            playerController.isInWater = true;
            if (playerController.isBig)
            {
                // Apply sinking force
                playerController.ApplyBuoyancy(-buoyancyForce);
                playerController.isWalkingOnWater = false;
                AudioManager.Instance.PlayOneShotWithRandomPitch(waterIn, false, 0.8f, 1.2f); 
            }
            else
            {
                // Ensure the player walks over water 
                playerController.EnableWalkingOnWater();
                playerController.ApplyBuoyancy(buoyancyForce);
                playerController.isWalkingOnWater = true;
                AudioManager.Instance.PlayOneShotWithRandomPitch(waterOut, false, 0.8f, 1.2f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController player))
        {
            playerController = player;
            if (playerController.isBig)
            {
                playerController.ApplyBuoyancy(-buoyancyForce);
            }
            else
            {
                playerController.ApplyBuoyancy(buoyancyForce);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController player))
        {
            playerController = player;
            playerController.isInWater = false;
            playerController.isWalkingOnWater = false;
            playerController.DisableBuoyancy();
            playerController.DisableWalkingOnWater();
        }
    }
}
