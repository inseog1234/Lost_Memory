using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public int itemId;
    public string name;
    public string description;
    public int rank;
    public int type;
    public int type_A;
    public int type_B;
    public int count;
    public int count_lim;
    public bool isEmpty;

    public InventorySlot()
    {
        isEmpty = true;
    }

    public InventorySlot(int itemId, string name, string description, int count, int rank, int type, int type_A, int type_B, int count_lim)
    {
        this.itemId = itemId;
        this.name = name;
        this.description = description;
        this.rank = rank;
        this.type = type;
        this.type_A = type_A;
        this.type_B = type_B;
        this.count = count;
        this.count_lim = count_lim;
        this.isEmpty = false;
    }
}
public class Player_Controller : MonoBehaviour
{
    public enum AttackState { Idle = 0, Attacking1 = 1, Attacking2 = 2 }
    public enum JumpState { None = 0, Start = 1, Jumping = 2, Falling = 3, Landing = 4 }
    public enum MoveState { Idle = 0, Moving = 1, Dead = 2 }

    public float speed;
    public float Dir;
    public int Dir_C;
    public Rigidbody2D rb;
    public Player_Animator_Controller Animator_Controller;
    public bool Jump;
    public float Jump_power;

    public float Attack;
    public bool Stop_Moving;

    public MoveState Move_Sys;
    public JumpState Jump_Sys;
    public AttackState Attack_Sys;
    public bool Idle_Sys;

    public int Damage_ran;
    public float Damage_C;
    public List<GameObject> Target = new List<GameObject>();

    public bool dash;
    public float Dash_speed;
    public int Dash_speed_C;

    public float position_y;

    public float Collision_X;
    public float Collision_Y;

    public int coin;

    public LayerMask coinLayer;
    public float overlapRadius;

    // public int Max_Hp { get; private set; }
    // public int HP { get; private set; }
    // public int Max_ST { get; private set; }
    // public int ST { get; private set; }
    // public int MT { get; private set; }

