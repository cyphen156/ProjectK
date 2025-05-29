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

    public void ChangePlayerState(PlayerState state)
    {
        if (isStateLock || currentPlayerState == state)
        {
            return;
        }
        PlayerState previousPlayerState = currentPlayerState;

        currentPlayerState = state;

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
            case PlayerState.Dodge:
                SetPlayerAnimatorTrigger(currentPlayerState);
                StartCoroutine(ChangePlayerStateCoroutine(currentPlayerState, 0.8f));
                break;
            default:
                Logger.Log("FatalErreo :: Tried State Change is Not Allowed");
                break;
        }
    }

    public IEnumerator ChangePlayerStateCoroutine(PlayerState inState, float inDelay)
    {
        isStateLock = true;
        yield return new WaitForSeconds(inDelay);
        isStateLock = false;

        if (currentPlayerState == inState)
        {
            SetPlayerAnimatorBool(PlayerState.Walk, false);
            ChangePlayerState(PlayerState.Idle);
        }
    }
    public void SetPlayerAnimatorTrigger(PlayerState inState)
    {
        animator.SetTrigger(inState.ToString());
    }

    public void SetPlayerAnimatorBool(PlayerState inState, bool inBoolState)
    {
        animator.SetBool(inState.ToString(), inBoolState);
    }
    public PlayerState GetCurrentPlayerState()
    {
        return currentPlayerState;
    }
}
