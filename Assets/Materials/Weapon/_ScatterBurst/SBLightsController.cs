using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBLightsController : MonoBehaviour
{
    public GameObject Mag2Round;
    //public GameObject Mag4Round;
    public GameObject Body;
    public GameObject Focus8Shot;

    MeshRenderer BodyMR; //3 is lights
    MeshRenderer Focus8ShotMR; //1 is lights
    MeshRenderer Mag2RoundMR;//4 is slug 1,0 is slug 2

    bool rnd1_acitve = true;
    bool rnd2_acitve = true;
    bool rnd3_acitve = true;
    bool rnd4_acitve = true;

    public Material ActiveRound;
    public Material InactiveRound;

    public Material ActiveBarrel;
    public Material InactiveBarrel;

    bool RoundsTimerActive = false;
    bool BarrelTimerActive = false;
    float TimeToActivateRounds = 0.0f;
    float TimeToActivateBarrel = 0.0f;

    public float TimeToActivateRoundsOffset = 0.0f;
    public float TimeToActivateBarrelOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        BodyMR = Body.GetComponent<MeshRenderer>();
        Focus8ShotMR = Focus8Shot.GetComponent<MeshRenderer>();
        Mag2RoundMR = Mag2Round.GetComponent<MeshRenderer>();

        Debug.Log(InactiveRound);
    }

    // Update is called once per frame
    void Update()
    {
        if (RoundsTimerActive && Time.time >= TimeToActivateRounds)
        {
            ActivateRounds();
        }

        if (BarrelTimerActive && Time.time >= TimeToActivateBarrel)
        {
            ActivateBarrel();
        }
    }

    public void Fire2Rnd()
    {
        if (rnd1_acitve)
        {
            rnd1_acitve = false;

            Material[] mats = Mag2RoundMR.materials;
            mats[4] = InactiveRound;
            Mag2RoundMR.materials = mats;
        }
        else if (rnd2_acitve)
        {
            Material[] mats;

            rnd2_acitve = false;

            mats = Mag2RoundMR.materials;
            mats[0] = InactiveRound;
            Mag2RoundMR.materials = mats;

            mats = BodyMR.materials;
            mats[3] = InactiveBarrel;
            BodyMR.materials = mats;

            mats = Focus8ShotMR.materials;
            mats[1] = InactiveBarrel;
            Focus8ShotMR.materials = mats;
        }
    }

    public void Fire4Rnd()
    {
        
    }

    public void StartReloadTimers()
    {
        RoundsTimerActive = true;
        BarrelTimerActive = true;
        TimeToActivateRounds = Time.time + TimeToActivateRoundsOffset;
        TimeToActivateBarrel = Time.time + TimeToActivateBarrelOffset;
    }

    void ActivateRounds()
    {
        RoundsTimerActive = false;
        Material[] mats;

        //Round 1
        mats = Mag2RoundMR.materials;
        mats[4] = ActiveRound;
        Mag2RoundMR.materials = mats;

        //Round 2
        mats = Mag2RoundMR.materials;
        mats[0] = ActiveRound;
        Mag2RoundMR.materials = mats;
    }

    void ActivateBarrel()
    {
        BarrelTimerActive = false;
        Material[] mats;

        //Body
        mats = BodyMR.materials;
        mats[3] = ActiveBarrel;
        BodyMR.materials = mats;

        //Focus
        mats = Focus8ShotMR.materials;
        mats[1] = ActiveBarrel;
        Focus8ShotMR.materials = mats;

        rnd1_acitve = true;
        rnd2_acitve = true;
    }
}
