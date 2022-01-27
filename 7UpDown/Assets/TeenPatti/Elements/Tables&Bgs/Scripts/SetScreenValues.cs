using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetScreenValues : MonoBehaviour
{

    public static SetScreenValues Instance;

    [Header("TeenPattiVariables")]
    public Text PriceText;
    public Text MinPriceText;
    public Button plus;
    public Button minus;
    int plusLimit;
    public int currentPlay;
    public Sprite NO_VARIATION_SPRITE;
    public Sprite AK47_SPRITE;
    public Sprite JOKER_SPRITE;
    public Sprite LOWER_JOKER_SPRITE;
    public Sprite HIGHEST_JOKER_SPRITE;
    public Sprite FOURXBOOT_SPRITE;
    public Sprite MUFLIS_SPRITE;
    public Text CurrentGame;
    public Slider PriceSelector;
    public GameObject AddCashBtn;
    public GameObject PlayNowBtn;


    [Header("LudoVariables")]
    public Text LPriceText;
    public Text LMinPriceText;
    public Button Lplus;
    public Button Lminus;
    public int LplusLimit;
    public int LcurrentPlay;
    public Slider LPriceSelector;
    public GameObject LAddCashBtn;
    public GameObject LPlayNowBtn;
    public GameObject[] colorBg;
    public int SelectedColor;
    

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void SetTeenPattiMenu(){
        TeenPatti.RoomType GameName = MainDataManager.Instance.teenPattiRoomType;
        if(GameName == TeenPatti.RoomType.NO_VARIATION){
            CurrentGame.text =  "TEENPATTI - CLASSIC";
        }
        else if(GameName == TeenPatti.RoomType.AK47){
            CurrentGame.text =  "TEENPATTI - AK47";
        }
        else if(GameName == TeenPatti.RoomType.JOKER){
            CurrentGame.text =  "TEENPATTI - JOKER";
        }
        else if(GameName == TeenPatti.RoomType.LOWER_JOKER){
            CurrentGame.text =  "TEENPATTI - LOWEST JOKER";
        }
        else if(GameName == TeenPatti.RoomType.HIGHEST_JOKER){
            CurrentGame.text =  "TEENPATTI - HIGHEST JOKER";
        }
        else if(GameName == TeenPatti.RoomType.FOURXBOOT){
            CurrentGame.text =  "TEENPATTI - 4XBOOT";
        }
        else if(GameName == TeenPatti.RoomType.MUFLIS){
            CurrentGame.text =  "TEENPATTI - MUFLISH";
        }
        currentPlay = 0;

        PriceSelector.value = currentPlay;
        Debug.Log(MainDataManager.Instance.tableBootValues.Length);
        for(int i = 0;i<MainDataManager.Instance.tableBootValues.Length;i++){
                // Debug.Log(MainDataManager.Instance.playPoint+" " + MainDataManager.Instance.tableBootValues[i]*10);

            if(MainDataManager.Instance.playPoint>= MainDataManager.Instance.tableBootValues[i]*10){
                // Debug.Log(MainDataManager.Instance.playPoint+" " + MainDataManager.Instance.tableBootValues[i]*10);
                plusLimit=i;
            }
        }
        SwitchButton();
        minus.interactable = false;
        PriceText.text = "₹ "+MainDataManager.Instance.tableBootValues[currentPlay].ToString();
        MinPriceText.text = "₹ "+ (10*MainDataManager.Instance.tableBootValues[currentPlay]);

    }
    public void PlusButtonTeenPatti(){
        plus.interactable = true;
        minus.interactable = true;
        currentPlay++;
        PriceSelector.value = currentPlay;
        PriceText.text = "₹ "+MainDataManager.Instance.tableBootValues[currentPlay].ToString();
        MinPriceText.text = "₹ "+ (10*MainDataManager.Instance.tableBootValues[currentPlay]);
        if(currentPlay==MainDataManager.Instance.tableBootValues.Length-1){
            plus.interactable = false;
        }
        SwitchButton();

    }
    public void SliderValueChanged(){
        currentPlay = (int)PriceSelector.value;
        plus.interactable = true;
        minus.interactable = true;
        PriceSelector.value = currentPlay;
        PriceText.text = "₹ "+MainDataManager.Instance.tableBootValues[currentPlay].ToString();
        MinPriceText.text = "₹ "+ (10*MainDataManager.Instance.tableBootValues[currentPlay]);
        if(currentPlay==MainDataManager.Instance.tableBootValues.Length-1){
            plus.interactable = false;
        }
        if(currentPlay==0){
            minus.interactable = false;
        }
        SwitchButton();

    }

    
    public void MinusButtonTeenPatti(){
        plus.interactable = true;
        minus.interactable = true;
        if(currentPlay>0){
            currentPlay--;
            PriceSelector.value = currentPlay;
            PriceText.text = "₹ "+MainDataManager.Instance.tableBootValues[currentPlay].ToString();
            MinPriceText.text = "₹ "+ (10*MainDataManager.Instance.tableBootValues[currentPlay]);
        }
        if(currentPlay==0){
            minus.interactable = false;
        }
        SwitchButton();

    }
    public void SwitchButton(){
        if(currentPlay>plusLimit){
            AddCashBtn.SetActive(true);
            PlayNowBtn.SetActive(false);
        }
        else{
            AddCashBtn.SetActive(false);
            PlayNowBtn.SetActive(true);
        }
    }



    JSONObject betData;
    //LUDOOOOO
    public void SetLudoMenu(JSONObject data)
    {
        Debug.Log(".");
        betData = data;
        Debug.Log(".");

        LcurrentPlay = 0;
        LPriceSelector.maxValue = data.Count-1;
        Debug.Log(".");
        
        LPriceSelector.value = LcurrentPlay;
        for (int i = 0; i < data.Count; i++)
        {
            if (MainDataManager.Instance.playPoint >= int.Parse(data[i].GetField("bet").ToString()))
            {
                LplusLimit = i;
                Debug.Log(".");

            }
        }
        Debug.Log(".");
        SelectHover(0);

        SwitchButtonLudo();
        Lminus.interactable = false;
        LPriceText.text = "₹ " + (float.Parse(data[LcurrentPlay].GetField("bet").ToString()) - (float.Parse(data[LcurrentPlay].GetField("bet").ToString()) * (float.Parse(data[LcurrentPlay].GetField("rate").ToString()) / 100.0f))) * 4;
        LMinPriceText.text = "₹ " + int.Parse(data[LcurrentPlay].GetField("bet").ToString());

        //UpdateValues in Selector

    }

    public GameObject TourParent;
    public GameObject TourPrefab;
    public Text ResultTour;

    public void SetTourMenu(JSONObject data)
    {
        
        
        int count = 0;
        ResultTour.text = "";
        foreach (Transform child in TourParent.transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log(data.ToString());
        for (int i = 0; i < data.Count; i++)
        {
            GameObject gb = Instantiate(TourPrefab);
            count++;
            gb.transform.SetParent(TourParent.transform);
            //gb.transform.localScale = new Vector3(0.8329046f, 0.06772976f, 0.7403595f);
            gb.transform.Find("Jackpot").Find("Value").GetComponentInChildren<Text>().text = "₹ " + data[i].GetField("total_pool_price").ToString().Replace("\"", "");
            gb.transform.Find("RemainingTime").Find("Value").GetComponentInChildren<Text>().text = data[i].GetField("remaning_time").ToString().Replace("\"", "");
            gb.transform.Find("Title").Find("Value").GetComponentInChildren<Text>().text = data[i].GetField("type").ToString().Replace("\"", "");
            gb.transform.Find("PeopleOnline").Find("Value").GetComponentInChildren<Text>().text = data[i].GetField("join_users").ToString()+"";
            gb.transform.Find("JoinButton").Find("Value").GetComponentInChildren<Text>().text = "₹ " + data[i].GetField("bet").ToString().Replace("\"", "");
            gb.transform.Find("JoinButton").GetComponent<TourCallScript>().TourID  = data[i].GetField("_id").ToString();
        }
        if (count == 0)
        {
            NotEnoughData();
        }
    }
    public void NotEnoughData()
    {
        foreach (Transform child in TourParent.transform)
        {
            Destroy(child.gameObject);
        }
        ResultTour.text = "NO TOURNAMENTS AVAILABLE RIGHT NOW";

    }
    public void CallTournament(string value)
    {
        Debug.Log(value);
    }
    //Tounament UpdateValues

    public void SliderValueChangedLudo()
    {
        LcurrentPlay = (int)LPriceSelector.value;
        Lplus.interactable = true;
        Lminus.interactable = true;
        LPriceSelector.value = LcurrentPlay;
        LPriceText.text = "₹ " + (float.Parse(betData[LcurrentPlay].GetField("bet").ToString())-(float.Parse(betData[LcurrentPlay].GetField("bet").ToString()) * (float.Parse(betData[LcurrentPlay].GetField("rate").ToString()) / 100.0f))) * 4;
        LMinPriceText.text = "₹ " + int.Parse(betData[LcurrentPlay].GetField("bet").ToString());
        if (LcurrentPlay == betData.Count - 1)
        {
            Lplus.interactable = false;
        }
        if (LcurrentPlay == 0)
        {
            Lminus.interactable = false;
        }
        SwitchButtonLudo();

    }

    public void PlusButtonLudo()
    {
        Lplus.interactable = true;
        Lminus.interactable = true;
        LcurrentPlay++;
        LPriceSelector.value = LcurrentPlay;
        LPriceText.text = "₹ " + (float.Parse(betData[LcurrentPlay].GetField("bet").ToString()) - (float.Parse(betData[LcurrentPlay].GetField("bet").ToString()) * (float.Parse(betData[LcurrentPlay].GetField("rate").ToString()) / 100.0f))) * 4;
        LMinPriceText.text = "₹ " + int.Parse(betData[LcurrentPlay].GetField("bet").ToString());
        if (LcurrentPlay == betData.Count - 1)
        {
            Lplus.interactable = false;
        }
        SwitchButtonLudo();

    }

    public void MinusButtonLudo()
    {
        Lplus.interactable = true;
        Lminus.interactable = true;
        if (LcurrentPlay > 0)
        {
            LcurrentPlay--;
            LPriceSelector.value = LcurrentPlay;
            LPriceText.text = "₹ " + (float.Parse(betData[LcurrentPlay].GetField("bet").ToString()) - (float.Parse(betData[LcurrentPlay].GetField("bet").ToString()) * (float.Parse(betData[LcurrentPlay].GetField("rate").ToString()) / 100.0f))) * 4;
            LMinPriceText.text = "₹ " + int.Parse(betData[LcurrentPlay].GetField("bet").ToString());
        }
        if (LcurrentPlay == 0)
        {
            Lminus.interactable = false;
        }
        SwitchButtonLudo();

    }

    public void SwitchButtonLudo()
    {
        if (LcurrentPlay > LplusLimit)
        {
            LAddCashBtn.SetActive(true);
            LPlayNowBtn.SetActive(false);
        }
        else
        {
            LAddCashBtn.SetActive(false);
            LPlayNowBtn.SetActive(true);
        }
    }

    public void SelectHover(int num)
    {
        for (int i = 0; i < colorBg.Length; i++)
        {
            colorBg[i].SetActive(false);
        }
        colorBg[num].SetActive(true);
        SelectedColor = num;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
