using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    private GameObject Player;
    private Player_Controller player_Controller;
    private GameObject Cam;
    private Cam Cam_S;
    public float cutScene_Time;
    // public float cutScene_Time_first;
    public bool iscutScene;
    public bool iscutSceneEnd;
    public bool cutSceneMod;

    public Image Up;
    public Image Down;
    public Image Pade;
    public GameObject canvas;
    public float Base;
    public float min;
    public float max;
    public int cutSceneTime;
    public GameObject boat;
    public bool Trigger;
    public int Trigger_2;
    public bool Trigger_3;
    public int Trigger_4;
    public float Trigger_5;
    private UI_Manager uI_Manager;
    private Item_Manager item_Manager;
    private float Skip_Time;
    public int cut_Level;
    private float F_Cam_Size;
    public GameObject Said_1;
    public GameObject Said_2;
    public bool Basic_Game_Kit;
    [SerializeField] public List<GameObject> Monsters;
    public List<GameObject> Monsters_OBJ;
    public List<Monster> Monsters_Sc;
    public Data_Manager data_Manager;
    public float Game_Time;
    public bool Set;
    public bool Set_2;
    public GameObject Target_monster;
    public bool Loading;
    public bool Data_Load;
    public bool Data_Loading;
    void Start()
    {
        canvas = GameObject.FindWithTag("Main_Canvas");
        Player = GameObject.FindWithTag("Player");
        player_Controller = Player.GetComponent<Player_Controller>();
        Cam = GameObject.FindWithTag("MainCamera");
        Cam_S = Cam.GetComponent<Cam>();
        Up = GameObject.FindWithTag("cinematic_Up").GetComponent<Image>();
        Down = GameObject.FindWithTag("cinematic_Down").GetComponent<Image>();
        Pade = GameObject.FindWithTag("Map_Pade").GetComponent<Image>();
        uI_Manager = GameObject.FindWithTag("UI_Manager").GetComponent<UI_Manager>();
        item_Manager = GameObject.FindWithTag("Item_Manager").GetComponent<Item_Manager>();
        data_Manager = GameObject.FindWithTag("Data_Manager").GetComponent<Data_Manager>();
        cutSceneMod = true;

        Base = 0.5f;
        min = 0.1f;
        max = 2.4f;
        cutSceneTime = 5;
        cut_Level = 0;

        F_Cam_Size = Camera.main.orthographicSize;

        // CreateMonster(Monsters[2], new Vector2(-2, -0.4f));
        //data_Manager.Load(data_Manager.Index);
    }

    void CutScene_1()
    {
        if (cutScene_Time == 0)
        {
            Cam_S.SetPosition(-44f, 0);
            Player.transform.position = new Vector3(-60.35f, 0, 0);
            boat.transform.position = new Vector3(-61, -2, 0);
        }
        else if (cutScene_Time <= 18)
        {
            if (boat.transform.position.x < -27.8f)
            {
                boat.transform.position = new Vector3(boat.transform.position.x + 2 * Time.deltaTime, boat.transform.position.y, boat.transform.position.z);
                if (Trigger_2 == 0 && (int)cutScene_Time == 5)
                {
                    uI_Manager.Said("OoOop....", Player.transform.position.x, Player.transform.position.y + 1, 1f, new Vector3(255, 0, 0), 0.05f, 2f, 0.01f, 0.05f, true);
                    Trigger_2 = 1;
                }
                else if (Trigger_2 == 1 && (int)cutScene_Time == 10)
                {
                    uI_Manager.Said("x_x...", Player.transform.position.x, Player.transform.position.y + 1, 1f, new Vector3(255, 0, 0), 0.05f, 2f, 0.01f, 0.05f, true);
                    Trigger_2 = 2;
                }
            }
            else
            {
                boat.transform.position = new Vector3(-27.8f, boat.transform.position.y, boat.transform.position.z);
            }

            if (boat.transform.position.x >= -43)
            {
                Cam_S.CutSceneMoving(new Vector2(boat.transform.position.x, 0), 1f, true);
            }
        }
        else if (cutScene_Time <= 25)
        {
            if (Player.transform.position.x < -21.8f)
            {
                player_Controller.Moving(-22, 0);
            }
            else
            {
                if (player_Controller.Jump_Sys == Player_Controller.JumpState.None)
                {
                    player_Controller.Moving(Player.transform.position.x, 0);
                    if (Trigger_2 == 3 && (int)cutScene_Time == 21)
                    {
                        uI_Manager.Said("What..The..", Player.transform.position.x, Player.transform.position.y + 1, 1f, new Vector3(255, 255, 255), 0.15f, 1.5f, 0f, 0f, true);
                        Trigger_2 = 4;
                    }
                }
            }

            if (Player.transform.position.x >= -27 && !Trigger)
            {
                if (Trigger_2 == 2)
                {
                    uI_Manager.Said("hppp..!!", Player.transform.position.x, Player.transform.position.y + 1, 1f, new Vector3(255, 255, 255), 0.03f, 0.6f, 0f, 0f, true);
                    Trigger_2 = 3;
                }
                player_Controller.Jumping();
                Trigger = true;
            }
            else if (Player.transform.position.x < -27 && Trigger)
            {
                Trigger = false;
            }

            Cam_S.CutSceneMoving(Player.transform.position, 1f, true);
        }
        else if (cutScene_Time <= 30)
        {
            iscutSceneEnd = true;
            cut_Level = 1;
            Trigger = false;
            Trigger_2 = 0;
            Trigger_3 = false;
        }
    }

    void Tutorial_1()
    {
        if (cutScene_Time == 0)
        {
            Said_1 = uI_Manager.Said("Pressed the {A} or {D} Key to Move", Cam.transform.position.x, Cam.transform.position.y + 2, 1f, new Vector3(255, 255, 255), 0.03f, 4f, 0f, 0f, true);
            player_Controller.Move_Trigger = true;
            canvas.SetActive(true);
            Cam_S.CutSceneMoving(new Vector2(-15, Player.transform.position.y + 3f), 1f, true);
        }
        else if (cutScene_Time > 3)
        {
            Cam_S.CutSceneMoving(new Vector2(-15, Player.transform.position.y + 3f), 1f, true);
            if (Trigger_2 == 0)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    Trigger_4 = 1;
                    Trigger_2 = 1;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    Trigger_4 = 2;
                    Trigger_2 = 1;
                }

                for (int i = 0; i < Said_1.transform.childCount; i++)
                {
                    Font_Said font_Said = Said_1.transform.GetChild(i).GetComponent<Font_Said>();
                    font_Said.timer = 3.5f;
                }
            }
            if (Trigger_2 == 1)
            {
                if ((Trigger_4 == 1 && Input.GetKeyDown(KeyCode.D)) || (Trigger_4 == 2 && Input.GetKeyDown(KeyCode.A)))
                {
                    Trigger_2 = 2;
                    Trigger = false;
                }

                for (int i = 0; i < Said_1.transform.childCount; i++)
                {
                    Font_Said font_Said = Said_1.transform.GetChild(i).GetComponent<Font_Said>();
                    font_Said.timer = 3.5f;
                }
            }
            if (Trigger_2 == 2)
            {
                if (!Trigger)
                {
                    Said_1 = uI_Manager.Said("Pressed the {Space} Key to Jump", Cam.transform.position.x, Cam.transform.position.y + 2, 1f, new Vector3(255, 255, 255), 0.03f, 4f, 0f, 0f, true);
                    Trigger = true;
                    Trigger_5 = cutScene_Time;
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Trigger_2 = 3;
                        canvas.SetActive(false);
                        player_Controller.Move_Trigger = false;
                    }

                    if (cutScene_Time > Trigger_5 + 3.5f)
                    {
                        for (int i = 0; i < Said_1.transform.childCount; i++)
                        {
                            Font_Said font_Said = Said_1.transform.GetChild(i).GetComponent<Font_Said>();
                            font_Said.timer = 3.5f;
                        }
                    }

                }


            }

            if (Trigger_2 == 3)
            {
                if (Vector2.Distance(new Vector2(Player.transform.position.x, 0), new Vector2(-20f, 0)) >= 0.2f)
                {
                    player_Controller.Moving(-20, 0);
                }
                else
                {
                    player_Controller.Moving(Player.transform.position.x, 0);
                    iscutSceneEnd = true;
                    cut_Level = 2;
                    Trigger = false;
                    Trigger_2 = 0;
                    Trigger_3 = false;
                    Trigger_4 = 0;
                    Trigger_5 = 0;

                }
            }
        }
        else if (Said_1 != null)
        {
            for (int i = 0; i < Said_1.transform.childCount; i++)
            {
                Font_Said font_Said = Said_1.transform.GetChild(i).GetComponent<Font_Said>();
                font_Said.timer = 3.5f;
            }
        }
    }

    void Tutorial_2()
    {
        if (cutScene_Time == 0)
        {
            player_Controller.Dir_C = -1;
            Player.transform.position = new Vector2(-20, Player.transform.position.y);
        }
        else if (cutScene_Time <= 4.5f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 6f, 2f * Time.deltaTime);
            Cam_S.CutSceneMoving(new Vector2(Player.transform.position.x, Player.transform.position.y + 3f), 1f, true);
            
            if (Trigger_2 == 0 && (int)cutScene_Time == 3)
            {
                Said_1 = uI_Manager.Said("Pressed the {Tap} key", Cam.transform.position.x, Cam.transform.position.y + 2, 1f, new Vector3(255, 255, 255), 0.05f, 4f, 0f, 0f, true);
                player_Controller.Inventory_Trigger = true;
                Trigger_2 = 1;
            }
            if (Trigger_2 == 1 && cutScene_Time >= 3.5)
            {
                Said_2 = uI_Manager.Said("Open your inventory!", Cam.transform.position.x, Cam.transform.position.y + 0.6f, 1f, new Vector3(255, 255, 255), 0.05f, 4f, 0f, 0f, true);
                Trigger_2 = 2;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!Trigger)
                {
                    Trigger = true;
                    canvas.SetActive(true);
                }
                else
                {
                    bool it = false;

                    for (int i = 0; i < player_Controller.inventory.Count; i++)
                    {
                        if (player_Controller.inventory[i].itemId == 0 && !player_Controller.inventory[i].isEmpty)
                        {
                            it = true;
                            break;
                        }
                    }

                    if (!it && !player_Controller.inventory[55].isEmpty)
                    {
                        iscutSceneEnd = true;
                        cut_Level = 3;
                        Trigger = false;
                        Trigger_2 = 0;
                    }
                    else
                    {
                        iscutSceneEnd = true;
                        cut_Level = 2;
                        Trigger = false;
                        Trigger_2 = 0;
                    }
                }
            }

            if (!Trigger && Trigger_2 == 2 && Said_1 != null)
            {
                canvas.SetActive(false);
                for (int i = 0; i < Said_1.transform.childCount; i++)
                {
                    Font_Said font_Said = Said_1.transform.GetChild(i).GetComponent<Font_Said>();
                    font_Said.timer = 3f;
                }

                for (int i = 0; i < Said_2.transform.childCount; i++)
                {
                    Font_Said font_Said = Said_2.transform.GetChild(i).GetComponent<Font_Said>();
                    font_Said.timer = 3f;
                }
            }
        }
    }

    void Tutorial_5()
    {

    }

    void Tutorial_3()
    {
        if (cutScene_Time == 0)
        {
            player_Controller.Dir_C = -1;
            Player.transform.position = new Vector2(-20, Player.transform.position.y);
            canvas.SetActive(false);
        }
        else if (cutScene_Time <= 2)
        {

        }
        else if (cutScene_Time <= 4)
        {
            player_Controller.Dir_C = 1;

            if (Trigger_2 == 0)
            {
                Said_1 = uI_Manager.Said("!!!", Player.transform.position.x + 1.5f, Player.transform.position.y + 1.5f, 1f, new Vector3(255, 0, 0), 0.05f, 7f, 0.05f, 0.1f, true);
                Trigger_2 = 1;
            }
        }
        else if (cutScene_Time <= 6.5)
        {
            if (!Trigger)
            {
                Target_monster = CreateMonster(Monsters[2], new Vector2(-2, -0.4f));
                Trigger = true;
            }
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 9f, 2f * Time.deltaTime);
            Cam_S.CutSceneMoving(new Vector2(-11, 3.75f), 1f, true);
        }
        else if (cutScene_Time <= 7)
        {
            if (Trigger_2 == 1)
            {
                Said_1 = uI_Manager.Said(">.<", Target_monster.transform.position.x - 1.5f, Target_monster.transform.position.y - 0.5f, 1f, new Vector3(50, 50, 255), 0.0f, 2f, 0.01f, 0.03f, true);
                Trigger = false;
                Trigger_2 = 2;
            }
        }
        else if (cutScene_Time <= 30 && !Trigger)
        {
            Target_monster.GetComponent<Monster>().cutScene = true;
            bool isMoving = Target_monster.GetComponent<Monster>().Moving(new Vector2(-18, Target_monster.transform.position.y), 0.006f);

            if (!isMoving)
            {
                if (!Trigger)
                {
                    Target_monster.GetComponent<Animator>().speed = 0;
                    Player.GetComponent<Animator>().speed = 0;
                    Trigger = true;
                    Trigger_2 = 0;
                }
            }
            else
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 5f, 1f * Time.deltaTime);
                Cam_S.CutSceneMoving(new Vector2(-17, 1.6f), 0.8f, true);
            }
        }
        else if (Trigger)
        {
            if (Trigger_2 == 0)
            {
                Said_1 = uI_Manager.Said("Pressed {LeftMouseButton} key to Attack", Cam.transform.position.x, Cam.transform.position.y + 1, 0.5f, new Vector3(255, 255, 255), 0.03f, 4f, 0f, 0f, true);
                player_Controller.Attack_Trigger = false;
                Trigger_2 = 1;
            }

            if (Trigger_2 == 1)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Trigger_2 = 2;
                }
                else
                {
                    Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Mathf.Lerp(Pade.color.a, 0.4f, 1f * Time.deltaTime));

                    for (int i = 0; i < Said_1.transform.childCount; i++)
                    {
                        Said_1.transform.GetChild(i).GetComponent<Font_Said>().timer = 3.5f;
                    }
                }
            }
            else
            {
                Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Mathf.Lerp(Pade.color.a, 0.4f, 1f * Time.deltaTime));
            }

            if (Trigger_2 == 2)
            {
                Animator animator = Player.GetComponent<Animator>();
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player_atk_1" && stateInfo.normalizedTime % 1.0f >= 0.64f)
                {
                    if (!Trigger_3)
                    {
                        player_Controller.Attack_Trigger = false;
                        Said_1 = uI_Manager.Said("Pressed {LeftMouseButton} key to Combo Attack!!", Cam.transform.position.x, Cam.transform.position.y + 1, 0.5f, new Vector3(255, 255, 255), 0.03f, 4f, 0f, 0f, true);
                        Trigger_3 = true;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        Target_monster.GetComponent<Animator>().speed = 1;
                        Player.GetComponent<Animator>().speed = 1;
                        Trigger_2 = 3;
                        Target_monster.GetComponent<Rigidbody2D>().linearVelocityX += 2;
                    }
                    else
                    {
                        Target_monster.GetComponent<Animator>().speed = 0;
                        Player.GetComponent<Animator>().speed = 0;
                        Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Mathf.Lerp(Pade.color.a, 0.4f, 1f * Time.deltaTime));

                        for (int i = 0; i < Said_1.transform.childCount; i++)
                        {
                            Said_1.transform.GetChild(i).GetComponent<Font_Said>().timer = 3;
                        }
                    }
                }
                else
                {
                    Target_monster.GetComponent<Animator>().speed = 1;
                    Player.GetComponent<Animator>().speed = 1;
                    Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Mathf.Lerp(Pade.color.a, 0f, 1f * Time.deltaTime));
                }
            }

            if (Trigger_2 == 3) {
                if (player_Controller.Attack_Sys == Player_Controller.AttackState.Idle) {
                    iscutSceneEnd = true;
                    cut_Level = 4;
                    Trigger = false;
                    Trigger_2 = 0;
                    Trigger_3 = false;
                }
            }

        }
    }

    void Tutorial_4()
    {
        if (Trigger_2 == 0)
        {
            Animator animator = Target_monster.GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Enemy_Attack_1" && stateInfo.normalizedTime % 1.0f >= 0.2f)
            {
                if (!Trigger)
                {
                    Said_1 = uI_Manager.Said("Pressed {RightMouseButton} key to Block!!", Cam.transform.position.x, Cam.transform.position.y + 1, 0.5f, new Vector3(255, 255, 255), 0.03f, 4f, 0f, 0f, true);
                    Trigger = true;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    Target_monster.GetComponent<Rigidbody2D>().linearVelocityX += 8;
                    Target_monster.GetComponent<Animator>().speed = 1;
                    Target_monster.GetComponent<Monster>().cutScene = false;
                    Player.GetComponent<Animator>().speed = 1;
                    Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Mathf.Lerp(Pade.color.a, 0f, 1f * Time.deltaTime));
                    iscutSceneEnd = true;
                    cut_Level = 5;
                    Trigger = false;
                    Trigger_2 = 0;
                    Trigger_3 = false;
                }
                else
                {
                    Target_monster.GetComponent<Animator>().speed = 0;
                    Player.GetComponent<Animator>().speed = 0;
                    Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Mathf.Lerp(Pade.color.a, 0.4f, 1f * Time.deltaTime));
                    for (int i = 0; i < Said_1.transform.childCount; i++)
                    {
                        Said_1.transform.GetChild(i).GetComponent<Font_Said>().timer = 3;
                    }
                }
            }
        }
    }

    public GameObject CreateMonster(GameObject Monster_PreFab, Vector2 Pos)
    {
        GameObject Monster = Instantiate(Monster_PreFab);
        Monster.transform.position = Pos;
        Monsters_OBJ.Add(Monster);
        Monsters_Sc.Add(Monster.GetComponent<Monster>());
        return Monster;
    }

    void Spawn_Lojic()
    {
        GameObject monster = CreateMonster(Monsters[0], new Vector2(132.3f, 0f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);

        monster = CreateMonster(Monsters[0], new Vector2(142.3f, 0f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);

        monster = CreateMonster(Monsters[0], new Vector2(169.27f, 6.96f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);

        monster = CreateMonster(Monsters[0], new Vector2(152.3f, 0f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);

        monster = CreateMonster(Monsters[0], new Vector2(162.3f, 0f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);

        monster = CreateMonster(Monsters[1], new Vector2(163f, 5.24f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);

        monster = CreateMonster(Monsters[1], new Vector2(166.06f, 11.15f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);

        monster = CreateMonster(Monsters[1], new Vector2(169.41f, 12.79f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);

        monster = CreateMonster(Monsters[1], new Vector2(173.18f, 11.53f));
        monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);
    }

    void Update()
    {
        Game_Time += Time.deltaTime;

        if (Data_Loading)
        {
            data_Manager.Save_False();
            data_Manager.Save_False();
            Data_Loading = false;
        }

        if (Game_Time >= 0.1f && !Set && Data_Load)
        {
            data_Manager.Load(data_Manager.Index);
            Set = true;
        }

        for (int i = 0; i < Monsters_OBJ.Count; i++)
        {
            if (Monsters_Sc[i].monster == Monster.Monster_State.Dead)
            {
                Monsters_Sc.Remove(Monsters_Sc[i]);
                Monsters_OBJ.Remove(Monsters_OBJ[i]);
                break;
            }
        }

        if (!Basic_Game_Kit)
        {
            if (item_Manager.itemList.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Item item = new Item();
                    item.SetItem(item_Manager.itemList[i]);
                    player_Controller.inventory[i] = new InventorySlot(item.index, item.Name, item.Description, 1, item.Rank, item.Type, item.Type_A, item.Type_B, item.count_lim);
                }

                if (!Set_2)
                {
                    Spawn_Lojic();
                    Set_2 = true;
                }

                Basic_Game_Kit = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            int F_Count = item_Manager.Field_Items.Count + 4;
            
            item_Manager.Create_Item(new Vector2(Player.transform.position.x, 22), 2);
            item_Manager.Create_Item(new Vector2(Player.transform.position.x, 24), 3);
            item_Manager.Create_Item(new Vector2(Player.transform.position.x, 26), 4);
            item_Manager.Create_Item(new Vector2(Player.transform.position.x, 28), 5);
        }

        if (cutSceneMod || Loading)
        {
            if (!iscutScene)
            {
                cutScene_Time = 0;
                iscutScene = true;
                iscutSceneEnd = false;
                canvas.SetActive(false);
            }

            player_Controller.cutScene = true;
            Cam_S.SetCutScene(true);

            if (!Loading)
            {
                if (!canvas.activeSelf)
                {
                    Up.rectTransform.sizeDelta = Vector2.Lerp(Up.rectTransform.sizeDelta, new Vector2(Up.rectTransform.sizeDelta.x, 100f), 3f * Time.deltaTime);
                    Down.rectTransform.sizeDelta = Vector2.Lerp(Down.rectTransform.sizeDelta, new Vector2(Down.rectTransform.sizeDelta.x, 100f), 3f * Time.deltaTime);
                    Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Vector2.Lerp(new Vector2(Pade.color.a, 0), new Vector2(Base * Random.Range(min, max), 0), 3f * Time.deltaTime).x);
                }
                else
                {
                    Up.rectTransform.sizeDelta = Vector2.Lerp(Up.rectTransform.sizeDelta, new Vector2(Up.rectTransform.sizeDelta.x, 0f), 3f * Time.deltaTime);
                    Down.rectTransform.sizeDelta = Vector2.Lerp(Down.rectTransform.sizeDelta, new Vector2(Down.rectTransform.sizeDelta.x, 0f), 3f * Time.deltaTime);
                    Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Vector2.Lerp(new Vector2(Pade.color.a, 0), new Vector2(0, 0), 3f * Time.deltaTime).x);
                }

                /////

                if (Input.anyKeyDown && cut_Level != 1 && cut_Level != 2 && cut_Level != 3 && cut_Level != 4)
                {
                    Skip_Time = 0;
                }

                if (Input.anyKey && cut_Level != 1 && cut_Level != 2 && cut_Level != 3 && cut_Level != 4)
                {
                    Skip_Time += Time.deltaTime;
                }

                if (Skip_Time >= 3f)
                {

                    iscutSceneEnd = true;
                    if (cut_Level == 0)
                    {
                        Player.transform.position = new Vector2(-20, Player.transform.position.y);
                        player_Controller.Dir_C = -1;
                        boat.transform.position = new Vector3(-27.8f, boat.transform.position.y, boat.transform.position.z);
                        cut_Level = 1;
                    }
                    Trigger = false;
                    Trigger_2 = 0;
                    Trigger_3 = false;
                    Trigger_4 = 0;
                    Trigger_5 = 0;
                    Skip_Time = 0;
                }
                else
                {
                    if (cut_Level == 0)
                    {
                        CutScene_1();
                    }
                    else if (cut_Level == 1)
                    {
                        Tutorial_1();
                    }
                    else if (cut_Level == 2)
                    {
                        Tutorial_2();
                    }
                    else if (cut_Level == 3)
                    {
                        Tutorial_3();
                    }
                    else if (cut_Level == 4)
                    {
                        Tutorial_4();
                    }
                }
            }

            cutScene_Time += Time.deltaTime;

            if (iscutSceneEnd)
            {
                if (Loading) {
                    Loading = false;
                }

                cutSceneMod = false;
                player_Controller.cutScene = false;
                Cam_S.SetCutScene(false);
            }
        }
        else
        {
            iscutScene = false;
            Up.rectTransform.sizeDelta = Vector2.Lerp(Up.rectTransform.sizeDelta, new Vector2(Up.rectTransform.sizeDelta.x, 0f), 3f * Time.deltaTime);
            Down.rectTransform.sizeDelta = Vector2.Lerp(Down.rectTransform.sizeDelta, new Vector2(Down.rectTransform.sizeDelta.x, 0f), 3f * Time.deltaTime);
            Pade.color = new Color(Pade.color.r, Pade.color.g, Pade.color.b, Vector2.Lerp(new Vector2(Pade.color.a, 0), new Vector2(0, 0), 3f * Time.deltaTime).x);
            canvas.SetActive(true);

            if (cut_Level == 0 || cut_Level == 1 || cut_Level == 2 || cut_Level == 3 || cut_Level == 4)
            {
                cutSceneMod = true;
            }

        }
    }
}
