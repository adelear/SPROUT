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
    [SerializeField] public bool isDying; 
    [SerializeField] float currentWeight;
    [SerializeField] float currentJumpStrength;
    private CapsuleCollider2D collisionCollider; 

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

    [SerializeField] Transform[] spawnPointPositions;
    private int currentSpawnPoint = 0;
    private Animator anim;
    private SpriteRenderer sr; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isJumping = false;
        isBig = true;
        currentWeight = bigWeight;
        currentJumpStrength = bigJumpStrength;
        rb.mass = currentWeight;
        rb.gravityScale = bigGrav;
        collisionCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();    
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDying) return;
        anim.SetBool("isBig", isBig);
        hInput = Input.GetAxis("Horizontal");
        anim.SetFloat("hInput", Mathf.Abs(hInput)); 
        rb.velocity = new Vector2(hInput * speed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * currentJumpStrength, ForceMode2D.Impulse);
            anim.SetTrigger("Jump"); 
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

        if (hInput != 0)
            sr.flipX = (hInput > 0); 
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
        anim.SetBool("isBig", isLarge); 
        rb.mass = currentWeight;
    }

    public bool IsJumping()
    {
        return isGrounded && Input.GetButtonDown("Jump");
    }
    public void StartDying()
    {
        StartCoroutine(Dying()); 
    }
    private IEnumerator Dying()
    {
        Debug.Log("DYING");
        isDying = true;
        yield return new WaitForSeconds(3f);
        Respawn();
        isDying = false; 
    }

    public void UpdateSpawnPoint(int spawnNum)
    {
        currentSpawnPoint = spawnNum;
    }
    
    public int GetCurrentSpawnNum()
    {
        return currentSpawnPoint; 
    }

    public void Respawn()
    {
        if (spawnPointPositions.Length > 0 && currentSpawnPoint < spawnPointPositions.Length)
        {
            transform.position = spawnPointPositions[currentSpawnPoint].position;
        }
        else
        {
            Debug.LogWarning("Spawn point not set or invalid index.");
        }
    }

}
