using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/////////////// INFORMATION ///////////////
// This script is intended to work on a GameObject with
// a Rigidbody2D-component (tip: it probably needs constraints),
// a CapsuleCollider2D-component (or other Collider2D-shape that fits your character)
// and a PlayerInput-component.

// Additionally this script also needs to reference a Collider2D to use for ground-check,
// This groundcheck-collider does not need to be placed on the same object, it could instead be placed on a child object.
// This groundcheck-collider should be a trigger-collider.

// Use UnityEvents in the PlayerInput-component to call movement-functions in this script.

// This script has setup for playing animations and sounds.

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float raycastoriginVerticalOffset = 0.5f;
    
    
    // Physics and collision
    [FormerlySerializedAs("rigidbody")] [SerializeField] private Rigidbody2D myRigidbody;
    [SerializeField] private CircleCollider2D groundCheckCollider;
    [SerializeField] private LayerMask groundLayer = ~0; // ~0 is referring to EVERY layer.
    private LayerMask raycastLayerMask;
    // Handle input
    public bool controlEnabled { get; set; } = true; // You can edit this variable from Unity Events
    private Vector2 moveInput;
    private Vector2 velocity;
    private bool jumpInput;
    private bool wasGrounded;
    private bool isGrounded;

    [Header("Graphics")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        
        myRigidbody.gravityScale = 0; // Set gravity scale to 0 so player won't "fall". We will apply our own gravity!
        velocity = new Vector2(4,0);
        //animator = GetComponent<Animator>();
        raycastLayerMask = LayerMask.GetMask("Ground");
        moveInput = Vector2.right * maxSpeed;
    }
    
    void FixedUpdate()
    {
        Vector2 from =  new Vector2(transform.position.x, transform.position.y - raycastoriginVerticalOffset);
        Vector2 dir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        const float jumpLen = 1.0f;
        const float flipLen = 0.6f;
        if(Physics2D.Raycast(from, dir, jumpLen, raycastLayerMask)) //jump RC
        {
            if (Physics2D.Raycast(from + Vector2.up, dir, jumpLen, raycastLayerMask) == false)
            {
                jumpInput = true;
                moveInput = dir * 0.5f;
                Debug.DrawRay(from + Vector2.up, dir * jumpLen, Color.green, 0.2f);
            }
            else
            {
                Debug.DrawRay(from + Vector2.up, dir * jumpLen, Color.red, 0.2f);
            }
            Debug.DrawRay(from, dir * jumpLen, Color.yellow, 0.1f);
        }
        else
        {
            jumpInput = false;
            moveInput = dir;
            Debug.DrawRay(from, dir * jumpLen, Color.gray6, 0.1f);
            return;
        }
        
        
        if (Physics2D.Raycast(from, dir, flipLen, raycastLayerMask))
        {
            Debug.DrawRay(from, dir * flipLen, Color.red, 0.8f);
            //if(velocity.y <= 0.01) 
            moveInput = -dir;
        }
        else
        {
            Debug.DrawRay(from, dir * flipLen, Color.white, 0.3f);
        }
    }
    
    void Update()
    {
        //---Handle vertical velocity---
        
        isGrounded = IsGrounded();
        ApplyGravity();
        
        // Apply jump-input:
        if (jumpInput && wasGrounded)
        {
            velocity.y = jumpForce;
            //jumpInput = false;
        }
        
        // Check if character lost contact with ground this frame
        if (wasGrounded == true && isGrounded == false)
        {
            if (velocity.y < 0)
            {
                // Has fallen. Play fall sound and/or trigger fall animation etc
            }
            else
            {
                // Has jumped. Play jump sound and/or trigger jump animation etc
            }
        }
        // Check if character gained contact with ground this frame
        else if (wasGrounded == false && isGrounded == true)
        {
            // Has landed, play landing sound and trigger landing animation
        }
        
        myRigidbody.linearVelocity = velocity;
        wasGrounded = isGrounded;
        
        //---Handle horizontal velocity---
        velocity.x = moveInput.x * maxSpeed;
        
        //animator.SetFloat("HorizontalMovement", moveInput.x);
        //animator.SetBool("walkBool", Mathf.Abs(moveInput.x) > 0.1);
        
        if (moveInput.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (moveInput.x < -0.01f)
            spriteRenderer.flipX = true;
        
    }

    private bool IsGrounded()
    {
        return groundCheckCollider.IsTouchingLayers(groundLayer);
    }

    private void ApplyGravity()
    {
        // Applies a set gravity for when player is grounded
        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = -1.0f;
        }
        // Updates fall speed with gravity if object isn't grounded
        else
        {
            // Is Jumping upwards
            if (velocity.y > 0)
            {
                float deceleration = 1;
                if (!jumpInput) // shorter jump height when releasing the jump-key
                {
                    deceleration = 2.5f;
                }
                // you can add a gravity multiplier here... but how?
                velocity.y += Physics2D.gravity.y * deceleration * Time.deltaTime;
            }
            // Is Falling
            else
            {
                // you can add a gravity multiplier here... but how?
                velocity.y += Physics2D.gravity.y * Time.deltaTime * 2;
            }
        }
    }

    // Handle Move-input
    // This method can be triggered through the UnityEvent in PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        if (controlEnabled)
        {
            moveInput = context.ReadValue<Vector2>().normalized;
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }
    
}
