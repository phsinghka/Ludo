using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSevenUpDown : MonoBehaviour
{

    public GameObject TargetObject, PostionObject;
    public RectTransform TO;
    float TimeInSeconds, distance;
    public bool CoinAnimations = true;
    public bool IsPlayerCoin;

    public RectTransform CenterPoint;
    public bool WinTimeAni;
    Vector3 pos;

    public bool PlayTimeAdd;

    int Speed;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayTimeAdd)
        {
            PlayTimeAdd = false;
            return;
        }

        if (WinTimeAni)
        {
            distance = Vector3.Distance(PostionObject.transform.position, TO.transform.position);
            Invoke("DestroyAnimationCoin", 0.5f);
            pos = new Vector3(TO.rect.x, TO.rect.y, 0) + TO.transform.position;
            Speed = 10;
        }
        else
        {
            distance = Vector3.Distance(PostionObject.transform.position, TargetObject.transform.position);
            Invoke("StopAnimations", 1.2f);
            pos = new Vector3(Random.Range(TO.rect.xMin, TO.rect.xMax), Random.Range(TO.rect.yMin, TO.rect.yMax), 0) + TO.transform.position;
            Speed = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CoinAnimations)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, ((distance / 2) * Time.deltaTime) * Speed);
        }

    }

    void StopAnimations()
    {
        CoinAnimations = false;
    }

    public void MoveToCenterAni()
    {
        
        distance = Vector3.Distance(PostionObject.transform.position, CenterPoint.gameObject.transform.position);
        Invoke("DestroyAnimationCoin", 0.5f);
        pos = new Vector3(CenterPoint.rect.x, CenterPoint.rect.y, 0) + CenterPoint.transform.position;
        CoinAnimations = true;
        Speed = 10;
    }

    public void DestroyAnimationCoin()
    {
        Destroy(gameObject);
    }
}
