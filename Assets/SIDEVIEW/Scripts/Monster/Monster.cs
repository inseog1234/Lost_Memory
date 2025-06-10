using System.Collections.Generic;
using UnityEngine;


public class Monster : MonoBehaviour
{
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    public int coin_count { get; private set; }
    private GameObject player;
    public float Coin_posi_X;
    public float Coin_posi_Y;

    [Header("Monster Stats")]
    public float maxHP = 100f;
    public float currentHP;
    public float moveSpeed;
    private int Damage;
    public int Type;

    public Vector2 home;
    public enum Monster_State {
        Idle, Move, Attack_1, Attack_2, Damaged, Dead
    }

    public Monster_State monster = Monster_State.Idle;
    public List<GameObject> coin;
    private SpriteRenderer Renderer;
    private bool once_Damaged_Animation;
    private bool Attacked;
    private bool Attacking;
    private float DeadTime;
    public bool cutScene;

    public void SetType(int _type)
    {
        Type = _type;
    }
    public void SetGold(int Gold) {
        coin_count = Gold;
    }
    public void SetHome(Vector2 newPosition) {
        home = newPosition;
    }
    public void SetDamage(int Damage_) {
        Damage = Damage_;
    }
    public void SetHP(int HP_) {
        maxHP = HP_;
        currentHP = maxHP;
    }
    public bool Moving(Vector2 Target_, float Move_Speed)
    {
        float Dis = Vector2.Distance(transform.position, Target_);
        if (Dis > 0.1f)
        {
            monster = Monster_State.Move;
            animator.speed = 1;
            if (Target_.x - transform.position.x > 0)
            {
                transform.position = new Vector2(transform.position.x + Move_Speed * 600 * Time.deltaTime, transform.position.y);
                Flip_X(false);
            }
            else if (Target_.x - transform.position.x < 0)
            {
                transform.position = new Vector2(transform.position.x - Move_Speed * 600 * Time.deltaTime, transform.position.y);
                Flip_X(true);
            }
            return true;
        }
        else
        {
            monster = Monster_State.Idle;
            animator.speed = 1;
            return false;
        }

    }

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        Renderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player");

        Coin_posi_X = 15;
        Coin_posi_Y = 25;

