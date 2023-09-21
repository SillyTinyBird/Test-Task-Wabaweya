using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttack : Ability
{
    public override void Execute(HexCell point, int amountOfUnitsDoingDamage, int damageMin, int damageMax)
    {
        PlaybleCharacter target = point.characterOccupiedCell;
        if (target == null) { Debug.Log("Miss"); return; }
        target.RecieveDamage(amountOfUnitsDoingDamage * Random.Range(damageMin, damageMax));
    }
}
