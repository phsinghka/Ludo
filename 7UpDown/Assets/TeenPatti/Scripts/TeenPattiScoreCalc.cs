using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;

public class TeenPattiScoreCalc : MonoBehaviour
{
    //Cards Index 0 is 2 12th Index is A
    public static TeenPattiScoreCalc Instance;
    public Sprite[] cardImages;
    public Sprite cardBg;
    public Text[] cardScoreHolders;
    public Dictionary<string, double> allPossivle =  new Dictionary<string, double>();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DistributeCards(TPConstants.MAX_PLAYERS);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public double GetCardsScoreOnVariation(int[] cards, int joker=-1, TeenPatti.RoomType variation = TeenPatti.RoomType.NO_VARIATION){
        if(variation == TeenPatti.RoomType.AK47){
            return CalculateScore(ConvertAK47(cards));
        }
        else if(variation == TeenPatti.RoomType.JOKER){
            return CalculateScore(ConvertJokers(cards, joker));
        }
        else if(variation == TeenPatti.RoomType.LOWER_JOKER){
            return CalculateScore(ConvertLowerJokers(cards));
        }
        else if(variation == TeenPatti.RoomType.HIGHEST_JOKER){
            return CalculateScore(ConvertHighestJokers(cards));
        }
        else{
            return CalculateScore(cards);
        }
    }

    public int[] GetCardsOnVariation(int[] cards, int joker=-1, TeenPatti.RoomType variation = TeenPatti.RoomType.NO_VARIATION){
        if(variation == TeenPatti.RoomType.AK47){
            return ConvertAK47(cards);
        }
        else if(variation == TeenPatti.RoomType.JOKER){
            return ConvertJokers(cards, joker);
        }
        else if(variation == TeenPatti.RoomType.LOWER_JOKER){
            return ConvertLowerJokers(cards);
        }
        else if(variation == TeenPatti.RoomType.HIGHEST_JOKER){
            return ConvertHighestJokers(cards);
        }
        else{
            return cards;
        }
    }

