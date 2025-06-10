using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
public class UI_Manager : MonoBehaviour
{
    public Image Hp_Bar_BG_IMG, Hp_Bar_IMG, Hp_Bar_Lerp_IMG, Stamina_Bar_BG_IMG, Stamina_Bar_IMG, Mental_IMG;
    private bool Duration;
    private float Duration_Position;
    private float Duration_1st_Size;
    private float Duration_target_Po;
    public Canvas canvas_P;
    public Canvas canvas;
    public Player_Controller player_CTLR;
    public string fontSpriteSheetPath;
    public Sprite[] Eng_font;
    public List<GameObject> words_Type_1 = new List<GameObject>();
    public List<GameObject> words_Type_2 = new List<GameObject>();
    public List<GameObject> words_Type_3 = new List<GameObject>();
    public new GameObject camera { get; private set; }
    // public Camera camera_info { get; private set; }
    public Map_Manager map_Manager;
    public float first_Position;
    public float Amount;
    public Game_Manager game_Manager {get; private set;}
    public List<GameObject> Slot = new List<GameObject>();
    public Item_Manager item_Manager;
    public List<GameObject> Items;
    public GameObject inventory;
    public GraphicRaycaster raycaster;
    public Vector2 Mouse_Pos;
    public Vector2 Local_Mouse_Pos;
    public float Mouse_Clck_T;
    public bool Mouse_Event;
    public float Mouse_Distance;
    public GameObject Target_Item;
    public Transform Target_Item_Parent;
    public int Target_Item_Parent_Idx;
    public int Snap_idx;
    private Loading_UI loading_UI;

    public enum ESC_MENU_STATE
    {
        NONE, FIRST, CONTINUE, SETTINGS, QUIT
    }
    public ESC_MENU_STATE ESC_MENU;

    public Transform ESC_Menu_TS;

    public float Frame_Rate;
    void Awake()
    {
        player_CTLR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>();

        Hp_Bar_BG_IMG = GameObject.FindWithTag("HP_bar_BG").GetComponent<Image>();
        Hp_Bar_IMG = GameObject.FindWithTag("HP_bar").GetComponent<Image>();
        Hp_Bar_Lerp_IMG = GameObject.FindWithTag("HP_bar_Lerp").GetComponent<Image>();
        Stamina_Bar_BG_IMG = GameObject.FindWithTag("Stamina_bar_BG").GetComponent<Image>();
        Stamina_Bar_IMG = GameObject.FindWithTag("Stamina_bar").GetComponent<Image>();
        Mental_IMG = GameObject.FindWithTag("Mental_bar").GetComponent<Image>();
        item_Manager = GameObject.FindWithTag("Item_Manager").GetComponent<Item_Manager>();
        ESC_Menu_TS = GameObject.FindWithTag("ESC_Menu").transform;
        inventory = GameObject.FindWithTag("Inventory");
        GameObject slot = GameObject.FindWithTag("Slot");
        GameObject Equip_slot = GameObject.FindWithTag("Equip_Slot");

        if (slot != null) {
            for (int i = 0; i < slot.transform.childCount; i++)
            {
                for (int j = 0; j < slot.transform.GetChild(i).childCount; j++)
                {
                    player_CTLR.inventory.Add(null);
                    Slot.Add(slot.transform.GetChild(i).GetChild(j).gameObject);
                    Items.Add(null);
                }
            }

            for (int i = 0; i < Equip_slot.transform.childCount; i++) {
                for (int j = 0; j < Equip_slot.transform.GetChild(i).childCount; j++)
                {
                    player_CTLR.inventory.Add(null);
                    Slot.Add(Equip_slot.transform.GetChild(i).GetChild(j).gameObject);
                    Items.Add(null);
                }
            }
        }

        canvas = canvas_P;
        camera = GameObject.FindWithTag("MainCamera");
        fontSpriteSheetPath = "Font/Eng/Font";
        Eng_font = Resources.LoadAll<Sprite>(fontSpriteSheetPath);
        map_Manager = GameObject.FindWithTag("Map_Manager").GetComponent<Map_Manager>();
        loading_UI = GameObject.FindWithTag("Loading_UI").GetComponent<Loading_UI>();
        game_Manager = GameObject.FindWithTag("Game_Manager").GetComponent<Game_Manager>();
        first_Position = Hp_Bar_Lerp_IMG.rectTransform.anchoredPosition.x;
        Amount = Hp_Bar_IMG.fillAmount;

        if (inventory != null) {
            inventory.SetActive(false);
        }
        
    }

