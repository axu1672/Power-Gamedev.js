using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonNPC : MonoBehaviour, NPCController
{
    private Vector2 MoveTarget;
    private Transform trn;
    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    public Vector2 Position
    {
        get
        {
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
        rb.velocity = dir;
    }
}

public interface NPCController 
{
    public void SetTarget(Vector2 target);
}

