using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public void Move(float inHorizontal, float inVertical)
    {
        transform.position += new Vector3(inHorizontal, 0, inVertical);
    }
}

