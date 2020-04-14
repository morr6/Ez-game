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

    public Transform patrolStart;
    public Transform patrolEnd;
    public Transform currentPatrolPoint;

    public float speed;
    public float viewRange = 1f;

    public ParticleSystem deathParticles;

    void Start() {      
        animator = GetComponent<Animator>();
        enemyBody = GetComponent<Rigidbody2D>();

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


        if (Vector2.Distance(currentPatrolPoint.position, transform.position) < .5f)
            StartCoroutine(Patrol());         


        if (currentHealth <= 0)
            switchToDead();
    }

    void Moving() {
        transform.Translate(Vector2.right * Time.deltaTime * speed);

        if (speed > 0f) {
            GetComponent<SpriteRenderer>().flipX = true;
        } else if(speed < 0f) {
            GetComponent<SpriteRenderer>().flipX = false;
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

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Sword") {
            Debug.Log(currentHealth);

            currentHealth -= playerScript.playerDamage;

            Debug.Log(currentHealth);
        }
    }

    public void switchToDead() {
        Instantiate(deathParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }   
}



