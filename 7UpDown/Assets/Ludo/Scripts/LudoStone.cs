using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoStone : MonoBehaviour
{

    public int stoneID;
    public int stoneNum;

    [Header("ROUTES")]
    public LudoRoute commonRoute; //Outer LudoRoute
    public LudoRoute finalRoute; //final route

    public List<LudoNode> fullRoute = new List<LudoNode>();

    [Header("NODES")]
    public LudoNode startNode; // this is the node the stone will move to when it leaves it's base.
    public LudoNode baseNode; // this is the node the stone is starting at.
    public LudoNode currentNode; //this is the node we are currently on
    public LudoNode goalNode; //this is the node we want to end on. 

    int routePosition; // the position at which our stone is currently at.
    int startNodeIndex;

    int steps;
    int doneSteps;

    [Header("Booleans")]
    public bool isOut;
    bool isMoving;


    bool hasTurn; //It is for human input 

    [Header("Selector")]
    public GameObject selector;


    //ARC MOVEMENTS
    float amplitude = 0.5f;
    float cTime = 0;

    private void Start()
    {
        startNodeIndex =  commonRoute.RequestPosition(startNode.gameObject.transform);
        CreateFullRoute();
        selector.SetActive(false);
        currentNode = startNode;
    }

    

    void CreateFullRoute()
    {
        for (int i = 0; i < commonRoute.childNodeList.Count-1; i++)
        {
            int tempPos = startNodeIndex + i;
            tempPos %= commonRoute.childNodeList.Count;

            fullRoute.Add(commonRoute.childNodeList[tempPos].GetComponent<LudoNode>());
        }
        
        for (int i = 0; i < finalRoute.childNodeList.Count; i++)
        {
            fullRoute.Add(finalRoute.childNodeList[i].GetComponent<LudoNode>());
        }
    }
    IEnumerator Move(int diceNumber)
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        while (steps > 0)
        {
            routePosition++; // update current route position 

            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            Vector3 startPos = fullRoute[routePosition - 1].gameObject.transform.position;
            while (MoveToNextNode(nextPos, 8f)) { yield return null; }
            //while (MoveInArcToNextNode(startPos,nextPos, 4f)) { yield return null; }
            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;
        }

        goalNode = fullRoute[routePosition];
        //check possible kicks
        if (goalNode.isTaken)
        {
            if (goalNode.isStar)
            {
                //TODO Resize the element 
            }
            else
            {
                goalNode.stone.ReturnToBase();
            }
        }

        currentNode.stone = null;
        currentNode.isTaken = false;

        goalNode.stone = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;

        //report to game manager
        if (currentNode.isWin)
        {
            this.gameObject.SetActive(false);
        }
        //WIN CONDITION CHECK
        if (WinCondition())
        {
            LudoGameManager.instance.ReportWinning();
        }

        //switch player
        if (diceNumber < 6)
        {
            LudoGameManager.instance.state = LudoGameManager.States.SWITCH_PLAYER;
        }
        else
        {
            LudoGameManager.instance.state = LudoGameManager.States.ROLL_DICE;
        }
        

        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goalPos, float speed)
    {
        return goalPos != (transform.position = Vector3.MoveTowards(transform.position, goalPos, speed * Time.deltaTime));
    }
    public IEnumerator MoveToSpecificNode (int finalPosition)
    {
        //LeaveBase();
        isOut = true;
        Vector3 nextPos = fullRoute[finalPosition].gameObject.transform.position;
        while (MoveToNextNode(nextPos, 100f)) { yield return null; }
        goalNode = fullRoute[finalPosition];
        
        currentNode.stone = null;
        currentNode.isTaken = false;

        goalNode.stone = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;

        //report to game manager
        if (currentNode.isWin)
        {
            this.gameObject.SetActive(false);
        }

    }


    bool MoveInArcToNextNode(Vector3 startPos, Vector3 goalPos, float speed)
    {
        cTime = speed * Time.deltaTime;
        Vector3 myPosition = Vector3.Lerp(startPos, goalPos, cTime);

        myPosition.y += amplitude * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);

        return goalPos != (transform.position = Vector3.Lerp(transform.position, myPosition, cTime));
    }

    public bool ReturnIsOut()
    {
        return isOut;
    }

    public void LeaveBase()
    {
        steps = 1;
        isOut = true;
        routePosition = 0;
        StartCoroutine(MoveOut());
    }

    IEnumerator MoveOut()
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        while (steps > 0)
        {
            //routePosition++; // update current route position 

            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            Vector3 startPos = baseNode.gameObject.transform.position;
            while (MoveToNextNode(nextPos, 8f)) { yield return null; }
            //while (MoveInArcToNextNode(startPos,nextPos, 4f)) { yield return null; }
            yield return new WaitForSeconds(0.1f);
            cTime = 0;
            steps--;
            doneSteps++;

        }

        goalNode = fullRoute[routePosition];
        if (goalNode.isTaken)
        {
            if (goalNode.isStar)
            {
                //TODOresize the element
            }
            else
            {
                goalNode.stone.ReturnToBase();
            }
        }

        goalNode.stone = this;
        goalNode.isTaken = true;

        currentNode = goalNode;
        goalNode = null;

        LudoGameManager.instance.state = LudoGameManager.States.ROLL_DICE;

        isMoving = false;
    }

    public bool CheckPossible(int diceNumber)
    {
        int tempPos = routePosition + diceNumber;
        if(tempPos >= fullRoute.Count)
        {
            return false;
        }
        return !fullRoute[tempPos].isTaken;

    }

    public bool CheckPossibleKick(int stoneID, int diceNumber)
    {
        int tempPos = routePosition + diceNumber;
        if (tempPos >= fullRoute.Count)
        {
            return false;
        }
        if (fullRoute[tempPos].isTaken)
        {
            if(stoneID == fullRoute[tempPos].stone.stoneID)
            {
                return false;
            }
            return true;
        }
        return false;

    }

    public void StartTheMove(int dicenumber)
    {
        steps = dicenumber;
        StartCoroutine(Move(dicenumber));
    }

    public void ReturnToBase()
    {
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        LudoGameManager.instance.ReportTurnPossible(false);
        routePosition = 0;
        currentNode = null;
        goalNode = null;
        isOut = false;
        doneSteps = 0;

        Vector3 baseNodePosition = baseNode.gameObject.transform.position;
        while (MoveToNextNode(baseNodePosition, 100f)) { yield return null; }
        LudoGameManager.instance.ReportTurnPossible(true);

    }

    bool WinCondition()
    {
        //WinLogic
        //for (int i = 0; i < ; i++)
        { 
            //TODO : Add a win logic
        }
        return false;
    }


    public void SetSelector(bool on)
    {
        selector.SetActive(on);
        hasTurn = on;
    }

    public void OppoPlayerMove()
    {
        if (!isOut)
        {
            LeaveBase();
        }
        else
        {
            StartTheMove(LudoGameManager.instance.rolledHumanDice);
        }
        LudoGameManager.instance.DeactivateSelector();
    }


    void OnMouseDown()
    {
        Debug.Log("sdfsd");
        if (hasTurn)
        {
            /*Request:
            {
                "en" : "LUDO_MOVE_KUKARI",
                "data" : {
                    "move_kukari_index": 1
                }
            }*/

            JSONObject pawnMove = new JSONObject();
            pawnMove.AddField("move_kukari_index", stoneNum);

            JSONObject eventSend = new JSONObject();
            eventSend.AddField("en", "LUDO_MOVE_KUKARI");
            eventSend.AddField("data", pawnMove);

            TestSocketIO.Instance.SendData("req", eventSend);

            Debug.Log("sdfsd");

            /*if (!isOut)
            {
                LeaveBase();
            }
            else
            {
                StartTheMove(LudoGameManager.instance.rolledHumanDice);
            }
            LudoGameManager.instance.DeactivateSelector();*/
        }


        
    }
}
