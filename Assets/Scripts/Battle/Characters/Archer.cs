using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Archer : PlaybleCharacter
{
    private List<Ability> abilities = new List<Ability>() { new ArcherAttackTarget(), new ArcherAttackTarget() };
public override List<Ability> Abilities
    {
        get { return abilities; }
    }
}
