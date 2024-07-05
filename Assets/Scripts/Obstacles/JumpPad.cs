using System.Collections;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Spring Properties")]
    [SerializeField] float downHeight = 3f;
    [SerializeField] float compressedHeight = 0.5f;
    [SerializeField] float launchHeight = 4f;
    [SerializeField] float compressionSpeed = 2f;
    [SerializeField] float launchSpeed = 10f;
    [SerializeField] float resetSpeed = 5f;
    [SerializeField] float launchImpulse = 10f;

    [SerializeField] AudioClip compressSound;
    [SerializeField] AudioClip launchSound;

    private Vector3 originalPosition;
    private Vector3 downPosition;

    private bool isPlayerOnPad;
    private bool isCompressed;
    private bool isLaunching; 

    private PlayerController playerController;

    void Start()
    {
        originalPosition = transform.position;
        downPosition = new Vector3(originalPosition.x, originalPosition.y - downHeight, originalPosition.z);
        isPlayerOnPad = false;
        isCompressed = false; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            isPlayerOnPad = true;


            if (isCompressed && !playerController.isBig && isPlayerOnPad) // Ensure it only resets when not on the pad and compressed
            {
                // Reset the spring when the player is no longer on the pad
                StopAllCoroutines();
                StartCoroutine(ResetSpring());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            playerController = collision.gameObject.GetComponent<PlayerController>(); 
            isPlayerOnPad = false;
            /*
            if (!isPlayerOnPad && isCompressed) // Ensure it only resets when not on the pad and compressed
            {
                // Reset the spring when the player is no longer on the pad
                StopAllCoroutines();
                StartCoroutine(ResetSpring());
            }
            */ 
        }
    }

    void Update()
    {
        if (isPlayerOnPad) 
        {
            if (playerController.isBig && !isCompressed)
            {
                // Compress the spring
                StopAllCoroutines();
                StartCoroutine(CompressSpring());
            }
            else if (!playerController.isBig && isCompressed)
            {
                // Launch the player
                StopAllCoroutines();
                StartCoroutine(LaunchPlayer());
            }
        }

        if (isLaunching)
        {
            if (playerController.IsJumping())
            {
                if (playerController.GetComponent<Rigidbody2D>() != null)
                {
                    Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
                    rb.AddForce(Vector2.up * launchImpulse, ForceMode2D.Impulse);
                    AudioManager.Instance.PlayOneShot(launchSound, false); 
                }
            }
        }
    }

    IEnumerator CompressSpring()
    {
        Debug.Log("Compressing");
        AudioManager.Instance.PlayOneShot(compressSound, false); 
        isCompressed = true;
        while (Vector3.Distance(transform.position, downPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, downPosition, Time.deltaTime * compressionSpeed);
            yield return null;
        }
        transform.position = downPosition;
    }

    IEnumerator LaunchPlayer()
    {
        Debug.Log("Launching");
        isLaunching = true;
        Vector3 launchPosition = new Vector3(originalPosition.x, originalPosition.y + launchHeight, originalPosition.z);
        while (Vector3.Distance(transform.position, launchPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, launchPosition, Time.deltaTime * launchSpeed); 
            yield return null;  
        } 
        transform.position = launchPosition;
        isCompressed = false;
        isLaunching = false;
        //yield return new WaitForSeconds(0.05f); // Wait a short duration to ensure the player is launched
        StartCoroutine(ResetSpring()); 
    }

    IEnumerator ResetSpring()
    {
        if (!isCompressed) yield return null; 
        Debug.Log("Resetting"); 
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * resetSpeed);
            yield return null;
        }
        transform.position = originalPosition;
        isCompressed = false;
        // I want it to reset and then not change position 
    }
}
