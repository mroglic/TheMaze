﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Game Status")]
    public bool isGameFinished;
    public float totalTimeInSec = 50;
    public float remainingTimeInSec;

    [Header("Points")]
    public int maxAllowedNegativePoints = 10;
    public int pointsIncrement = 1;
    public int pointsDecrement = 1;

    [Header("GUI")]
    public GameObject scorePanel;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI timeTxt;
    public TextMeshProUGUI livesTxt;
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameScoreTxt;
    public TextMeshProUGUI endGameMessageTxt;

    [Header("Camera")]    
    public CameraViewType cameraViewType;
    public enum CameraViewType { Top, Player }    
    public GameObject playerCamera;
    public Camera mainCamera;

    [Header("Light")]
    public Light directLight;

    [Header("Player")]
    public Player player;

    [Header("City Builder")]
    public CityBuilder cityBuilder;

    [Header("Fog")]
    public bool fogOn;
    private VolumetricFog volumetricFog;

    void Start()
    {
        cameraViewType = CameraViewType.Top;
        player.transform.localScale = Vector3.one * 0.25f * cityBuilder.globalScale;

        volumetricFog = mainCamera.GetComponent<VolumetricFog>();

        restartGame();
    }

    void Update()
    {
        if (isGameFinished) return;

        // toggle fog on/off (Player View only)
        if (Input.GetKeyUp(KeyCode.F) && cameraViewType == CameraViewType.Player)
        {              
            fogOn = !fogOn;
            volumetricFog.enabled = fogOn;
        }

        // toggle camera view (Top view or Player view)
        if (Input.GetKeyUp(KeyCode.C))
        {   
            if (cameraViewType == CameraViewType.Top)
            {
                cameraViewType = CameraViewType.Player;
                player.transform.localScale = Vector3.one * 0.01f * cityBuilder.globalScale;

                // turn on volumetric lights in Player view
                volumetricFog.enabled = true;
            }
            else if (cameraViewType == CameraViewType.Player)
            {
                cameraViewType = CameraViewType.Top;
                player.transform.localScale = Vector3.one * 0.25f * cityBuilder.globalScale;

                // turn off volumetric lights in Top view
                volumetricFog.enabled = false;
            }

            playerCamera.gameObject.SetActive(!playerCamera.gameObject.activeSelf);            
        }

        // update time
        remainingTimeInSec -= Time.deltaTime;        

        // update points, time and lives in GUI
        scoreTxt.text = "Points: " + player.totalPoints;
        timeTxt.text = "Time: " + (int)remainingTimeInSec;
        livesTxt.text = "Lives: " + (maxAllowedNegativePoints - player.negativePoints);
                
        if (player.negativePoints > maxAllowedNegativePoints || remainingTimeInSec <= 0)
        {
            // end game - LOSE             
            endGame("Mission Failed!\r\n Better luck next time :)");
        }      
        else if (player.positivePoints == cityBuilder.maxPoints)
        {
            // end game - WIN
            endGame("Mission Accomplished!\r\n Congratulations!");
        }
        else if (player.transform.position.y < 0f)
        {
            // end game - player felt from platform
            endGame("Mission Failed!\r\n Be careful, edges are slippery!");
        }
    }

    void endGame(string message)
    {
        isGameFinished = true;

        // hide score panel, show end panel
        endGamePanel.gameObject.SetActive(true);
        scorePanel.gameObject.SetActive(false);

        // update message and score
        endGameMessageTxt.text = message;
        endGameScoreTxt.text = "Score: " + player.totalPoints;

        // reset (stop player)
        player.rb.velocity = Vector3.zero;
        player.rb.angularVelocity = Vector3.zero;
        player.gameObject.SetActive(false);
    }

    // called from "Play Again" button
    public void restartGame()
    {
        isGameFinished = false;
                
        // restart time
        remainingTimeInSec = totalTimeInSec;
    
        // hide end panel, show score panel
        endGamePanel.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(true);

        // restart player
        player.totalPoints = 0;
        player.negativePoints = 0;
        player.positivePoints = 0;
        player.gameObject.SetActive(true);

        // set player scale based on game view
        Vector3 initialHeight = Vector3.up;
        if (cameraViewType == CameraViewType.Player)
        {
            initialHeight *= player.transform.localScale.x/2;
        }
        else if (cameraViewType == CameraViewType.Top)
        {
            initialHeight *= player.transform.localScale.x/2;
        }
        player.transform.position = cityBuilder.freePositions[Random.Range(0, cityBuilder.freePositions.Count)] + initialHeight;
         
        // restart all obstacles
        cityBuilder.restartAllObstacles();
    }
}
