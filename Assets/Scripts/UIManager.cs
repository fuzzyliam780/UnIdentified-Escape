using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static Text ammoCounter;

    private void Start()
    {
        ammoCounter = FindObjectOfType<Text>();
    }

    public static void updateAmmoCounter(int currentAmmoInMag,int maxAmmoForMag,int currentAmmo)
    {
        ammoCounter.text = "" + currentAmmoInMag + "/" + maxAmmoForMag + "\n" + currentAmmo;
    }
}
