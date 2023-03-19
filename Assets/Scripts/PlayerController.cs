using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 3;
    public float jumpHeight = 10;
    public float minJumpTime = 0.1f;
    public float maxJumpTime = 1.5f;
    public float jumpSpeedMultiplier = 4f;
    public float reflectForce = 1.5f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 groundSize = new Vector2(0.3f, .1f);
    public Collider2D wallCollider;


    float vx,spvx;
    bool isGrounded, wasGrounded, isJumping, canMove = true;
    bool rightFlag;
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
            }
            if (Input.GetKeyUp("space"))
            {
                wallCollider.enabled = true;
                if (spvx < 0)
                    rightFlag = false; //점프 눌렀을떄 방향
                else if (spvx > 0)
                    rightFlag = true;
                sprite.flipX = !rightFlag;
                jumpTimeout = Mathf.Clamp(jumpTimeout, minJumpTime, maxJumpTime);
                rbody.velocity = new Vector2(spvx * jumpSpeedMultiplier, Mathf.Sqrt(2 * 9.81f * jumpHeight) * jumpTimeout);
                jumpTimeout = 0;
                isJumping = false; //2중 x
            }
        }

        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isMoving", vx != 0);
    }

    void FixedUpdate()
    {
        float yVel = Mathf.Abs(rbody.velocity.y);
        if (yVel <= 0.01f)
            yVel = 0f; //잠깐 움직이기 방지

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
            rbody.AddForce(collision.contacts[0].normal * reflectForce, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(groundCheck.position, groundSize);
    }

}
