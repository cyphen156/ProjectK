using System.Collections;
using UnityEngine;

/// <summary>
/// Idle,Walk,Run,Attack,Reload,Aim,Die
/// </summary>
public enum PlayerState 
{ 
    Idle, 
    Walk,
    Run,
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
            currentPlayerState = state;

            switch (currentPlayerState)
            {
                case PlayerState.Idle:
                    SetPlayerAnimatorTrigger(currentPlayerState);
                    break;
                case PlayerState.Walk:
                    SetPlayerAnimatorBool(currentPlayerState, true);
                    break;
                case PlayerState.Run:
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
