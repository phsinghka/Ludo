using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoRoute : MonoBehaviour
{
    Transform[] childNodes;
    public List<Transform> childNodeList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        FillChildNodes();
    }

    void FillChildNodes()
    {
        childNodeList.Clear();

        childNodes = GetComponentsInChildren<Transform>();

        foreach(Transform child in childNodes)
        {
            if (child != this.transform)
            {
                childNodeList.Add(child);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        FillChildNodes();
        for(int i = 0; i < childNodeList.Count; i++)
        {
            Vector3 pos = childNodeList[i].position;
            if (i > 0)
            {
                Vector3 prev = childNodeList[i - 1].position;
                Gizmos.DrawLine(prev, pos);
            }
        }
    }
    public int RequestPosition(Transform nodeTransform)
    {
        return childNodeList.IndexOf(nodeTransform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
