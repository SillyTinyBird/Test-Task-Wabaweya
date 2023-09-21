using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    //we can add stuff like manacost and whatnot
    public virtual void Execute(HexCell point,int amountOfUnitsDoingDamage,int damageMin,int damageMax)
    {
        
    }
}
