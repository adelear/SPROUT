using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;

    [Header("Player Properties")]
    [SerializeField] public bool isBig;
    [SerializeField] public bool isJumping;
    [SerializeField] public bool isWalkingOnWater;
    [SerializeField] public bool isInWater;
    [SerializeField] float currentWeight;
    [SerializeField] float currentJumpStrength;

    [Header("Sizing Properties")]
    [SerializeField] float bigGrav = 2f;
    [SerializeField] float lilGrav = 1f;
    [SerializeField] float bigWeight = 100f;
    [SerializeField] float lilWeight = 10f;
    [SerializeField] float bigJumpStrength = 5f;
    [SerializeField] float lilJumpStrength = 10f;

    [Header("Movement Properties")]
    [SerializeField] float speed = 5f;
    [SerializeField] float hInput;
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isJumping = false;
        isBig = true;
        currentWeight = bigWeight;
        currentJumpStrength = bigJumpStrength;
        rb.mass = currentWeight;
        rb.gravityScale = bigGrav;
    }

    void Update()
    {
        hInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(hInput * speed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * currentJumpStrength, ForceMode2D.Impulse);
            Debug.Log("Jumping");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            isBig = !isBig;
            ToggleBig(isBig);
            Debug.Log("Toggle");
        }

        if (!isBig && isInWater)
        {
            ApplyBuoyancy(10f);
        }
        if (isWalkingOnWater) ApplyBuoyancy(10f); 
    }

    public void ApplyBuoyancy(float force)
    {
        rb.AddForce(Vector2.up * force, ForceMode2D.Force);
    }

    public void DisableBuoyancy()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f); // Stop vertical movement
    }

    public void EnableWalkingOnWater()
    {
        isWalkingOnWater = true;
        isInWater = false;
        rb.gravityScale = 0f; // Disable gravity while walking on water
        rb.velocity = new Vector2(rb.velocity.x, 0f);
    }

    public void DisableWalkingOnWater()
    {
        isWalkingOnWater = false;
        rb.gravityScale = isBig ? bigGrav : lilGrav; // Restore gravity based on size
    }

    void ToggleBig(bool isLarge)
    {
        if (isLarge)
        {
            currentWeight = bigWeight;
            currentJumpStrength = bigJumpStrength;
            rb.gravityScale = bigGrav;
            transform.localScale = Vector2.one;
        }
        else
        {
            currentWeight = lilWeight;
            currentJumpStrength = lilJumpStrength;
            rb.gravityScale = lilGrav;
            transform.localScale = new Vector2(0.5f, 0.5f);
        }

        rb.mass = currentWeight;
    }

    public bool IsJumping()
    {
        return isGrounded && Input.GetButtonDown("Jump");
    }
}
