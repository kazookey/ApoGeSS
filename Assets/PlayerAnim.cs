using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement pm;

    void Start()
    {
        anim = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // If there is movement input → walking
        if (pm.moveInput.sqrMagnitude > 0.01f)
        {
            anim.Play("PlayerWalk");
        }
        else
        {
            anim.Play("PlayerStand");
        }
    }
}