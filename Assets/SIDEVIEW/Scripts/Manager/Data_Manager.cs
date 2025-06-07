using System;
using System.Collections.Generic;
using System.IO;
using Codice.Client.BaseCommands;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int HP;
    public int MaxHP;
    public int ST;
    public int MaxST;
    public int MT;
    public int Dir_C;

    public Vector2 position;
    public Player_Controller.AttackState attackState;
    public Player_Controller.JumpState jumpState;
    public Player_Controller.MoveState moveState;
    public bool idleState;
    public int coin;
    public bool cutScene;
    public bool Attack_Trigger;
    public bool Move_Trigger;
    public List<InventorySlot> inventory = new List<InventorySlot>();
}

[System.Serializable]
public class MonsterData
{
    public int Type;
    public Vector2 position;
    public Monster.Monster_State monsterState;
    public int coin;
}

[System.Serializable]
public class Item_Data
{
    public int index;
    public Vector2 position;
}

[System.Serializable]
public class MapData
{
    public bool Scene;
    public string mapName;
    public float PlayTime;
    public int tutorial_Step;
    public int C_Map_C;
    public int P_Map;
    public int C_Map;
    public Vector2 Cam_Pos;
    public bool Cam_Scene;
    public List<MonsterData> monsters = new List<MonsterData>();
    public List<Item_Data> Field_Items = new List<Item_Data>();
    public bool Basic_Game_Kit;
    public bool Set_2;
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
    private Item_Manager item_Manager;

    private string saveDirectory = @"C:\LostMemory";
    private string saveFile => Path.Combine(saveDirectory, "Data.json");
    private string saveFile_False => Path.Combine(saveDirectory, "Data_Old.json");
    public List<SaveData> Datas = new List<SaveData>();
    public SaveData curent_Data; //{ get; private set; }
    public bool Data_Scan; //{ get; private set; }
    public int Index;
    public bool Data_Main;
    public bool Data_RePlace;

