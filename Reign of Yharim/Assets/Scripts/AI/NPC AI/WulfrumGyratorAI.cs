using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WulfrumGyratorAI : NPC
{
    private bool isGrounded = false;
    private int curTargetPos;
    private bool spotted;
    private Color color;

    const string GyratorSpin = "WulfrumGyrator_spin";
    const string ChargeGyratorSpin = "WulfrumGyrator_chargespin";
    public override void SetDefaults()
    {
        NPCName = "WulfrumGyrator";
        damage = 15;
        lifeMax = 18;
        life = lifeMax;
        healthBar.SetMaxHealth(lifeMax);
        spotted = false;

        bc2d = GetComponent<BoxCollider2D>();
    }
    public override void AI()
    {
        if (target != null)
        {
            animator.speed = 0.8f;
            ChangeAnimationState(GyratorSpin);

            ai[0]++;//increment ai[0] by 1 every frame.(the framerate is capped at 60)
            velocity *= 0.97f;//this is for smoothing the movement.

            if (GetDistanceToPlayer() > 20f && spotted == false) //if the player isn't close and the bool spotted equals false
            {
                color = Color.green; //set color variable to green

                if (ai[0] == 70.0f) //if it has been 70 frames, jump.
                    velocity.y = 0.5f; //jump up vertically

                if (ai[0] > 100.0f && isGrounded)
                {
                    velocity = Vector2.zero; //stop the slime from moving
                    ai[0] = 0.0f;//reset ai[0] so we can jump again.
                }
            }
            else
            {
                if (isGrounded)
                {
                    ai[0] = 0.0f;
                    spotted = true;
                    color = Color.red;
                    if (ai[0] == 0.0f)
                    {
                        velocity.x = DirectionTo(target.transform.position).x * 0.12f; //speed of 
                    }

                    /*if (//placeholder)
                    {
                        ai[0]++;
                        velocity *= 0.97f;
                        if (ai[0] == 1.0f) 
                        {
                            curTargetPos = GetTargetDirectionX();
                            velocity.x = curTargetPos * 0.2f; //jump towards the player
                            velocity.y = 0.2f; //at the same time jump up
                        }
                        if (ai[0] > 1.0f && !isGrounded)
                        {
                            velocity.x = curTargetPos * 0.1f; //continue going horizontally
                        }
                        if (ai[0] > 100.0f && isGrounded) //if ai[0] is greater than 100.0f (if this wasnt here, the slime would constantly reset when touching ground) and if the slime is grounded
                        {
                            velocity = Vector2.zero; //stop the slime from moving
                            ai[0] = 0.0f;//reset ai[0] so we can jump again.
                        }
                    }*/
                }

            }

            if (!IsVisibleFromCamera())
            {
                spotted = false;
                ai[0] = 0.0f;
            }

            DrawDistanceToPlayer(color);

            if (GetDistanceToPlayer() > 60f && spotted == false)
            {
                Destroy(gameObject); //despawn the object
            }
        }
    }
    private void FixedUpdate()
    {
        float extraHeight = 0.4f; //new float extraHeight equals 0.1
        Color rayColor; //new color variable rayColor 

        RaycastHit2D hit = Physics2D.Raycast(bc2d.bounds.center, Vector2.down, bc2d.bounds.extents.y + extraHeight, groundLayer); //new raycast2d called hit that starts from the center of the player rigidbody, goes down, and goes the extent of the rigidbody downwards + extraHeight. it only collides with the groundlayer variable

        //Debug.Log(hit.collider);
        if (hit.collider != null) //if the raycast is hitting something;
        {
            isGrounded = true; //isgrounded is true
            rayColor = Color.green; //the raycolor is green
        }
        else
        {
            isGrounded = false; //isgrounded is false
            rayColor = Color.red; //the raycolor is red
        }
        Debug.DrawRay(bc2d.bounds.center, Vector2.down * (bc2d.bounds.extents.y + extraHeight), rayColor); //draw the ray 
    }
}
