using UnityEngine;

public class Player_Animator_Controller : MonoBehaviour
{
    public Animator animator;
    private Player_Controller player;
    public Player_Distance Attack_Distance;
    public GameObject Attack_Distance_Object;
    public SpriteRenderer render;
    private AnimatorStateInfo stateInfo;
    public CapsuleCollider2D Collider;
    public GameObject Pivot_position;
    public bool combo;
    public bool attacked;
    public bool turn;
    public int last_Dir_C;
    public int last_Dir_C_2;

    public string last_Animation;
    public bool Dash_End = false;

    public float offset_X;
    public float base_X;
    public float position_X;

    public bool Dash_attacking;

    public UI_Manager uI_Manager;
    void Awake()
    {
        offset_X = 0.8f;
        animator = GetComponent<Animator>();
        player = GetComponent<Player_Controller>();
        render = GetComponent<SpriteRenderer>();
        Collider = GetComponent<CapsuleCollider2D>();
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        uI_Manager = GameObject.FindWithTag("UI_Manager").GetComponent<UI_Manager>();
    }

    void Last_Animation_Keep()
    {
        if (animator.GetCurrentAnimatorClipInfo(0).Length > 0 && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != null)
        {
            last_Animation = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        }
    }


    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (player.Move_Sys != Player_Controller.MoveState.Dead) {
            if (!(player.Attack_Sys != Player_Controller.AttackState.Idle || player.Move_Sys == Player_Controller.MoveState.Moving || !player.Jump || player.dash || Dash_attacking))
            {
                player.Idle_Sys = true;
            }
            else
            {
                player.Idle_Sys = false;
            }

            position_X = transform.position.x;

            Move_Anim();
            Attack_Anim();
            Dash_Anim();
            Dash_Atk();

            if (player.Dir_C > 0)
            {
                render.flipX = false;
                Collider.offset = new Vector2(-0.48f, Collider.offset.y);
                if (!turn)
                {
                    last_Dir_C = 1;
                    if (last_Dir_C != last_Dir_C_2)
                    {
                        base_X = position_X + offset_X;
                        transform.position = new Vector2(base_X, transform.position.y);
                        last_Dir_C_2 = 1;
                        turn = true;
                    }
                }
                Pivot_position.transform.position = new Vector2(transform.position.x - 0.35f, Pivot_position.transform.position.y);
                Attack_Distance_Object.transform.position = new Vector2(transform.position.x - 0.5f, Attack_Distance_Object.transform.position.y);
            }
            else if (player.Dir_C < 0)
            {
                render.flipX = true;
                Collider.offset = new Vector2(0.48f, Collider.offset.y);
                if (!turn)
                {
                    last_Dir_C = -1;
                    if (last_Dir_C != last_Dir_C_2)
                    {
                        base_X = position_X - offset_X;
                        transform.position = new Vector2(base_X, transform.position.y);
                        last_Dir_C_2 = -1;
                        turn = true;
                    }
                }
                 Pivot_position.transform.position = new Vector2(transform.position.x + 0.35f, Pivot_position.transform.position.y);
                Attack_Distance_Object.transform.position = new Vector2(transform.position.x + 0.5f, Attack_Distance_Object.transform.position.y);
            }

            if (last_Dir_C != player.Dir_C)
            {
                turn = false;
            }
        }
        else {
            Dead_Anim();
        }
    }

    void Dead_Anim() {
        
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Dead" && stateInfo.normalizedTime % 1.0f >= 0.99f) {
            animator.Play("Player_Dead", 0, 0.99f);
        }
        else {
            animator.Play("Player_Dead");
        }
        Destroy(GetComponent<Player_Controller>(), 0);
        Destroy(GetComponent<Rigidbody2D>(), 0);
        Destroy(GetComponent<CapsuleCollider2D>(), 0);
        Destroy(transform.GetChild(0).gameObject, 0);
        Destroy(transform.GetChild(1).gameObject, 0);
        this.gameObject.tag = "Untagged";
    }

    void Dash_Anim()
    {
        if (player.dash)
        {
            if (player.Attack_Sys == Player_Controller.AttackState.Idle && !Dash_attacking)
            {
                if (player.Dash_speed > player.Dash_speed / 5 * 4)
                {
                    if (last_Animation == "Player_Dash")
                    {
                        if (!Dash_End)
                        {
                            animator.Play("Player_Dash", 0, 0.5f);
                            Dash_End = true;
                        }

                        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Dash" && stateInfo.normalizedTime % 1.0f >= 0.95f)
                        {
                            Dash_End = false;
                        }
                    }
                    else
                    {
                        animator.Play("Player_Dash");
                        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Dash" && stateInfo.normalizedTime % 1.0f >= 0.5f)
                        {
                            Last_Animation_Keep();
                        }
                    }
                }
                else if (player.Jump)
                {
                    if (player.Dir > 0 || player.Dir < 0)
                    {
                        player.dash = false;
                    }
                    else
                    {
                        animator.Play("Player_Dash_Break");
                        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Dash_Break" && stateInfo.normalizedTime % 1.0f >= 0.98f)
                        {
                            player.dash = false;
                        }
                    }
                }
                else if (!player.Jump)
                {
                    player.dash = false;
                    player.Jump_Sys = Player_Controller.JumpState.Jumping;
                }
            }
            else
            {
                Dash_End = false;
                Dash_attacking = true;
                attacked = false;
            }
        }
    }

    void Dash_Atk()
    {
        if (Dash_attacking)
        {
            if (player.Attack_Sys != Player_Controller.AttackState.Idle)
            {
                player.Attack_Sys = Player_Controller.AttackState.Idle;
            }

            animator.Play("Player_Dash_atk");

            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Dash_atk" && stateInfo.normalizedTime % 1.0f >= 0.8f)
            {
                if (!attacked && Attack_Distance.targets != null)
                {
                    player.Damage_C = (player.Damage + Random.Range(0, player.Damage_ran + 1)) * 1.5f;

                    for (int i = 0; i < Attack_Distance.targets.Count; i++)
                    {
                        // uI_Manager.DMG_UI(0, Attack_Distance.targets[i].transform.position.x, Attack_Distance.targets[i].transform.position.y);
                        uI_Manager.FONT_RENDER($"{player.Damage_C}", Attack_Distance.targets[i].transform.position.x, Attack_Distance.targets[i].transform.position.y, 0.5f,  new Vector3(1, 0.4f, 0.4f), 0);
                        Attack_Distance.targets[i].TakeDamage(player.Damage_C);
                    }

                    attacked = true;
                }
            }

            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Dash_atk" && stateInfo.normalizedTime % 1.0f >= 0.95f)
            {
                Dash_attacking = false;
                player.dash = false;
            }
        }
    }

    void Attack_Anim()
    {
        if (Input.GetMouseButtonUp(1)) {
            for (int i = 0; i < Attack_Distance.targets.Count; i++) {
                // Attack_Distance.targets;
            }
        }

        if (player.Attack_Sys != Player_Controller.AttackState.Idle && !player.dash)
        {
            if (player.Attack_Sys == Player_Controller.AttackState.Attacking1)
            {
                animator.Play("Player_atk_1");

                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_atk_1" && stateInfo.normalizedTime % 1.0f >= 0.6f)
                {
                    if (!attacked && Attack_Distance.targets != null)
                    {
                        player.Damage_C = player.Damage + Random.Range(0, player.Damage_ran + 1);

                        for (int i = 0; i < Attack_Distance.targets.Count; i++)
                        {
                            // uI_Manager.DMG_UI(0, Attack_Distance.targets[i].transform.position.x, Attack_Distance.targets[i].transform.position.y);
                            uI_Manager.FONT_RENDER($"{player.Damage_C}", Attack_Distance.targets[i].transform.position.x, Attack_Distance.targets[i].transform.position.y, 0.5f, new Vector3(1, 0.4f, 0.4f), 0);
                            Attack_Distance.targets[i].TakeDamage(player.Damage_C);
                        }
                        attacked = true;
                    }

                    if (player.Attack >= 0.001f && player.Attack <= 0.1f)
                    {
                        player.Attack_Sys = Player_Controller.AttackState.Attacking2;
                        attacked = false;
                    }
                }

                if (player.Attack_Sys == Player_Controller.AttackState.Attacking1 && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_atk_1" && stateInfo.normalizedTime % 1.0f >= 0.95f)
                {
                    Last_Animation_Keep();
                    player.Attack_Sys = Player_Controller.AttackState.Idle;
                    attacked = false;
                }
            }
            else if (player.Attack_Sys == Player_Controller.AttackState.Attacking2)
            {
                animator.Play("Player_atk_2");
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_atk_2" && stateInfo.normalizedTime % 1.0f >= 0.5f)
                {
                    if (!attacked && Attack_Distance.targets != null)
                    {
                        for (int i = 0; i < Attack_Distance.targets.Count; i++)
                        {
                            player.Damage_C *= 1.4f;
                            // uI_Manager.DMG_UI(0, Attack_Distance.targets[i].transform.position.x, Attack_Distance.targets[i].transform.position.y);
                            uI_Manager.FONT_RENDER($"{player.Damage_C}", Attack_Distance.targets[i].transform.position.x, Attack_Distance.targets[i].transform.position.y, 0.5f, new Vector3(1, 0.4f, 0.4f), 0);
                            Attack_Distance.targets[i].TakeDamage(player.Damage_C);
                        }
                        attacked = true;
                    }
                }

                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_atk_2" && stateInfo.normalizedTime % 1.0f >= 0.95f)
                {
                    Last_Animation_Keep();
                    player.Attack_Sys = Player_Controller.AttackState.Idle;
                    attacked = false;
                }
            }
            else
            {
                player.Attack_Sys = Player_Controller.AttackState.Idle;
                player.Attack = 0;
                attacked = false;
            }
        }
    }

    void Jump_Anim()
    {
        if (player.Jump_Sys != Player_Controller.JumpState.None)
        {
            if (player.Jump_Sys == Player_Controller.JumpState.Start)
            {
                animator.Play("Player_Jump_Start");

                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Jump_Start" && stateInfo.normalizedTime % 1.0f >= 0.9f)
                {
                    player.Jump_Sys = Player_Controller.JumpState.Jumping;
                }
            }
            else if (player.Jump_Sys == Player_Controller.JumpState.Jumping)
            {
                animator.Play("Player_Jump");

                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Jump" && stateInfo.normalizedTime % 1.0f >= 0.9f)
                {
                    player.Jump_Sys = Player_Controller.JumpState.Falling;
                }
            }
            else if (player.Jump_Sys == Player_Controller.JumpState.Falling)
            {
                animator.Play("Player_Jump_End");

                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_Jump_End" && stateInfo.normalizedTime % 1.0f >= 0.9f)
                {
                    player.Jump_Sys = Player_Controller.JumpState.Landing;
                }
            }
            else if (player.Jump_Sys == Player_Controller.JumpState.Landing)
            {
                animator.Play("Player_Jump_End_2");
            }

            Last_Animation_Keep();
        }
    }

    void Move_Anim()
    {
        if (!player.Stop_Moving && !player.dash)
        {
            if (player.Attack_Sys == Player_Controller.AttackState.Idle)
            {
                Jump_Anim();
                if (player.Move_Sys == Player_Controller.MoveState.Moving && player.Jump_Sys == Player_Controller.JumpState.None)
                {
                    Collider.size = new Vector2(1.67f, 1.8f);
                    animator.Play("Player_Move");
                    Last_Animation_Keep();
                }
                else
                {
                    Collider.size = new Vector2(1.23f, 2.1f);
                }
            }
        }
        else
        {
            player.Move_Sys = Player_Controller.MoveState.Idle;
            if (!player.dash)
                player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
        }

        if (player.Idle_Sys)
        {
            animator.Play("Player_Idle");
        }
    }
}