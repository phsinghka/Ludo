using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoGameManager : MonoBehaviour
{
    public static LudoGameManager instance;
    [System.Serializable]
    public class Entity
    {
        public string playerName;
        public LudoStone[] myStones;
        public bool hasTurn;
        public enum PlayerType
        {
            HUMAN,
            CPU,
            NO_PLAYER
        }
        public PlayerType playerType;
        public bool hasWon;

    }

    public List<Entity> playerList = new List<Entity>();

    public enum States
    {
        WAITING,
        ROLL_DICE,
        SWITCH_PLAYER

    }

    public States state;

    private void Awake()
    {
        instance = this;
        ActivateButton(false);
        
    }

    public int activePlayer;
    bool switchingPlayer;
    bool turnPossible = true;




    //HUMAN INPUT
    public GameObject rollButton;
    public int rolledHumanDice;

    private void Update()
    {
        if(playerList[activePlayer].playerType == Entity.PlayerType.CPU)
        {


            switch (state)
            {
                case States.ROLL_DICE:
                    {
                        if (turnPossible)
                        {
                            StartCoroutine(RollDiceDelay());
                            state = States.WAITING;

                        }
                    }
                    break;
                case States.WAITING:
                    {
                        //IDLE
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (turnPossible)
                        {
                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;

            }
        }
        if(playerList[activePlayer].playerType == Entity.PlayerType.HUMAN)
        {


            switch (state)
            {
                case States.ROLL_DICE:
                    {
                        if (turnPossible)
                        {
                            ActivateButton(true);
                            state = States.WAITING;

                        }
                    }
                    break;
                case States.WAITING:
                    {
                        //IDLE
                    }
                    break;
                case States.SWITCH_PLAYER:
                    {
                        if (turnPossible)
                        {
                            StartCoroutine(SwitchPlayer());
                            state = States.WAITING;
                        }
                    }
                    break;

            }
        }
    }

    void RollDice()
    {
        int diceNumber = Random.Range(1, 7);
        Debug.Log("Rolled Dice " + diceNumber);
        if (diceNumber == 6)
        {
            CheckStartNode(diceNumber);
            
        }
        if(diceNumber < 6)
        {
            MoveAStone(diceNumber);
        }

    }

    IEnumerator RollDiceDelay()
    {
        yield return new WaitForSeconds(2);
        RollDice();
    }
    bool startNodeFull = false;
    void CheckStartNode(int diceNumber) 
    {
        
        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].currentNode == playerList[activePlayer].myStones[i].startNode)
            {
                startNodeFull = true;
                break;

            }
        }
        if (startNodeFull)
        {
            MoveAStone(diceNumber);
            Debug.Log("The start node is full");
        }
        else
        {
            for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
            {
                if (!playerList[activePlayer].myStones[i].ReturnIsOut())
                {
                    playerList[activePlayer].myStones[i].LeaveBase();
                    state = States.WAITING;
                    return;
                }
            }
            MoveAStone(diceNumber);
        }
    }

    void MoveAStone(int diceNumber)
    {
        List<LudoStone> moveableStones = new List<LudoStone>();
        List<LudoStone> moveableKickStones = new List<LudoStone>();

        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].ReturnIsOut())
            {
                if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneID,diceNumber))
                {
                    moveableKickStones.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }

                if (playerList[activePlayer].myStones[i].CheckPossible(diceNumber))
                {
                    moveableStones.Add(playerList[activePlayer].myStones[i]);
                }
            }
        }

        //PERFORM KICK IF POSSIBLE
        if (moveableKickStones.Count > 0)
        {
            int num = Random.Range(0, moveableKickStones.Count);
            moveableKickStones[num].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }



        //PERFORM MOVE IF POSSIBLE
        if (moveableStones.Count > 0)
        {
            int num = Random.Range(0, moveableStones.Count);
            moveableStones[num].StartTheMove(diceNumber);
            state = States.WAITING;
            return;
        }

        state = States.SWITCH_PLAYER;
        //NONE IS POSSIBLE SWITCH PLAYER

    }

    IEnumerator SwitchPlayer()
    {
        if (switchingPlayer)
        {
            yield break;
        }

        switchingPlayer = true;

        yield return new WaitForSeconds(2f);
        SetNextActivePlayer();

        switchingPlayer = false;
    }

    void SetNextActivePlayer()
    {
        activePlayer++;
        activePlayer %= playerList.Count;

        int available = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (!playerList[i].hasWon)
            {
                available++;
            }
        }

        if (playerList[activePlayer].hasWon && available>1)
        {
            SetNextActivePlayer();
            return;
        }
        else if (available < 2)
        {
            state = States.WAITING;
        }

        state = States.ROLL_DICE;   
    }

    public void ReportTurnPossible(bool possible)
    {
        turnPossible = possible;
    }

    public void ReportWinning()
    {
        playerList[activePlayer].hasWon = true;
    }



    //-------------------------------------------HUMAN INPUTS-----------------------------------

    void ActivateButton(bool on)
    {
        rollButton.SetActive(on);
    }

    public void DeactivateSelector()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            for (int j = 0; j < playerList[i].myStones.Length; j++)
            {
                playerList[i].myStones[j].SetSelector(false);
            }
        }
    }

    public void HumanRollDice()
    {
        ActivateButton(false);
        rolledHumanDice = Random.Range(1, 7);


        List<LudoStone> moveableStones = new List<LudoStone>();

        

        //STart Node Full Check

        //Number <6
        if (rolledHumanDice < 6)
        {
            /*for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
            {
                if (playerList[activePlayer].myStones[i].ReturnIsOut())
                {
                    if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneID, rolledHumanDice))
                    {
                        moveableStones.Add(playerList[activePlayer].myStones[i]);
                        continue;
                    }

                    if (playerList[activePlayer].myStones[i].CheckPossible(rolledHumanDice))
                    {
                        moveableStones.Add(playerList[activePlayer].myStones[i]);
                    }
                }
            }*/

            moveableStones.AddRange(PossibleStones());
        }

        //Number == 6 && !startnode

        if (rolledHumanDice == 6 && !startNodeFull)
        {
            for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
            {
                if (!playerList[activePlayer].myStones[i].ReturnIsOut())
                {
                    moveableStones.Add(playerList[activePlayer].myStones[i]);
                }
            }
            moveableStones.AddRange(PossibleStones());
        }


        //NUMBER ==6 && startNode
        else if(rolledHumanDice == 6 && startNodeFull)
        {
            moveableStones.AddRange(PossibleStones());
        }

        //ACTIVATE ALL POSSIBLE SELECT
        if (moveableStones.Count > 0)
        {
            for (int i = 0; i < moveableStones.Count; i++)
            {
                moveableStones[i].SetSelector(true);
            }
        }
        else
        {
            state = States.SWITCH_PLAYER;
        }
        


    }

    public void OnlineHumanRoll(int[] possibleMoves)
    {
        //ActivateButton(false);
        

        List<LudoStone> moveableStones = new List<LudoStone>();
        for (int i = 0; i < possibleMoves.Length; i++)
        {
            moveableStones.Add(playerList[activePlayer].myStones[possibleMoves[i]]);
        }

        for (int i = 0; i < moveableStones.Count; i++)
        {
            moveableStones[i].SetSelector(true);
        }

    }

    



    List<LudoStone> PossibleStones() {
        List<LudoStone> tempList = new List<LudoStone>();

        for (int i = 0; i < playerList[activePlayer].myStones.Length; i++)
        {
            if (playerList[activePlayer].myStones[i].ReturnIsOut())
            {
                if (playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneID, rolledHumanDice))
                {
                    tempList.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }

                if (playerList[activePlayer].myStones[i].CheckPossible(rolledHumanDice))
                {
                    tempList.Add(playerList[activePlayer].myStones[i]);
                }
            }
        }

        return tempList;
    }
}


