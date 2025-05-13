using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerState currentPlayerState; // 현제 플레이어의 애니메이션 상태

    public void AnimationConrtrol(PlayerState state)
    {
        if (currentPlayerState != state)
        {
            currentPlayerState = state;

            switch (currentPlayerState)
            {
                case PlayerState.Idle:
                    StopMoveAnimation();
                    break;
                case PlayerState.Walk:
                    StartMoveAnimation();
                    break;
                case PlayerState.Run:
                    break;
                case PlayerState.Attack:
                    PlayAttackAnimation();
                    break;
                case PlayerState.Reload:
                    PlayReloadAnimation();
                    break;
                case PlayerState.Die:
                    PlayDieAnimation();
                    break;
                default:
                    break;
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayReloadAnimation()
    {
        animator.SetTrigger("Reload");
    }

    public void PlayDieAnimation()
    {
        animator.SetTrigger("Die");
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    public void StartMoveAnimation()
    {
        animator.SetBool("IsMove", true);
    }

    public void StopMoveAnimation()
    {
        animator.SetBool("IsMove", false);
    }
}
