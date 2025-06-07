using System.Collections;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// using UnityEngine.UI;
public class Title_Manager : MonoBehaviour
{
    private GameObject[] targetObjects = new GameObject[5];
    // [SerializeField] private Volume volume;
    private Bloom[] blooms = new Bloom[5];

    private Camera mainCamera;
    private enum Title_State { Start, Play, Setting, Credits, Quit };
    private Title_State title_State = Title_State.Start;
    private bool Blink;
    private Bloom Click_Obj_Blm;
    private Material Click_Obj_Ma;
    public GameObject Click_Obj;

    [SerializeField] private GameObject Game_Data_UI;
    [SerializeField] private Item_Manager item_Manager;
    private Data_Manager data_Manager;
    private GameObject Data_Slot;


    void Start()
    {
        mainCamera = Camera.main;
        blooms = new Bloom[5];
        targetObjects = new GameObject[5];
        item_Manager = GameObject.FindWithTag("Item_Manager").GetComponent<Item_Manager>();
        data_Manager = GameObject.FindWithTag("Data_Manager").GetComponent<Data_Manager>();
        Data_Slot = GameObject.FindWithTag("Slot");

        for (int i = 0; i < transform.childCount; i++)
        {
            targetObjects[i] = transform.GetChild(i).gameObject;
            transform.GetChild(i).GetComponent<Volume>().profile = Instantiate(transform.GetChild(i).GetComponent<Volume>().profile);
            if (transform.GetChild(i).GetComponent<Volume>().profile.TryGet(out Bloom bloom))
            {
                blooms[i] = bloom;
            }

        }
    }


    Color originalColor;
    Color brightColor = new Color(1f, 1f, 1f, 0f);
    Color dimColor = new Color(0.77f, 0.77f, 0.77f, 0f);
    float blinkLerp = 0f;
    float blinkSpeed = 2f;
    int blinkPhase = 0;

