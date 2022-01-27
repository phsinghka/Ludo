using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Tranfer Ownership on Pause
//TODO: Access all data again on Focus

namespace TeenPatti{
    public enum RoomType {NO_VARIATION, AK47, JOKER, LOWER_JOKER, HIGHEST_JOKER, FOURXBOOT,MUFLIS};
    // public enum MatchStatus {NOTREADY, READY, INGAME, END};   
    public enum PlayerPosition {POS_1, POS_2, POS_3, POS_4, POS_5};
    public enum PlayerCardStatus {BLIND, SEEN};
    public enum PlayerLastPlay {BOOT, CHAL, PACK};
    public enum RoomStatus {WAITING, READY, INGAME};
    public enum PlayerStatus {WAITING, READY, INGAME};
    public enum DecideWinner {ALL_PACK, POT_LIMIT, SHOW, SIDE_SHOW};
}
public static class TPConstants
{
    //VARIATIONS
    public const string NO_VARIATION = "NO_VARIATION";
    public const string AK47 = "AK47";
    public const string JOKER = "JOKER";
    public const string LOWER_JOKER = "LOWER_JOKER";
    public const string HIGHEST_JOKER = "HIGHEST_JOKER";
    public const string FOURXBOOT = "FOURXBOOT";
    public const string MUFLIS = "MUFLIS";


    //PLAYER CUSTOM PROPERTIES CONSTANTS
    public const string PLAYER_NAME = "PLAYER_NAME";
    public const string AVATAR_ID = "AVATAR_ID";
    public const string PLAY_POINT = "PLAY_POINT";
    public const string BET_TYPE = "BET_TYPE";

    //ROOM CUSTOM PROPERTIES CONSTANTS
    public const string VARIATION_TYPE = "VARIATION_TYPE";
    public const string BOOT_VALUE = "BOOT_VALUE";

    //BET TYPE
    public const string CHAAL = "CHAAL";
    public const string BOOT = "BOOT";
    public const string SHOW = "SHOW";
    public const string SIDE_SHOW = "SIDE_SHOW";
    public const string WIN = "WIN";

    public const string PREVIOUS_ROOM = "PREVIOUS_ROOM";


    public const int MAX_PLAYERS = 5;
    public const int PLAYER_TTL = 15;
    public const int COMMISSION = 10 ;
    public const int BLIND_COUNT = 4;
    public const int NUM_OF_CARDS = 3;
    public const int CARDS_COUNT = 52;
    public const int TEEN_PATTI_SCENE = 1;
    public const int MAIN_MENU_SCENE = 0;
    public const float PLAYER_WAITING_TIME = 10;
    public const float PLAYER_TURN_WATING_TIME = 15;
    public const float MATCH_RESTART_TIME = 5f;
    public const float DELAY_TIME = 1f;
}
