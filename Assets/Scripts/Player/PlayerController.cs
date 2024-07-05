using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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

    [Header("Wall Jump Properties")]
    [SerializeField] float wallJumpForce = 5f;
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallCheckDistance = 0.1f;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] private bool isOnWall = false;
    [SerializeField] private bool canWallJump = true;
    [SerializeField] private GameObject currentWall = null; 
    //When against a wall when NOT big, gravity gets halved, and their jump gets replenished (they can jump again) 
    //The jump has a little impulse opposite to the wall they are sliding on 
    //If they jump off their wall using their replenished jump, and land back on the wall they were on, their jump does not get replenished and they cannot slide on the wall
    // Must jump onto a new wall when using their replenished jump 


    [SerializeField] Transform[] spawnPointPositions;
    private int currentSpawnPoint = 0;
    private CapsuleCollider2D smallCollider;
    private CircleCollider2D bigCollider;
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
        smallCollider = GetComponent<CapsuleCollider2D>();
        bigCollider = GetComponent<CircleCollider2D>(); 
        anim = GetComponent<Animator>();    
        sr = GetComponent<SpriteRenderer>();

        smallCollider.enabled = false; 
    }

    void Update()
    {
        if (isDying) return;
        anim.SetBool("isBig", isBig);
        hInput = Input.GetAxis("Horizontal");
        anim.SetFloat("hInput", Mathf.Abs(hInput));
        rb.velocity = new Vector2(hInput * speed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        Vector2 wallCheckDirection = sr.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, wallCheckDirection, wallCheckDistance, wallLayer);
        isOnWall = hit.collider != null;

        Debug.DrawRay(wallCheck.position, Vector2.right * (sr.flipX ? -1 : 1) * wallCheckDistance, Color.red); 

        if (isGrounded)
        {
            canWallJump = true; // Reset wall jump ability when grounded
            currentWall = null;
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector2.up * currentJumpStrength, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
            Debug.Log("Jumping");
        }
        else if (!isGrounded && isOnWall)
        {
            if (currentWall != hit.collider.gameObject) // Check if the player is on a different wall
            {
                currentWall = hit.collider.gameObject; // Update current wall
                canWallJump = true; 
            }
        }
        if (Input.GetButtonDown("Jump") && isOnWall && canWallJump && !isGrounded)
        {
            //rb.velocity = new Vector2(rb.velocity.x, 0); // Reset vertical velocity
            Vector2 wallNormal = hit.normal; 
            rb.AddForce(new Vector2(-wallCheckDirection.x * wallJumpForce * 10, Mathf.Abs(wallCheckDirection.x) * wallJumpForce * Mathf.Sin(0.785f)), ForceMode2D.Impulse);  
            anim.SetTrigger("Jump");
            Debug.Log("Wall Jumping");
            canWallJump = false; // Disable further wall jumps until grounded or on a different wall
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
            sr.flipX = (hInput < 0);
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
            smallCollider.enabled = false;
            bigCollider.enabled = true;
        }
        else
        {
            currentWeight = lilWeight;
            currentJumpStrength = lilJumpStrength;
            rb.gravityScale = lilGrav;
            bigCollider.enabled = false;
            smallCollider.enabled = true;
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
