using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {

    private ParticleSystem particle;

    void Start() {
        particle = GetComponent<ParticleSystem>();

        if (particle.playOnAwake == true) {
            Destroy(gameObject, particle.duration + particle.startLifetime);
        }
    }
}
