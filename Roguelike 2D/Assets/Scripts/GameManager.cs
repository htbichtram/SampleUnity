﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Randon= UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private BoardManager boardScript;
    private int level = 1;

    void Awake()
	{
        boardScript = GetComponent<BoardManager>();
		InitGame();
    }

    void InitGame()
	{
        boardScript.SetupScene(level);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
