using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField] float      speed = 3.75f;
    [SerializeField] float      jumpForce = 5f;
    [SerializeField] float      knockbackForceX = 100f;
    [SerializeField] float      knockbackForceY = 3f;

    private Rigidbody2D         playerBody;
    public float                maxHealth = 50f, currentHealth;
    

    private Animator            animator;
    private Sensor_Bandit       groundSensor;
    private bool                grounded = false;
    private bool                combatIdle = false;
    private bool                isDead = false;
    private bool                facingRight;
    private bool                isAttacking;    

    public GameObject           swordRight, swordLeft;
    
    void Start () {
        animator = GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody2D>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();

        currentHealth = maxHealth;
    }
	
	void Update () {
        facingRight = GetComponent<SpriteRenderer>().flipX;
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");


        if (currentHealth <= 0) { 
            switchToDeath();
        }

        //Check if character just landed on the ground
        if (!grounded && groundSensor.State()) {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }

        //Check if character just started falling
        if(grounded && !groundSensor.State()) {
            grounded = false; 
            animator.SetBool("Grounded", grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        
        if (!isDead && !isAttacking) {
            if (inputX > 0)
                GetComponent<SpriteRenderer>().flipX = true;
            else if (inputX < 0)
                GetComponent<SpriteRenderer>().flipX = false;
        }

        // Move
        if (!isAttacking && !isDead)
            playerBody.velocity = new Vector2(inputX * speed, playerBody.velocity.y);      
        else if (isAttacking) {
            playerBody.velocity = new Vector2(inputX, playerBody.velocity.y);      
        }

        //Set AirSpeed in animator
        animator.SetFloat("AirSpeed", playerBody.velocity.y);

        // -- Handle Animations --
        //Recover
        if (Input.GetKeyDown("e")) {
            if (isDead) {
                currentHealth = maxHealth;
                isDead = false;
                animator.SetTrigger("Recover");
            }
        }
            
        //Hurt
        if (Input.GetKeyDown("q"))
            animator.SetTrigger("Hurt");

        //Attack
        if(Input.GetKey("space") && !isAttacking && !isDead) {
            animator.SetTrigger("Attack");
        }

        //Change between idle and combat idle
        if (Input.GetKeyDown("f"))
            combatIdle = !combatIdle;

        //Jump        
        if ( (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !isAttacking && grounded && !isDead ) {
            grounded = false;

            animator.SetTrigger("Jump");
            animator.SetBool("Grounded", grounded);
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce);
            groundSensor.Disable(0.2f);
        }

        //Run
        if (Mathf.Abs(inputX) > Mathf.Epsilon)
            animator.SetInteger("AnimState", 2);

        //Combat Idle
        if (combatIdle)
            animator.SetInteger("AnimState", 1);

        //Idle
        if (!Input.anyKey)
            animator.SetInteger("AnimState", 0);
    }   

    // Collision with enemy 
    void OnCollisionEnter2D(Collision2D other) { 
        if (other.gameObject.tag == "Enemy" && !isDead) {
            animator.SetTrigger("Hurt");
            playerBody.AddForce(new Vector2(facingRight ? -knockbackForceX : knockbackForceX, 5f), ForceMode2D.Impulse);

            currentHealth -= 10f;
        }
    }

    public void switchToDeath() {
        if (currentHealth <= 0 && !isDead) {
            animator.SetTrigger("Die"); 
        } 

        
        isDead = true;      
    } 

    // Sword Attack
    public void SwordAttack() {
        if (facingRight) {
            swordRight.SetActive(true);
        } else {
            swordLeft.SetActive(true);
        }
    }
    
    public void SwordAttackDone() {
        swordRight.SetActive(false);
        swordLeft.SetActive(false);        
    }
}
