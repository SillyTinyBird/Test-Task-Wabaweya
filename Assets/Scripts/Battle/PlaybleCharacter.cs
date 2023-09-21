using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class PlaybleCharacter : MonoBehaviour
{
    [SerializeField] private int amountOfUnits;
    [SerializeField] private int health;
    [SerializeField] private int maxDistance;
    [SerializeField] private int damageMin;
    [SerializeField] private int damageMax;
    [SerializeField] private int initiative;
    [SerializeField] private BattleSide side;
    private TMP_Text nameLabel;
    [SerializeField] private TMP_Text labelPrefab;
    private HexCell occupiedCell;
    [SerializeField] private List<Ability> abilities;
    [SerializeField] private Canvas gridCanvas;

    public int GetInitiative() => initiative;
    public BattleSide GetSide() => side;
    public HexCell GetOccupiedHexCell() => occupiedCell;
    public void Init() //everyone starts on the random cell
    {
        HexCell cellToOccupy;
        do
        {
            cellToOccupy = HexGrid.getInstance().GetRandomCell();
        }
        while(cellToOccupy.characterOccupiedCell != null);
        occupiedCell = cellToOccupy;
        occupiedCell.characterOccupiedCell = this;
        transform.position = occupiedCell.transform.position;
        nameLabel = Instantiate(labelPrefab);
        nameLabel.rectTransform.SetParent(gridCanvas.transform, false);
        nameLabel.color = side == BattleSide.APLHA ? Color.green : Color.red;
        UpdateLabel();
    }
    public void PerformAction(int abilityNumber, HexCell cell)
    {
        if (abilityNumber < 0 || abilityNumber > abilities.Count) { return; }
        abilities[abilityNumber].Execute(cell,amountOfUnits,damageMin,damageMax);
    }
    public virtual void RecieveDamage(int damage)
    {
        int totalhealth = health * amountOfUnits;
        if (totalhealth < damage) { 
            Debug.Log(gameObject.name + "dead");
            amountOfUnits = 0;
            UpdateLabel();
            return;
        }
        int unitsBefore = amountOfUnits;

        amountOfUnits = totalhealth/damage;//yeah if unit is injured its out

        Debug.Log(gameObject.name + " recieved " + totalhealth + "\n" + amountOfUnits + " out of " + unitsBefore + " dead");
        UpdateLabel();
    }
    public virtual bool Move(HexCell cell)//returns true if move succesfull
    {
        if (cell == null) return false;
        int distance = occupiedCell.coordinates.DistanceTo(cell.coordinates);
        if (distance > maxDistance || distance == 0)
            return false;
        occupiedCell.characterOccupiedCell = null;
        occupiedCell = cell;
        occupiedCell.characterOccupiedCell = this;
        transform.position = occupiedCell.transform.position;
        UpdateLabel();
        return true;
    }
    private void UpdateLabel()
    {
        nameLabel.gameObject.transform.position = transform.position;
        //nameLabel.rectTransform.anchoredPosition = new Vector2(transform.position.x, transform.position.y);
        nameLabel.text = amountOfUnits.ToString() + " of " + gameObject.name;
    }
}
