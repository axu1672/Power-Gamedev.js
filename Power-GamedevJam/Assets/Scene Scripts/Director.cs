using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public Canvas GUI_Canvas;
    public TMP_Text GUI_TemporaryTimer;
    // Start is called before the first frame update

    //Game State Management
    private GameRound round;
    private CurrentGameState gameState;

    void Start()
    {
        if (Player == null) throw new Exception("Director needs Player Target");
        PC = Player.GetComponent<PlayerController>();
        if (PC == null) throw new Exception("Director could not find Player Controller");
        if (Enemy_Prefab == null) throw new Exception("Resource: Enemy Prefab is missing at runtime...");
        if (PartyMember_Prefab == null) throw new Exception("Resource: Party Member Prefab is missing at runtime...");
        if (GUI_Canvas == null) throw new Exception("Missing Canvas reference");

        EnemyControllers = new Dictionary<string, NPCController>();
        Enemies = new Dictionary<string, GameObject>();

        if (DebugMode)
        {
            //for (int i = 0; i < 10; i ++)
            //{
            //    var spawned = SpawnNPC(new Vector2(i, i));
            //}
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

        //GUI_TemporaryTimer = GUI_Canvas.GetComponent<TMP_Text>();

        round = new GameRound();
        // Initialized on combat state
        gameState = CurrentGameState.Combat;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == CurrentGameState.Combat)
        {
            if (round.RoundTimer >= round.nextSpawnThreshold)
            {
                var playerPos = PC.PlayerPosition;
                var pos = round.SpawnWave_Vectors(playerPos);

                round.nextSpawnThreshold += 15;
                foreach (var p in pos)
                {
                    var spawned = SpawnNPC(p);
                }
            }

            if (DebugMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log($"Position: {Input.mousePosition}");
                }
            }
            UpdateEnemyTarget();

            round.RoundTimer += Time.deltaTime;
            GUI_TemporaryTimer.text = $"{round.RoundTimer} seconds";
            if (round.RoundTimer > round.MAX_DURATION_SECONDS)
            {
                // Check if the player is still alive
                //TransitionToIntermission();
            }
        }
        else
        {

        }
        
    }

    private void TransitionToIntermission()
    {
        gameState = CurrentGameState.Intermission;
    }

    private void UpdateEnemyTarget()
    {
        var target = PC.PlayerPosition;
        foreach (NPCController n in EnemyControllers.Values)
        {
            var current = n.Position;
            if (current != Vector2.negativeInfinity)
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
                    target = closestTarget;
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

public enum CurrentGameState 
{
    Combat,
    Intermission
}

public class GameRound
{
    public int MAX_DURATION_SECONDS
    {
        get
        {
            return 300;
        }
    }
    public int ROUND_NUMBER
    {
        get;set;
    }
    public int MAX_SPAWN_COUNT
    {
        get
        {
            return ROUND_NUMBER * 100;
        }
    }
    public int SPAWN_RATE
    {
        get
        {
            var fifteenSecondBatch = MAX_SPAWN_COUNT / 15;
            return fifteenSecondBatch;
        }
    }

    public float RoundTimer = 0f;
    public int CurrentSpawned = 0;

    public int nextSpawnThreshold = 15;

    public GameRound()
    {
        ROUND_NUMBER = 1;
    }

    public void IncrementRound()
    {
        ROUND_NUMBER++;
        CurrentSpawned = 0;
    }

    public List<Vector2> SpawnWave_Vectors(Vector2 playerPos)
    {
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i < SPAWN_RATE; i ++)
        {
            if (CurrentSpawned < MAX_SPAWN_COUNT)
            {
                var vi = GenerateSpawnPoint(playerPos);
                result.Add(vi);
                CurrentSpawned++;
            }          
        }
        return result;
    }

    private Vector2 GenerateSpawnPoint(Vector2 playerPos)
    {
        float x =  UnityEngine.Random.Range(-1f, 1f);
        float y = UnityEngine.Random.Range(-1f, 1f);
        var dir = new Vector2(x, y);
        float dist = UnityEngine.Random.Range(10, 30);

        return (dir.normalized * dist) + playerPos;
    }
}

public interface NPCController
{
    public bool IsDead { get; }
    public void SetTarget(Vector2 target);

    public Vector2 Position { get; }

    public void Bump(Vector2 force);
}

