using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleState { START, ALPHATURN, BETATURN, END }//states for two player battle
[DefaultExecutionOrder(1)]
public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<PlaybleCharacter> alpha;//ill sitck with having characters on filed beforehand
    [SerializeField] private List<PlaybleCharacter> beta;//for now

    //ui is here


    private BattleState battleState;
    private List<PlaybleCharacter> turnOrder;
    private int turnOrderIndex;
    private bool hasClicked = true; //to restrict amount of button presses or something

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
        Debug.Log("Its " + turnOrder[turnOrderIndex].gameObject.name + " turn");
        HexGrid.getInstance().SetActiveCell(turnOrder[turnOrderIndex].GetOccupiedHexCell());
    }
    IEnumerator AlphaTurn()
    {
        // probably display some message 
        // stating it's player's turn here
        yield return new WaitForSeconds(1);
        hasClicked = false;
    }
    IEnumerator BetaTurn()
    {
        // probably display some message 
        // stating it's player's turn here
        yield return new WaitForSeconds(1);
        hasClicked = false;
    }
    public void OnMoveButtonPressed()
    {
        // allow only a single action per turn
        if (!hasClicked)
        {
            if (turnOrder[turnOrderIndex].GetSide() == BattleSide.APLHA)
            {
                StartCoroutine(MoveAlpha());
            }
            else
            {
                StartCoroutine(MoveBeta());
            }
            hasClicked = true;
        }
    }

    public void OnAttackButtonPress()
    {
        // allow only a single action per turn
        if (!hasClicked)
        {
            if(turnOrder[turnOrderIndex].GetSide() == BattleSide.APLHA)
            {
                StartCoroutine(PerformAlphaAction());
            }
            else
            {
                StartCoroutine(PerformBetaAction());
            }
            hasClicked = true;
        }
    }
    IEnumerator MoveAlpha()//we need separation so i could setup HUD respectively
    {
        turnOrder[turnOrderIndex].Move(HexGrid.getInstance().GetLastTouchedCell());
        yield return NextTurnInOrder();
    }
    IEnumerator MoveBeta()
    {
        turnOrder[turnOrderIndex].Move(HexGrid.getInstance().GetLastTouchedCell());
        yield return NextTurnInOrder();
    }
    IEnumerator PerformBetaAction()
    {
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
        turnOrder[turnOrderIndex].PerformAction(0,HexGrid.getInstance().GetLastTouchedCell());

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
        if(alpha.Count <= 0)
        {
            Debug.Log("Beta won");
        }
        else
        {
            Debug.Log("Alpha won");
        }
        yield return new WaitForSeconds(1);
    }

}
