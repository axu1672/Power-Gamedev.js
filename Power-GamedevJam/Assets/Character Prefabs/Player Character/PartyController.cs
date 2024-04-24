using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PartyController : MonoBehaviour, NPCController
{
    private Vector2 MoveTarget;
    private Transform trn;
    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    public bool IsDead => false;

    private bool initialized = false;
    public bool Initialized { get { return initialized; } }

    private PartyMember member;
    public PartyMember Member { get { return member; } }

    private int CrowdControlEffectTimer = 0;

    public void SetTarget(Vector2 target)
    {
        MoveTarget = target;
    }
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

    public void InitializeMember(PartyMember m)
    {
        trn = gameObject.transform;
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        member = m;
        var img = Resources.Load<Sprite>($"Sprites/{m.BaseClass.SpriteName}");
        renderer.sprite = img;

        initialized = true;
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized && CrowdControlEffectTimer == 0)
        {
            Vector2 dir = MoveTarget - Position;
            float dist = dir.magnitude;

            if (dir.magnitude <= 0.5f)
            {
                Vector2 reverse = Position - MoveTarget;
                rb.velocity = reverse * 1.6f;
            }
            else
            {
                Debug.DrawLine(Position, MoveTarget, Color.red);
                rb.velocity = dir * 1.6f;
            }
        }
        if (CrowdControlEffectTimer > 0)
            CrowdControlEffectTimer--;
    }

    public void Bump(Vector2 force)
    {
        if (CrowdControlEffectTimer == 0)
        {
            rb.velocity = new Vector2();
            rb.AddForce(force);
            CrowdControlEffectTimer += 60;
        }
    }
}

public class PartyMember
{
    private PartyMemberClass cls;
    public PartyMemberClass CharacterClass
    {
        get
        {
            return cls;
        }
    }

    private PartyMemberBaseClass baseClass;
    public PartyMemberBaseClass BaseClass
    {
        get
        {
            return baseClass;
        }
    }

    public PartyMember(PartyMemberClass cls)
    {
        this.cls = cls;
        setBaseClass();
    }

    private void setBaseClass()
    {
        switch (cls)
        {
            case PartyMemberClass.Knight:
                baseClass = new KnightClass_Base();
                break;
            case PartyMemberClass.Ranger:
                baseClass = new RangerClass_Base();
                break;
            case PartyMemberClass.Mage:
                baseClass = new MageClass_Base();
                break;
            case PartyMemberClass.Cleric:
                baseClass = new ClericClass_Base();
                break;
            default:
                return;
        }
    }
}

public enum PartyMemberClass
{
    Knight,
    Ranger,
    Mage,
    Cleric
}

public abstract class PartyMemberBaseClass 
{
    public virtual int BASE_STAMINA => 1000;
    public abstract string SpriteName { get; }
    public virtual int INVINCIBILITY_FRAMES => 100;
    private int health;
    public int Health
    {
        get
        {
            return health;
        }
    }

    private int powerLevel = 0;
    public int PowerLevel
    {
        get
        {
            return powerLevel;
        }
    }

    public PartyMemberBaseClass()
    {

    }

    public virtual void LevelUp()
    {

    }

    public abstract void Attack();
}

public class KnightClass_Base : PartyMemberBaseClass
{
    public override string SpriteName => "knight";

    public KnightClass_Base() : base()
    {
        
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}

public class RangerClass_Base : PartyMemberBaseClass
{
    public override string SpriteName => "ranger";
    public RangerClass_Base() : base()
    {

    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}

public class MageClass_Base : PartyMemberBaseClass
{
    public override string SpriteName => "mage";
    public MageClass_Base() : base()
    {

    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}

public class ClericClass_Base : PartyMemberBaseClass
{
    public override string SpriteName => "cleric";
    public ClericClass_Base() : base()
    {

    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
