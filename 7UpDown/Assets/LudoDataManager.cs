using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class LudoPlayersData
{
    public int seat_index;
    public int[] dice_counts = new int[4];
    public int local_seat_index;
    public string _id;
    public string user_name;
    public int avatar_id;
    public int isRobot;
    public string color;
    public string status;
    public int turn_miss;
    public string winnerStatus;
    public int scoreTour;
}

public class LudoDataManager : MonoBehaviour
{


    public LudoPlayersData[] playersData = new LudoPlayersData[4];
    public int[] TempLocalIndex = new int[4];
    public string[] colors;
    public bool isTournament;
    public int remainingTime;
    public int maxPlayers;


    public static LudoDataManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void SetPlayersData(JSONObject data, string eventType)
    {
        //playersData = new LudoPlayersData[int.Parse(data.GetField("max_seat").ToString())];
        JSONObject player_info = data.GetField("player_info");
        Debug.Log("."+ player_info.Count);
        TempLocalIndex = new int[player_info.Count];
        int localPlayerIndex = 0;

        maxPlayers = player_info.Count;
        

        if (data.GetField("tournament").ToString() == "true")
        {
            isTournament = true;
        }
        else
        {
            isTournament = false;
        }
        
        remainingTime = int.Parse(data.GetField("remaing_time").ToString());


        //Finding Local Index
        for (int i = 0; i < player_info.Count; i++)
        {
            Debug.Log(".."+ MainDataManager.Instance._id +" "+player_info[i].GetField("user_info").GetField("_id").ToString().Trim(new char[] { '"' }));
            if (MainDataManager.Instance._id == player_info[i].GetField("user_info").GetField("_id").ToString().Trim(new char[] { '"' }))
            {
                localPlayerIndex = i;
                Debug.Log(".In");
                for (int j = 0; j < TempLocalIndex.Length; j++)
                {
                    if (j - i >= 0)
                    {
                        Debug.Log(".InIn");
                        TempLocalIndex[j] = j - i;

                    }
                    else
                    {
                        TempLocalIndex[j] = j - i + TempLocalIndex.Length;
                    }
                }
            }
        }
        Debug.Log("dfd");
        int localPlayerColor = System.Array.IndexOf(colors, player_info[localPlayerIndex].GetField("color").ToString().Trim(new char[] { '"' }));
        Debug.Log("dfd");

        if (localPlayerColor - localPlayerIndex >= 0)
        {
            localPlayerColor = localPlayerColor - localPlayerIndex;
        }
        else
        {
            localPlayerColor = localPlayerColor - localPlayerIndex + player_info.Count;
        }

        for (int i = 0; i < player_info.Count; i++)
        {

            playersData[i].color = colors[localPlayerColor];
            localPlayerColor++;
            if (localPlayerColor >= player_info.Count)
            {
                localPlayerColor = 0;
            }
        }

        Debug.Log("Seat Index . " );
        for (int i = 0; i < player_info.Count; i++)
        {
            Debug.Log("Seat Index . ");

            Debug.Log("Seat Index . "+i+" "+ int.Parse(player_info[i].GetField("seat_index").ToString())+" "+(player_info[i].GetField("seat_index").ToString()));
            
                        Debug.Log(".InIn");
            playersData[i].seat_index = int.Parse(player_info[i].GetField("seat_index").ToString());

            for (int j = 0; j < player_info[i].GetField("dice_slot").Count; j++)
            {
                playersData[i].dice_counts[j] = int.Parse(player_info[i].GetField("dice_slot")[j].ToString());
            }
                        Debug.Log(".InIn");
            playersData[i]._id = player_info[i].GetField("user_info").GetField("_id").ToString().Trim(new char[] { '"' });
                        Debug.Log(".InIn");
            playersData[i].user_name = player_info[i].GetField("user_info").GetField("user_name").ToString().Trim(new char[] { '"' });
                        Debug.Log(".InIn");
            //playersData[i].color = player_info[i].GetField("color").ToString().Trim(new char[] { '"' });
            //playersData[i].status = player_info[i].GetField("status").ToString().Trim(new char[] { '"' });
            //playersData[i].isRobot = int.Parse(player_info[i].GetField("is_robot").ToString());
                        Debug.Log(".InIn");
            playersData[i].seat_index = int.Parse(player_info[i].GetField("seat_index").ToString());
                        Debug.Log(".InIn");
            playersData[i].local_seat_index = TempLocalIndex[i];
            //playersData[i].scoreTour = int.Parse(player_info[i].GetField("score").ToString());
        }
        for (int i = 0; i < player_info.Count; i++)
        {
            if(i+localPlayerIndex > player_info.Count)
            {

            }
        }

        SceneManager.LoadScene(2);
    }

    // Update is called once per frame
    
}
