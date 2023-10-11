using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public Ability() { }
    //we can add stuff like manacost and whatnot
    public virtual void Execute(HexCell point,int amountOfUnitsDoingDamage,int damageMin,int damageMax, int range)
    {
        
    }
}
