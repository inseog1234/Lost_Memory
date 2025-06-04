using UnityEngine;

public class Item : MonoBehaviour
{
    public int index;
    public string Name;
    public string Description;
    public int Rank { get; private set; }
    public int Type { get; private set; } // 무기 : 0, 방어구 : 1, 소모품 : 2, 재료 : 3, 재화 : 4, 중요한 물건 : 5
    public int Type_A { get; private set; }
    public int Type_B { get; private set; }
    public int Type_Armor { get; private set; } // 방어구 | 투구 : 0, 흉갑 : 1, 각반 : 2, 부츠 : 3
    public int Type_Weapon { get; private set; } // 무기 | 왼손 : 0, 오른손 : 1, 양손 : 2
    public int Type_Weapon_Type { get; private set; } // 무기 | 단검 : 0, 너클 : 1, 장검 : 2, 세검 : 3, 철퇴 : 4
    public int count_lim { get; private set; }
    public bool Fake;

    private Item_Manager item_Manager;

    void Awake()
    {
        item_Manager = FindAnyObjectByType<Item_Manager>();
    }

    void Update()
    {
        if (Name == "" && Description == "" && !Fake)
        {
            Destroy(this.gameObject);
            Fake = true;
        }
    }

    public void SetItem(ItemData data)
    {
        index = data.index;
        Name = data.Name;
        Description = data.description;
        Rank = data.rank;
        Type = data.type;
        Type_A = data.type_A;
        Type_B = data.type_B;
        count_lim = data.count_lim;

    }
}