    void Inventory()
    {
        for (int i = 0; i < player_CTLR.inventory.Count; i++)
        {
            if (loading_UI.loading == 0 && !player_CTLR.inventory[i].isEmpty)
            {
                if (Items[i] == null)
                {
                    GameObject item = new GameObject($"{player_CTLR.inventory[i].itemId}");
                    item.transform.SetParent(Slot[i].transform);

                    RectTransform rectTransform = item.AddComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(0, 0);
                    rectTransform.localScale = new Vector3(1, 1, 0);

                    Image img = item.AddComponent<Image>();
                    img.sprite = item_Manager.images[player_CTLR.inventory[i].itemId];
                    if (i == 54)
                    {
                        Color color = img.GetComponent<Image>().color;
                        color = new Color(150, 150, 150, color.a);
                    }
                    img.SetNativeSize();
                    Items[i] = item;

                    Item item_info = item.AddComponent<Item>();
                    item_info.SetItem(item_Manager.itemList[player_CTLR.inventory[i].itemId]);
                }
            }
            else
            {
                if (Slot[i].transform.childCount > 1)
                {
                    Items[i] = null;
                    Destroy(Slot[i].transform.GetChild(1).gameObject);
                }
            }
        }

        PointerEventData data = new PointerEventData(null) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(data, results);



        for (int m = 0; m < Items.Count; m++)
        {
            GameObject item = Items[m];
            if (item == null) continue;

            bool isOver = results.Exists(r => r.gameObject == item);

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Mouse_Pos = Input.mousePosition;
                Mouse_Clck_T = 0f;
                Mouse_Distance = 0;
                Mouse_Event = false;
                if (isOver)
                {
                    Target_Item = item;
                    Target_Item_Parent = Target_Item.transform.parent.parent;
                    Target_Item_Parent_Idx = Target_Item.transform.parent.GetSiblingIndex();
                    Target_Item.transform.parent.SetParent(inventory.transform);
                }
            }

            if (Target_Item != null)
            {

                if (Input.GetMouseButton(0))
                {
                    Mouse_Distance = Vector2.Distance(Mouse_Pos, Input.mousePosition);
                    if (Mouse_Distance > 5 || Mouse_Event)
                    {
                        // 드래그

                        Mouse_Clck_T = 0;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            Target_Item.transform.parent.GetComponent<RectTransform>(),
                            Input.mousePosition,
                            Camera.main,
                            out Local_Mouse_Pos
                        );

                        float F_Pos_Dis = 10000f;
                        Vector2 F_Pos = Vector2.zero;
                        int F_idx = Items.IndexOf(Target_Item);

                        for (int i = 0; i < Slot.Count; i++)
                        {
                            float item_Dis = Vector2.Distance(Input.mousePosition, RectTransformUtility.WorldToScreenPoint(Camera.main, Slot[i].transform.position));
                            if (item_Dis < F_Pos_Dis && (Items[i] == null || Items[i] == Target_Item))
                            {
                                F_Pos_Dis = item_Dis;
                                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                    Target_Item.transform.parent.GetComponent<RectTransform>(),
                                    RectTransformUtility.WorldToScreenPoint(Camera.main, Slot[i].transform.position),
                                    Camera.main,
                                    out F_Pos
                                );

                                F_idx = i;
                            }
                        }

                        if (F_Pos_Dis <= 40)
                        {
                            Target_Item.GetComponent<RectTransform>().anchoredPosition = F_Pos;
                            Snap_idx = F_idx;
                        }
                        else
                        {
                            Snap_idx = Items.IndexOf(Target_Item);
                            Target_Item.GetComponent<RectTransform>().anchoredPosition = Local_Mouse_Pos;
                        }
                        Mouse_Event = true;
                    }
                    else if (!Mouse_Event)
                    {
                        Mouse_Clck_T += Frame_Rate;
                    }
                }

                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    Target_Item.transform.parent.SetParent(Target_Item_Parent);
                    Target_Item.transform.parent.SetSiblingIndex(Target_Item_Parent_Idx);

