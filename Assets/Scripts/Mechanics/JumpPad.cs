using System.Collections;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Spring Properties")]
    [SerializeField] float downHeight = 5f;
    [SerializeField] float compressedHeight = 0.5f;
    [SerializeField] float launchHeight = 2f;
    [SerializeField] float compressionSpeed = 2f;
    [SerializeField] float launchSpeed = 10f;
    [SerializeField] float resetSpeed = 2f;

    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 downPosition;
    private bool isPlayerOnPad;
    private bool isCompressed;
    private PlayerController playerController;

    void Start()
    {
        originalPosition = transform.position;
        downPosition = new Vector3(originalPosition.x, originalPosition.y - downHeight, originalPosition.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            isPlayerOnPad = true;

            // Check if the player is big and the trampoline is not already compressed
            if (playerController.isBig && !isCompressed)
            {
                StopAllCoroutines();
                StartCoroutine(CompressSpring());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            isPlayerOnPad = false;

            // Check if the trampoline is compressed and reset it
            if (isCompressed)
            {
                StopAllCoroutines();
                StartCoroutine(ResetSpring());
            }
        }
    }

    void Update()
    {
        // Check if the player is on the pad, trampoline is compressed, player is small, and player is jumping
        if (isPlayerOnPad && isCompressed && playerController != null && !playerController.isBig && playerController.IsJumping())
        {
            StopAllCoroutines();
            StartCoroutine(LaunchPlayer());
        }
    }

    IEnumerator CompressSpring()
    {
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
        Vector3 launchPosition = new Vector3(originalPosition.x, originalPosition.y + launchHeight, originalPosition.z);
        while (Vector3.Distance(transform.position, launchPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, launchPosition, Time.deltaTime * launchSpeed);
            yield return null;
        }
        transform.position = launchPosition;
        yield return new WaitForSeconds(0.1f); // Wait a short duration to ensure the player is launched
        StartCoroutine(ResetSpring());
    }

    IEnumerator ResetSpring()
    {
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * resetSpeed);
            yield return null;
        }
        transform.position = originalPosition;
        isCompressed = false;
    }
}