    public int[] DistributeCards(int numOfPlayers){
        allPossivle.Clear();
        int numOfCards = TPConstants.NUM_OF_CARDS;
        int[] selectedRandomCards = new int[numOfPlayers*numOfCards];
        for (int i = 0; i <selectedRandomCards.Length; i++){
            bool inserted = false;
            while(!inserted){
                int randomNum = Random.Range(0,52);
                int pos = Array.IndexOf(selectedRandomCards, randomNum);
                if(pos<=-1){
                    selectedRandomCards[i]= randomNum;
                    inserted = true;
                }
            }
        }
        // selectedRandomCards[0] = 0;
        // selectedRandomCards[1] = 0;
        // selectedRandomCards[2] = 0;
        // selectedRandomCards[3] = 1;
        // selectedRandomCards[4] = 1;
        // selectedRandomCards[5] = 1;
        // selectedRandomCards[6] = 2;
        // selectedRandomCards[7] = 2;
        // selectedRandomCards[8] = 2;
        // selectedRandomCards[9] = 3;
        // selectedRandomCards[10] = 3;
        // selectedRandomCards[11] = 3;
        // selectedRandomCards[12] = 4;
        // selectedRandomCards[13] = 4;
        // selectedRandomCards[14] = 4;
        return selectedRandomCards;
        // --> Calculate Two Pairs
        
        // for (int i = 0; i <selectedRandomCards.Length; i++){
        //     cardImagesPlaceHolders[i].sprite = cardImages[selectedRandomCards[i]];
        // }
        // for (int i = 0; i <numOfPlayers;i++){
        //     int temp = i*numOfCards;
        //     int[] cards = {selectedRandomCards[temp],selectedRandomCards[temp+1],selectedRandomCards[temp+2]};
        //     cardScoreHolders[i].text = CalculateScore(cards).ToString();
        // }
        
    }
    public double CalculateScore(int [] cards){
        double cardScore = 0;
        int[] color = new int[cards.Length];
        int[] newCards = new int[cards.Length];
        for(int i = 0; i <cards.Length; i++){
            if(cards[i]>38){
                color[i] = 3;
                newCards[i] = cards[i]-39;
            }
            else if(cards[i]>25){
                color[i] = 2;
                newCards[i] = cards[i]-26;
            }
            else if(cards[i]>12){
                color[i] = 1;
                newCards[i] = cards[i]-13;
            }
            else{
                color[i] = 0;
                newCards[i] = cards[i];
            }
        } 
        System.Array.Sort(newCards);
        System.Array.Reverse(newCards);
        //Color 
        if(color[0]==color[1] && color[1]==color[2]){
            cardScore += Math.Pow(10,6);
        }

        //Tripple
        if(newCards[0] == newCards[1] && newCards[1] == newCards[2]){
            cardScore += 
                Math.Pow(10,8)+
                Math.Pow(2, newCards[0]) +
                Math.Pow(2, newCards[1]) +
                Math.Pow(2, newCards[2]);
        }
        //Sequence
        else if(newCards[0] - newCards[1] == 1 && newCards[1] - newCards[2] == 1){
            cardScore += 
                Math.Pow(10,7)+
                Math.Pow(2, newCards[0]) +
                Math.Pow(2, newCards[1]) +
                Math.Pow(2, newCards[2]);
        }
        //Double
        else if(newCards[0] == newCards[1] || newCards[1] == newCards[2]){
            if (newCards[0] == newCards[1]) {
                cardScore += Math.Pow(10,5)+(newCards[0] + 13) * (newCards[1] + 13) + newCards[2];
            } 
            else 
            {
                cardScore += Math.Pow(10,5)+(newCards[1] + 13) * (newCards[2] + 13) + newCards[0];
            }
            
        }
        //High Card
        else{
            cardScore += 
                Math.Pow(2, newCards[0]) +
                Math.Pow(2, newCards[1]) +
                Math.Pow(2, newCards[2]);

        }
        return cardScore;
    }
    //Variations
    public int[] ConvertJokers(int[] cards, int joker){
        if(joker>38){
            joker = joker-39;
        }
        else if(joker>25){
            joker = joker-26;
        }
        else if(joker>12){
            joker = joker-13;
        }
        int[] newJoker = {joker, joker+13, joker+26, joker+39};
        return ConvertCards(cards, newJoker);
    }
    public int[] ConvertAK47(int[] cards){
        int[] newJoker = {2,5,11,12,15,18,24,25,28,31,37,38,41,44,50,51};
        return ConvertCards(cards, newJoker);
    }
    public int[] ConvertLowerJokers(int[] cards){
        int[] newCards = new int[cards.Length];
        for(int i = 0; i <cards.Length; i++){
            if(cards[i]>38){
                newCards[i] = cards[i]-39;
            }
            else if(cards[i]>25){
                newCards[i] = cards[i]-26;
            }
            else if(cards[i]>12){
                newCards[i] = cards[i]-13;
            }
            else{
                newCards[i] = cards[i];
            }
        } 
        System.Array.Sort(newCards);
        
        int[] newJoker = {newCards[0], newCards[0]+13, newCards[0]+26, newCards[0]+39};
        return ConvertCards(cards, newJoker);
    }
    public int[] ConvertHighestJokers(int[] cards){
        int[] newCards = new int[cards.Length];
        for(int i = 0; i <cards.Length; i++){
            if(cards[i]>38){
                newCards[i] = cards[i]-39;
            }
            else if(cards[i]>25){
                newCards[i] = cards[i]-26;
            }
            else if(cards[i]>12){
                newCards[i] = cards[i]-13;
            }
            else{
                newCards[i] = cards[i];
            }
        } 
        System.Array.Sort(newCards);
        System.Array.Reverse(newCards);
        
        int[] newJoker = {newCards[0], newCards[0]+13, newCards[0]+26, newCards[0]+39};
        return ConvertCards(cards, newJoker);
    }
    public int[] ConvertCards(int[] cards, int[] joker){
        
        int count = 0;
        for (int i = 0; i <cards.Length; i++){
            if(Array.IndexOf(joker,cards[i])<=-1){
                count++;
            }
        }
        int[] rcard = new int[count];
        count = 0;
        for (int i = 0; i <cards.Length; i++){
            if(Array.IndexOf(joker,cards[i])<=-1){
                rcard[count] = cards[i];
                count++;
            }
        }
        //Return AAA Because all cards are joker
        if(rcard.Length == 0){
            int[] convertedCards = {12,25,38};
            return convertedCards;
        }
        //Return Trail of that one card which is not joker  
        else if(rcard.Length == 1){
            if(rcard[0]<26){
                int[] convertedCards = {rcard[0], rcard[0] + 13, rcard[0] + 13};
                return convertedCards;
            }
            else{
                int[] convertedCards = {rcard[0], rcard[0] - 13, rcard[0] - 13};
                return convertedCards;
            }
        }
        //Decide Best card to give
        else if (rcard.Length == 2){
            int[] color = new int[rcard.Length];
            int[] newCards = new int[rcard.Length];
            for(int i = 0; i <rcard.Length; i++){
                if(rcard[i]>38){
                    color[i] = 3;
                    newCards[i] = rcard[i]-39;
                }
                else if(rcard[i]>25){
                    color[i] = 2;
                    newCards[i] = rcard[i]-26;
                }
                else if(rcard[i]>12){
                    color[i] = 1;
                    newCards[i] = rcard[i]-13;
                }
                else{
                    color[i] = 0;
                    newCards[i] = rcard[i];
                }
            } 
            System.Array.Sort(newCards);
            System.Array.Reverse(newCards);
            System.Array.Sort(rcard);
            System.Array.Reverse(rcard);
            //Return Trail
            if(newCards[0] == newCards[1]){
                int[] convertedCards = {rcard[0], rcard[1], newCards[0]};
                return convertedCards;
            }
            else if (newCards[0] == 12 && newCards[1] == 0){
                int[] convertedCards = {rcard[0], rcard[1]+1, rcard[1]};
                return convertedCards;
            }
            else if (newCards[0] == 12 && newCards[1] == 1){
                int[] convertedCards = {rcard[0], rcard[1]-1, rcard[1]};
                return convertedCards;
            }
            else if (newCards[0] - newCards[1] == 1){
                if(color[0]== color[1]){
                    if(newCards[0]==12){
                        int[] convertedCards = {rcard[1], rcard[1]-1, rcard[0]};
                        return convertedCards; 
                    }
                    else{
                        int[] convertedCards = {rcard[1], rcard[0]+1, rcard[0]};
                        return convertedCards;
                    }
                }
                else{
                    if(newCards[0]==12){
                        int[] convertedCards = {rcard[1], newCards[1]-1, rcard[0]};
                        return convertedCards; 
                    }
                    else{
                        int[] convertedCards = {rcard[1], newCards[0]+1, rcard[0]};
                        return convertedCards;
                    }
                }
                    
            }
            else if (newCards[0] - newCards[1] == 2){
                int[] convertedCards = {rcard[1], newCards[1]+1, rcard[0]};
                return convertedCards;
            }
            else{
                if (color[0] == color[1]) {
                    if (color[0] == 0){
                        int[] convertedCards = {rcard[0], rcard[1], 12};
                        return convertedCards;
                    }
                    else if (color[0] == 1){
                        int[] convertedCards = {rcard[0], rcard[1], 25};
                        return convertedCards;
                    }
                    else if (color[0] == 2){
                        int[] convertedCards = {rcard[0], rcard[1], 38};
                        return convertedCards;
                    }
                    else if (color[0] == 3){
                        int[] convertedCards = {rcard[0], rcard[1], 51};
                        return convertedCards;
                    }
                }
                else {
                        if (rcard[1] > 39) {
                            int[] convertedCards = {rcard[0], rcard[1], newCards[0] };
                            return convertedCards;
                        }
                        else{
                            int[] convertedCards = {rcard[0], rcard[1], newCards[0] };
                            return convertedCards;
                        }
                }

            }
        }
        else{
            return cards;
        }
        return cards;
    }


    

}
