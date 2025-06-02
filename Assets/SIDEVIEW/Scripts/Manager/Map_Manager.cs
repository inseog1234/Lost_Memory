using System.Collections.Generic;
using Codice.Client.BaseCommands;
using Codice.Client.BaseCommands.LayoutFilters;
using Codice.CM.SEIDInfo;
using Unity.VisualScripting.YamlDotNet.Serialization.NamingConventions;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
public class Map_Manager : MonoBehaviour
{
    private List<GameObject> childColliders;
    private int P_Map;
    private int C_Map;
    private GameObject BackGround_Layer_1;
    private List<GameObject> BackGround_Layer_1_L = new List<GameObject>();
    public List<Sprite> BG_L1; 
    private GameObject BackGround_Layer_2;
    private List<GameObject> BackGround_Layer_2_L = new List<GameObject>();
    public List<Sprite> BG_L2; 
    private GameObject BackGround_Layer_3;
    private List<GameObject> BackGround_Layer_3_L = new List<GameObject>();
    public List<Sprite> BG_L3;
    private GameObject BackGround_Layer_4;
    private List<GameObject> BackGround_Layer_4_L = new List<GameObject>();
    public List<Sprite> BG_L4;
    private List<List<GameObject>> BackGround_Layer = new List<List<GameObject>>();
    public List<Vector2> Scene_Pos;
    public bool Pade_Start {get; private set;}
    public Image Pade;
    public bool Pade_is;
    public float Pade_T;
    public int C_Map_C;
    public UI_Manager uI_Manager;
    public bool Scene_Refs;
    public int Scene_Pos_idx;

