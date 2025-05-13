using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public void Move(float horizontal, float vertical)
    {
        transform.position += new Vector3(horizontal, 0, vertical);
    }
}

