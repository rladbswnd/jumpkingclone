using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 3;
    public float jumpHeight = 10;
    public float minJumpTime = 0.2f;
    public float maxJumpTime = 0.8f;
    public float jumpSpeedMultiplier = 4f;
    public float reflectForce = 1.5f;
    public float fallingSpeedLimit = -15f;

    [Header("Pre-Assignments")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 groundSize = new Vector2(0.3f, .1f);
    public Collider2D wallCollider;

    float vx,spvx;
    bool isGrounded, wasGrounded, isJumping, canMove = true;
    bool rightFlag, fulljump;
    float jumpTimeout;

    Rigidbody2D rbody;
    SpriteRenderer sprite;
    Animator anim;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundSize, 0f, groundLayer);

        if (!wasGrounded && isGrounded)
        {
            wallCollider.enabled = false;
            canMove = true;
        }

        vx = Input.GetAxisRaw("Horizontal") * speed;

        if (vx < 0)
            rightFlag = false;
        else if (vx > 0)
            rightFlag = true;

        if (canMove)
            sprite.flipX = !rightFlag;

        if (isGrounded && Mathf.Abs(rbody.velocity.y) <= 0.01f)
        {
            if (Input.GetKeyDown("space"))
            {
                spvx = vx;
            }
            if (Input.GetKey("space"))
            {
                jumpTimeout += Time.deltaTime;
                rbody.velocity = Vector2.zero;
                isJumping = true;
                canMove = false;
                if (jumpTimeout >= maxJumpTime)
                {
                    fulljump = true;
                }
            }
            if (Input.GetKeyUp("space") || fulljump == true)
            {
                fulljump = false;
                wallCollider.enabled = true;
                if (spvx < 0)
                    rightFlag = false; //���� �������� ����
                else if (spvx > 0)
                    rightFlag = true;
                sprite.flipX = !rightFlag;
                jumpTimeout = Mathf.Clamp(jumpTimeout, minJumpTime, maxJumpTime);
                rbody.velocity = new Vector2(spvx * jumpSpeedMultiplier, Mathf.Sqrt(2 * 9.81f * jumpHeight) * jumpTimeout);
                jumpTimeout = 0;
                isJumping = false; //2�� x
            }
        }

        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isMoving", vx != 0);
    }

    void FixedUpdate()
    {
        float yVel = rbody.velocity.y;
        if (Mathf.Abs(yVel) <= 0.01f)
            yVel = 0f; //��� �����̱� ����

        if (yVel < fallingSpeedLimit)
        {
            rbody.velocity = new Vector2(rbody.velocity.x, fallingSpeedLimit);
        }

        if (canMove && yVel == 0f)
        {
            rbody.velocity = new Vector2(vx, rbody.velocity.y);
        }
        anim.SetFloat("yVelocity", yVel);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canMove && collision.otherCollider == wallCollider)
        {
            rbody.AddForce(collision.contacts[0].normal * reflectForce + Vector2.up * (reflectForce/2), ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(groundCheck.position, groundSize);
    }

}
