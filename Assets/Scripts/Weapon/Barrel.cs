using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public Vector3 Barrel_Ext_Pos;
    public AudioSource weaponSound;

    public float Damage;
    public int WeaponRange;
    public float FireRateModifier;
    public float AccuracyModifier;
    public float MoveSpeedPenalty;
}
