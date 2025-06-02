using UnityEngine;

public class Eyeling : Monster
{
    protected override void Start()
    {
        base.Start();
        
        moveSpeed = 0.05f;
        SetType(1);
        SetGold(5);
        SetHome(new Vector2(transform.position.x, transform.position.y));
        SetDamage(2);
        SetHP(35);
    }

    protected override void Move()
    {
        base.Move();
    }

    protected override void Die()
    {
        base.Die();
    }
}
