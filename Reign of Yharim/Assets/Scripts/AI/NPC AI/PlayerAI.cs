using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerAI : NPC //basically, this script is a copy of the npc script and all of it's values. the main differences are that each value can be overriden from the base script for the new one, and this one can be attached to gameobjects.
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration;
    [SerializeField] private float deacceleration;
    [SerializeField] private float velPower;
    private float xAxis;

    [Header("Jumping")]
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float jumpReleaseMod;
    private bool isJumping;
    private bool isFalling;

    [Header("Ground Detection")]
    [SerializeField] private CapsuleCollider2D cc2d;
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private float rayHeight;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    private Vector2 bottomPoint;
    private bool isGrounded;
    private float facingDirection;

    //constants can't be changed
    const string PlayerIdle = "Player_idle";
    const string PlayerWalk = "Player_walk";
    const string PlayerJump = "Player_jump";
    const string PlayerRun = "Player_run";
    const string PlayerAttack = "Player_attack";


    public override void SetDefaults()
    {
        NPCName = "Player";
        damage = 0; //Note to future developers/self, this can be used for times when the player does deal contact damage to enemies. armor sets are an example. right now, it's useless.
        lifeMax = 100;
        life = lifeMax;
        healthBar.SetMaxHealth(lifeMax);

        rb = GetComponent<Rigidbody2D>(); //PlayerAI.rb equals the rigidbody2d of the player
        cc2d = GetComponent<CapsuleCollider2D>();
        spr = GetComponent<SpriteRenderer>();

        rb.velocity = new Vector2(rb.velocity.x, Vector2.zero.y);
    }
    public override void AI() //every frame (Update)
    {
        Physics2D.IgnoreLayerCollision(10, 3); //Layer 10 (WalkThroughNPCSPlayer) will ignore collisions with layer 3 (NPCS) the child gameobjects don't use layer 10, so they can still detect collisions

        xAxis = Input.GetAxis("Horizontal"); //sets horizontal to -1 or 1 based on the player's input

        if (Input.GetButtonDown("Jump")) //if the jump button is being pressed...
        {
            if (isGrounded) //and the player is grounded...
            {
                StartCoroutine(JumpWithDelay()); //start the JumpWithDelay coroutine
            }
        }

        if (Input.GetButtonUp("Jump")) //if the jump button is released...
        {
            OnJumpUp(); //trigger the OnJumpUp method
        }

        bottomPoint = new Vector2(cc2d.bounds.center.x, cc2d.bounds.min.y); //the bottompoint variable equals the bottommost y point and center x point of the capsule collider

        //Debug.Log("IsJumping: " + isJumping + " IsFalling: " + isFalling);

        //Debug.Log(rb.velocity.y);

        /* if (isGrounded)
            isFalling = false;

        if (rb.velocity.y < -3f)
        {
            isFalling = true;
        }
        */
    }
    public override void OnKill()
    {
        GameObject.Find("WorldManager").SendMessage("Respawn"); //tell the worldmanager to respawn the player
    }
    private void Movement()
    {
        float targetSpeed = xAxis * moveSpeed;

        float speedDif = targetSpeed - rb.velocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deacceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);

        animator.speed = Mathf.Abs(targetSpeed / 10);

        if (isGrounded) //if the player is grounded and isn't attacking
        {
            if (xAxis != 0) //if the player isn't still
            {
                ChangeAnimationState(PlayerWalk); //set the animation to walking
            }
            else
            {
                ChangeAnimationState(PlayerIdle); //set the animation to idle
            }
        }
        if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, 1); //facing right
            facingDirection = 1f;
        }
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, 1); //facing left
            facingDirection = -1f;
        }
    }
    private void Jump()
    {
        isJumping = true; //set isjumping to true

        rb.velocity = new Vector2 (rb.velocity.x, Vector2.zero.y);

        rb.AddForce(Vector2.up * jumpingPower, ForceMode2D.Impulse); //the jumping script
    }
    public void OnJumpUp()
    {
        if (isJumping) //if the player is jumping
        {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpReleaseMod), ForceMode2D.Impulse); //apply downward force to cut the player's jump
        }
    }
    private void FixedUpdate() //for physics
    {
        #region GroundDetection
        Color rayCol;
        RaycastHit2D hit = Physics2D.Raycast(bottomPoint, Vector2.down, rayHeight, groundLayer);

        if (hit)
        {
            if (!isJumping)
            {
                float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity); //math stuff
                float otherDirVel = Vector2.Dot(Vector2.down, Vector2.zero);
                float relVel = rayDirVel - otherDirVel;
                float x = hit.distance - rideHeight;
                float force = (x * rideSpringStrength) - (relVel * rideSpringDamper);

                rb.AddForce(Vector2.down * force);
            }

            rayCol = Color.green;
            isGrounded = true;
        }
        else
        {
            rayCol = Color.red;
            isGrounded = false;
        }
        #endregion

        Movement();
    }
    private IEnumerator JumpWithDelay() //this entire thing just triggers jump and waits until the player is falling to change the variable
    {
        Jump(); //trigger the jump method

        while (rb.velocity.y > 0) //while the player is still going up
        {
            yield return null; //return null (creates a loop until rb.velocity is less than ot equal to zero
        }

        isJumping = false; //set isjumping to false (doesn't trigger until the above loop is done)
    }
}
