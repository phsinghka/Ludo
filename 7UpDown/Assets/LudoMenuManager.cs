using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LudoMenuManager : MonoBehaviour
{

    public static LudoMenuManager Instance;

    public GameObject LudoPriceSelector;
    public GameObject LudoTournamentSelector;
    public string[] BetIds;
    public string[] SelectedColorText;


    [Header("Match Making Menu")]
    public GameObject[] MatchMenuPlayers;
    public Text TimeMatchMenu;
    public GameObject MatchMakingMenu;
    bool isBetCollect = false;
    public Animator betCollectAnim;
    public Text AmountText;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DisplayAllLudoGames(JSONObject data)
    {
        
        //Debug.Log("NOW DISPLAYING" + data[0].GetField("bet").ToString().Trim(new char[] { '"' }));
        data = data.GetField("betList");
        BetIds = new string[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            BetIds[i] = data[i].GetField("_id").ToString();
        }

        LudoPriceSelector.SetActive(true);

        SetScreenValues.Instance.SetLudoMenu(data);
    }
    public void DisplayAllTournamentGames(JSONObject data)
    {

        //Debug.Log("NOW DISPLAYING" + data[0].GetField("bet").ToString().Trim(new char[] { '"' }));
        data = data.GetField("list");
        BetIds = new string[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            BetIds[i] = data[i].GetField("_id").ToString();
        }

        LudoTournamentSelector.SetActive(true);

        SetScreenValues.Instance.SetTourMenu(data);
    }
    public void CloseTournament()
    {
        LudoTournamentSelector.SetActive(false);
        JSONObject eventSend = new JSONObject();
        eventSend.AddField("en", "LUDO_TOUR_LIST_CLOSE");
        eventSend.AddField("data", "");

        TestSocketIO.Instance.SendData("req", eventSend);
    }

    public void StartSearchUser()
    {
        /*Request:
        ->betId : _id of GET_BET_LISTS event response bet list data.
        -> color : kukari color
        {
            "en" : "LUDO_FIND_TABlE",
            "data" : {
                "betId" : "616fdb521189a1fb1a5a0f7e",
                "color" : "blue", 
                "max_seat" : 2
            }
        }*/

        JSONObject findTable = new JSONObject();
        findTable.AddField("betId", BetIds[(int)SetScreenValues.Instance.LPriceSelector.value].ToString().Trim(new char[] { '"' }));

        findTable.AddField("color", SelectedColorText[SetScreenValues.Instance.SelectedColor]);
        findTable.AddField("sub_type", PlayerPrefs.GetString("LudoSubType"));
        findTable.AddField("max_seat", 4);

        JSONObject eventSend = new JSONObject();
        eventSend.AddField("en", "LUDO_FIND_TABLE");
        eventSend.AddField("data", findTable);

        TestSocketIO.Instance.SendData("req", eventSend);


    }

    public void StartMatchMaking(JSONObject data)
    {
        Debug.Log("MatchMaking");
        MatchMakingMenu.SetActive(true);
        //Screen.orientation = ScreenOrientation.Portrait;

        for (int i = 0; i < MatchMenuPlayers.Length; i++)
        {
            MatchMenuPlayers[i].transform.GetComponent<Animator>().enabled = true;
        }



        JSONObject player_info = data.GetField("player_info");
        StartCoroutine(TimeDecreasing(int.Parse(data.GetField("player_not_found_remaning_timer").ToString().Trim(new char[] { '"' }))));

        for (int i = 0; i < player_info.Count; i++)
        {
            if (player_info[i].HasField("user_info"))
            {
                MatchMenuPlayers[i].transform.GetComponent<Animator>().enabled = false;
                MatchMenuPlayers[i].transform.Find("Image").Find("Name").GetComponentInChildren<Text>().text = player_info[i].GetField("user_info").GetField("user_name").ToString().Trim(new char[] { '"' });
                //MatchMenuPlayers[i].transform.Find("Image").GetComponentInChildren<Image>().sprite = player_info[i].GetField("user_info").GetField("user_name").ToString().Trim(new char[] { '"' });
            }
            else
            {
                MatchMenuPlayers[i].transform.GetComponent<Animator>().enabled = true;
            }
        }
    }

    public void JoinedMatchMaking(JSONObject data)
    {
        int seat_index = int.Parse(data.GetField("seat_index").ToString());

        MatchMenuPlayers[seat_index].transform.GetComponent<Animator>().enabled = false;
        MatchMenuPlayers[seat_index].transform.Find("Image").Find("Name").GetComponentInChildren<Text>().text = data.GetField("user_info").GetField("user_name").ToString().Trim(new char[] { '"' });
       
    }

    public void BetCollectAnimation(JSONObject data)
    {
        /* "data":{ "bet":10,"game_id":69}*/
        
        MainDataManager.Instance.lastGameID = data.GetField("game_id").ToString().Trim(new char[] { '"' });
        int bet = int.Parse(data.GetField("bet").ToString().Trim(new char[] { '"' }));
        AmountText.text = (bet * 4).ToString();
        betCollectAnim.Play("BetCollect");

        


    }


    IEnumerator TimeDecreasing(int time)
    {
        SB:
        time -= 1;
        TimeMatchMenu.text = "00:"+time.ToString("00");
        yield return new WaitForSeconds(1f);

        if (time > 0)
        {
            goto SB;
        }
        else
        {
            NetworkManagerMain.Instance.errorText.text = "No Online Players !! Please Try Again after Sometime";
            MatchMakingMenu.SetActive(false);
            LudoPriceSelector.SetActive(false);
            yield return new WaitForSeconds(2f);
            NetworkManagerMain.Instance.errorText.text = "";
        }
    }
}
