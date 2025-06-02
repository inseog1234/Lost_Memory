using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;


public class Player_Distance : MonoBehaviour
{
    public List<Monster> targets;
    public PolygonCollider2D polygonCollider;// {get; private set;}
    public Player_Animator_Controller player;
    public List<float> point_x = new List<float>();
    public List<float> point_y = new List<float>();
    public List<Vector2> point_position = new List<Vector2>();
    
    void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();

        for (int i = 0; i < polygonCollider.points.Count(); i++)
        {
            point_x.Add(polygonCollider.points[i].x);
            point_y.Add(polygonCollider.points[i].y);
        }
    }
    
    void Update()
    {
        if (player.render.flipX == false)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        Monster target = collider.gameObject.GetComponent<Monster>();
        if (target != null) {
            if (!targets.Contains(target)) {
                targets.Add(target);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Monster target = collider.gameObject.GetComponent<Monster>();
        if (target != null) {
            if (targets.Contains(target)) {
                targets.Remove(target);
            }
        }
    }
}
