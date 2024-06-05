using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator player_animator;
    public Player player;
    void Start()
    {

    }

    private void Update()
    {
        if(player.isWalking)
        {
            player_animator.SetBool("IsWalking", true);
        }
        else
        {
            player_animator.SetBool("IsWalking", false);
        }
    }
}
