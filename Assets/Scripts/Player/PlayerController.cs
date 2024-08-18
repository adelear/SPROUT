using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
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
    [SerializeField] float ceilingCheckDistance = 2f; 

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
    private bool isWallJumping;

    [Header("Sound Properties")]
    [SerializeField] AudioClip[] popSounds;
    [SerializeField] AudioClip deadSound;
    [SerializeField] AudioClip jumpSound; 
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
    [SerializeField] private bool canToggle = true; 

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
        if (GameManager.Instance.GetGameState() != GameManager.GameState.GAME) return; 
        anim.SetBool("isBig", isBig);
        if (!isWallJumping) hInput = Input.GetAxis("Horizontal");
        anim.SetFloat("hInput", Mathf.Abs(hInput));
        if (!isWallJumping) rb.velocity = new Vector2(hInput * speed, rb.velocity.y);
        if (hInput != 0 || !isWallJumping)
            sr.flipX = (hInput < 0);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        Vector2 wallCheckDirection = sr.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, wallCheckDirection, wallCheckDistance, wallLayer);
        isOnWall = hit.collider != null;

        Debug.DrawRay(wallCheck.position, Vector2.right * (sr.flipX ? -1 : 1) * wallCheckDistance, Color.red);

        if (isGrounded && !isBig)
        {
            canWallJump = true; // Reset wall jump ability when grounded
            currentWall = null;
            isWallJumping = false; 
        }

        if ((isGrounded && Input.GetButtonDown("Jump")) || isGrounded && Input.GetKeyDown(KeyCode.LeftControl)) 
        {
            rb.AddForce(Vector2.up * currentJumpStrength, ForceMode2D.Impulse);
            AudioManager.Instance.PlayOneShotWithRandomPitch(jumpSound, false, 0.8f, 1.2f); 
            anim.SetTrigger("Jump");
            Debug.Log("Jumping");
        }
        else if (!isGrounded && isOnWall && !isBig)
        {
            if (currentWall != hit.collider.gameObject) // Check if the player is on a different wall
            {
                currentWall = hit.collider.gameObject; // Update current wall
                canWallJump = true;
                Vector2 wallNormal = hit.normal;
                //sr.flipX = wallNormal.x < 0;
                StopAllCoroutines();

                //rb.velocity = new Vector2(rb.velocity.x, 0f);
                //lilGrav = 0.5f;
                //rb.gravityScale = lilGrav;

            }
        }
        else
        {
            //lilGrav = 1f;
            //rb.gravityScale = lilGrav;
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
        if ((Input.GetButtonDown("Jump") && isOnWall && canWallJump && !isBig) || (Input.GetKeyDown(KeyCode.LeftControl) && isOnWall && canWallJump && !isBig)) 
        {
            //rb.velocity = new Vector2(rb.velocity.x, 0); // Reset vertical velocity
            Vector2 wallNormal = hit.normal; 
            rb.AddForce(new Vector2(-wallCheckDirection.x * wallJumpForce, Mathf.Abs(wallCheckDirection.x) * (wallJumpForce/1.5f) * Mathf.Sin(0.785f)), ForceMode2D.Impulse);  
            anim.SetTrigger("Jump");
            AudioManager.Instance.PlayOneShotWithRandomPitch(jumpSound, false, 0.8f, 1.2f);
            isWallJumping = true;
            //sr.flipX = !sr.flipX;  
            //sr.flipX = wallNormal.x < 0; 
            StartCoroutine(ResetIsWallJumping()); 
            Debug.Log("Wall Jumping");
            canWallJump = false; // Disable further wall jumps until grounded or on a different wall
        }

        if ((Input.GetKeyDown(KeyCode.T) && canToggle) || (Input.GetKeyDown(KeyCode.LeftAlt) && canToggle))
        {
            isBig = !isBig;
            ToggleBig(isBig);
            int randomNum = Random.Range(1, 3);
            AudioManager.Instance.PlayOneShot(popSounds[randomNum], false); 
            Debug.Log("Toggle");
        }

        if (!isBig && isInWater)
        {
            ApplyBuoyancy(10f); 
        }
        if (isWalkingOnWater) ApplyBuoyancy(10f);

        if (isWallJumping) rb.velocity = new Vector2(-wallCheckDirection.x * speed, rb.velocity.y); 

        if (!isBig)
        {
            RaycastHit2D hitCeiling = Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, groundLayer);
            RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, ceilingCheckDistance, groundLayer);
            if (hitCeiling.collider != null && hitGround.collider !=null) canToggle = false;
            else canToggle = true;
            Debug.DrawRay(transform.position, Vector2.up * ceilingCheckDistance, Color.red);
        }
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

    IEnumerator ResetIsWallJumping()
    {
        yield return new WaitForSeconds(0.5f);
        isWallJumping = false; 
    }
    public bool IsJumping()
    {
        return isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.LeftControl)); 
    }
    public void StartDying()
    {
        StartCoroutine(Dying());
    } 
    private IEnumerator Dying()
    {
        Debug.Log("DYING");
        AudioManager.Instance.PlayOneShot(deadSound, false); 
        isDying = true;
        rb.velocity = Vector2.zero; 
        anim.SetTrigger("Death"); 
        yield return new WaitForSeconds(2f);
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