    public void OnButtonClicked(int index)
    {
        data_Manager.Index = index;
        data_Manager.Load(data_Manager.Index);
        
    }
    void BLINK(Material Target)
    {
        if (!Blink) return;

        if (blinkPhase == 0)
        {
            blinkLerp = Mathf.MoveTowards(blinkLerp, 1f, blinkSpeed * Time.deltaTime);
            Color newColor = Color.Lerp(originalColor, brightColor, blinkLerp);
            Target.SetColor("_GlowColor", newColor);

            if (blinkLerp >= 1f)
            {
                blinkPhase = 1;
                blinkLerp = 0f;
            }
        }
        else if (blinkPhase == 1)
        {
            blinkLerp = Mathf.MoveTowards(blinkLerp, 1f, blinkSpeed * Time.deltaTime);
            Color newColor = Color.Lerp(brightColor, dimColor, blinkLerp);
            Target.SetColor("_GlowColor", newColor);

            if (blinkLerp >= 1f)
            {
                Blink = false;
                blinkPhase = 0;
                blinkLerp = 0f;

                if (Click_Obj == targetObjects[1])
                {
                    title_State = Title_State.Play;
                    for (int i = 0; i < Data_Slot.transform.childCount; i++)
                    {
                        if (i >= 1)
                        {
                            Destroy(Data_Slot.transform.GetChild(i).gameObject);
                            break;
                        }
                    }

                    if (data_Manager.Datas.Count == 0)
                    {
                        for (int i = 0; i < Data_Slot.transform.childCount; i++)
                        {
                            if (i == 0)
                            {
                                Transform Child = Data_Slot.transform.GetChild(i);
                                Button btn = Child.GetComponent<Button>();
                                int Temp = i;
                                btn.onClick.AddListener(() => OnButtonClicked(Temp));
                                for (int j = 0; j < Child.childCount; j++)
                                {
                                    Text Info = Child.GetChild(0).GetComponent<Text>();
                                    Info.alignment = TextAnchor.MiddleCenter;
                                    Info.text = "New Game";

                                    Child.GetChild(1).gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                Destroy(Data_Slot.transform.GetChild(i).gameObject);
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = Data_Slot.transform.childCount-1; i > 0; i--) {
                            Destroy(Data_Slot.transform.GetChild(i).gameObject);
                        }

                        for (int i = 0; i < data_Manager.Datas.Count + 1; i++)
                        {
                            if (i == data_Manager.Datas.Count)
                            {
                                GameObject newSlot = Instantiate(Data_Slot.transform.GetChild(0).gameObject);
                                Transform Child = newSlot.transform;
                                Child.SetParent(Data_Slot.transform);
                                Child.localScale = new Vector3(1.3f, 1, 1);
                                Button btn = Child.GetComponent<Button>();
                                int Temp = i;
                                btn.onClick.AddListener(() => OnButtonClicked(Temp));
                                for (int j = 0; j < Child.childCount; j++)
                                {
                                    Text Info = Child.GetChild(0).GetComponent<Text>();
                                    Info.alignment = TextAnchor.MiddleCenter;
                                    Info.text = "New Game";

                                    Child.GetChild(1).gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                SaveData data = data_Manager.Datas[i];
                                if (i == 0)
                                {
                                    Transform Child = Data_Slot.transform.GetChild(i);
                                    Button btn = Child.GetComponent<Button>();
                                    int Temp = i;
                                    btn.onClick.AddListener(() => OnButtonClicked(Temp));
                                    for (int j = 0; j < Child.childCount; j++)
                                    {
                                        Text Info = Child.GetChild(0).GetComponent<Text>();
                                        Info.text = $"{data.map.mapName} / {data.map.PlayTime}\n{data.LastPlayTime}\nHP: {data.player.HP}/{data.player.MaxHP} / ST: {data.player.ST}/{data.player.MaxST}\nMT: {data.player.MT}\nGold: {data.player.coin}";
                                        Transform Inventory = Child.GetChild(1);
                                        if (!data.player.inventory[52].isEmpty)
                                            Inventory.GetChild(0).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[52].itemId];
                                        if (!data.player.inventory[53].isEmpty)
                                            Inventory.GetChild(1).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[53].itemId];
                                        if (!data.player.inventory[56].isEmpty)
                                            Inventory.GetChild(2).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[56].itemId];
                                        if (!data.player.inventory[57].isEmpty)
                                            Inventory.GetChild(3).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[57].itemId];

                                        if (!data.player.inventory[54].isEmpty)
                                            Inventory.GetChild(4).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[54].itemId];
                                        if (!data.player.inventory[55].isEmpty)
                                            Inventory.GetChild(5).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[55].itemId];
                                    }
                                }
                                else
                                {
                                    GameObject newSlot = Instantiate(Data_Slot.transform.GetChild(0).gameObject);
                                    Transform Child = newSlot.transform;
                                    Child.SetParent(Data_Slot.transform);
                                    Child.localScale = new Vector3(1.3f, 1, 1);
                                    Button btn = Child.GetComponent<Button>();
                                    int Temp = i;
                                    btn.onClick.AddListener(() => OnButtonClicked(Temp));
                                    for (int j = 0; j < Child.childCount; j++)
                                    {
                                        Text Info = Child.GetChild(0).GetComponent<Text>();
                                        Info.text = $"{data.map.mapName} / {data.map.PlayTime}\n{data.LastPlayTime}\nHP: {data.player.HP}/{data.player.MaxHP} / ST: {data.player.ST}/{data.player.MaxST}\nMT: {data.player.MT}\nGold: {data.player.coin}";
                                        Transform Inventory = Child.GetChild(1);
                                        if (!data.player.inventory[52].isEmpty)
                                            Inventory.GetChild(0).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[52].itemId];
                                        if (!data.player.inventory[53].isEmpty)
                                            Inventory.GetChild(1).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[53].itemId];
                                        if (!data.player.inventory[56].isEmpty)
                                            Inventory.GetChild(2).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[56].itemId];
                                        if (!data.player.inventory[57].isEmpty)
                                            Inventory.GetChild(3).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[57].itemId];

                                        if (!data.player.inventory[54].isEmpty)
                                            Inventory.GetChild(4).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[54].itemId];
                                        if (!data.player.inventory[55].isEmpty)
                                            Inventory.GetChild(5).GetChild(0).GetComponent<Image>().sprite = item_Manager.images[data.player.inventory[55].itemId];
                                    }
                                }
                            }

                        }
                    }
                }
                else if (Click_Obj == targetObjects[2])
                {
                    title_State = Title_State.Setting;
                }
                else if (Click_Obj == targetObjects[3])
                {
                    title_State = Title_State.Credits;
                }
                else if (Click_Obj == targetObjects[4])
                {
                    title_State = Title_State.Quit;
                }
            }
        }
    }



    void Lojic()
    {
        if (!Blink)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                title_State = Title_State.Start;
                Click_Obj = null;
                Click_Obj_Blm = null;
                Click_Obj_Ma = null;
            }

            for (int i = 1; i < blooms.Length; i++)
            {
                if (Click_Obj_Ma != null)
                {
                    Material material = targetObjects[i].GetComponent<SpriteRenderer>().material;
                    if (targetObjects[i] == DetectHoveredObject2D())
                    {
                        material.SetColor("_GlowColor", Color.Lerp(material.GetColor("_GlowColor"), new Color(0.77f, 0.77f, 0.77f, 0f), 4 * Time.deltaTime));
                    }
                    else if (material != Click_Obj_Ma)
                    {
                        material.SetColor("_GlowColor", Color.Lerp(material.GetColor("_GlowColor"), new Color(0f, 0f, 0f, 1f), 4 * Time.deltaTime));
                    }
                }
                else
                {
                    if (targetObjects[i] == DetectHoveredObject2D())
                    {
                        Material material = targetObjects[i].GetComponent<SpriteRenderer>().material;
                        material.SetColor("_GlowColor", Color.Lerp(material.GetColor("_GlowColor"), new Color(0.77f, 0.77f, 0.77f, 0f), 4 * Time.deltaTime));
                    }
                    else
                    {
                        Material material = targetObjects[i].GetComponent<SpriteRenderer>().material;
                        material.SetColor("_GlowColor", Color.Lerp(material.GetColor("_GlowColor"), new Color(0f, 0f, 0f, 1f), 4 * Time.deltaTime));
                    }
                }

            }

            if (Input.GetMouseButtonDown(0) && DetectHoveredObject2D() != null)
            {

                if (DetectHoveredObject2D().GetComponent<Volume>().profile.TryGet(out Bloom bloom) && (Click_Obj == null || Click_Obj != DetectHoveredObject2D()))
                {
                    Click_Obj = DetectHoveredObject2D();
                    Click_Obj_Ma = Click_Obj.GetComponent<SpriteRenderer>().material;
                    Click_Obj_Blm = bloom;
                    Blink = true;
                }
            }
        }
    }

    void SetCollider(BoxCollider2D collider2D, Vector2 size, Vector2 offset, float speed)
    {
        if (size.y == 0)
            collider2D.size = Vector2.Lerp(collider2D.size, new Vector2(size.x, collider2D.size.y), speed * Time.deltaTime);
        else
            collider2D.size = Vector2.Lerp(collider2D.size, size, speed * Time.deltaTime);
        
        if (offset.y == 0)
            collider2D.offset = Vector2.Lerp(collider2D.offset, new Vector2(offset.x,  collider2D.offset.y), speed * Time.deltaTime);
        else
            collider2D.offset = Vector2.Lerp(collider2D.offset, offset, speed * Time.deltaTime);
        

    }

    void MenuMoving(int Step, float speed, bool logo)
    {
        if (Step == 0)
        {
            Vector3 Pos = new Vector3(0, 0, 0);

            transform.localPosition = Vector3.Lerp(transform.localPosition, Pos, speed * Time.deltaTime);

            targetObjects[0].transform.localPosition = Vector3.Lerp(targetObjects[0].transform.localPosition, new Vector3(0, 5, 10), speed * Time.deltaTime);
            targetObjects[1].transform.localPosition = Vector3.Lerp(targetObjects[1].transform.localPosition, new Vector3(2.4f, 0, 10), speed * Time.deltaTime);
            SetCollider(targetObjects[1].GetComponent<BoxCollider2D>(), new Vector2(12, 0), new Vector2(-2.4f, 0), speed);

            targetObjects[2].transform.localPosition = Vector3.Lerp(targetObjects[2].transform.localPosition, new Vector3(0.8f, -2.2f, 10), speed * Time.deltaTime);
            SetCollider(targetObjects[2].GetComponent<BoxCollider2D>(), new Vector2(12, 0), new Vector2(-0.8f, 0), speed);

            targetObjects[3].transform.localPosition = Vector3.Lerp(targetObjects[3].transform.localPosition, new Vector3(1.25f, -4.4f, 10), speed * Time.deltaTime);
            SetCollider(targetObjects[3].GetComponent<BoxCollider2D>(), new Vector2(12, 0), new Vector2(-1.25f, 0), speed);

            targetObjects[4].transform.localPosition = Vector3.Lerp(targetObjects[4].transform.localPosition, new Vector3(2.5f, -6.6f, 10), speed * Time.deltaTime);
            SetCollider(targetObjects[4].GetComponent<BoxCollider2D>(), new Vector2(12, 0), new Vector2(-2.5f, 0), speed);

            RectTransform rect = Game_Data_UI.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, new Vector2(1300, 0), 3 * Time.deltaTime);
        }
        else if (Step == 1)
        {
            Vector3 Pos = new Vector3(-10.5f, 0, 0);

            transform.localPosition = Vector3.Lerp(transform.localPosition, Pos, speed * Time.deltaTime);

            targetObjects[0].transform.localPosition = Vector3.Lerp(targetObjects[0].transform.localPosition, new Vector3(0, 5, 10), speed * Time.deltaTime);

            targetObjects[1].transform.localPosition = Vector3.Lerp(targetObjects[1].transform.localPosition, new Vector3(0.4f, 0, 10), speed * Time.deltaTime);
            SetCollider(targetObjects[1].GetComponent<BoxCollider2D>(), new Vector2(11, 0), new Vector2(-0.4f, 0), speed);

            targetObjects[2].transform.localPosition = Vector3.Lerp(targetObjects[2].transform.localPosition, new Vector3(0.4f, -2.2f, 10), speed * Time.deltaTime);
            SetCollider(targetObjects[2].GetComponent<BoxCollider2D>(), new Vector2(11, 0), new Vector2(-0.4f, 0), speed);

            targetObjects[3].transform.localPosition = Vector3.Lerp(targetObjects[3].transform.localPosition, new Vector3(0.4f, -4.4f, 10), speed * Time.deltaTime);
            SetCollider(targetObjects[3].GetComponent<BoxCollider2D>(), new Vector2(11, 0), new Vector2(-0.4f, 0), speed);

            targetObjects[4].transform.localPosition = Vector3.Lerp(targetObjects[4].transform.localPosition, new Vector3(0.4f, -6.6f, 10), speed * Time.deltaTime);
            SetCollider(targetObjects[4].GetComponent<BoxCollider2D>(), new Vector2(11, 0), new Vector2(-0.4f, 0), speed);

            RectTransform rect = Game_Data_UI.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, new Vector2(0, 0), 3 * Time.deltaTime);
        }
    }

    void Update()
    {
        
        BLINK(Click_Obj_Ma);
        Lojic();



        switch (title_State)
        {
            case Title_State.Start:
                MenuMoving(0, 3, true);
                break;

            case Title_State.Play:
                MenuMoving(1, 3, true);
                break;

            case Title_State.Setting:
                MenuMoving(0, 3, true);
                break;

            case Title_State.Credits:
                MenuMoving(1, 3, true);
                break;

        }

    }

    public GameObject DetectHoveredObject2D()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;

            for (int i = 0; i < targetObjects.Length; i++)
            {
                if (hitObject == targetObjects[i])
                {
                    return targetObjects[i];
                }
            }
        }

        return null;
    }
}
