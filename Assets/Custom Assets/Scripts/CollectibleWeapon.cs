using UnityEngine;
using System.Collections;

public class CollectibleWeapon : MonoBehaviour
{
    public float recoil;
    public float firerate;
    public float damage;
    public float aoe;
    public float piercing;
    public float numberofprojectiles;
    public float spread;
    public float clip;
    public float pushback;
    public float jam;

    public float[] getArray()
    {
        return new float[] {
            recoil,
            firerate,
            damage,
            aoe,
            piercing,
            numberofprojectiles,
            spread,
            clip,
            pushback,
            jam,
        };
    }
}
