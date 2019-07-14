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
    private Rigidbody m_Rigidbody;
    public float RotationSpeed;
    public VictimInfo LastVictim;

    public static float MIN_DISTANCE = 0.2f;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        Speed = Resources.Sprint;
        RotationSpeed = 7.5f;
    }

    private void FixedUpdate()
    {
        if(IsInLastSeenPosition() 
            || IsDead() 
            || GetComponent<ShooterAwarness>().FoundVictim != null
            || GetComponent<Shooting>().ShootToVictim())
        {
            Speed = Resources.Stay;
            LastVictim = null;
        }

        if(LastVictim != null)
        {
            Speed = Resources.Sprint;
            GotoLastVictimPosition();
        }
    }

    public void GotoLastVictimPosition()
    {
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
        direction = (LastVictim.Position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }

    public void SetLastVictim(Transform lastVictim)
    {
        LastVictim = new VictimInfo { Transform = lastVictim, Position = lastVictim.position };
    }

    public bool IsInLastSeenPosition()
    {
        if (LastVictim == null) return true;
        return Vector3.Distance(transform.position, LastVictim.Position) < MIN_DISTANCE;
    }

    public bool IsDead()
    {
        return LastVictim.Transform.GetComponent<PersonStats>().Health <= 0f;
    }
}
