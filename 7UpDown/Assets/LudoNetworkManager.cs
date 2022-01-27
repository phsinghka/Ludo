using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;



public class LudoNetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] PlayerObjects;
    public Sprite[] diceImages;
    public PlayerPawns[] Pawns;
    public Image[] boardImages;
    public Image[] boardImagesRoute;
    public Material blueMat;
    public Material redMat;
    public Material greenMat;
    public Material yellowMat;

    public static LudoNetworkManager Instance;
    float timeRemain = 10;
    float maxTime = 10;

    void Start()
    {   
        Instance = this;
        SetDataFromLoad();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemain >= 0)
        {
            timeRemain -= Time.deltaTime;
            PlayerObjects[0].transform.Find("TimeName").Find("Time").GetComponentInChildren<Image>().fillAmount = (float)timeRemain/ (float)maxTime;
            PlayerObjects[1].transform.Find("TimeName").Find("Time").GetComponentInChildren<Image>().fillAmount = (float)timeRemain / (float)maxTime;
            PlayerObjects[2].transform.Find("TimeName").Find("Time").GetComponentInChildren<Image>().fillAmount = (float)timeRemain / (float)maxTime;
            PlayerObjects[3].transform.Find("TimeName").Find("Time").GetComponentInChildren<Image>().fillAmount = (float)timeRemain / (float)maxTime;
            //Debug.Log((float)timeRemain / (float)maxTime);
        }
    }
    public void SetDataFromLoad()
    {
        for (int i = 0; i < LudoDataManager.Instance.playersData.Length; i++)
        {
            
            int tempIndex = LudoDataManager.Instance.TempLocalIndex[i];
            PlayerObjects[tempIndex].transform.Find("TimeName").Find("Name").GetComponentInChildren<Text>().text = LudoDataManager.Instance.playersData[tempIndex].user_name;
            PlayerObjects[tempIndex].transform.Find("TimeName").Find("Time").gameObject.SetActive(false);
            timeRemain =  LudoDataManager.Instance.remainingTime;
            
            for (int j = 0; j < LudoDataManager.Instance.playersData[tempIndex].dice_counts.Length; j++)
            {
                SetBoardColor(tempIndex,j);
                if (LudoDataManager.Instance.playersData[tempIndex].dice_counts[j]!= -1){
                    StartCoroutine( Pawns[tempIndex].Pawns[j].MoveToSpecificNode(LudoDataManager.Instance.playersData[tempIndex].dice_counts[j]));
                }
                else
                {
                    Pawns[tempIndex].Pawns[j].ReturnToBase();
                }
            }


            //PlayerObjects[tempIndex].transform.Find("Dice").gameObject.SetActive(false);
        }

    }

    public void SetBoardColor(int tempIndex,int j)
    {
        if (LudoDataManager.Instance.playersData[tempIndex].color == "blue")
        {
            boardImages[tempIndex].color = blueMat.color;
            boardImagesRoute[tempIndex].color = blueMat.color;
            Pawns[tempIndex].Pawns[j].GetComponent<MeshRenderer>().material = blueMat;
        }
        if (LudoDataManager.Instance.playersData[tempIndex].color == "red")
        {
            boardImages[tempIndex].color = redMat.color;
            boardImagesRoute[tempIndex].color = redMat.color;
            Pawns[tempIndex].Pawns[j].GetComponent<MeshRenderer>().material = redMat;
        }
        if (LudoDataManager.Instance.playersData[tempIndex].color == "green")
        {
            boardImages[tempIndex].color = greenMat.color;
            boardImagesRoute[tempIndex].color = greenMat.color;
            Pawns[tempIndex].Pawns[j].GetComponent<MeshRenderer>().material = greenMat;
        }
        if (LudoDataManager.Instance.playersData[tempIndex].color == "yellow")
        {
            boardImages[tempIndex].color = yellowMat.color;
            boardImagesRoute[tempIndex].color = yellowMat.color;
            Pawns[tempIndex].Pawns[j].GetComponent<MeshRenderer>().material = yellowMat;
        }
    }

    public void SetPlayerTurn(JSONObject data)
    {
//        Debug.Log("dd");
        for (int i = 0; i < PlayerObjects.Length; i++)
        {
            PlayerObjects[i].transform.Find("TimeName").Find("Time").gameObject.SetActive(false);
        }
        //{"pt":0,"nt":1,"timer":10}
        LudoGameManager.instance.activePlayer = LudoDataManager.Instance.TempLocalIndex[int.Parse(data.GetField("nt").ToString().Trim(new char[] { '"' }))];


        Debug.Log(LudoGameManager.instance.activePlayer + "  ");

        if (LudoGameManager.instance.activePlayer == 0)
        {
            PlayerObjects[0].transform.Find("Dice").gameObject.SetActive(true);
        }
        else
        {
            //PlayerObjects[0].transform.Find("Dice").gameObject.SetActive(false);
            PlayerObjects[0].transform.Find("Dice").GetComponentInChildren<Animator>().enabled = false;

        }
        PlayerObjects[LudoGameManager.instance.activePlayer].transform.Find("TimeName").Find("Time").gameObject.SetActive(true);
        timeRemain = float.Parse(data.GetField("timer").ToString().Trim(new char[] { '"' }));
        maxTime = float.Parse(data.GetField("timer").ToString().Trim(new char[] { '"' }));
//        Debug.Log("dd");



    }
    public void PlayerTurnMiss(JSONObject data)
    {

        int maxMiss = 5;
        //"data": { "seat_index": 1,"turn_miss": 5 }
        int tempIndex = LudoDataManager.Instance.TempLocalIndex[int.Parse(data.GetField("seat_index").ToString().Trim(new char[] { '"' }))];
        int turnMiss = int.Parse(data.GetField("turn_miss").ToString().Trim(new char[] { '"' }));
        PlayerObjects[tempIndex].transform.Find("Heart").Find("HeartFilled").GetComponentInChildren<Image>().fillAmount = 1f - ((float)turnMiss / (float)maxMiss);

    }
    public void PlayerRollDice(JSONObject data)
    {
        /*"data": {
            "seat_index": 0,
            "dice_value": 5,
            "move_kukari_flag": true,
            "move_kukari_values": [
                true,
                false,
                false,
                false
            ],
            "auto_move": true
        }*/

        //Debug.Log("sf");

        if (data.HasField("seat_index"))
        {
            LudoGameManager.instance.activePlayer = LudoDataManager.Instance.TempLocalIndex[int.Parse(data.GetField("seat_index").ToString().Trim(new char[] { '"' }))];
        }

        int diceValue = int.Parse(data.GetField("dice_value").ToString().Trim(new char[] { '"' }));
        Debug.Log(LudoGameManager.instance.activePlayer + "  " + diceValue);
        PlayerObjects[LudoGameManager.instance.activePlayer].transform.Find("Dice").GetComponent<Image>().sprite = diceImages[diceValue-1];
        if(data.GetField("move_kukari_flag").ToString().Trim(new char[] { '"' }) == "true")
        {
            int count = 0;

            
            for (int i = 0; i < 4; i++)
            {
                if (data.GetField("move_kukari_values")[i].ToString() == "true") {
                    count++;
                }
            }


            int[] pawnIndexes = new int[count];
            count = 0;

            for (int i = 0; i < 4; i++)
            {
                if (data.GetField("move_kukari_values")[i].ToString() == "true")
                {
                    pawnIndexes[count] = i;
                    count++;
                }
            }

            LudoGameManager.instance.rolledHumanDice = diceValue;
            if(data.GetField("auto_move").ToString().Trim(new char[] { '"' }) == "true")
            {
                //TODO AutoMove
                //LudoGameManager.instance.playerList[LudoGameManager.instance.activePlayer].myStones[pawnIndexes[0]].OppoPlayerMove();
            }
            else
            {
                LudoGameManager.instance.OnlineHumanRoll(pawnIndexes);
            }
            
        }



    }
    public void LocalPlayerRoll()
    {
        JSONObject eventSend = new JSONObject();
        eventSend.AddField("en", "LUDO_ROTATE_DICE");
        eventSend.AddField("data", "");
        PlayerObjects[0].transform.Find("Dice").GetComponentInChildren<Animator>().enabled = true;
        PlayerObjects[0].transform.Find("Dice").GetComponentInChildren<Animator>().Play("DiceAnim");


        TestSocketIO.Instance.SendData("req", eventSend);
    }

    public void LudoKukariMove(JSONObject data)
    {
        LudoGameManager.instance.rolledHumanDice = int.Parse(data.GetField("dice_value").ToString().Trim(new char[] { '"' }));
        LudoGameManager.instance.activePlayer = int.Parse(data.GetField("seat_index").ToString().Trim(new char[] { '"' }));
        int pawnIndex = int.Parse(data.GetField("move_kukari_index").ToString().Trim(new char[] { '"' }));
        LudoGameManager.instance.playerList[LudoGameManager.instance.activePlayer].myStones[pawnIndex].OppoPlayerMove(); 
    }



}
