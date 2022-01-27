using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetManagerSevenUpDown : MonoBehaviour
{
    public static BetManagerSevenUpDown Instance;
    public GameObject CurrentSelectCoins;
    public int CurrentCoinAmount;

    public GameObject PlayerPostion , OtherPostion;

    public RectTransform[] Coins , CoinShadow; 

    void Start()
    {
        Instance = this;
    }
}