    private GameObject Loading_Canvas;
    private Loading_UI loading_UI;
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
        if (GameObject.FindWithTag("Item_Manager") != null) 
            item_Manager = GameObject.FindWithTag("Item_Manager").GetComponent<Item_Manager>();
        if (GameObject.FindWithTag("Loading_UI") != null)
        {
            Loading_Canvas = GameObject.FindWithTag("Loading_UI");
            loading_UI = Loading_Canvas.GetComponent<Loading_UI>();
        }

        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);

            RootSaveData emptyData = new RootSaveData();
            string emptyJson = JsonUtility.ToJson(emptyData, true);
            File.WriteAllText(saveFile, emptyJson);
        }
        Load();
    }

    void Update()
    {
        // if (player == null && Data_Main)
        // {


        // }
        
        if (GameObject.FindWithTag("Player") != null && player == null)
        {
            player = GameObject.FindWithTag("Player");
            player_Controller = player.GetComponent<Player_Controller>();
        }

        if (GameObject.FindWithTag("Map_Manager") != null && map_Manager== null)
            map_Manager = GameObject.FindWithTag("Map_Manager").GetComponent<Map_Manager>();
        if (GameObject.FindWithTag("Game_Manager") != null && game_Manager== null)
            game_Manager = GameObject.FindWithTag("Game_Manager").GetComponent<Game_Manager>();
        if (GameObject.FindWithTag("Item_Manager") != null &&item_Manager == null)
            item_Manager = GameObject.FindWithTag("Item_Manager").GetComponent<Item_Manager>();

        if (Input.GetKeyDown(KeyCode.Alpha7) && Data_Main)
        {
            Save();
        }

        Loading_Canvas.SetActive(true);
    }
    public void Save_False()
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
            inventory = player_Controller.inventory,
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
        File.WriteAllText(saveFile_False, jsonString);
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
            idleState = player_Controller.Idle_Sys,
            coin = player_Controller.coin,
            cutScene = player_Controller.cutScene,
            Attack_Trigger = player_Controller.Attack_Trigger,
            Move_Trigger = player_Controller.Move_Trigger,
            inventory = player_Controller.inventory
        };

        List<MonsterData> monsterDatas = new List<MonsterData>();
        for (int i = 0; i < game_Manager.Monsters_OBJ.Count; i++)
        {
            if (game_Manager.Monsters_OBJ != null)
            {
                MonsterData monsterData = new MonsterData
                {
                    Type = game_Manager.Monsters_Sc[i].Type,
                    position = game_Manager.Monsters_OBJ[i].transform.position,
                    monsterState = game_Manager.Monsters_Sc[i].monster,
                    coin = game_Manager.Monsters_Sc[i].coin_count
                };
                monsterDatas.Add(monsterData);
            }
        }

        List<Item_Data> item_Datas = new List<Item_Data>();
        for (int i = 0; i < item_Manager.Field_Items.Count; i++) {
            if (item_Manager.Field_Items != null)
            {
                Item_Data item_Data = new Item_Data
                {
                    index = item_Manager.Field_Items[i].GetComponent<Item>().index,
                    position = item_Manager.Field_Items[i].transform.position
                };
                item_Datas.Add(item_Data);
            }
        }
        
        newSave.map = new MapData
        {
            Scene = game_Manager.cutSceneMod,
            tutorial_Step = game_Manager.cut_Level,
            PlayTime = game_Manager.Game_Time,
            mapName = map_Manager.Map_Name,
            C_Map_C = map_Manager.C_Map_C,
            P_Map = map_Manager.P_Map,
            C_Map = map_Manager.C_Map,
            Cam_Pos = map_Manager.uI_Manager.camera.transform.position,
            Cam_Scene = map_Manager.uI_Manager.camera.GetComponent<Cam>().cutScene,
            monsters = monsterDatas,
            Field_Items = item_Datas,
            Basic_Game_Kit = game_Manager.Basic_Game_Kit,
            Set_2 = game_Manager.Set_2
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
        
        
        if (player != null && !Data_RePlace)
        {
            int Count = curent_Data.map.Field_Items.Count;
            
            player.transform.position = curent_Data.player.position;
            player_Controller.Attack_Sys = curent_Data.player.attackState;
            player_Controller.Jump_Sys = curent_Data.player.jumpState;
            player_Controller.Move_Sys = curent_Data.player.moveState;
            player_Controller.Idle_Sys = curent_Data.player.idleState;
            player_Controller.coin = curent_Data.player.coin;
            player_Controller.cutScene = curent_Data.player.cutScene;
            player_Controller.Attack_Trigger = curent_Data.player.Attack_Trigger;
            player_Controller.Move_Trigger = curent_Data.player.Move_Trigger;
            player_Controller.inventory = curent_Data.player.inventory;

            int Step_count = game_Manager.Monsters_OBJ.Count;
            for (int i = 0; i < Step_count; i++)
            {
                // game_Manager.Monsters_OBJ[i]
                
                Destroy(game_Manager.Monsters_OBJ[i], 0.01f);
                game_Manager.Monsters_OBJ.Remove(game_Manager.Monsters_OBJ[i]);
                game_Manager.Monsters_Sc.Remove(game_Manager.Monsters_Sc[i]);
            }

            Step_count = curent_Data.map.monsters.Count;
            for (int i = 0; i < Step_count; i++)
            {
                if (i < Step_count) {
                    GameObject monster = game_Manager.CreateMonster(game_Manager.Monsters[curent_Data.map.monsters[i].Type], curent_Data.map.monsters[i].position);
                    monster.GetComponent<Monster>().SetGold(curent_Data.map.monsters[i].coin);
                    monster.transform.SetParent(GameObject.FindWithTag("WeakMapGrid").transform);
                }
            }

            Step_count = curent_Data.map.Field_Items.Count;
            for (int i = 0; i < Step_count; i++) {
                GameObject Item = item_Manager.Create_Item(curent_Data.map.Field_Items[i].position, curent_Data.map.Field_Items[i].index);
            }


            game_Manager.Game_Time = curent_Data.map.PlayTime;
            game_Manager.cutSceneMod = curent_Data.map.Scene;
            game_Manager.cut_Level = curent_Data.map.tutorial_Step;
            map_Manager.Map_Name = curent_Data.map.mapName;
            map_Manager.C_Map_C = curent_Data.map.C_Map_C;
            map_Manager.P_Map = curent_Data.map.P_Map;
            map_Manager.C_Map = curent_Data.map.C_Map;
            map_Manager.uI_Manager.camera.GetComponent<Cam>().SetBorder(map_Manager.Cam_Mov_Bd[map_Manager.C_Map_C]);
            map_Manager.uI_Manager.camera.GetComponent<Cam>().SetPosition(curent_Data.map.Cam_Pos.x, curent_Data.map.Cam_Pos.y);
            map_Manager.uI_Manager.camera.GetComponent<Cam>().SetCutScene(curent_Data.map.Cam_Scene);
            game_Manager.Basic_Game_Kit = curent_Data.map.Basic_Game_Kit;
            game_Manager.Set_2 = curent_Data.map.Set_2;
            Data_RePlace = true;
        }
    }
}