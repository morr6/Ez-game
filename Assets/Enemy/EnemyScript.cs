using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {
    private GameObject player;

    private Animator animator;

    public Transform patrolStart;
    public Transform patrolEnd;
    public Transform currentPatrolPoint;

    public float speed;
    public float viewRange = 1f;

    void Start() {      
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        
        currentPatrolPoint = patrolStart;

        if (currentPatrolPoint.position.x < transform.position.x) {
            speed = -0.75f;
        } else {
            speed = 0.75f;
        }
    }

    IEnumerator Patrol() {
        if (Vector2.Distance(currentPatrolPoint.position, transform.position) < .5f) {
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
    }

    void Moving() {
        transform.Translate(Vector2.right * Time.deltaTime * speed);

        if (speed > 0f) {
            GetComponent<SpriteRenderer>().flipX = true;
        } else if(speed < 0f) {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void Update() {
        if(speed != 0) {            
            animator.SetInteger("AnimState", 2);
        } else {
            animator.SetInteger("AnimState", 0);
        }


        Moving();
        StartCoroutine(Patrol());
        
    }
}



