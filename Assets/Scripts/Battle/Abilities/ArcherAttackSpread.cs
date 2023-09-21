using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttackSpread : Ability
{
    public override void Execute(HexCell point, int amountOfUnitsDoingDamage, int damageMin, int damageMax, int range)
    {
        List<HexCell> spread = GetSread(point);
        List<PlaybleCharacter> targets = new List<PlaybleCharacter>();
        spread.ForEach(cell =>
        {
            if (cell == null) return;
            cell.EnableHighlight(Color.yellow);
            PlaybleCharacter target = cell.characterOccupiedCell;
            if (target != null)
                targets.Add(target);
        });
        if (targets.Count == 0)
        {
            Debug.Log("Miss");
            MessageBox.PutTextInMessageBox("Miss");
            return;
        }
        targets.ForEach(target => { target.RecieveDamage(amountOfUnitsDoingDamage * Random.Range(damageMin, damageMax)/2); });
    }
    private List<HexCell> GetSread(HexCell point)
    {
        List<HexCell> spread = new List<HexCell>
        {
            point,
            point.GetNeighbor(HexDirection.E),
            point.GetNeighbor(HexDirection.NE),
            point.GetNeighbor(HexDirection.NE).GetNeighbor(HexDirection.W),
            point.GetNeighbor(HexDirection.NE).GetNeighbor(HexDirection.NW),
            point.GetNeighbor(HexDirection.NE).GetNeighbor(HexDirection.NE),
            point.GetNeighbor(HexDirection.NE).GetNeighbor(HexDirection.E),
            point.GetNeighbor(HexDirection.SE),
            point.GetNeighbor(HexDirection.SE).GetNeighbor(HexDirection.W),
            point.GetNeighbor(HexDirection.SE).GetNeighbor(HexDirection.SW),
            point.GetNeighbor(HexDirection.SE).GetNeighbor(HexDirection.SE),
            point.GetNeighbor(HexDirection.SE).GetNeighbor(HexDirection.E),
        };
        spread.Remove(null);
        return spread;
    }
}
