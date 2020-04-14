﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEffect : MonoBehaviour
{
    [SerializeField] float time;

    void Start()
    {
        Destroy(gameObject, time);
    }
}
