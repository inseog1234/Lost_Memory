using UnityEngine;

public class Mush : Monster
{
    protected override void Start()
    {
        base.Start();

        moveSpeed = 0.05f;
        SetType(0);
        SetGold(35);
        SetHome(new Vector2(transform.position.x, transform.position.y));
        SetDamage(6);
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
