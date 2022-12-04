using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce = 15f;
    private bool isGrouded = false;
    float horizontalMove = 0f;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;  


    private void FixedUpdate()
    {
        CheckGround();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim= GetComponent<Animator>();
        sprite = rb.GetComponentInChildren<SpriteRenderer>();
    }

    private void Run()
    {
        if (isGrouded) State = States.run;

        Vector3 dir = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        sprite.flipX = dir.x < 0.0f;
    }
    private void Jump()
    {
       
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDashing)
        {
            return;
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * speed;

        anim.SetFloat("Spead", Mathf.Abs(horizontalMove));


        if (Input.GetButton("Horizontal"))
            Run();
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
            anim.SetBool("IsJumping", true); 
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    private void CheckGround()
    {
     //   if (!isGrouded) State = States.jump;

        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrouded = collider.Length > 1;
    }
    public enum States
    {
         idle,
         run,
         jump 
    }
    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state",(int)value); }
    }

    public  void OnLanding()
    {
        anim.SetBool("IsJumping", false);
    }


/*    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
    }*/
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
