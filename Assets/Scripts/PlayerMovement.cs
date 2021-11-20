using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    idle, run, crouch, air, dead
}

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    Animator animCollider, animChar;
    SpriteSwap spriteSwap;

    AudioClip seJump;

    Vector3 movePlayer;
    public PlayerState state = PlayerState.run;

    string strAnimation = "";
    [SerializeField] float flJumpForce = 5;

    public bool blJump = false;

    bool InTheGround()
    {
        return Physics.Linecast(transform.position, transform.position + (Vector3.down * 0.7f));
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        animCollider = GetComponent<Animator>();
        animChar = GameObject.Find("spr_Duck").GetComponent<Animator>();
        spriteSwap = animChar.gameObject.GetComponent<SpriteSwap>();

        seJump = Resources.Load<AudioClip>("Audio/se/se_Jump");
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PlayerState.idle:
                break;

            case PlayerState.run:
                if (Input.GetButtonDown("Up"))
                {
                    vJump();
                }
                else if (Input.GetButton("Down"))
                {
                    vCrouch();
                }

                
                break;

            case PlayerState.crouch:
                if (Input.GetButtonUp("Down"))
                {
                    vChangeState(PlayerState.run);
                }

                
                break;

            case PlayerState.air:
                if ((rb.velocity.y < 0) && (InTheGround()))
                {
                    vChangeState(PlayerState.run);
                }
                break;

            case PlayerState.dead:
                
                break;

            default:
                break;
        }

        rb.velocity = movePlayer;
    }

    private void FixedUpdate()
    {
        movePlayer.y -= 1.4f;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Obstacle":
                vChangeState(PlayerState.dead);
                LevelManager.scr.vLose();
                break;

            default:
                break;
        }
    }

    void vChangeState(PlayerState newState)
    {
        state = newState;

        switch (newState)
        {
            case PlayerState.idle:
                break;

            case PlayerState.run:
                vPlayAnimation("Run");
                animCollider.Play("0");
                spriteSwap.SpriteSheetName = "Sprites/InGame/spr_DuckAlive";
                break;

            case PlayerState.crouch:
                vPlayAnimation("Crouch");
                animCollider.Play("1");
                break;

            case PlayerState.dead:
                animChar.speed = 0;
                rb.isKinematic = true;
                spriteSwap.SpriteSheetName = "Sprites/InGame/spr_DuckDead";
                break;

            case PlayerState.air:
                vPlayAnimation("Jump");
                break;

            default:
                break;
        }
    }

    void vJump()
    {
        GameManager.scr.PlaySE(seJump);
        vChangeState(PlayerState.air);
        movePlayer.y = flJumpForce;
        blJump = true;
        Invoke("vJumpFinish", 0.2f);
    }

    void vJumpFinish()
    {
        blJump = false;
    }

    void vCrouch()
    {
        vChangeState(PlayerState.crouch);
    }

    void vPlayAnimation(string animat)
    {
        if (strAnimation != animat)
        {
            animChar.Play(animat);
            strAnimation = animat;
        }
    }
}
