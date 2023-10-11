using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;

public enum BattleState { START, ALPHATURN, BETATURN, END }//states for two player battle
[DefaultExecutionOrder(1)]
public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<PlaybleCharacter> alpha;//ill sitck with having characters on filed beforehand
    [SerializeField] private List<PlaybleCharacter> beta;//for now

    [SerializeField] private GameObject winGroup;
    [SerializeField] private TMP_Text abilities;
    [SerializeField] private TMP_InputField attackId;
    [SerializeField] private TMP_Text turnIndicator;


    //ui is here


    private BattleState battleState;
    private List<PlaybleCharacter> turnOrder;
    private int turnOrderIndex;

    private void RemovePLayableFromOrder(PlaybleCharacter dead)
    {
        if(dead.GetSide() == BattleSide.APLHA)
        {
            alpha.Remove(dead);
        }
        else
        {
            beta.Remove(dead);
        }
        turnOrder.Remove(dead);
    }
    void Start()
    {
        battleState = BattleState.START;
        StartCoroutine(BeginBattle());//nested coroutines so we can add wait for animations and stuff in future
    }

    IEnumerator BeginBattle()
    {
        // init characters on the hexgrid
        alpha.ForEach(chara => { chara.Init(); });//we can change stuff here if we want for characcters to spawn in dedicated fields
        beta.ForEach(chara => { chara.Init(); });
        // set up HUD displays
        FormTurnOrder();
        yield return NextTurnInOrder();
    }

    private void FormTurnOrder()
    {
        turnOrder = new List<PlaybleCharacter>();
        turnOrder = alpha.Concat(beta).ToList();
        turnOrder.Sort((x, y) => x.GetInitiative().CompareTo(y.GetInitiative()));
        turnOrderIndex = turnOrder.Count - 1;//so the next turn will land on 0
    }
    private IEnumerator NextTurnInOrder()
    {
        turnOrderIndex = turnOrderIndex == turnOrder.Count - 1 ? 0 : turnOrderIndex + 1;
        if (turnOrder[turnOrderIndex].GetSide() == BattleSide.APLHA)
        {
            battleState = BattleState.ALPHATURN;
            yield return StartCoroutine(AlphaTurn());
        }
        else
        {
            battleState = BattleState.BETATURN;
            yield return StartCoroutine(BetaTurn());
        }
        string abilitieList = "";
        int id = 0;
        //List<Ability> abilitiesFromTHing = turnOrder[turnOrderIndex].Abilities;
        turnOrder[turnOrderIndex].Abilities.ForEach(x => {
            abilitieList += id + ": " + x.GetType().Name + "\n"; 
            id++; 
        });
        abilities.text = abilitieList;
        Debug.Log("Its " + turnOrder[turnOrderIndex].gameObject.name + " turn");
        MessageBox.PutTextInMessageBox("Its " + turnOrder[turnOrderIndex].gameObject.name + " turn");
        HexGrid.getInstance().SetActiveCell(turnOrder[turnOrderIndex].GetOccupiedHexCell());
    }
    IEnumerator AlphaTurn()
    {
        turnIndicator.color =  Color.green;
        turnIndicator.text = "Alpha turn";
        // probably display some message 
        // stating it's player's turn here
        yield return new WaitForSeconds(0);
    }
    IEnumerator BetaTurn()
    {
        turnIndicator.color = Color.red;
        turnIndicator.text = "Beta turn";
        // probably display some message 
        // stating it's player's turn here
        yield return new WaitForSeconds(0);
    }
    public void OnMoveButtonPressed()
    {
        if (turnOrder[turnOrderIndex].GetSide() == BattleSide.APLHA)
        {
            StartCoroutine(MoveAlpha());
        }
        else
        {
            StartCoroutine(MoveBeta());
        }
    }
    public void OnPassButtonPressed()
    {
        StartCoroutine(NextTurnInOrder());
    }

    public void OnAttackButtonPress()
    {
        int inputNumber = 0;
        int.TryParse(attackId.text, out inputNumber);
        if(inputNumber > turnOrder[turnOrderIndex].GetAbilities().Count - 1 || inputNumber < 0)
        {
            Debug.Log("Invalid ability Id");
            MessageBox.PutTextInMessageBox("Invalid ability Id");
            return;
        }
        if (turnOrder[turnOrderIndex].GetSide() == BattleSide.APLHA)
        {
            StartCoroutine(PerformAlphaAction());
        }
        else
        {
            StartCoroutine(PerformBetaAction());
        }
    }
    IEnumerator MoveAlpha()//we need separation so i could setup HUD respectively
    {
        if (!turnOrder[turnOrderIndex].Move(HexGrid.getInstance().GetLastTouchedCell()))
        {
            yield break;
        }
        yield return NextTurnInOrder();
    }
    IEnumerator MoveBeta()//(they did not separated HUD respectively)
    {
        if (!turnOrder[turnOrderIndex].Move(HexGrid.getInstance().GetLastTouchedCell()))
        {
            yield break;
        }
        yield return NextTurnInOrder();
    }
    IEnumerator PerformBetaAction()//they are actually the same
    {
        HexCell targetCell = HexGrid.getInstance().GetLastTouchedCell();
        if (targetCell == null || targetCell == turnOrder[turnOrderIndex].GetOccupiedHexCell())
        {
            Debug.Log("No target selected");
            MessageBox.PutTextInMessageBox("No target selected");
        }
        int inputNumber = 2;
        int.TryParse(attackId.text, out inputNumber);
        turnOrder[turnOrderIndex].PerformAction(inputNumber, targetCell);
        if (targetCell.characterOccupiedCell != null)//its stacked so i could awoid null ptr on second if
        {
            if (targetCell.characterOccupiedCell.GetAmountOfUnits() <= 0)
            {
                RemovePLayableFromOrder(targetCell.characterOccupiedCell);
                Destroy(targetCell.characterOccupiedCell);
            }
        }
        if (alpha.Count <= 0 || beta.Count <= 0)
        {
            battleState = BattleState.END;
            yield return StartCoroutine(EndBattle());
        }
        else
        {
            yield return NextTurnInOrder();
        }
    }
    IEnumerator PerformAlphaAction()
    {
        HexCell targetCell = HexGrid.getInstance().GetLastTouchedCell();
        if (targetCell == null || targetCell == turnOrder[turnOrderIndex].GetOccupiedHexCell())
        {
            Debug.Log("No target selected");
            MessageBox.PutTextInMessageBox("No target selected");
        }
        turnOrder[turnOrderIndex].PerformAction(int.Parse(attackId.text), targetCell);
        if (targetCell.characterOccupiedCell != null)//its stacked so i could awoid null ptr on second if
        {
            if (targetCell.characterOccupiedCell.GetAmountOfUnits() <= 0)
            {
                RemovePLayableFromOrder(targetCell.characterOccupiedCell);
                Destroy(targetCell.characterOccupiedCell);
            }
        }
        if (alpha.Count <= 0 || beta.Count <= 0)
        {
            battleState = BattleState.END;
            yield return StartCoroutine(EndBattle());
        }
        else
        {
            yield return NextTurnInOrder();
        }
    }
    IEnumerator EndBattle()
    {
        string winText;
        if (alpha.Count <= 0)
        {
            winText = "Beta won";
            Debug.Log("Beta won");
            MessageBox.PutTextInMessageBox("Beta won");
        }
        else
        {
            winText = "Alpha won";
            Debug.Log("Alpha won");
            MessageBox.PutTextInMessageBox("Alpha won");
        }
        winGroup.SetActive(true);
        winGroup.GetComponentInChildren<TMP_Text>().text = winText;
        yield return new WaitForSeconds(1);
    }

}
