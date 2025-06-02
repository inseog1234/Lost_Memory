using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class ItemData {
    public int index;
    public string Name;
    public string description;
    public int rank;
    public int type;
    public int type_A;
    public int type_B;
    public int count_lim;
}


public class Item_Manager : MonoBehaviour
{
    public string csvURL;
    public GameObject Item_Prefabs;
    public List<ItemData> itemList = new List<ItemData>();
    public List<Sprite> images;
    public List<GameObject> Field_Items;
    public ItemData Create_Item(Vector2 position, int index)
    {
        GameObject item = Instantiate(Item_Prefabs, position, Quaternion.identity);
        ItemData itemData = new ItemData();
        itemData.index = index;
        itemData.Name = itemList[index].Name;
        itemData.description = itemList[index].description;
        itemData.rank = itemList[index].rank;
        itemData.type = itemList[index].type;
        itemData.type_A = itemList[index].type_A;
        itemData.type_B = itemList[index].type_B;
        itemData.count_lim = itemList[index].count_lim;
        Item item_Property = item.GetComponent<Item>();
        item.GetComponent<SpriteRenderer>().sprite = images[index];


        if (itemList[index].rank == 1)
        {
            item.GetComponent<SpriteRenderer>().color = new Color(50f / 255f, 50f / 255f, 255f / 255f);
        }
        else if (itemList[index].rank == 2)
        {
            item.GetComponent<SpriteRenderer>().color = new Color(255f / 255f, 50f / 255f, 255f / 255f);
        }

        item_Property.SetItem(itemData);
        Field_Items.Add(item);
        return itemData;
    }

    private void Start() {
        csvURL = "https://docs.google.com/spreadsheets/d/1Hur5QYDkFhI9mZumyyFZUXRNp6jXbrskP6ilE-9rFrI/export?format=csv&gid=1081823634";
        StartCoroutine(LoadCSV());    
    }

    private IEnumerator LoadCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(csvURL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string[] lines = www.downloadHandler.text.Split('\n');

            for (int i = 15; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] data = ParseCSVLine(lines[i]);

                if (data.Length < 21) continue;

                string rawName = data[16].Trim();

                if (string.IsNullOrWhiteSpace(rawName)) continue;

                ItemData item = new ItemData();

                int.TryParse(data[15].Trim(), out item.index);
                item.Name = rawName.Replace("\\n", "\n");
                item.description = data[17].Trim().Replace("\\n", "\n");
                int.TryParse(data[18].Trim(), out item.rank);
                int.TryParse(data[19].Trim(), out item.type);
                int.TryParse(data[20].Trim(), out item.type_A);
                int.TryParse(data[21].Trim(), out item.type_B);
                int.TryParse(data[22].Trim(), out item.count_lim);

                itemList.Add(item);
            }
        }
    }

    private string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        result.Add(current);

        return result.ToArray();
    }
}
