using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float bSpeed = 0.1f;
    private Transform trn;
    private SpriteRenderer renderer;

    private PartyMemberBaseClass baseClass = null;
    public List<PartyController> partyMembers = new List<PartyController>();

    private int stamina;
    public int Stamina
    {
        get
        {
            return stamina;
        }
        set
        {
            stamina = value;
        }
    }

    private int STAMINA_RECOVERY_DELAY = 0;

    public Vector2 PlayerPosition
    {
        get
        {
            if (trn == null) return new Vector2();
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

        // Default TODO add a way to change this
        baseClass = new KnightClass_Base();
        //Init_Renderer(); TODO uncomment this to allow dynamic party leader

        stamina = baseClass.BASE_STAMINA;

    }

    private void Init_Renderer()
    {
        string filename = baseClass.SpriteName;
        renderer.sprite = Resources.Load<Sprite>(filename);
    }

    // Update is called once per frame
    void Update()
    {
        bool Sprinting = Input.GetKey(KeyCode.LeftShift);
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

        if (Sprinting && stamina > 0)
        {
            rb.velocity = direction.normalized * 2;
            stamina--;
            STAMINA_RECOVERY_DELAY = 200;
        }
        else
        {
            rb.velocity = direction.normalized;
            if (STAMINA_RECOVERY_DELAY > 0) STAMINA_RECOVERY_DELAY--;
            if (stamina < baseClass.BASE_STAMINA && STAMINA_RECOVERY_DELAY == 0) stamina ++;
        }  

        foreach (var pm in partyMembers)
        {
            pm.SetTarget(PlayerPosition);
        }
    }
}
