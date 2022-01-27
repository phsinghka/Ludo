using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetButtonSevenUpDown : MonoBehaviour
{
    public Button BetButton;
    public RectTransform TargerPostion;
    public bool isUp = false;
    public bool isDown = false;
    public bool isMiddle = false;

    // Start is called before the first frame update
    void Start()
    {
        BetButton = gameObject.GetComponent<Button>();
        BetButton.onClick.AddListener(BetButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void BetButtonClick()
    {
        if(SevenUpDownManager.Instance.NoMoreBet)
            return;

        if(MainDataManager.Instance.playPoint <= BetManagerSevenUpDown.Instance.CurrentCoinAmount)
        {
            return;
        }
        Vector3 pos = BetManagerSevenUpDown.Instance.PlayerPostion.transform.position;
        GameObject Prefeb = Instantiate(BetManagerSevenUpDown.Instance.CurrentSelectCoins, pos, Quaternion.identity);
        Prefeb.transform.parent = TargerPostion.gameObject.transform;
        Prefeb.transform.localScale = new Vector3(1, 1, 1);
        // TargerPostion.transform.localPosition = new Vector3(Random.Range(-45 , 150) , Random.Range(-180, 150) , 0);

        Prefeb.GetComponent<CoinSevenUpDown>().TargetObject = TargerPostion.gameObject;
        Prefeb.GetComponent<CoinSevenUpDown>().IsPlayerCoin = true;
        Prefeb.GetComponent<CoinSevenUpDown>().TO = TargerPostion;
        Prefeb.GetComponent<CoinSevenUpDown>().PostionObject = BetManagerSevenUpDown.Instance.PlayerPostion;
        Prefeb.GetComponent<CoinSevenUpDown>().CenterPoint = SevenUpDownManager.Instance.CenterPoint;
        MainDataManager.Instance.playPoint -= BetManagerSevenUpDown.Instance.CurrentCoinAmount;
        SevenUpDownManager.Instance.PlayerInfoUpdate();

        JSONObject placeBet = new JSONObject();

        placeBet.AddField("userId", MainDataManager.Instance.id);
        if(isUp){
            placeBet.AddField("up",BetManagerSevenUpDown.Instance.CurrentCoinAmount);
            placeBet.AddField("middle", 0);
            placeBet.AddField("down", 0);
            SevenUpDownManager.Instance.PlayerAddBetLeftSideTextUpdate(BetManagerSevenUpDown.Instance.CurrentCoinAmount);
            SevenUpDownManager.Instance.TotalAmountLeftSide += BetManagerSevenUpDown.Instance.CurrentCoinAmount;
            SevenUpDownManager.Instance.TotalAddBetLeftSideTextUpdate();
        }
        if(isDown){
            placeBet.AddField("up",0);
            placeBet.AddField("middle", 0);
            placeBet.AddField("down", BetManagerSevenUpDown.Instance.CurrentCoinAmount);
            SevenUpDownManager.Instance.PlayerAddBetRightSideTextUpdate(BetManagerSevenUpDown.Instance.CurrentCoinAmount);
            SevenUpDownManager.Instance.TotalAmountRightSide += BetManagerSevenUpDown.Instance.CurrentCoinAmount;
            SevenUpDownManager.Instance.TotalAddBetRightSideTextUpdate();
        }
        if(isMiddle){
            placeBet.AddField("up",0);
            placeBet.AddField("middle", BetManagerSevenUpDown.Instance.CurrentCoinAmount);
            placeBet.AddField("down", 0);
            SevenUpDownManager.Instance.PlayerAddBetMiddleTextUpdate(BetManagerSevenUpDown.Instance.CurrentCoinAmount);
            SevenUpDownManager.Instance.TotalAmountMiddle += BetManagerSevenUpDown.Instance.CurrentCoinAmount;
            SevenUpDownManager.Instance.TotalAddBetMiddleTextUpdate();
        }
        placeBet.AddField("betPoint", BetManagerSevenUpDown.Instance.CurrentCoinAmount);
        Debug.Log(placeBet);
        TestSocketIO.Instance.SendData("place7UpBet", placeBet);


    }
}
