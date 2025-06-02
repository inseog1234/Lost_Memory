using Unity.VisualScripting;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float Speed;
    public float offset;
    public GameObject Target;
    
    public GameObject player;
    
    private Vector2 border;
    private Vector2 cutSceneTarget;
    private float cutSceneMovS;
    public bool cutScene {get; private set;}
    private bool Lerp;

    public void SetCutScene(bool T) {
        cutScene = T;
    }

    public void CutSceneMoving(Vector2 _position, float _cutSceneMovS, bool _Lerp)
    {
        cutSceneTarget = _position;
        cutSceneMovS = _cutSceneMovS;
        Lerp = _Lerp;
    }

    public void SetBorder(Vector2 position) {
        border = position;
    }

    public void SetPosition(float X, float Y) {
        transform.position = new Vector3(X, Y, transform.position.z);
    }

    void Awake()
    {
        Speed = 3;
        offset = 3;
        
        player = GameObject.FindWithTag("Player");
        border = new Vector2(10, 10);
        cutScene = false;
    }

    void FixedUpdate()
    {
        

        if (!cutScene)
        {
            Camera cam = Camera.main;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 9, 1 * Time.deltaTime);

            if (player.transform.position.x <= border.x)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(border.x, player.transform.position.y + offset, transform.position.z), Speed * Time.deltaTime);
            }
            else if (player.transform.position.x >= border.y)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(border.y, player.transform.position.y + offset, transform.position.z), Speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y + offset, transform.position.z), Speed * Time.deltaTime);
            }
        }
        else
        {
            if (Lerp)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(cutSceneTarget.x, cutSceneTarget.y, -10), cutSceneMovS * Time.deltaTime);
            }
            else
            {
                if (transform.position.x > cutSceneTarget.x)
                {
                    if (transform.position.y > cutSceneTarget.y)
                    {
                        transform.position = new Vector3(transform.position.x - cutSceneMovS * Time.deltaTime, transform.position.y - cutSceneMovS * Time.deltaTime, transform.position.z);
                    }
                    else if (transform.position.y < cutSceneTarget.y)
                    {
                        transform.position = new Vector3(transform.position.x - cutSceneMovS * Time.deltaTime, transform.position.y + cutSceneMovS * Time.deltaTime, transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(transform.position.x - cutSceneMovS * Time.deltaTime, transform.position.y, transform.position.z);
                    }

                }
                else
                {
                    if (transform.position.y > cutSceneTarget.y)
                    {
                        transform.position = new Vector3(transform.position.x + cutSceneMovS * Time.deltaTime, transform.position.y - cutSceneMovS * Time.deltaTime, transform.position.z);
                    }
                    else if (transform.position.y < cutSceneTarget.y)
                    {
                        transform.position = new Vector3(transform.position.x + cutSceneMovS * Time.deltaTime, transform.position.y + cutSceneMovS * Time.deltaTime, transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(transform.position.x + cutSceneMovS * Time.deltaTime, transform.position.y, transform.position.z);
                    }
                }

            }

        }
        
    }
}
