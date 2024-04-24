using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonNPC : MonoBehaviour, NPCController
{
    private Vector2 MoveTarget;
    private Transform trn;
    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    private const int ATTACK_COOLDOWN_FRAMES = 60;
    private int attackCooldown = 0;

    public bool IsDead
    {
        get
        {// TODO
            return false;
        }
    }

    public Vector2 Position
    {
        get
        {
            if (trn == null) return Vector2.negativeInfinity;
            return new Vector2(trn.position.x, trn.position.y);
        }
        private set
        {
            trn.position = new Vector3(value.x, value.y, trn.position.z);
        }
    }
    public void SetTarget(Vector2 target)
    {
        MoveTarget = target;
    }

    // Start is called before the first frame update
    void Start()
    {
        trn = gameObject.transform;
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = MoveTarget - Position;
        rb.velocity = dir.normalized * 0.6f;
        if (attackCooldown > 0)
            attackCooldown--;
    }

    private void Attack(GameObject op)
    {
        Debug.Log($"Hitting: {op.name} for 1 dmg");
        if (op.name.Contains("Player"))
        {
            var controller = op.GetComponent<PlayerController>();
        }
        else if (op.name.Contains("Party"))
        {
            var controller = op.GetComponent<PartyController>();
            if (controller != null)
            {
                Vector2 force = (MoveTarget - Position).normalized * 100;
                controller.Bump(force);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.name.Contains("Player") || c.gameObject.name.Contains("Party"))
        {
            if (attackCooldown == 0)
            {
                Attack(c.gameObject);
                attackCooldown += ATTACK_COOLDOWN_FRAMES;
            }
        }
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.gameObject.name.Contains("Player"))
        {
            if (attackCooldown == 0)
            {
                Attack(c.gameObject);
                attackCooldown += ATTACK_COOLDOWN_FRAMES;
            }
        }
    }

    public void Bump(Vector2 force)
    {

    }
}

public class SkeletonBase
{
    private int health = 100;
    public int Health
    {
        get
        {
            return health;
        }
    }

    
}
