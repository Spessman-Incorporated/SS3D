using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes objects persist throughout the scenes
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