    public GameObject Player;
    public GameObject cam;
    public float BackGround_speed;
    public Vector2 Base_Offset_Pos;
    public List<Vector2> Cam_Mov_Bd = new List<Vector2>();
    public bool Title_PopUp;
    public Game_Manager game_Manager;
    public GameObject BK;
    private float[] parallaxFactors;
    public string Map_Name { get; private set; }
    void Awake()
    {
        childColliders = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Collider2D>() != null)
            {
                childColliders.Add(child.gameObject);
            }
        }

        parallaxFactors = new float[] { 0.3f, 0.4f, 0.7f, 0.9f };

        Cam_Mov_Bd.Add(new Vector2(-15f, 34f));
        Cam_Mov_Bd.Add(new Vector2(108f, 161f));
        // Cam_Mov_Bd.Add(new Vector2());

        cam = GameObject.FindWithTag("MainCamera");
        BackGround_speed = 2;

        Pade = GameObject.FindWithTag("Map_Pade").GetComponent<Image>();
        Pade_is = false;
        Pade_Start = false;
        BackGround_Layer_1 = GameObject.FindWithTag("BG_L1");
        BackGround_Layer_2 = GameObject.FindWithTag("BG_L2");
        BackGround_Layer_3 = GameObject.FindWithTag("BG_L3");
        BackGround_Layer_4 = GameObject.FindWithTag("BG_L4");
        BK = GameObject.FindWithTag("BK");
        game_Manager = GameObject.FindWithTag("Game_Manager").GetComponent<Game_Manager>();

        for (int i = 0; i < BackGround_Layer_1.transform.childCount; i++)
        {
            BackGround_Layer_1_L.Add(BackGround_Layer_1.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < BackGround_Layer_2.transform.childCount; i++)
        {
            BackGround_Layer_2_L.Add(BackGround_Layer_2.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < BackGround_Layer_3.transform.childCount; i++)
        {
            BackGround_Layer_3_L.Add(BackGround_Layer_3.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < BackGround_Layer_4.transform.childCount; i++)
        {
            BackGround_Layer_4_L.Add(BackGround_Layer_4.transform.GetChild(i).gameObject);
        }

        BackGround_Layer.Add(BackGround_Layer_1_L);
        BackGround_Layer.Add(BackGround_Layer_2_L);
        BackGround_Layer.Add(BackGround_Layer_3_L);
        BackGround_Layer.Add(BackGround_Layer_4_L);

        uI_Manager = GameObject.FindWithTag("UI_Manager").GetComponent<UI_Manager>();
        Player = GameObject.FindWithTag("Player");

        Scene_Refs = false;
        Scene_Pos_idx = 0;
        Title_PopUp = false;

        C_Map_C = 0;
        P_Map = 0;
        C_Map = 0;
        Map_Name = "Duto Land";
    }

    [System.Obsolete]
    void Update() {
        PadeIn();
        BK.SetActive(!game_Manager.cutSceneMod);
        
        foreach (var child in childColliders)
        {
            var collider = child.GetComponent<Collider2D>();
            if (collider == null) continue;

            Collider2D[] results = new Collider2D[10];
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            int count = collider.OverlapCollider(filter, results);

            for (int i = 0; i < count; i++)
            {
                if (results[i] != null)
                {
                    int index = childColliders.IndexOf(child);
                    if (results[i].gameObject.tag == "Player")
                    {
                        for (int j = 0; j < child.transform.childCount; j++)
                        {
                            var subCollider = child.transform.GetChild(j).GetComponent<Collider2D>();
                            if (subCollider == null) continue;

                            Collider2D[] subResults = new Collider2D[10];
                            int subCount = subCollider.OverlapCollider(filter, subResults);
                            for (int k = 0; k < subCount; k++)
                            {

                                if (subResults[k] != null && subResults[k].gameObject.tag == "Player")
                                {
                                    if (!Pade_Start)
                                    {
                                        if (index == 0)
                                        {
                                            if (j == 0)
                                            {
                                                C_Map_C = 1;
                                                Scene_Pos_idx = 0;
                                            }
                                        }
                                        else if (index == 1)
                                        {
                                            if (j == 0)
                                            {
                                                C_Map_C = 0;
                                                Scene_Pos_idx = 1;
                                            }
                                            // else if (j == 1) {
                                            //     C_Map_C = 2;
                                            // }
                                        }

                                        if (P_Map != C_Map_C)
                                        {
                                            Pade_Start = true;
                                            P_Map = C_Map_C;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        Refresh();
        Pllx();
    }

    void Refresh() {
        if (!Scene_Refs) {
            int idx = 0;
            if (C_Map_C == 0) {
                idx = Scene_Pos_idx;
            }
            else if (C_Map_C == 1) {
                idx = Scene_Pos_idx + 2;
            }

            Player.transform.position = Scene_Pos[idx];

            for (int i = 0; i < BackGround_Layer_1.transform.childCount; i++) {
                BackGround_Layer_1.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = BG_L1[C_Map];
            }
            for (int i = 0; i < BackGround_Layer_2.transform.childCount; i++) {
                BackGround_Layer_2.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = BG_L2[C_Map];
            }
            for (int i = 0; i < BackGround_Layer_3.transform.childCount; i++) {
                BackGround_Layer_3.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = BG_L3[C_Map];
            }
            for (int i = 0; i < BackGround_Layer_4.transform.childCount; i++) {
                BackGround_Layer_4.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = BG_L4[C_Map];
            }
            cam.GetComponent<Cam>().SetBorder(Cam_Mov_Bd[C_Map_C]);
            cam.GetComponent<Cam>().SetPosition(Cam_Mov_Bd[C_Map_C].x, cam.GetComponent<Cam>().Target.transform.position.y);
            Base_Offset_Pos = Scene_Pos[idx];
            
            Scene_Refs = true;
        }
    }
    
    void UpdateParallax(Transform layer, float factor)
    {
        Vector2 offset = (Vector2)cam.transform.position - new Vector2(0, 2);
        Vector2 parallaxPos = new Vector2(0, 5) + new Vector2(offset.x, 0) * factor;
        layer.position = new Vector3(parallaxPos.x, parallaxPos.y, layer.position.z);
    }

    void Pllx()
    {
            UpdateParallax(BackGround_Layer_1.transform, parallaxFactors[0]);
            UpdateParallax(BackGround_Layer_2.transform, parallaxFactors[1]);
            UpdateParallax(BackGround_Layer_3.transform, parallaxFactors[2]);
            UpdateParallax(BackGround_Layer_4.transform, parallaxFactors[3]);

        for (int i = 0; i < BackGround_Layer.Count; i++)
        {
            for (int j = 0; j < BackGround_Layer[i].Count; j++)
            {
                int PosX = 0;
                if ((int)(cam.transform.position.x - BackGround_Layer[i][j].transform.position.x) > 22)
                {
                    PosX = (int)-((cam.transform.position.x - BackGround_Layer[i][j].transform.position.x) / 24) - 1;
                }
                else if ((int)(cam.transform.position.x - BackGround_Layer[i][j].transform.position.x) < -22)
                {
                    PosX = (int)-((cam.transform.position.x - BackGround_Layer[i][j].transform.position.x) / 24) + 1;
                }

                if (PosX < -1)
                {
                    int TargetX = 0;
                    if (j == 0)
                    {
                        TargetX = 2;
                    }
                    else if (j == 1)
                    {
                        TargetX = 0;
                    }
                    else if (j == 2)
                    {
                        TargetX = 1;
                    }

                    BackGround_Layer[i][j].transform.position = new Vector2(BackGround_Layer[i][TargetX].transform.position.x + 16, BackGround_Layer[i][j].transform.position.y);
                    Debug.Log(TargetX);
                }
                else if (PosX > 1)
                {
                    int TargetX = 0;
                    if (j == 0)
                    {
                        TargetX = 1;
                    }
                    else if (j == 1)
                    {
                        TargetX = 2;
                    }
                    else if (j == 2)
                    {
                        TargetX = 0;
                    }

                    BackGround_Layer[i][j].transform.position = new Vector2(BackGround_Layer[i][TargetX].transform.position.x - 16, BackGround_Layer[i][j].transform.position.y);
                    Debug.Log(TargetX);
                }
            }
        }
        

    }

    void PadeIn() {
        if (Pade_Start) {
            if (!Pade_is) {
                Pade_T = Time.time;
                Title_PopUp = false;
                Pade_is = true;
                Player.GetComponent<Player_Controller>().cutScene = true;
            }
            float Duration = 1.5f;
            
            if (Mathf.Sin((Time.time - Pade_T) * Duration) < 0) {
                Pade_is = false;
                Pade_Start = false;
                Player.GetComponent<Player_Controller>().cutScene = false;
            }
            else {
                Pade.color = new Color(0, 0, 0, Mathf.Abs(Mathf.Sin((Time.time - Pade_T) * Duration)));
            }
            float Dur_T =  Mathf.Sin((Time.time - Pade_T) * Duration);
            if ((Time.time - Pade_T) * Duration >= 1.5f && Dur_T > 0.990) {
                if (!Title_PopUp) {
                    Map_Title();
                    Scene_Refs = false;
                    Title_PopUp = true;
                }

            }

            if (Mathf.Sin((Time.time - Pade_T) * Duration) >= 0.997f) {
                C_Map = C_Map_C;
            }
        }
    }

    void Map_Title()
    {
        Map_Name = "Duto Land";
        Vector3 Color = new Vector3(1, 1, 1);
        switch (C_Map_C)
        {
            case 0:
                Map_Name = "Duto Land";
                break;
            case 1:
                Map_Name = "Weak forest";
                break;
        }

        uI_Manager.FONT_RENDER(Map_Name, uI_Manager.camera.transform.position.x, uI_Manager.camera.transform.position.y + 0.6f, 2f, Color, 1);
        
    }
}