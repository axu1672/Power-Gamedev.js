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
    public bool SpawnPartyMember = false;
    // Spawn References
    public GameObject Enemy_Prefab;
    public GameObject PartyMember_Prefab;
    // Start is called before the first frame update
    void Start()
    {
        if (Player == null) throw new Exception("Director needs Player Target");
        PC = Player.GetComponent<PlayerController>();
        if (PC == null) throw new Exception("Director could not find Player Controller");
        if (Enemy_Prefab == null) throw new Exception("Resource: Enemy Prefab is missing at runtime...");
        if (PartyMember_Prefab == null) throw new Exception("Resource: Party Member Prefab is missing at runtime...");

        EnemyControllers = new Dictionary<string, NPCController>();
        Enemies = new Dictionary<string, GameObject>();

        if (DebugMode)
        {
            for (int i = 0; i < 10; i ++)
            {
                var spawned = SpawnNPC(new Vector2(i, i));
            }
            //var spawned = SpawnNPC(new Vector2(1, 1));
            if (SpawnPartyMember)
            {
                PartyMember m = new PartyMember(PartyMemberClass.Ranger);
                AddCharacterToParty(m);

                PartyMember m2 = new PartyMember(PartyMemberClass.Mage);
                AddCharacterToParty(m2);
            }
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
            var current = n.Position;
            if (current != null)
            {
                var dist = (target - current).magnitude;
                if (dist <= 2)
                {
                    var mindist = dist;
                    Vector2 closestTarget = target;
                    foreach (PartyController p in PC.partyMembers)
                    {
                        var pdist = (p.Position - current).magnitude;
                        if (pdist < mindist)
                        {
                            mindist = pdist;
                            closestTarget = p.Position;
                        }
                    }
                }
            }
            n.SetTarget(target);
        }
    }

    private GameObject SpawnNPC(Vector2 spawn, string id = "Enemy", bool spawnActive = true)
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

    private void KillNPC(string gid, bool destroy = true)
    {
        var delete = Enemies[gid];
        Enemies.Remove(gid);
        EnemyControllers.Remove(gid);

        delete.active = false;
        if (destroy) Destroy(delete);

        SpawnedIDs.Remove(gid);
    }

    private GameObject AddCharacterToParty(PartyMember toSpawn)
    {
        float x = UnityEngine.Random.Range(-1, 1.1f);
        float y = UnityEngine.Random.Range(-1, 1.1f);
        Vector2 offset = new Vector2(x, y);
        Vector2 pos = PC.PlayerPosition + offset;
        var spawned = Instantiate(PartyMember_Prefab, pos, new Quaternion());
        string gid = $"Party-{Guid.NewGuid()}";
        spawned.name = gid;
        var controller = spawned.GetComponent<PartyController>();
        PC.partyMembers.Add(controller);
        controller.InitializeMember(toSpawn);
        return null;
    }
}

public interface NPCController
{
    public bool IsDead { get; }
    public void SetTarget(Vector2 target);

    public Vector2 Position { get; }
}
