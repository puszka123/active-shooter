using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimInfo
{
    public Transform Transform;
    public Vector3 Position;
}

public class FollowVictim : MonoBehaviour {
    public float Speed;
    public float TargetSpeed;
    private Rigidbody m_Rigidbody;
    public float RotationSpeed;
    public VictimInfo LastVictim;
    public Pathfinder Pathfinder;
    public List<Node> Path;
    public int CurrentIndex;
    public bool Executing;

    public static float MIN_DISTANCE = 0.2f;

    private void Start()
    {
        Pathfinder = new Pathfinder();
        m_Rigidbody = GetComponent<Rigidbody>();
        Speed = Resources.Stay;
        TargetSpeed = Resources.Run;
        RotationSpeed = 10f;
        Executing = false;
    }

    private void FixedUpdate()
    {
        if(IsInLastSeenPosition() 
            || IsDead() 
            || GetComponent<ShooterAwarness>().FoundVictim != null
            || GetComponent<Shooting>().ShootToVictim())
        {
            LastVictim = null;
        }

        if(LastVictim != null && CurrentIndex < Path.Count)
        {
            if(Speed < TargetSpeed)
            {
                Speed += 0.03f;
            }
            else
            {
                Speed -= 0.1f;
            }
            GotoLastVictimPosition();
        }
        else
        {
            Executing = false;
        }
    }

    public void GotoLastVictimPosition()
    {
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door");
        if(!Physics.Linecast(transform.position, LastVictim.Position, layerMask))
        {
            InitPath(LastVictim.Position);
        }
        if(Pathfinder.CheckDistance(gameObject, Path[CurrentIndex]))
        {
            CurrentIndex++;
        }
        if(CurrentIndex >= Path.Count)
        {
            Executing = false;
            return;
        }
        RotateToVictim();
        Move();
    }

    private void Move()
    {
        m_Rigidbody.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
    }

    public void RotateToVictim()
    {
        Vector3 direction;
        direction = (Path[CurrentIndex].Position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }

    public void SetLastVictim(Transform lastVictim)
    {

        LastVictim = new VictimInfo { Transform = lastVictim, Position = lastVictim.position };
        InitPath();
        Executing = true;
    }

    public bool IsInLastSeenPosition()
    {
        if (LastVictim == null) return true;
        return Utils.Distance(transform.position, LastVictim.Position) < MIN_DISTANCE;
    }

    public bool IsDead()
    {
        return LastVictim.Transform.GetComponent<PersonStats>().Health <= 0f;
    }

    public void InitPath()
    {
        PersonMemory memory = GetComponent<Person>().PersonMemory;
        Path = Pathfinder.FindWay(memory.Graph[memory.CurrentFloor], gameObject, memory.FindNearestLocation(LastVictim.Position), memory);
        CurrentIndex = 0;
    }

    public void InitPath(Vector3 position)
    {
        PersonMemory memory = GetComponent<Person>().PersonMemory;
        Path = new List<Node> { new Node { Name = "XD", Position = position } };
        CurrentIndex = 0;
    }
}
