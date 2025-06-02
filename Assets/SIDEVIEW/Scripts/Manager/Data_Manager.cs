using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int HP;
    public int MaxHP;
    public int ST;
    public int MaxST;
    public int MT;
    public Vector2 position;
    public Player_Controller.AttackState attackState;
    public Player_Controller.JumpState jumpState;
    public Player_Controller.MoveState moveState;
    public int coin;
    public List<InventorySlot> inventory = new List<InventorySlot>();
}

[System.Serializable]
public class MonsterData
{
    public Vector2 position;
    public Monster.Monster_State monsterState;
    public int coin;
}

[System.Serializable]
public class MapData
{
    public string mapName;
    public float PlayTime;
    public int tutorial_Step;
    public List<MonsterData> monsters = new List<MonsterData>();
}

[System.Serializable]
public class SaveData
{
    public float LastPlayTime;
    public PlayerData player;
    public MapData map;
}

[System.Serializable]
public class RootSaveData
{
    public List<SaveData> Datas = new List<SaveData>();
}

public class Data_Manager : MonoBehaviour
{
    private GameObject player;
    private Player_Controller player_Controller;

    private Map_Manager map_Manager;
    private Game_Manager game_Manager;

    private string saveDirectory = @"C:\LostMemory";
    private string saveFile => Path.Combine(saveDirectory, "Data.json");
    public List<SaveData> Datas = new List<SaveData>();
    public SaveData curent_Data; //{ get; private set; }
    public bool Data_Scan; //{ get; private set; }
    public int Index;
    public bool Data_Main;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player_Controller = player.GetComponent<Player_Controller>();
        }

        if (GameObject.FindWithTag("Map_Manager") != null)
            map_Manager = GameObject.FindWithTag("Map_Manager").GetComponent<Map_Manager>();
        if (GameObject.FindWithTag("Game_Manager") != null)
            game_Manager = GameObject.FindWithTag("Game_Manager").GetComponent<Game_Manager>();

        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
            Debug.Log("폴더 생성됨: " + saveDirectory);

            RootSaveData emptyData = new RootSaveData();
            string emptyJson = JsonUtility.ToJson(emptyData, true);
            File.WriteAllText(saveFile, emptyJson);
            Debug.Log("빈 JSON 파일 생성됨: " + saveFile);
        }
        Load(0);
    }

    void Update()
    {
        if (player == null && Data_Main) {
            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player_Controller = player.GetComponent<Player_Controller>();
            }

            if (GameObject.FindWithTag("Map_Manager") != null)
                map_Manager = GameObject.FindWithTag("Map_Manager").GetComponent<Map_Manager>();
            if (GameObject.FindWithTag("Game_Manager") != null)
                game_Manager = GameObject.FindWithTag("Game_Manager").GetComponent<Game_Manager>();
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) && Data_Main) {
            Save();
        }
    }
    
    public void Save()
    {
        RootSaveData rootData;

        if (File.Exists(saveFile))
        {
            string jsonOld = File.ReadAllText(saveFile);
            rootData = JsonUtility.FromJson<RootSaveData>(jsonOld);
        }
        else
        {
            rootData = new RootSaveData();
        }

        if (rootData.Datas == null)
            rootData.Datas = new List<SaveData>();

        float totalPlayTimeInHours = Time.realtimeSinceStartup / 3600f;

        SaveData newSave = new SaveData();
        newSave.LastPlayTime = (float)Math.Round(totalPlayTimeInHours, 1);

        newSave.player = new PlayerData
        {
            HP = player_Controller.C_Hp,
            MaxHP = player_Controller.C_Max_Hp,
            ST = player_Controller.C_St,
            MaxST = player_Controller.C_Max_St,
            MT = player_Controller.MT,
            position = player.transform.position,
            attackState = player_Controller.Attack_Sys,
            jumpState = player_Controller.Jump_Sys,
            moveState = player_Controller.Move_Sys,
            coin = player_Controller.coin,
            inventory = player_Controller.inventory
        };

        List<MonsterData> monsterDatas = new List<MonsterData>();
        for (int i = 0; i < game_Manager.Monsters_OBJ.Count; i++)
        {
            if (game_Manager.Monsters_OBJ != null)
            {
                MonsterData monsterData = new MonsterData
                {
                    position = game_Manager.Monsters_OBJ[i].transform.position,
                    monsterState = game_Manager.Monsters_Sc[i].monster,
                    coin = game_Manager.Monsters_Sc[i].coin_count
                };
                monsterDatas.Add(monsterData);
            }
        }

        newSave.map = new MapData
        {
            tutorial_Step = game_Manager.cut_Level,
            PlayTime = 1.1f,
            mapName = map_Manager.Map_Name,
            monsters = monsterDatas
        };

        // 기존 리스트에 Index번째 덮어쓰기 or 새로 추가
        if (rootData.Datas.Count > Index)
        {
            rootData.Datas[Index] = newSave;
        }
        else
        {
            // Index가 범위를 초과하면 빈 슬롯을 채우고 추가
            while (rootData.Datas.Count < Index)
                rootData.Datas.Add(new SaveData()); // 빈 데이터로 채움

            rootData.Datas.Add(newSave);
        }

        string jsonString = JsonUtility.ToJson(rootData, true);
        File.WriteAllText(saveFile, jsonString);
        Debug.Log($"저장 완료: {saveFile} (Index: {Index})");
    }

    public void Load(int index)
    {
        if (!File.Exists(saveFile))
        {
            Debug.LogWarning("저장된 파일이 존재하지 않습니다.");
            return;
        }

        string jsonString = File.ReadAllText(saveFile);
        RootSaveData rootData = JsonUtility.FromJson<RootSaveData>(jsonString);
        Datas = new List<SaveData>();
        Debug.Log($"데이터 초기화 완료");

        for (int i = 0; i < rootData.Datas.Count; i++)
        {
            Datas.Add(rootData.Datas[i]);
            Debug.Log($"로드 완료: {i}번째 데이터");
        }

        Data_Scan = true;

        if (rootData.Datas.Count <= index)
        {
            Debug.LogWarning("해당 인덱스의 세이브 데이터가 없습니다.");
            return;
        }

        curent_Data = rootData.Datas[index];
        
        Debug.Log("로드 완료: " + JsonUtility.ToJson(curent_Data, true));

        // 필요한 데이터 적용은 여기서 구현
    }
}