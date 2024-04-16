using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float bSpeed = 0.1f;
    private Transform trn;
    private SpriteRenderer renderer;
    public Vector2 PlayerPosition
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
    bool move = false;
    private Vector2 TargetLocation;
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
        bool Up = Input.GetKey(KeyCode.W);
        bool Down = Input.GetKey(KeyCode.S);
        bool Left = Input.GetKey(KeyCode.A);
        bool Right = Input.GetKey(KeyCode.D);
        int HorizontalMovement = 0;
        int VerticalMovement = 0;
        if ((!Up && !Down) || (Up && Down))
        {
            VerticalMovement = 0;
        }
        else
        {
            VerticalMovement = Up ? 1 : -1;
        }

        if ((!Left && !Right) || (Left && Right))
        {
            HorizontalMovement = 0;
        }
        else
        {
            HorizontalMovement = Left ? -1 : 1;
            renderer.flipX = Left;
        }

        Vector2 direction = new Vector2(HorizontalMovement, VerticalMovement);
        rb.velocity = direction;
    }
}
