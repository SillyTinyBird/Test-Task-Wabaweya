using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttack : Ability
{
    public override void Execute(HexCell point, int amountOfUnitsDoingDamage, int damageMin, int damageMax, int range)
    {
        point.EnableHighlight(Color.yellow);
        PlaybleCharacter target = point.characterOccupiedCell;
        if (target == null || range > 1) { 
            Debug.Log("Miss");
            MessageBox.PutTextInMessageBox("Miss");
            return; 
        }
        target.RecieveDamage(amountOfUnitsDoingDamage * Random.Range(damageMin, damageMax));
    }
}
