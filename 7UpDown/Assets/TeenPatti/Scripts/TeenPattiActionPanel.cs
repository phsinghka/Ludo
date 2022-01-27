using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeenPattiActionPanel : MonoBehaviour
{
    public static TeenPattiActionPanel Instance;
    public Text Amount;
    public GameObject PlusButton;
    public GameObject MinusButton;
    public GameObject PackButton;
    public GameObject ChalButton;
    public GameObject ShowButton;
    public GameObject SideShowButton;
    int sideShowValue = -1;

    float currAmount;
    bool isPlus;
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateActionPanelValues(){
        int playerTurnIndex = System.Array.IndexOf(TeenPattiTableManager.Instance.roomData.playersTurns,0);
        int sideShowIndex = -1;
        ShowButton.SetActive(false);
        SideShowButton.SetActive(false);
        ChalButton.SetActive(true);
        isPlus = false;
        currAmount = 0;
        if(TeenPattiTableManager.Instance.playersData[TeenPattiTableManager.Instance.localPlayerIndex].playerCardStatus == TeenPatti.PlayerCardStatus.BLIND){
            currAmount = TeenPattiTableManager.Instance.roomData.currentChalValue;
            Amount.text = ""+currAmount;
            ChalButton.transform.Find("Text").GetComponentInChildren<Text>().text = "BLIND";
            if(TeenPattiTableManager.Instance.roomData.currentChalValue>=TeenPattiTableManager.Instance.roomData.ChalLimit/2){
                PlusButton.transform.GetComponentInChildren<Button>().interactable = false;
            }
            else{
                PlusButton.transform.GetComponentInChildren<Button>().interactable = true;
            }
        }
        else{
            currAmount = TeenPattiTableManager.Instance.roomData.currentChalValue*2;
            Amount.text = ""+currAmount;
            ChalButton.transform.Find("Text").GetComponentInChildren<Text>().text = "CHAAL";
            if(TeenPattiTableManager.Instance.roomData.currentChalValue*2>=TeenPattiTableManager.Instance.roomData.ChalLimit){
                PlusButton.transform.GetComponentInChildren<Button>().interactable = false;
            }
            else{
                PlusButton.transform.GetComponentInChildren<Button>().interactable = true;
            }
        }
        MinusButton.transform.GetComponentInChildren<Button>().interactable = false;
        if(TeenPattiTableManager.Instance.roomData.playersTurns.Length == 2){
            SideShowButton.SetActive(false);
            ShowButton.SetActive(true);
        }

        else if(playerTurnIndex!=-1){
            if(playerTurnIndex==0){
                sideShowIndex = TeenPattiTableManager.Instance.roomData.playersTurns.Length-1;
            }
            else{
                sideShowIndex = playerTurnIndex-1;
            }
            sideShowValue = TeenPattiTableManager.Instance.roomData.playersTurns[sideShowIndex];
            sideShowValue =  System.Array.IndexOf(TeenPattiTableManager.Instance.TablePosition,sideShowValue);
            if(sideShowValue != -1){
                if(TeenPattiTableManager.Instance.playersData[sideShowValue].playerCardStatus == TeenPatti.PlayerCardStatus.SEEN && TeenPattiTableManager.Instance.playersData[TeenPattiTableManager.Instance.localPlayerIndex].playerCardStatus == TeenPatti.PlayerCardStatus.SEEN){
                    SideShowButton.SetActive(true);
                    ShowButton.SetActive(false);
                } 
            }
        }
        if(MainDataManager.Instance.playPoint<currAmount){
            SideShowButton.SetActive(false);
            ChalButton.SetActive(false);
            ShowButton.SetActive(false);
        }
        if(MainDataManager.Instance.playPoint<currAmount*2){
            PlusButton.transform.GetComponentInChildren<Button>().interactable = false;
            MinusButton.transform.GetComponentInChildren<Button>().interactable = false;
        }
    }

    public void PlusButtonClick(){
        PlusButton.transform.GetComponentInChildren<Button>().interactable = false;
        MinusButton.transform.GetComponentInChildren<Button>().interactable = true;
        isPlus = true;
        Amount.text = ""+currAmount*2;

    }
    public void MinusButtonClick(){
        PlusButton.transform.GetComponentInChildren<Button>().interactable = true;
        MinusButton.transform.GetComponentInChildren<Button>().interactable = false;
        isPlus = false;
        Amount.text = ""+currAmount;
    }

    public void OnNextButtonClick(){
        TeenPattiTableManager.Instance.NextPlayer();
    }






//BUTTONS

    public void PackButtonClick(){
        TeenPattiGameManager.Instance.SeeBtn.SetActive(false);
        TeenPattiTableManager.Instance.PlayerPackCards(TeenPattiTableManager.Instance.localPlayerIndex);
    }
    public void ChaalButtonClick(){
        TeenPattiTableManager.Instance.PlayerChaal(isPlus, TeenPattiTableManager.Instance.localPlayerIndex);
        TeenPattiTableManager.Instance.NextPlayer();
        if(isPlus){
            UpdateMoney(-currAmount*2,TPConstants.CHAAL);
        }
        else{
            UpdateMoney(-currAmount,TPConstants.CHAAL);
        }
    }
    public void ShowButtonClick(){
        UpdateMoney(-currAmount,TPConstants.SHOW);
        TeenPattiTableManager.Instance.ShowCards();
    }
    public void SideShowButtonClick(){
        UpdateMoney(-currAmount,TPConstants.SIDE_SHOW);
        TeenPattiTableManager.Instance.SideShowCards(TeenPattiTableManager.Instance.localPlayerIndex,sideShowValue);
    }
    


    public void UpdateMoney(float amount, string betType){
        JSONObject balUpdate = new JSONObject();
        balUpdate.AddField("token", "Bearer " + MainDataManager.Instance.token);
        balUpdate.AddField("amount", amount);
        balUpdate.AddField("betType", betType);
        TestSocketIO.Instance.SendData("balanceUpdate", balUpdate);
    }
    

}
