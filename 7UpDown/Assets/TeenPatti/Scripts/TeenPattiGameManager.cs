using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeenPattiGameManager : MonoBehaviour
{
    [Header("------------------- Cards Animations ------------------------")]
    public Animator CardAnimations;
    public Animator GirlAnimations;
    public GameObject[] CurrentPlayerCards , Player1cards , Player2Cards , Player3Cards , Player4Cards;
    public GameObject CurrentPlayerCardsStatus , Player1cardsStatus , Player2CardsStatus , Player3CardsStatus , Player4CardsStatus;
    public GameObject[] AmountCollection;
    public Text[] LastAmount;
    public GameObject SeeBtn;
    public GameObject SideShowPanel;

    public bool Player1Online , Player2Online , Player3Online , Player4Online;
    public Sprite[] playerSprites;
    public static TeenPattiGameManager Instance;
    public Color[] timerColors;
    Color lerpedColor = Color.green;
    public GameObject[] PlayerTimerObject;
    public GameObject[] TimerNeedleRotation;
    public Image[] TimerNeedleImage;
    public Image[] TimerCircleImage;
    public Image[] TimerCircleChildImage;
    public RectTransform StarGB;




    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        SeeBtn.SetActive(false);
        SideShowPanel.SetActive(false);


        PlayerTimerObject = new GameObject[TeenPattiTableManager.Instance.PlayerList.Length];
        TimerNeedleRotation = new GameObject[TeenPattiTableManager.Instance.PlayerList.Length];
        TimerNeedleImage = new Image[TeenPattiTableManager.Instance.PlayerList.Length];
        TimerCircleImage = new Image[TeenPattiTableManager.Instance.PlayerList.Length];
        TimerCircleChildImage = new Image[TeenPattiTableManager.Instance.PlayerList.Length];
        for (int i = 0; i < TeenPattiTableManager.Instance.PlayerList.Length; i++)
        {
            PlayerTimerObject[i] = TeenPattiTableManager.Instance.PlayerList[i].transform.Find("Timer").gameObject;
            TimerNeedleRotation[i] = PlayerTimerObject[i].transform.Find("Needle").gameObject;
            TimerNeedleImage[i] = TimerNeedleRotation[i].transform.Find("Image").GetComponentInChildren<Image>();
            TimerCircleImage[i] = PlayerTimerObject[i].transform.Find("Circle").GetComponentInChildren<Image>();
            TimerCircleChildImage[i] = PlayerTimerObject[i].transform.Find("CircleChild").GetComponentInChildren<Image>();
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(SideShowPanel.activeInHierarchy){
            SideShowPanel.transform.Find("TimerText").GetComponentInChildren<Text>().text = ((int)TeenPattiTableManager.Instance.roomData.playersTurnsTimer-1) +"s";
            if(TeenPattiTableManager.Instance.roomData.playersTurnsTimer<=1.1){
                TeenPattiTableManager.Instance.SideShowDecline();
            }
        }
        StarAnimation();
        
    }
    public void StarAnimation(){
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // Update the Text on the screen depending on current position of the touch each frame
            StarGB.position = touch.position;
            StarGB.gameObject.SetActive(true);
            Invoke("StopAnimation",0.5f);


        }
        if(Input.GetButtonDown("Fire1")){
            Vector3 mousePos = Input.mousePosition;
            StarGB.position = mousePos;
            StarGB.gameObject.SetActive(true);
            Invoke("StopAnimation",0.5f);


        }
    }
    void StopAnimation(){
        StarGB.gameObject.SetActive(false);
    }
    int currentPlayIndex = 0;
    public void ActivateTimerObject(int indexNumber){
        for (int i = 0; i < PlayerTimerObject.Length; i++)
        {
            PlayerTimerObject[i].SetActive(false);
            TimerNeedleRotation[i].transform.rotation = Quaternion.Euler(0, 0, 0); 
            TimerNeedleImage[i].color = timerColors[0];
            TimerCircleImage[i].color = timerColors[0];
            TimerCircleImage[i].fillAmount = 1;
            TimerCircleChildImage[i].fillAmount = 1;
        }
        currentPlayIndex = indexNumber;
        PlayerTimerObject[indexNumber].SetActive(true);
    }
    public void ActivateTimerObjectOff(){
        for (int i = 0; i < PlayerTimerObject.Length; i++)
        {
            PlayerTimerObject[i].SetActive(false);
            TimerNeedleRotation[i].transform.rotation = Quaternion.Euler(0, 0, 0); 
            TimerNeedleImage[i].color = timerColors[0];
            TimerCircleImage[i].color = timerColors[0];
            TimerCircleImage[i].fillAmount = 1;
            TimerCircleChildImage[i].fillAmount = 1;
        }
    }

    public void UpdateTimerTime(float turnTime){
        lerpedColor = Color.Lerp(timerColors[0], timerColors[1], (TPConstants.PLAYER_TURN_WATING_TIME - turnTime)*((float)1/TPConstants.PLAYER_TURN_WATING_TIME));
        float Degree = (TPConstants.PLAYER_TURN_WATING_TIME - turnTime)*((float)360/TPConstants.PLAYER_TURN_WATING_TIME);
        TimerNeedleRotation[currentPlayIndex].transform.rotation = Quaternion.Euler(0, 0, Degree+5); 
        TimerNeedleImage[currentPlayIndex].color = lerpedColor;    
        TimerCircleImage[currentPlayIndex].color = lerpedColor;    
        TimerCircleImage[currentPlayIndex].fillAmount = 1- (TPConstants.PLAYER_TURN_WATING_TIME - turnTime)*((float)1/TPConstants.PLAYER_TURN_WATING_TIME);
        TimerCircleChildImage[currentPlayIndex].fillAmount = 1- (TPConstants.PLAYER_TURN_WATING_TIME - turnTime)*((float)1/TPConstants.PLAYER_TURN_WATING_TIME);
    }

    public void ShowSideShowPanel(string name, int avatarId, int sideshow0, int sideshow1){
        SideShowPanel.transform.Find("AvatarID").GetComponentInChildren<Image>().sprite = playerSprites[avatarId];
        SideShowPanel.transform.Find("PlayerName").GetComponentInChildren<Text>().text = name;
        SideShowPanel.SetActive(true);


    }
    #region  Player Cards Animation On Start 
    public void ChaalAnimation(int index, float amount, bool isWin){
        AmountCollection[index].transform.Find("AmountText").GetComponentInChildren<Text>().text = amount+" ₹";
        if(isWin){
            Debug.LogError("adfasd " + index + "  amount " + amount+" winReason ");
            AmountCollection[index].GetComponent<Animator>().Play("RPlayer"+index);
        }
        else{
            LastAmount[index].text = amount+"";
            Debug.LogError("adfasd " + index + "  amount " + amount);
            AmountCollection[index].GetComponent<Animator>().Play("Player"+index);
        }
    }
    // public void WinAnimation(int index, float amount){
    //     Debug.LogError("adfasd " + index + "  amount " + amount+" winReason ");
    //     // AmountCollection[index].transform.Find("AmountText").GetComponentInChildren<Text>().text = amount+" ₹";
    //     // AmountCollection[index].GetComponent<Animator>().Play("RPlayer"+index);
    //     if(index == 0){
    //         TeenPattiActionPanel.Instance.UpdateMoney(amount, TPConstants.WIN);            
    //     }
    // }
    public void CardAnimationPlaying()
    {
        SeeBtn.SetActive(false);

        CardAnimations.gameObject.SetActive(true);
        StartCoroutine(StartAnimation());
    }
    

    IEnumerator StartAnimation()
    {           

            TeenPattiActionPanel.Instance.UpdateMoney(-TeenPattiTableManager.Instance.roomData.BootValue, TPConstants.BOOT);            
            if(Player2Online){
                AmountCollection[2].transform.Find("AmountText").GetComponentInChildren<Text>().text = TeenPattiTableManager.Instance.roomData.BootValue+" ₹";
                LastAmount[2].text = TeenPattiTableManager.Instance.roomData.BootValue+"";
                AmountCollection[2].GetComponent<Animator>().Play("Player2");
            }
            if(Player1Online){
                AmountCollection[1].transform.Find("AmountText").GetComponentInChildren<Text>().text = TeenPattiTableManager.Instance.roomData.BootValue+" ₹";
                LastAmount[1].text = TeenPattiTableManager.Instance.roomData.BootValue+"";
                AmountCollection[1].GetComponent<Animator>().Play("Player1");
            }
            if(Player4Online){
                AmountCollection[4].transform.Find("AmountText").GetComponentInChildren<Text>().text = TeenPattiTableManager.Instance.roomData.BootValue+" ₹";
                LastAmount[4].text = TeenPattiTableManager.Instance.roomData.BootValue+"";
                AmountCollection[4].GetComponent<Animator>().Play("Player4");
            }
            if(Player3Online){
                AmountCollection[3].transform.Find("AmountText").GetComponentInChildren<Text>().text = TeenPattiTableManager.Instance.roomData.BootValue+" ₹";
                LastAmount[3].text = TeenPattiTableManager.Instance.roomData.BootValue+"";
                AmountCollection[3].GetComponent<Animator>().Play("Player3");
            }
            AmountCollection[0].transform.Find("AmountText").GetComponentInChildren<Text>().text = TeenPattiTableManager.Instance.roomData.BootValue+" ₹";
            LastAmount[0].text = TeenPattiTableManager.Instance.roomData.BootValue+"";
            AmountCollection[0].GetComponent<Animator>().Play("Player0");
            

        for (int i = 0; i < 3; i++)
        {
            //Player 2
            if(Player2Online)
            {
                CardAnimations.Play("Player2CardAnimation");
                GirlAnimations.Play("Player2");
                yield return new WaitForSeconds(0.35f);
                Player2Cards[i].SetActive(true);
            }

            //Player 1
            if(Player1Online)
            {
                CardAnimations.Play("Player1CardAnimation");
                GirlAnimations.Play("Player1");
                yield return new WaitForSeconds(0.35f);
                Player1cards[i].SetActive(true);
            }

            //Current Player
            CardAnimations.Play("CurrentPlayerAnimation");
                GirlAnimations.Play("CurrPlayer");
            yield return new WaitForSeconds(0.35f);
            CurrentPlayerCards[i].SetActive(true);

            //Player4
            if(Player4Online)
            {
                CardAnimations.Play("Player4CardAnimation");
                GirlAnimations.Play("Player4");
                yield return new WaitForSeconds(0.35f);
                Player4Cards[i].SetActive(true);
            }

            //Player 3
            if(Player3Online)
            {
                CardAnimations.Play("Player3CardAnimation");
                GirlAnimations.Play("Player3");
                yield return new WaitForSeconds(0.35f);
                Player3Cards[i].SetActive(true);
            }

        }
        if(Player2Online)
        {
            Player2CardsStatus.SetActive(true);
        }
        //Player 1
        if(Player1Online)
        {
            Player1cardsStatus.SetActive(true);
        }
        //Current Player
            CurrentPlayerCardsStatus.SetActive(true);
        //Player4
        if(Player4Online)
        {
            Player4CardsStatus.SetActive(true);
        }
        //Player 3
        if(Player3Online)
        {
            Player3CardsStatus.SetActive(true);
        }
        CardAnimations.gameObject.SetActive(false);
        SeeBtn.SetActive(true);
        if(TeenPattiTableManager.Instance.roomData.playersTurns[0]==0){
            TeenPattiActionPanel.Instance.UpdateActionPanelValues();
            TeenPattiTableManager.Instance.actionPanel.SetActive(true);
        }
        else{
            TeenPattiTableManager.Instance.actionPanel.SetActive(false);
        }
        TeenPattiTableManager.Instance.ResetPlayerTurnTime();


    }

    #endregion

}