                    if (Mouse_Distance > 5 && Mouse_Clck_T == 0)
                    {

                        if (Snap_idx != Items.IndexOf(Target_Item))
                        {
                            Item item_ = Target_Item.GetComponent<Item>();
                            int idx = Snap_idx - (Slot.Count - 6);

                            if (Snap_idx >= Slot.Count - 6)
                            {
                                if (item_.Type == 0)
                                {
                                    if ((item_.Type_A == 0 && idx == 2) || (item_.Type_A == 1 && idx == 3))
                                    {
                                        player_CTLR.inventory[Snap_idx] = player_CTLR.inventory[Items.IndexOf(Target_Item)];
                                        player_CTLR.inventory[Items.IndexOf(Target_Item)] = new InventorySlot();
                                        Items[Items.IndexOf(Target_Item)] = null;
                                    }
                                    else if (item_.Type_A == 2 && (idx == 2 || idx == 3))
                                    {
                                        player_CTLR.inventory[54] = player_CTLR.inventory[Items.IndexOf(Target_Item)];
                                        player_CTLR.inventory[55] = player_CTLR.inventory[Items.IndexOf(Target_Item)];
                                        player_CTLR.inventory[Items.IndexOf(Target_Item)] = new InventorySlot();
                                        Items[Items.IndexOf(Target_Item)] = null;
                                    }
                                    else
                                    {
                                        Target_Item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                                    }
                                }
                                else if (item_.Type == 1)
                                {
                                    if ((item_.Type_A == 0 && idx == 0) || (item_.Type_A == 1 && idx == 1) || (item_.Type_A == 2 && idx == 4) || (item_.Type_A == 3 && idx == 5))
                                    {
                                        player_CTLR.inventory[Snap_idx] = player_CTLR.inventory[Items.IndexOf(Target_Item)];
                                        player_CTLR.inventory[Items.IndexOf(Target_Item)] = new InventorySlot();
                                        Items[Items.IndexOf(Target_Item)] = null;
                                    }
                                    else
                                    {
                                        Target_Item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                                    }
                                }
                                else
                                {
                                    Target_Item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                                }
                            }
                            else
                            {
                                player_CTLR.inventory[Snap_idx] = player_CTLR.inventory[Items.IndexOf(Target_Item)];
                                if (Items.IndexOf(Target_Item) == 54 || Items.IndexOf(Target_Item) == 55)
                                {
                                    player_CTLR.inventory[54] = new InventorySlot();
                                    player_CTLR.inventory[55] = new InventorySlot();
                                }
                                else
                                {
                                    player_CTLR.inventory[Items.IndexOf(Target_Item)] = new InventorySlot();
                                    Items[Items.IndexOf(Target_Item)] = null;
                                }

                            }

                        }
                        else
                        {
                            Target_Item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                        }

                    }
                    else
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            // 우클
                        }

                        if (Input.GetMouseButtonUp(1))
                        {
                            // 좌클
                            player_CTLR.inventory[Items.IndexOf(Target_Item)] = new InventorySlot();
                            Items[Items.IndexOf(Target_Item)] = null;

                        }
                    }

                    Target_Item = null;
                    Local_Mouse_Pos = Vector2.zero;
                }
            }
        }
    }

    GameObject Interaction_Item()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        GameObject Target = hit.gameObject;

        // if (hit != null)
        // {
        //     if (Target.tag != "Untagged")
        //     {
        //         Debug.Log($"Tag: {Target.tag}");
        //     }
        //     else
        //     {
        //         Debug.Log($"Name: {Target.name}");
        //     }

        // }

        return Target;
    }
    


    public GameObject FONT_RENDER(string letter, float x, float y, float size, Vector3 color, int Type)
    {
        GameObject Word = null;
        if (Type == 0) {
            float spacing = size * 0.4f;
            float startX = x - (letter.Length - 1) * spacing / 2f;

            Word = new GameObject($"Damage_{words_Type_1.Count}");
            Word.transform.SetParent(canvas.transform);
            Font_Damage font_Damage = Word.AddComponent<Font_Damage>();
            font_Damage.SetColor(color);
            font_Damage.SetSize(size);
            RectTransform rect_ = Word.AddComponent<RectTransform>();
            rect_.anchoredPosition = Vector2.zero;
            rect_.position = new Vector3(x, y, 0);

            for (int i = 0; i < letter.Length; i++)
            {
                int ascii = (int)letter[i];
                int index = ascii - 33;

                if (index < 0 || index >= Eng_font.Length)
                {
                    Debug.LogWarning("Unsupported character: " + letter[i]);
                    continue;
                }

                GameObject charGO = new GameObject("FontChar_" + letter[i]);
                charGO.transform.SetParent(Word.transform);

                RectTransform rect = charGO.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = Vector2.zero;
                rect.position = new Vector3(startX + spacing * i, y, 0);

                Image img = charGO.AddComponent<Image>();
                img.sprite = Eng_font[index];
            }
            words_Type_1.Add(Word);
        }
        else if (Type == 1) {
            for (int i = 0; i < words_Type_2.Count; i++) {
                Destroy(words_Type_2[i]);
                words_Type_2.Remove(words_Type_2[i]);
            }

            float spacing = size * 0.4f;
            float startX = x - (letter.Length - 1) * spacing / 2f;

            Word = new GameObject($"Title_{words_Type_2.Count}");
            Word.transform.SetParent(canvas.transform);
            Font_Title font_title = Word.AddComponent<Font_Title>();
            // font_title.SetDur(50);
            RectTransform rect_ = Word.AddComponent<RectTransform>();
            rect_.anchoredPosition = Vector2.zero;
            rect_.position = new Vector3(x, y, 0);

            for (int i = 0; i < letter.Length; i++)
            {
                int ascii = (int)letter[i];
                int index = ascii - 33;

                if (index < 0 || index >= Eng_font.Length)
                {
                    Debug.LogWarning("Unsupported character: " + letter[i]);
                    continue;
                }

                GameObject charGO = new GameObject("FontChar_" + letter[i]);
                charGO.transform.SetParent(Word.transform);

                RectTransform rect = charGO.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = Vector2.zero;
                rect.position = new Vector3(startX + spacing * i, y, 0);

                Image img = charGO.AddComponent<Image>();
                img.sprite = Eng_font[index];
            }

            words_Type_2.Add(Word);
        }

        return Word;
    }

    public GameObject Said(string letter, float x, float y, float size, Vector3 color, float delayTime, float Life_Time, float VibMin, float VibMax, bool isRect)
    {
        GameObject Word = null;
        float spacing = size * 0.5f;
        if (isRect)
        {
            spacing = size * 0.5f;
        }
        else
        {
            spacing = size;
        }

        float startX = x - (letter.Length - 1) * spacing / 2f;

        Word = new GameObject($"Said_{words_Type_3.Count}");
        if (isRect)
        {
            Word.transform.SetParent(canvas.transform);
        }
        else {
            Word.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
        words_Type_3.Add(Word);

        float baseTime = Time.time;

        for (int i = 0; i < letter.Length; i++)
        {
            int ascii = (int)letter[i];
            int index = ascii - 33;

            if (index < 0 || index >= Eng_font.Length)
            {
                Debug.LogWarning("Unsupported character: " + letter[i]);
                continue;
            }

            GameObject charGO = new GameObject("FontChar_" + letter[i]);
            charGO.transform.SetParent(Word.transform);

            if (isRect)
            {
                RectTransform rect = charGO.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = Vector2.zero;
                rect.position = new Vector3(startX + spacing * i, y, 0);

                Image img = charGO.AddComponent<Image>();
                img.sprite = Eng_font[index];
            }
            else
            {
                Transform rect = charGO.GetComponent<Transform>();
                rect.localScale = new Vector3(size, size, 1);
                rect.position = new Vector3(startX + spacing * i, y, 0);

                SpriteRenderer img = charGO.AddComponent<SpriteRenderer>();
                img.sprite = Eng_font[index];
            }




            Font_Said font_said = charGO.AddComponent<Font_Said>();
            font_said.delayTime = baseTime + i * delayTime;
            font_said.SetSize(size);
            font_said.Life_Time = Life_Time;
            font_said.VibMin = VibMin;
            font_said.VibMax = VibMax;
            font_said.SetColor(color);
        }

        return Word;
    }
    public void DMG_UI(int Type, float x, float y)
    {
        int random_Msg_Num = Random.Range(0, 5);
        string random_Msg = "힐링!";
        if (Type == 0)
        {
            // switch (random_Msg_Num)
            // {
            //     case 0:
            //         random_Msg = "ㅇ..아프다..!";
            //         break;                
            //     case 1:
            //         random_Msg = "으악..!";
            //         break;
            //     case 2:
            //         random_Msg = "그만해!!!";
            //         break;
            //     case 3:
            //         random_Msg = "미안하다ㅠㅠ";
            //         break;
            //     case 4:
            //         random_Msg = "너무해ㅠㅠ";
            //         break;
            //     default:
            //         random_Msg = "이 메세지는 버그입니다.";
            //         break;
            // }

            switch (random_Msg_Num)
            {
                case 0:
                    random_Msg = "Oof... that hurt!";
                    break;
                case 1:
                    random_Msg = "Yikes!!";
                    break;
                case 2:
                    random_Msg = "Chill!!";
                    break;
                case 3:
                    random_Msg = "My bad..";
                    break;
                case 4:
                    random_Msg = "That was rude";
                    break;
                default:
                    random_Msg = "Uh oh, glitch vibes!";
                    break;
            }

            GameObject word = FONT_RENDER($"{random_Msg}", x, y, 0.5f, new Vector3(1, 0.4f, 0.4f), 0);
        }

        if (Type == 1)
        {
            // switch (random_Msg_Num)
            // {
            //     case 0:
            //         random_Msg = "살겠다..";
            //         break;                
            //     case 1:
            //         random_Msg = "흐하학!!";
            //         break;
            //     case 2:
            //         random_Msg = "풀피!!";
            //         break;
            //     case 3:
            //         random_Msg = "힐링...";
            //         break;
            //     case 4:
            //         random_Msg = "노곤하당..ㅎ";
            //         break;
            //     default:
            //         random_Msg = "이 메세지는 버그입니다.";
            //         break;
            // }

            switch (random_Msg_Num)
            {
                case 0:
                    random_Msg = "I can breathe again.";
                    break;
                case 1:
                    random_Msg = "Heheh, I'm alive!";
                    break;
                case 2:
                    random_Msg = "Full HP, let's go!";
                    break;
                case 3:
                    random_Msg = "Feeling fresh.";
                    break;
                case 4:
                    random_Msg = "Kinda cozy now.";
                    break;
                default:
                    random_Msg = "Oops, that's a bug message.";
                    break;
            }

            GameObject word = FONT_RENDER($"{random_Msg}", x, y, 0.5f, new Vector3(0.4f, 1, 0.4f), 0);
        }

        if (Type == 2)
        {
            // switch (random_Msg_Num)
            // {
            //     case 0:
            //         random_Msg = "찌릿찌릿";
            //         break;                
            //     case 1:
            //         random_Msg = "뭐야!!";
            //         break;
            //     case 2:
            //         random_Msg = "으그극ㄱㅡ극";
            //         break;
            //     default:
            //         random_Msg = "에붸부에ㅂ";
            //         break;
            // }

            switch (random_Msg_Num)
            {
                case 0:
                    random_Msg = "Zapped!";
                    break;
                case 1:
                    random_Msg = "Yo, what was that?!";
                    break;
                case 2:
                    random_Msg = "Gahhh- that hurt!";
                    break;
                default:
                    random_Msg = "Blblbl- system crash?";
                    break;
            }

            FONT_RENDER($"{random_Msg}", 0, 0, 1.5f, new Vector3(1, 1, 0.4f), 0);
        }
    }




    void IN_GAME_UI() {
        for (int i = words_Type_3.Count - 1; i >= 0; i--)
        {
            if (words_Type_3[i] == null)
            {
                words_Type_3.RemoveAt(i);
            }

            if (words_Type_3[i] != null && words_Type_3[i].transform.childCount == 0)
            {
                Destroy(words_Type_3[i]);
                words_Type_3.RemoveAt(i);
            }
        }


        Hp_Bar_BG_IMG.fillAmount = (float)player_CTLR.Max_Hp / 100 + (float)player_CTLR.P_Max_Hp / 400 + 0.06f;
        Hp_Bar_IMG.fillAmount = (Hp_Bar_BG_IMG.fillAmount - 0.054f) * (100f / ((float)player_CTLR.C_Max_Hp / (float)player_CTLR.C_Hp) / 100) + ((player_CTLR.Hp - 30) * 0.00074f) + (player_CTLR.P_Hp * (0.00074f / 4));

        // 바뀐 부분만!
        float fillGap = Hp_Bar_BG_IMG.fillAmount - 0.050f - Hp_Bar_IMG.fillAmount;
        float currentPos = -2.0f + 766.0f * fillGap + 10;

        if (Mathf.Abs(Duration_target_Po - Mathf.Floor(fillGap * 1000f) / 1000f) > 0.001f) {
            float currentSize = Mathf.Floor(currentPos * 1000f) / 1000f - Duration_1st_Size;
            Hp_Bar_Lerp_IMG.rectTransform.sizeDelta = new Vector2(currentSize, Hp_Bar_Lerp_IMG.rectTransform.sizeDelta.y);
            Amount = Hp_Bar_IMG.fillAmount;
            Duration_target_Po = Mathf.Floor(fillGap * 1000f) / 1000f;
        }
        else if (Hp_Bar_Lerp_IMG.rectTransform.sizeDelta.x > 0) {
            if (Hp_Bar_Lerp_IMG.rectTransform.sizeDelta.x <= 0.01f) {
                Hp_Bar_Lerp_IMG.rectTransform.sizeDelta = new Vector2(0, Hp_Bar_Lerp_IMG.rectTransform.sizeDelta.y);
            }
            else {
                Hp_Bar_Lerp_IMG.rectTransform.sizeDelta = Vector2.Lerp(
                    Hp_Bar_Lerp_IMG.rectTransform.sizeDelta,
                    new Vector2(0, Hp_Bar_Lerp_IMG.rectTransform.sizeDelta.y),
                    Frame_Rate * 4f
                );

                Duration_1st_Size = Vector2.Lerp(
                    new Vector2(Duration_1st_Size, 0),
                    new Vector2(currentPos, 0),
                    Frame_Rate * 4f
                ).x;
            }

            Hp_Bar_Lerp_IMG.rectTransform.anchoredPosition = new Vector2(
                -2.0f - 766.0f * fillGap + first_Position,
                Hp_Bar_Lerp_IMG.rectTransform.anchoredPosition.y
            );
        }

        if (Hp_Bar_IMG.fillAmount > Amount) {
            Hp_Bar_Lerp_IMG.rectTransform.anchoredPosition = new Vector2(
                -2.0f - 766.0f * fillGap + first_Position,
                Hp_Bar_Lerp_IMG.rectTransform.anchoredPosition.y
            );

            Hp_Bar_Lerp_IMG.rectTransform.sizeDelta = new Vector2(0, Hp_Bar_Lerp_IMG.rectTransform.sizeDelta.y);
        }

        Stamina_Bar_BG_IMG.fillAmount = (float)player_CTLR.Max_St / 100 + (float)player_CTLR.P_Max_St / 500 + 0.058f;
        Stamina_Bar_IMG.fillAmount = (Stamina_Bar_BG_IMG.fillAmount - 0.01f) * (100f / ((float)player_CTLR.C_Max_St / (float)player_CTLR.C_St) / 100f);
        Mental_IMG.fillAmount = (float)player_CTLR.MT / 100;    
    }

    void ESC_Menu()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if ((!player_CTLR.cutScene || player_CTLR.Inventory_Trigger)) {
                inventory.SetActive(!inventory.activeSelf);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventory.activeSelf)
            {
                inventory.SetActive(!inventory.activeSelf);
            }
            else if (ESC_MENU == ESC_MENU_STATE.NONE)
            {
                ESC_MENU = ESC_MENU_STATE.FIRST;
            }
            else if (ESC_MENU == ESC_MENU_STATE.FIRST)
            {
                ESC_MENU = ESC_MENU_STATE.NONE;
            }
        }

        GameObject BK0 = ESC_Menu_TS.GetChild(0).gameObject;
        CanvasGroup BK0_Group = BK0.GetComponent<CanvasGroup>();
        RectTransform BK0_rect = BK0.GetComponent<RectTransform>();

        GameObject BK1 = ESC_Menu_TS.GetChild(1).gameObject;
        CanvasGroup BK1_Group = BK1.GetComponent<CanvasGroup>();
        RectTransform BK1_rect = BK1.GetComponent<RectTransform>();

        GameObject BK2 = ESC_Menu_TS.GetChild(2).gameObject;
        CanvasGroup BK2_Group = BK2.GetComponent<CanvasGroup>();
        RectTransform BK2_rect = BK2.GetComponent<RectTransform>();

        if (Time.timeScale > 0) {
            Frame_Rate = Time.deltaTime;
        }
        
        switch (ESC_MENU)
        {
            case ESC_MENU_STATE.NONE:
                BK0_Group.alpha = Mathf.Lerp(BK0_Group.alpha, 0, 4 * Frame_Rate);
                BK0_rect.sizeDelta = Vector2.Lerp(BK0_rect.sizeDelta, new Vector2(140, BK0_rect.sizeDelta.y), 4 * Frame_Rate);
                BK1_Group.alpha = Mathf.Lerp(BK1_Group.alpha, 0, 4 * Frame_Rate);
                BK1_rect.sizeDelta = Vector2.Lerp(BK1_rect.sizeDelta, new Vector2(110, BK1_rect.sizeDelta.y), 4 * Frame_Rate);
                BK2_Group.alpha = Mathf.Lerp(BK2_Group.alpha, 0, 4 * Frame_Rate);
                BK2_rect.sizeDelta = Vector2.Lerp(BK2_rect.sizeDelta, new Vector2(80, BK2_rect.sizeDelta.y), 4 * Frame_Rate);

                if (BK2_Group.alpha <= 0.1f)
                {
                    BK0_rect.anchoredPosition = new Vector2(2560, 0);
                    BK0_Group.alpha = 0;
                    BK1_rect.anchoredPosition = new Vector2(3840, 0);
                    BK1_Group.alpha = 0;
                    BK2_rect.anchoredPosition = new Vector2(5120, 0);
                    BK2_Group.alpha = 0;

                    Time.timeScale = 1;
                    player_CTLR.cutScene = false;
                }

                break;

            case ESC_MENU_STATE.FIRST:
                BK0_Group.alpha = Mathf.Lerp(BK0_Group.alpha, 1, 4 * Frame_Rate);
                BK0_rect.sizeDelta = Vector2.Lerp(BK0_rect.sizeDelta, new Vector2(70, BK0_rect.sizeDelta.y), 4 * Frame_Rate);
                BK1_Group.alpha = Mathf.Lerp(BK1_Group.alpha, 1, 4 * Frame_Rate);
                BK1_rect.sizeDelta = Vector2.Lerp(BK1_rect.sizeDelta, new Vector2(55, BK1_rect.sizeDelta.y), 4 * Frame_Rate);
                BK2_Group.alpha = Mathf.Lerp(BK2_Group.alpha, 1, 4 * Frame_Rate);
                BK2_rect.sizeDelta = Vector2.Lerp(BK2_rect.sizeDelta, new Vector2(40, BK2_rect.sizeDelta.y), 4 * Frame_Rate);

                BK0_rect.anchoredPosition = Vector2.Lerp(BK0_rect.anchoredPosition, Vector2.zero, 4 * Frame_Rate);
                BK1_rect.anchoredPosition = Vector2.Lerp(BK1_rect.anchoredPosition, Vector2.zero, 4 * Frame_Rate);
                BK2_rect.anchoredPosition = Vector2.Lerp(BK2_rect.anchoredPosition, Vector2.zero, 4 * Frame_Rate);

                if (Interaction_Item() != null && Input.GetMouseButtonDown(0))
                {
                    if (Interaction_Item().tag == "ESC_Menu_Continue")
                    {
                        ESC_MENU = ESC_MENU_STATE.CONTINUE;
                    }
                    else if (Interaction_Item().tag == "ESC_Menu_Settings")
                    {
                        ESC_MENU = ESC_MENU_STATE.SETTINGS;
                    }
                    else if (Interaction_Item().tag == "ESC_Menu_Quit")
                    {
                        ESC_MENU = ESC_MENU_STATE.QUIT;
                    }
                    else if (Interaction_Item().tag == "ESC_Menu_Back")
                    {
                        ESC_MENU = ESC_MENU_STATE.NONE;
                    }
                }

                Time.timeScale = 0;
                player_CTLR.cutScene = true;
                break;

            case ESC_MENU_STATE.CONTINUE:
                BK0_rect.sizeDelta = Vector2.Lerp(BK0_rect.sizeDelta, new Vector2(-2560, BK0_rect.sizeDelta.y), 4 * Frame_Rate);
                BK1_rect.sizeDelta = Vector2.Lerp(BK1_rect.sizeDelta, new Vector2(-3840, BK1_rect.sizeDelta.y), 4 * Frame_Rate);
                BK2_rect.sizeDelta = Vector2.Lerp(BK2_rect.sizeDelta, new Vector2(-5120, BK2_rect.sizeDelta.y), 4 * Frame_Rate);

                BK0_rect.anchoredPosition = Vector2.Lerp(BK0_rect.anchoredPosition, Vector2.zero, 4 * Frame_Rate);
                BK1_rect.anchoredPosition = Vector2.Lerp(BK1_rect.anchoredPosition, Vector2.zero, 4 * Frame_Rate);
                BK2_rect.anchoredPosition = Vector2.Lerp(BK2_rect.anchoredPosition, Vector2.zero, 4 * Frame_Rate);
                break;

            case ESC_MENU_STATE.SETTINGS:
                break;

            case ESC_MENU_STATE.QUIT:
                break;
        } 
    }

    void LOJIC() {
        ESC_Menu();

        if (inventory.activeSelf)
        {
            Inventory();
        }

        

        if (Input.GetKey(KeyCode.Z))
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                player_CTLR.Stealing(1);
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                player_CTLR.Stealing(-1);
            }
        }
        else if (Input.GetKey(KeyCode.X))
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                player_CTLR.Stealing_Point(1);
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                player_CTLR.Stealing_Point(-1);
            }
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                player_CTLR.Healing_Point(1);
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                player_CTLR.Healing_Point(-1);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                player_CTLR.Healing(1);
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                player_CTLR.Healing(-1);
            }
        }
    }

    void Update()
    {
        if (player_CTLR == null && GameObject.FindGameObjectWithTag("Player") != null) 
            player_CTLR = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>();
            
        LOJIC();
        IN_GAME_UI();
    }
}
