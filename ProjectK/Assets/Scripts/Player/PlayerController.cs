using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private PlayerMove playerMove;
    private float inputHorizontal;
    private float inputVertical;

    private void Awake()
    {
        moveSpeed = 5.0f;
        playerMove = GetComponent<PlayerMove>();
    }

    private void Update()
    {



        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        playerMove.Move(inputHorizontal * Time.deltaTime * moveSpeed, inputVertical * Time.deltaTime * moveSpeed);
    }
}
