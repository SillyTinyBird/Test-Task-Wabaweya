using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustAGuy : PlaybleCharacter
{
    public override List<Ability> Abilities
    {
        get { return new List<Ability>() { new SimpleAttack() }; }
    }
    //we can override stuff if we want
    //but for this one ill keep defaults
}
