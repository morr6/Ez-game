using UnityEngine;
using System.Collections;
public class Player : MonoBehaviour {
    [SerializeField] float speed = 3.75f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float knockbackForceX = 3f;
    [SerializeField] float knockbackForceY = 3f;

    public float playerDamage = 10f, maxHealth = 50f, currentHealth;

    private Rigidbody2D playerBody;
    public GameObject swordRight, swordLeft;
    private Animator  animator;
    private Sensor_Bandit groundSensor;

    private bool grounded = false;
    private bool combatIdle = false;
    private bool isDead = false;
    private bool facingRight;
    private bool isAttacking; 
    private bool justAfterHit;   

    void Start () {
        animator = GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody2D>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        currentHealth = maxHealth;
    }

    IEnumerator HitStop() {
        justAfterHit = true;
        yield return new WaitForSeconds(0.5f);
        justAfterHit = false;
    }

    void OnCollisionEnter2D(Collision2D other) { 
        if (other.gameObject.tag == "Enemy" && !isDead) {
            animator.SetTrigger("Hurt");

            var hitFromRight = other.transform.position.x > gameObject.transform.position.x;

            var knockbackVector = new Vector2(hitFromRight ? -knockbackForceX : knockbackForceX, knockbackForceY);

            playerBody.AddForce(knockbackVector, ForceMode2D.Impulse);

            currentHealth -= playerDamage;

            StartCoroutine(HitStop());
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

    //Jump   
    void Jump() {
        grounded = false;
        animator.SetTrigger("Jump");
        animator.SetBool("Grounded", grounded);
        playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce);
        groundSensor.Disable(0.2f);
    }   

    void FixedUpdate() {
        float inputX = Input.GetAxis("Horizontal");

        // Move
        if (!isDead && !justAfterHit )
            playerBody.velocity = new Vector2(inputX * speed, playerBody.velocity.y); 

        //Set AirSpeed in animator
        animator.SetFloat("AirSpeed", playerBody.velocity.y);

        // stop on dead
        if (isDead) {
            playerBody.velocity = new Vector2(0, 0); 
        }
    }

    void Update () {
        facingRight = GetComponent<SpriteRenderer>().flipX;
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        float inputX = Input.GetAxis("Horizontal");

        if (currentHealth <= 0) { 
            switchToDeath();
        }

        if (!isAttacking) {
            SwordAttackDone();
        }

        if (!grounded && groundSensor.State()) {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }

        if(grounded && !groundSensor.State()) {
            grounded = false; 
            animator.SetBool("Grounded", grounded);
        }

        // Swap direction of sprite depending on walk direction
        
        if (!isDead && !isAttacking) {
            if (inputX > 0)
                GetComponent<SpriteRenderer>().flipX = true;
            else if (inputX < 0)
                GetComponent<SpriteRenderer>().flipX = false;
        }

        // Jump
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !isAttacking && grounded && !isDead) {
            Jump();
        }

        // -- Handle Animations --
        //Recover
        if (Input.GetKeyDown("e")) {
            if (isDead) {
                currentHealth = maxHealth;
                isDead = false;
                animator.SetTrigger("Recover");
            }
        }
        //Attack
        if(Input.GetKey("space") && !isAttacking && !isDead) {
            animator.SetTrigger("Attack");
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
}

