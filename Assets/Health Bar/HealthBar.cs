using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    private Transform bar;
    private GameObject player;
    private Player playerScript;

    private void Start() {
        bar = transform.Find("Bar");
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
    }

    private void Update() {
        bar.localScale = new Vector3(playerScript.currentHealth / playerScript.maxHealth * 1f, 1f);
    }
}
