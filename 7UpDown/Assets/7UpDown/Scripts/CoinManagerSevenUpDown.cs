using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManagerSevenUpDown : MonoBehaviour
{
    public int CoinAmount;
    public int Index;
    public Button CoinButton;
    public GameObject Prefeb;

    public bool IsSelected;

    // Start is called before the first frame update
    void Start()
    {
        CoinButton = gameObject.GetComponent<Button>();
        CoinButton.onClick.AddListener(CoinButtonClick);

        if(IsSelected)
        {
            CoinButtonClick();
        }
    }



    public void CoinButtonClick()
    {
        BetManagerSevenUpDown.Instance.CurrentSelectCoins = Prefeb;
        BetManagerSevenUpDown.Instance.CurrentCoinAmount = CoinAmount;

        for (int i = 0; i < BetManagerSevenUpDown.Instance.Coins.Length; i++)
        {
            if(i == Index)
            {
                BetManagerSevenUpDown.Instance.Coins[i].localPosition = new Vector3(BetManagerSevenUpDown.Instance.Coins[i].localPosition.x , -25f ,0);
                BetManagerSevenUpDown.Instance.CoinShadow[i].localPosition = new Vector3(BetManagerSevenUpDown.Instance.CoinShadow[i].localPosition.x , 25f ,0);
                BetManagerSevenUpDown.Instance.CoinShadow[i].gameObject.SetActive(true);
            }
            else
            {
                BetManagerSevenUpDown.Instance.Coins[i].localPosition = new Vector3(BetManagerSevenUpDown.Instance.Coins[i].localPosition.x , -40f ,0);
                BetManagerSevenUpDown.Instance.CoinShadow[i].localPosition = new Vector3(BetManagerSevenUpDown.Instance.CoinShadow[i].localPosition.x , 0f ,0);
                BetManagerSevenUpDown.Instance.CoinShadow[i].gameObject.SetActive(false);

            }
        }
    }

}
