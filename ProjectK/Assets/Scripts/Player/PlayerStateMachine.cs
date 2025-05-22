using System.Collections;
using UnityEngine;

public enum PlayerState 
{ 
    Idle, 
    Walk,
    Dodge,
    Die 
}


public class PlayerStateMachine : MonoBehaviour
{
    private Animator animator;
    private PlayerState currentPlayerState;
    private bool isStateLock;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        isStateLock = false;
    }

    public PlayerState ChangePlayerState(PlayerState state)
    {
        if (currentPlayerState != state)
        {
            PlayerState previousPlayerState = currentPlayerState;

            currentPlayerState = state;
            Logger.Warning($"PlayerStateChanged :{previousPlayerState} -> {currentPlayerState}");

            switch (currentPlayerState)
            {
                case PlayerState.Idle:
                    SetPlayerAnimatorBool(previousPlayerState, false);
                    break;
                case PlayerState.Walk:
                    SetPlayerAnimatorBool(currentPlayerState, true);
                    break;
                case PlayerState.Die:
                    SetPlayerAnimatorTrigger(currentPlayerState);
                    break;
                default:
                    Logger.Log("FatalErreo :: Tried State Change is Not Allowed");
                    break;
            }
        }
        return currentPlayerState;
    }

    public IEnumerator ChangePlayerStateCoroutine(PlayerState inState, float inDelay)
    {
        isStateLock = true;
        yield return new WaitForSeconds(inDelay);
        SetPlayerAnimatorBool(currentPlayerState, false);
        isStateLock = false;
    }

    public void SetPlayerAnimatorTrigger(PlayerState inState)
    {
        animator.SetTrigger(inState.ToString());
    }

    public void SetPlayerAnimatorBool(PlayerState inState, bool inBoolState)
    {
        animator.SetBool(inState.ToString(), inBoolState);
    }
}
