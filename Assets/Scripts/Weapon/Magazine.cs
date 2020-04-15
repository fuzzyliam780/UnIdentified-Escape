using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
    normal,
    fire,
    ice,
    electric,
    explosive,
    acid,
}

public class Magazine : MonoBehaviour
{
    public AmmoType AmmoType;
    public float Damage;
    public int MagazineCapacity;
    public float ReloadSpeed;
    public float MoveSpeedPenalty;
}
