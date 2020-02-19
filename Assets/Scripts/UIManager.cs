using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Game Play Panels
    public static GameObject roundInfoPanel;
    public static GameObject roundResultPanel;
    public static GameObject ScorePanel;
    public static GameObject HealthPanel;
    public static GameObject AmmoCounterPanel;
    public static GameObject crosshair;

    //Inspect Panels
    public static GameObject BarrelCycler;
    public static GameObject GripCycler;

    //Skills Panels
    public static GameObject SkillsPanel;
    public static Button Skill1Button;
    public static Button Skill2Button;
    public static Button Skill3Button;
    public static Button Skill4Button;

    //Text Objects
    public static Text ammoCounter;
    public static Text roundInfo;
    public static Text score;
    public static Text health;
    public static Text roundResult;
    public static Text SkillPointIndicator;

    //Other Values
    public static int scorePts = 0;
    public static int HealthPts = 30;
    public static bool inspecting = false;
    public static bool SkillMenuActive = false;

    private void Start()
    {
        ammoCounter = GameObject.Find("Ammo Counter Text").GetComponent<Text>();
        roundInfo = GameObject.Find("Round Info Text").GetComponent<Text>();
        score = GameObject.Find("Score Text").GetComponent<Text>();
        health = GameObject.Find("Health Text").GetComponent<Text>();
        roundResult = GameObject.Find("Round Result Text").GetComponent<Text>();
        SkillPointIndicator = GameObject.Find("Skill Points Indicator").GetComponent<Text>();

        crosshair = GameObject.Find("Crosshair");
        ScorePanel = GameObject.Find("Score Panel");
        HealthPanel = GameObject.Find("Health Panel");
        AmmoCounterPanel = GameObject.Find("Ammo Counter Panel");


        //Panels that should not be shown unless activated need to start activated to be found, 
        //after which they can be disabled
        roundInfoPanel = GameObject.Find("Round Info");
        roundInfoPanel.SetActive(false);

        roundResultPanel = GameObject.Find("Round Result");
        roundResultPanel.SetActive(false);

        BarrelCycler = GameObject.Find("Barrel Cycler");
        BarrelCycler.SetActive(false);

        GripCycler = GameObject.Find("Grip Cycler");
        GripCycler.SetActive(false);

        SkillsPanel = GameObject.Find("Skills Panel");
        SkillsPanel.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !inspecting)//Open Skils Menu
        {
            toggleGameplayUI();
            toggleSkillsUI();
        }
    }

    public static void toggleMouseLock()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    public static void updateAmmoCounter(int currentAmmoInMag,int maxAmmoForMag,int currentAmmo)
    {
        ammoCounter.text = "" + currentAmmoInMag + "/" + maxAmmoForMag + "\n" + currentAmmo;
    }

    public static void updateRoundCountdown(float currnetTime)
    {
        if (currnetTime >= 10f)
        {
            roundInfo.text = currnetTime.ToString("00");
        }
        else
        {
            roundInfo.text = currnetTime.ToString("0.0");
        }
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

    public static void updateSkillPointIndicator(int currentSkillPoints)
    {
        SkillPointIndicator.text = "" + currentSkillPoints;
    }

    public static void updateBarrekCyclerPOS(Vector3 BarrelPos)
    {
        BarrelCycler.transform.position = Camera.main.WorldToScreenPoint(BarrelPos);
    }

    public static void updateGripCyclerPOS(Vector3 GripPos)
    {
        GripCycler.transform.position = Camera.main.WorldToScreenPoint(GripPos);
    }

    public static void toggleGameplayUI()
    {
        CharacterController.toggleMouseLock();
        crosshair.SetActive(!crosshair.activeInHierarchy);
        roundInfoPanel.SetActive(!roundInfoPanel.activeInHierarchy);
        ScorePanel.SetActive(!ScorePanel.activeInHierarchy);
        HealthPanel.SetActive(!HealthPanel.activeInHierarchy);
        AmmoCounterPanel.SetActive(!AmmoCounterPanel.activeInHierarchy);
    }

    public static void toggleInspectUI()
    {
        inspecting = !inspecting;
        toggleGameplayUI();
        BarrelCycler.SetActive(!BarrelCycler.activeInHierarchy);
        GripCycler.SetActive(!GripCycler.activeInHierarchy);
    }

    public static void toggleSkillsUI()
    {
        SkillMenuActive = !SkillMenuActive;
        SkillsPanel.SetActive(!SkillsPanel.activeInHierarchy);
    }
}
