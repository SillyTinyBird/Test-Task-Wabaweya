using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class PlaybleCharacter : MonoBehaviour
{
    [SerializeField] private int amountOfUnits;
    [SerializeField] private int health;
    private int injuredUnitHealth;
    [SerializeField] private int maxDistance;
    [SerializeField] private int damageMin;
    [SerializeField] private int damageMax;
    [SerializeField] private int initiative;
    [SerializeField] private BattleSide side;
    private TMP_Text nameLabel;
    [SerializeField] private TMP_Text labelPrefab;
    [SerializeField] private HexCoordinates startingCoordinates;
    private HexCell occupiedCell;
    public virtual List<Ability> Abilities { get; private set; }
    //[SerializeField] private List<Ability> abilities;
    [SerializeField] private Canvas gridCanvas;

    public List<Ability> GetAbilities() => Abilities;
    public int GetInitiative() => initiative;
    public BattleSide GetSide() => side;
    public HexCell GetOccupiedHexCell() => occupiedCell;
    public int GetAmountOfUnits() => amountOfUnits;
    public void Init() //everyone starts on the random cell
    {
        SearchForPlaceForUnit(startingCoordinates);
        nameLabel = Instantiate(labelPrefab);
        nameLabel.rectTransform.SetParent(gridCanvas.transform, false);
        nameLabel.color = side == BattleSide.APLHA ? Color.green : Color.red;
        UpdateLabel();
    }
    private void SearchForPlaceForUnit(HexCoordinates cords)
    {
        HexCell cellToOccupy = HexGrid.getInstance().GetHexCellByCoordinates(cords);
        if(cellToOccupy == null)
            PlaceUnitRandom();
        if (cellToOccupy.characterOccupiedCell == null){//means we can place characcter in this cell
            PlaceUnit(cellToOccupy);
            return;
        }
        HexCell cellCandidate;
        for(int i = 0; i < cellToOccupy.neighbors.Length; i++)
        {
            cellCandidate = cellToOccupy.neighbors[i];
            if (cellCandidate.characterOccupiedCell == null)
            {
                PlaceUnit(cellCandidate);
                return;
            }
        }
        //if we are still here than all hope is lost and we go for random neighbor
        PlaceUnitRandom();

    }
    private void PlaceUnitRandom()
    {
        HexCell cellToOccupy;
        do
        {
            cellToOccupy = HexGrid.getInstance().GetRandomCell();
        }
        while (cellToOccupy.characterOccupiedCell != null);
        PlaceUnit(cellToOccupy);
    }
    private void PlaceUnit(HexCell cellToOccupy)
    {
        occupiedCell = cellToOccupy;
        occupiedCell.characterOccupiedCell = this;
        transform.position = occupiedCell.transform.position;
    }
    public void PerformAction(int abilityNumber, HexCell cell)
    {
        if (abilityNumber < 0 || abilityNumber > Abilities.Count) { return; }
        int distance = occupiedCell.coordinates.DistanceTo(cell.coordinates);
        Abilities[abilityNumber].Execute(cell,amountOfUnits,damageMin,damageMax, distance);
    }
    public virtual void RecieveDamage(int damage)
    {
        int totalhealth = health * amountOfUnits;
        if (totalhealth < damage) { 
            Debug.Log(gameObject.name + " is dead");
            MessageBox.PutTextInMessageBox(gameObject.name + " is dead");
            amountOfUnits = 0;
            UpdateLabel();
            return;
        }
        totalhealth -= damage;
        int unitsBefore = amountOfUnits;
        amountOfUnits = totalhealth/ health;//yeah if unit is injured its out
                                            //im sorry its late and i dont wanna fix it right now
        if (amountOfUnits <= 0)
        {
            Debug.Log(gameObject.name + " is dead");
            MessageBox.PutTextInMessageBox(gameObject.name + " is dead");
            amountOfUnits = 0;
            UpdateLabel();
            return;
        }
        Debug.Log(gameObject.name + " recieved " + totalhealth + "\n" + amountOfUnits + " out of " + unitsBefore + " dead");
        MessageBox.PutTextInMessageBox(gameObject.name + " recieved " + totalhealth + "\n" + amountOfUnits + " out of " + unitsBefore + " dead");
        UpdateLabel();
    }
    public virtual bool Move(HexCell cell)//returns true if move succesfull
    {
        if (cell == null) return false;
        int distance = occupiedCell.coordinates.DistanceTo(cell.coordinates);
        if (distance > maxDistance || distance == 0)
        {
            Debug.Log("Invalid length");
            MessageBox.PutTextInMessageBox("Invalid length");
            return false;
        }
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
