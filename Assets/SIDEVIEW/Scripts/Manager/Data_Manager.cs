using System;
using System.Collections.Generic;
using System.IO;
using Codice.Client.BaseCommands;
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
    public bool Data_RePlace;

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

            RootSaveData emptyData = new RootSaveData();
            string emptyJson = JsonUtility.ToJson(emptyData, true);
            File.WriteAllText(saveFile, emptyJson);
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

        if (Input.GetKeyDown(KeyCode.Alpha7) && Data_Main)
        {
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
        Load(Index);
    }

    public void Load()
    {
        if (!File.Exists(saveFile))
        {
            return;
        }

        string jsonString = File.ReadAllText(saveFile);
        RootSaveData rootData = JsonUtility.FromJson<RootSaveData>(jsonString);
        Datas = new List<SaveData>();

        for (int i = 0; i < rootData.Datas.Count; i++)
        {
            Datas.Add(rootData.Datas[i]);
        }

        Data_Scan = true;
        // 필요한 데이터 적용은 여기서 구현
    }

    public void Load(int index)
    {
        if (!File.Exists(saveFile))
        {
            return;
        }

        string jsonString = File.ReadAllText(saveFile);
        RootSaveData rootData = JsonUtility.FromJson<RootSaveData>(jsonString);
        Datas = new List<SaveData>();

        for (int i = 0; i < rootData.Datas.Count; i++)
        {
            Datas.Add(rootData.Datas[i]);
        }

        Data_Scan = true;

        if (rootData.Datas.Count <= index)
        {
            curent_Data = null;
            return;
        }

        curent_Data = rootData.Datas[index];

        // 필요한 데이터 적용은 여기서 구현

        while (true)
        {
            if (player != null && !Data_RePlace)
            {
                player.transform.position = curent_Data.player.position;
                player_Controller.Attack_Sys = curent_Data.player.attackState;
                player_Controller.Jump_Sys = curent_Data.player.jumpState;
                player_Controller.Move_Sys = curent_Data.player.moveState;
                player_Controller.coin = curent_Data.player.coin;
                player_Controller.inventory = curent_Data.player.inventory;

                Debug.Log("ss");

                for (int i = 0; i < game_Manager.Monsters_OBJ.Count; i++)
                {
                    // game_Manager.Monsters_OBJ[i]
                    Destroy(game_Manager.Monsters_OBJ[i], 0.01f);
                    game_Manager.Monsters_OBJ.Remove(game_Manager.Monsters_OBJ[i]);
                    game_Manager.Monsters_Sc.Remove(game_Manager.Monsters_Sc[i]);
                }

                for (int i = 0; i < curent_Data.map.monsters.Count; i++)
                {
                    GameObject monster = game_Manager.CreateMonster(game_Manager.Monsters[2], curent_Data.map.monsters[i].position);
                    monster.GetComponent<Monster>().SetGold(curent_Data.map.monsters[i].coin);
                }

                game_Manager.cut_Level = curent_Data.map.tutorial_Step;
                map_Manager.Map_Name = curent_Data.map.mapName;
                Data_RePlace = true;
                break;
            }
        }
    }
}