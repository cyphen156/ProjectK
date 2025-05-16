using UnityEngine;

public class Camerafollow : MonoBehaviour
{
    private Vector3 offSet; //카메라 기본 위치

    private void Awake()
    {
        offSet = Camera.main.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
       Camera.main.transform.position = transform.position + offSet;
    }
}