        home = new Vector2(0, 1);
    }

    void FixedUpdate()
    {
        Logic();
    }
    protected virtual void Update() {
        Move();
        Animation();
    }
    void Coin_Sys(bool Start) {
        if (Start && coin_count > 0)
        {
            GameObject coin_I = null;

            if (coin_count - 50 >= 0)
            {
                coin_I = Instantiate(this.coin[4].gameObject);
                coin_count -= 50;
            }
            else if (coin_count - 20 >= 0)
            {
                coin_I = Instantiate(this.coin[3].gameObject);
                coin_count -= 20;
            }
            else if (coin_count - 10 >= 0)
            {
                coin_I = Instantiate(this.coin[2].gameObject);
                coin_count -= 10;
            }
            else if (coin_count - 5 >= 0)
            {
                coin_I = Instantiate(this.coin[1].gameObject);
                coin_count -= 5;
            }
            else if (coin_count - 1 >= 0)
            {
                coin_I = Instantiate(this.coin[0].gameObject);
                coin_count -= 1;
            }

            coin_I.transform.position = this.transform.position;
            Rigidbody2D Coin_rb = coin_I.GetComponent<Rigidbody2D>();

            Vector2 Coin_posi = new Vector2(Random.Range(-Coin_posi_X, Coin_posi_X), Random.Range(Coin_posi_Y / 2f, Coin_posi_Y) - 1);
            Coin_rb.AddForce(Coin_posi);
            
        }
    }

    float Flip_X(bool Left) {
        if (Left) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        return transform.rotation.y;
    }

    float Flip_X() {
        return transform.rotation.y;
    }

    protected virtual void Logic() {
        switch(Type) {
            case 0:
                if (monster != Monster_State.Damaged && monster != Monster_State.Dead) {
                    float Distance = 0;
                    if (Flip_X() == -1) {
                        Distance = (player.GetComponent<SpriteRenderer>().flipX == false) ? Vector2.Distance(new Vector2(transform.position.x, 0),  new Vector2(player.transform.position.x, 0)) + 0.5f : Vector2.Distance(new Vector2(transform.position.x, 0),  new Vector2(player.transform.position.x, 0));
                        
                    }
                    else {
                        Distance = (player.GetComponent<SpriteRenderer>().flipX == false) ? Vector2.Distance(new Vector2(transform.position.x, 0),  new Vector2(player.transform.position.x, 0)) : Vector2.Distance(new Vector2(transform.position.x, 0),  new Vector2(player.transform.position.x, 0)) + 0.8f;
                    }

                    if (Distance <= 2f)
                    {
                        monster = Monster_State.Attack_1;
                        Attacked = false;
                    }
                    else if (monster != Monster_State.Attack_1 && monster != Monster_State.Attack_2)
                    {

                        if (!cutScene)
                        {
                            if (Distance <= 8)
                            {
                                monster = Monster_State.Move;
                                animator.speed = 1;
                                if (player.transform.position.x - transform.position.x > 0)
                                {
                                    transform.position = new Vector2(transform.position.x + moveSpeed, transform.position.y);
                                    Flip_X(false);
                                }
                                else if (player.transform.position.x - transform.position.x < 0)
                                {
                                    transform.position = new Vector2(transform.position.x - moveSpeed, transform.position.y);
                                    Flip_X(true);
                                }
                            }
                            else if (Distance > 8)
                            {
                                if (Vector2.Distance(new Vector2(home.x, 0), new Vector2(transform.position.x, 0)) >= 0.3f)
                                {
                                    animator.speed = 0.9f;
                                    if (home.x - transform.position.x > 0)
                                    {
                                        transform.position = new Vector2(transform.position.x + moveSpeed / 3 * 2, transform.position.y);
                                        Flip_X(false);
                                    }
                                    else if (home.x - transform.position.x < 0)
                                    {
                                        transform.position = new Vector2(transform.position.x - moveSpeed / 3 * 2, transform.position.y);
                                        Flip_X(true);
                                    }
                                }
                                else
                                {
                                    animator.speed = 1;
                                    monster = Monster_State.Idle;
                                }
                            }
                        }
                        else
                        {
                            Attacked = true;
                        }
                    }
                    else
                    {
                        Attacked = true;
                    }
                }

                break;
            case 1:
                if (monster != Monster_State.Damaged && monster != Monster_State.Dead)
                {
                    float Distance = Vector2.Distance(transform.position, player.transform.position);

                    if (Distance <= 2f)
                    {
                        monster = Monster_State.Attack_1;
                        Attacked = false;
                    }
                    else if (monster != Monster_State.Attack_1 && monster != Monster_State.Attack_2)
                    {
                        Attacked = true;
                        if (Distance <= 8)
                        {
                            monster = Monster_State.Move;
                            animator.speed = 1;

                            Vector2 direction = (player.transform.position - transform.position).normalized;
                            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed);

                            if (player.transform.position.x - transform.position.x > 0)
                            {
                                Flip_X(false);
                            }
                            else if (player.transform.position.x - transform.position.x < 0)
                            {
                                Flip_X(true);
                            }
                        }
                        else if (Distance > 8)
                        {
                            float homeDistance = Vector2.Distance(home, transform.position);

                            if (homeDistance >= 0.3f)
                            {
                                animator.speed = 0.9f;
                                Vector2 direction = (home - new Vector2(transform.position.x, transform.position.y)).normalized;
                                transform.position = Vector2.MoveTowards(transform.position, home, moveSpeed * (2f / 3f));

                                if (home.x - transform.position.x > 0)
                                {
                                    Flip_X(false);
                                }
                                else if (home.x - transform.position.x < 0)
                                {
                                    Flip_X(true);
                                }
                            }
                            else
                            {
                                animator.speed = 1;
                                monster = Monster_State.Idle;
                            }
                        }
                    }
                    else
                    {

                        Attacked = true;
                    }
                }
                break;
            default:
                break;
        }
    }

    protected virtual void Animation() {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        switch(monster) {
            case Monster_State.Idle:
                animator.Play("Enemy_Idle");
                Attacking = false;
                break;
            case Monster_State.Move:
                if (Type == 0) {
                    animator.Play("Enemy_Walk");
                }
                else if (Type == 1) {
                    animator.Play("Enemy_Idle");
                }
                else {
                    animator.Play("Enemy_Walk");
                }
                break;
            case Monster_State.Attack_1:
                animator.Play("Enemy_Attack_1");
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Enemy_Attack_1" && stateInfo.normalizedTime % 1.0f >= 0.8367f && !Attacked && !Attacking) {
                    player.GetComponent<Player_Controller>().TakeDameged(Damage);
                    Attacking = true;
                }
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Enemy_Attack_1" && stateInfo.normalizedTime % 1.0f >= 0.99f) {
                    monster = Monster_State.Idle;
                    // if (Type == 1) {
                    //     Debug.Log("aa");
                    // }
                    
                }
                else {
                    player.GetComponent<Player_Controller>();
                }
                break;
            case Monster_State.Attack_2:
                animator.Play("Enemy_Attack_2");
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Enemy_Attack_2" && stateInfo.normalizedTime % 1.0f >= 0.99f && !Attacked && !Attacking) {
                    player.GetComponent<Player_Controller>().TakeDameged(Damage);
                    Attacking = true;
                }
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Enemy_Attack_2" && stateInfo.normalizedTime % 1.0f >= 0.99f) {
                    monster = Monster_State.Idle;
                }
                break;
            case Monster_State.Damaged:
                if (once_Damaged_Animation) {
                    animator.Play("Enemy_Damaged", 0, 0);
                    once_Damaged_Animation = false;
                }   
                else {
                    animator.Play("Enemy_Damaged");
                }
                
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Enemy_Damaged" && stateInfo.normalizedTime % 1.0f >= 0.99f) {
                    monster = Monster_State.Idle;
                }
                break;
            case Monster_State.Dead:
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Enemy_Dead") {
                    if (stateInfo.normalizedTime % 1.0f >= 0.99f) {
                        animator.Play("Enemy_Dead", 0, 0.99f);
                        Die();
                    }
                }
                else {
                    animator.Play("Enemy_Dead");
                }

                break;
        }
    }

    protected virtual void Move() {
        // 기본 이동 로직
        // transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    public virtual void TakeDamage(float damage) {
        currentHP -= damage;
        animator.StopPlayback();
        if (currentHP <= 0)
        {
            monster = Monster_State.Dead;
        }
        else {
            monster = Monster_State.Damaged;
            once_Damaged_Animation = true;
        }
    }

    protected virtual void Die() {
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());
        Coin_Sys(true);
        DeadTime += Time.deltaTime;
        
        if (Renderer.color.a >= 0 && DeadTime >= 5) {
            Renderer.color = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, Renderer.color.a - Time.deltaTime);
        }

        if (coin_count <= 0 && DeadTime >= 5) {
            Destroy(gameObject, 0);
        }
        
    }
}
