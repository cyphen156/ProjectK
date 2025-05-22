using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public void Move(float inHorizontal, float inVertical)
    {
        transform.position += new Vector3(inHorizontal, 0, inVertical);
    }

    public void RotateCharacter(Vector3 inDirection)
    {
        transform.LookAt(transform.position + inDirection);
    }
}