    public int Max_Hp;
    public int P_Max_Hp;
    public int Hp;
    public int P_Hp;
    public int Max_St;
    public int P_Max_St;
    public int St;
    public int P_St;
    public int C_Max_Hp;
    public int C_Hp;
    public int C_Max_St;
    public int C_St;
    public int MT;
    public int Damage;
    public bool cutScene;
    public bool cutScene_Refs;
    public List<GameObject> coins;
    public bool Attack_Trigger;
    public bool Move_Trigger;
    public List<InventorySlot> inventory = new List<InventorySlot>();
    private Player_Distance player_Distance;
    void Awake()
    {
        speed = 12;
        Jump_power = 60;
        Damage = 5;
        Damage_ran = Damage - Damage / 2;
        Dash_speed = 1;
        Dash_speed_C = 50;
        Max_Hp = 30;
        Hp = Max_Hp;
        C_Max_Hp = Max_Hp + P_Max_Hp;
        C_Hp = C_Max_Hp;
        Max_St = 30;
        St = Max_St;
        C_Max_St = Max_St + P_Max_St;
        C_St = C_Max_St;
        MT = 15;
        rb = GetComponent<Rigidbody2D>();
        Animator_Controller = GetComponent<Player_Animator_Controller>();
        cutScene = false;
        cutScene_Refs = false;
        Attack_Trigger = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Player_Distance>() != null)
            {
                player_Distance = transform.GetChild(i).GetComponent<Player_Distance>();
                break;
            }
            
        }
    }

    public void Healing(int PlusHP)
    {
        Max_Hp += PlusHP;
        Hp = Max_Hp;
    }
    public void Healing_Point(int PlusHP)
    {
        P_Max_Hp += PlusHP;
        P_Hp = P_Max_Hp;
    }
    public void Stealing(int PlusSt)
    {
        Max_St += PlusSt;
        St = Max_St;
    }
    public void Stealing_Point(int PlusSt)
    {
        P_Max_St += PlusSt;
        P_St = P_Max_St;
    }
    public void TakeDameged(int Damaged)
    {
        if (P_Hp > 0)
        {
            P_Hp -= Damaged;
        }
        else
        {
            Hp -= Damaged;
        }

        C_Hp = Hp + P_Hp;

        if (C_Hp <= 0)
        {
            Move_Sys = MoveState.Dead;
        }
    }

    void FixedUpdate()
    {
        if (Hp >= Max_Hp) { C_Hp = Max_Hp; }
        if (P_Hp >= P_Max_Hp) { P_Hp = P_Max_Hp; }
        C_Max_Hp = Max_Hp + P_Max_Hp;
        C_Hp = Hp + P_Hp;

        if (St >= Max_St) { St = Max_St; }
        if (P_St >= P_Max_St) { P_St = P_Max_St; }
        C_Max_St = Max_St + P_Max_St;
        C_St = St + P_St;
    }

    void Cam_Zoom()
    {
        
    }

    void Update()
    {
        if (Move_Sys != MoveState.Dead && !cutScene)
        {
            cutScene_Refs = false;
            DetectCoin();

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                Animator_Controller.last_Dir_C = -Animator_Controller.last_Dir_C;
                if (Dir_C != Animator_Controller.last_Dir_C)
                {
                    Animator_Controller.turn = false;
                }
            }

            if (Attack_Sys == AttackState.Idle)
            {
                if (!dash)
                {
                    Player_Move();
                }
            }
            else
            {
                Move_Sys = MoveState.Idle;
            }

            Player_Attack();
            Dash();
            Cam_Zoom();
        }
        else if (cutScene)
        {
            if (!Attack_Trigger)
            {
                Player_Attack();
            }

            if (Attack_Sys == AttackState.Idle && Move_Trigger)
            {
                if (!dash)
                {
                    Player_Move();
                }
            }
            else
            {
                Move_Sys = MoveState.Idle;
            }

            // if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
            //     Move_Trigger = false;
            // }

            if (!cutScene_Refs)
            {
                Dir = 0;
                rb.linearVelocityX = 0;
                Move_Sys = MoveState.Idle;
                Attack_Sys = AttackState.Idle;
                Jump_Sys = JumpState.None;
                cutScene_Refs = true;
                rb.gravityScale = 5;
            }
        }
    }

    void DetectCoin()
    {
        List<GameObject> toRemove = new List<GameObject>();

        for (int i = 0; i < coins.Count; i++)
        {
            string coin_name = coins[i].name.Replace("(Clone)", "").Trim();
            int b;
            if (int.TryParse(coin_name, out b))
            {
                coin += b;
                toRemove.Add(coins[i]);
            }
        }

        foreach (GameObject c in toRemove)
        {
            Destroy(c, 0.1f);
            coins.Remove(c);
        }
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!dash)
            {
                dash = true;
                Dash_speed = Dash_speed_C;
            }
        }

        if (Dash_speed > 0)
        {
            Dash_speed -= Time.deltaTime * (Dash_speed_C * 4);
            rb.linearVelocity = new Vector2(Dir_C * Dash_speed, 0);
            rb.gravityScale = 0;
        }
        else
        {
            Dash_speed = 0;
            rb.gravityScale = 5;
        }
    }

    void Player_Attack()
    {
        for (int i = 0; i < player_Distance.targets.Count; i++)
        {
            Animator animator = player_Distance.targets[i].GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Enemy_Attack_1" && stateInfo.normalizedTime % 1.0f >= 0.2f && stateInfo.normalizedTime % 1.0f < 0.4f)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    player_Distance.targets[i].GetComponent<Rigidbody2D>().linearVelocityX += 8f * Dir_C;
                }
            }
        }

        var clipInfo = Animator_Controller.animator.GetCurrentAnimatorClipInfo(0);
        if (Attack == 0 && clipInfo.Length > 0 && clipInfo[0].clip.name == null)
        {
            Attack_Sys = AttackState.Idle;
        }

        if (Input.GetMouseButton(0))
        {
            Attack += 1f * Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Attack_Sys != AttackState.Attacking2)
            {
                Attack_Sys = AttackState.Attacking1;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Attack_Trigger = true;
            Attack = 0;
        }
    }

    public void Moving(float x, float y)
    {
        if (Collision_X == 0)
        {
            if (Dir > 1)
            {
                Dir = 1;
            }
            else if (Dir < -1)
            {
                Dir = -1;
            }

            if (Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(x, 0)) > 0.03f)
            {
                if (transform.position.x - x < 0)
                {
                    Dir += Time.deltaTime * 2.5f;
                    Dir_C = 1;
                }
                else
                {
                    Dir -= Time.deltaTime * 2.5f;
                    Dir_C = -1;
                }

                Move_Sys = MoveState.Moving;
                rb.linearVelocity = new Vector2(Dir * speed, rb.linearVelocity.y);
            }
            else
            {
                Move_Sys = MoveState.Idle;

                if (Dir != 0 && Jump)
                {
                    if (Dir >= 0)
                        Dir -= Time.deltaTime * 3f;
                    if (Dir < 0)
                    {
                        Dir = 0;
                    }
                    else
                        Dir += Time.deltaTime * 3f;
                    if (Dir > 0)
                    {
                        Dir = 0;
                    }

                    rb.linearVelocity = new Vector2(Dir * speed / 3 * 2, rb.linearVelocity.y);
                }
                else
                {
                    Dir = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && Jump && transform.position.y <= position_y)
            {
                Jump = false;
                Jump_Sys = JumpState.Jumping;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (Jump_power / 3));
            }

        }
        else if (Collision_X > 0)
        {
            Move_Sys = MoveState.Idle;

            rb.linearVelocity = new Vector2(1 * speed / 5 * 1, rb.linearVelocity.y);
        }
        else
        {
            Move_Sys = MoveState.Idle;
            rb.linearVelocity = new Vector2(-1 * speed / 5 * 1, rb.linearVelocity.y);
        }
    }

    public void Jumping()
    {
        if (Jump && transform.position.y <= position_y)
        {
            Jump = false;
            Jump_Sys = JumpState.Jumping;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (Jump_power / 3));
        }
    }

    void Player_Move()
    {
        if (Collision_X == 0)
        {
            Input.GetAxisRaw("Horizontal");

            if (Dir > 1)
            {
                Dir = 1;
            }
            else if (Dir < -1)
            {
                Dir = -1;
            }

            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    Dir += Time.deltaTime * 2.5f;
                    Dir_C = 1;
                }
                else
                {
                    Dir -= Time.deltaTime * 2.5f;
                    Dir_C = -1;
                }

                Move_Sys = MoveState.Moving;
                rb.linearVelocity = new Vector2(Dir * speed, rb.linearVelocity.y);
            }
            else
            {
                Move_Sys = MoveState.Idle;

                if (Dir != 0 && Jump)
                {
                    if (Dir >= 0)
                        Dir -= Time.deltaTime * 3f;
                    if (Dir < 0)
                    {
                        Dir = 0;
                    }
                    else
                        Dir += Time.deltaTime * 3f;
                    if (Dir > 0)
                    {
                        Dir = 0;
                    }

                    rb.linearVelocity = new Vector2(Dir * speed / 3 * 2, rb.linearVelocity.y);
                }
                else
                {
                    Dir = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && Jump && transform.position.y <= position_y)
            {
                Jump = false;
                Jump_Sys = JumpState.Jumping;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (Jump_power / 3));
            }
        }
        else if (Collision_X > 0)
        {
            Move_Sys = MoveState.Idle;

            rb.linearVelocity = new Vector2(1 * speed / 5 * 1, rb.linearVelocity.y);
        }
        else
        {
            Move_Sys = MoveState.Idle;
            rb.linearVelocity = new Vector2(-1 * speed / 5 * 1, rb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Jump = true;
            Jump_Sys = JumpState.None;
        }

    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.CompareTag("coin"))
        {
            if (!coins.Contains(collider2D.gameObject))
            {
                coins.Add(collider2D.gameObject);
            }
        }

        if (collider2D.gameObject.CompareTag("Item"))
        {
            
            int itemIndex = collider2D.gameObject.GetComponent<Item>().index;

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != null && !inventory[i].isEmpty && inventory[i].itemId == itemIndex && inventory[i].count+1 <= inventory[i].count_lim)
                {
                    inventory[i].count++;
                    Destroy(collider2D.gameObject);
                    return;
                }
            }

            for (int i = 0; i < inventory.Count; i++)
            {
                Item item = collider2D.gameObject.GetComponent<Item>();
                if (inventory[i].isEmpty)
                {
                    inventory[i] = new InventorySlot(itemIndex, item.Name, item.Description, 1, item.Rank, item.Type, item.Type_A, item.Type_B, item.count_lim);
                    Destroy(collider2D.gameObject);
                    return;
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            position_y = transform.position.y;

            Vector2 Contact = other.contacts[0].normal;
            Collision_X = Contact.x;
            Collision_Y = Contact.y;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {

    }
}   
