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
    [SerializeField] float currentWeight;
    [SerializeField] float currentJumpStrength; 

    [Header("Sizing Properties")]
    [SerializeField] float bigWeight = 100f;
    [SerializeField] float lilWeight = 10f;
    [SerializeField] float bigJumpStrength = 5f;
    [SerializeField] float lilJumpStrength = 10f;

    [Header("MovementProperties")] 
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
            toggleBig(isBig);
            Debug.Log("Toggle");
        }
    }

    void toggleBig(bool isLarge)
    {
        if (isLarge)
        {
            currentWeight = bigWeight;
            currentJumpStrength = bigJumpStrength; 
        }
        else
        {
            currentWeight = lilWeight;
            currentJumpStrength = lilJumpStrength; 
        }

        rb.mass = currentWeight; 
    }

    public bool IsJumping()
    {
        return isGrounded && Input.GetButtonDown("Jump");
    }
}
