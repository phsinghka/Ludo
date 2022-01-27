using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourCallScript : MonoBehaviour
{
    public string TourID;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void StartTournament()
    {
        /*Request:
        {
            "en" : "LUDO_TOUR_JOIN",
            "data" : {
                "tour_id" : ""
            }
        }*/
        JSONObject tourJoin = new JSONObject();

        tourJoin.AddField("tour_id", TourID.ToString().Replace("\"",""));

        JSONObject eventSend = new JSONObject();
        eventSend.AddField("en", "LUDO_TOUR_JOIN");
        eventSend.AddField("data", tourJoin);

        TestSocketIO.Instance.SendData("req", eventSend);

        Debug.Log(TourID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
