using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public GameObject Player;
    public PlayerController PC;
    private List<string> SpawnedIDs = new List<string>();
    public Dictionary<string, GameObject> Enemies;
    public Dictionary<string, NPCController> EnemyControllers;
    public bool DebugMode = false;
    // Spawn References
    public GameObject Enemy_Prefab;
    // Start is called before the first frame update
    void Start()
    {
        if (Player == null) throw new Exception("Director needs Player Target");
        PC = Player.GetComponent<PlayerController>();
        if (PC == null) throw new Exception("Director could not find Player Controller");
        if (Enemy_Prefab == null) throw new Exception("Resource: Enemy Prefab is missing at runtime...");

        EnemyControllers = new Dictionary<string, NPCController>();
        Enemies = new Dictionary<string, GameObject>();

        if (DebugMode)
        {
            var spawned = SpawnNPC(new Vector2(1, 1));
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Position: {Input.mousePosition}");
            }
        }
        UpdateEnemyTarget();
    }

    private void UpdateEnemyTarget()
    {
        var target = PC.PlayerPosition;
        foreach (NPCController n in EnemyControllers.Values)
        {
            n.SetTarget(target);
        }
    }

    private GameObject SpawnNPC(Vector2 spawn, string id = "ID", bool spawnActive = true)
    {
        var spawned = Instantiate(Enemy_Prefab, spawn, new Quaternion());
        string gid = $"{id}-{Guid.NewGuid()}";
        spawned.name = gid;
        Enemies[gid] = spawned;
        EnemyControllers[gid] = spawned.GetComponent<NPCController>();
        spawned.active = spawnActive;
        SpawnedIDs.Add(gid);
        return spawned;
    }

    private void KillNPC(string gid)
    {

    }
}
