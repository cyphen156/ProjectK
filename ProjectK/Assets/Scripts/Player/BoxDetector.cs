using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BoxDetector : NetworkBehaviour
{
    private List<DropBox> inRangeBoxList; //범위 안에 들어온 상자들

    private void Awake()
    {
        inRangeBoxList = new();
    }

    public DropBox GetNearestBox()
    {
        DropBox detectedBox = null;
        float minDisstance = float.MaxValue;
        //상자리스트들 중 가장 가까운애를 반환
        for (int i = 0; i < inRangeBoxList.Count; i++)
        {
            float curDistance = Vector3.Distance(inRangeBoxList[i].transform.position, transform.position);
            if (curDistance < minDisstance)
            {
                detectedBox = inRangeBoxList[i];
                minDisstance = curDistance;
            }
        }
        return detectedBox;
    }

    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log("박스디텍터에서 출입" + other.gameObject.name);
        DropBox box = other.gameObject.GetComponent<DropBox>();
        if(box == null)
        {
            return;
        }
        inRangeBoxList.Add(box);
    }

 
    private void OnTriggerExit(Collider other)
    {
      //  Debug.Log("박스디텍터에서 반출" + other.gameObject.name);
        DropBox box = other.gameObject.GetComponent<DropBox>();
        if (box == null)
        {
            return;
        }
        if (IsOwner)
        {
            box.CloseBox();
        }
        inRangeBoxList.Remove(box);
    }
}
