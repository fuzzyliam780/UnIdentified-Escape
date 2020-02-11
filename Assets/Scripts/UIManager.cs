using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static GameObject roundInfoPanel;
    public static GameObject roundResultPanel;
    public static GameObject crosshair;
    public static Text ammoCounter;
    public static Text roundInfo;
    public static Text score;
    public static Text health;
    public static Text roundResult;
    public static int scorePts = 0;
    public static int HealthPts = 30;

    private void Start()
    {
        ammoCounter = GameObject.Find("Ammo Counter Text").GetComponent<Text>();
        roundInfo = GameObject.Find("Round Info Text").GetComponent<Text>();
        score = GameObject.Find("Score Text").GetComponent<Text>();
        health = GameObject.Find("Health Text").GetComponent<Text>();
        roundResult = GameObject.Find("Round Result Text").GetComponent<Text>();
        crosshair = GameObject.Find("Crosshair");
        roundInfoPanel = GameObject.Find("Round Info");
        roundInfoPanel.SetActive(false);
        roundResultPanel = GameObject.Find("Round Result");
        roundResultPanel.SetActive(false);

    }

    public static void updateAmmoCounter(int currentAmmoInMag,int maxAmmoForMag,int currentAmmo)
    {
        ammoCounter.text = "" + currentAmmoInMag + "/" + maxAmmoForMag + "\n" + currentAmmo;
    }

    public static void updateRoundCountdown(int currnetTime)
    {
        roundInfo.text = "" + currnetTime;
    }

    public static void updateRoundEnemies(int remainingEnemies)
    {
        roundInfo.text = "" + remainingEnemies;
    }

    public static void updateScore(int scoreToAdd)
    {
        scorePts += scoreToAdd;
        score.text = "" + scorePts;
    }

    public static void setMaxHealth(int maxHealth)
    {
        HealthPts = maxHealth;
    }

    public static void updateHealth(int HealthToAdd)
    {
        HealthPts += HealthToAdd;
        health.text = "" + HealthPts;
    }

    public static void updateRoundResult(string result)
    {
        roundResultPanel.SetActive(true);
        roundResult.text = result;
    }

    public static void toggleCrosshair()
    {
        crosshair.SetActive(!crosshair.activeInHierarchy);
    }
}
