using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    private Transform bar;
    private GameObject playerObject;
    private Player playerScript;

    private void Start() {
        bar = transform.Find("Bar");
        playerObject = GameObject.Find("Player");
        playerScript = playerObject.GetComponent<Player>();
    }

    private void Update() {
        bar.localScale = new Vector3(playerScript.currentHealth / playerScript.maxHealth * 1f, 1f);
    }
}
