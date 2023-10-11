using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : PlaybleCharacter
{
    private int damageReductionTicks = 3;
    private List<Ability> abilities = new List<Ability>() { new SimpleAttack() };
    public override List<Ability> Abilities
    {
        get { return abilities; }
    }
    public override bool Move(HexCell cell)//returns true if move succesfull
    {
        if (base.Move(cell) && Random.Range(0,20) > 9)
        {
            Debug.Log("You can continue moving");
            MessageBox.PutTextInMessageBox("You can continue moving");
            return false;
        }
        else
        {
            return true;
        }
    }
    public override void RecieveDamage(int damage)
    {
        base.RecieveDamage(damage - (int)(damage * 0.33f * damageReductionTicks));
        damageReductionTicks = damageReductionTicks > 0 ? damageReductionTicks - 1 : damageReductionTicks;
    }
}
