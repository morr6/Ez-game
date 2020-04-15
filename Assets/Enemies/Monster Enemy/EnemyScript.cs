using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    [SerializeField] float maxHealth = 30f;

    private float currentHealth;

    private GameObject player;
    private Player playerScript;

    private Animator animator;
    private Rigidbody2D enemyBody;
    private SpriteRenderer spriteRenderer;

    public Transform patrolStart;
    public Transform patrolEnd;
    public Transform currentPatrolPoint;

    public float speed;
    public float viewRange = 1f;

    public ParticleSystem deathParticles;

    void Start() {      
        animator = GetComponent<Animator>();
        enemyBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        
        currentPatrolPoint = patrolStart;

        if (currentPatrolPoint.position.x < transform.position.x) {
            speed = -0.75f;
        } else {
            speed = 0.75f;
        }

        currentHealth = maxHealth;
    }

    void Update() {
        Moving();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt")) {
            animator.SetTrigger("Walk");
        }


        if (Vector2.Distance(currentPatrolPoint.position, transform.position) < .5f)
            StartCoroutine(Patrol());         


        if (currentHealth <= 0)
            switchToDead();
    }

    void Moving() {
        transform.Translate(Vector2.right * Time.deltaTime * speed);

        if (speed > 0f) {
            spriteRenderer.flipX = true;
        } else if(speed < 0f) {
            spriteRenderer.flipX = false;
        }
    }

    IEnumerator Patrol() {
        speed = 0;
        yield return new WaitForSeconds(3);

        if (speed == 0) {
            if (currentPatrolPoint == patrolStart) {
                currentPatrolPoint = patrolEnd;
            } else {
                currentPatrolPoint = patrolStart;
            }

            if (currentPatrolPoint.position.x < transform.position.x) {
                speed = -0.75f;
            } else {
                speed = 0.75f;
            }
        }      
    }

    IEnumerator OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Sword") {
            currentHealth -= playerScript.playerDamage;

            spriteRenderer.color = new Color(255f, 0f, 0f, 0.75f);
            yield return new WaitForSeconds(0.1f);           
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(255f, 0f, 0f, 0.75f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    public void switchToDead() {
        deathParticles.playOnAwake = true;
        
        Instantiate(deathParticles, gameObject.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }   
}



