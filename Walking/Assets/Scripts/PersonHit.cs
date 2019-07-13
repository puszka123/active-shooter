using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonHit : MonoBehaviour
{

    public float ChestHit;
    public float HeadHit;
    public float LegsHit;

    private void Start()
    {
        HeadHit = 100f;
        ChestHit = 50f;
        LegsHit = 33.4f;
    }

    public void YouAreHit(object[] args)
    {
        RaycastHit hit = (RaycastHit)args[0];
        Transform activeShooter = (Transform)args[1];
        float myHeight = GetComponent<CapsuleCollider>().bounds.size.y;
        float center = transform.position.y;
        float hitPoint = hit.point.y;
        float head = myHeight / 7.5f;
        float chest = 3 * head;
        float legs = 3.5f * head;

        float damage;
        if (hitPoint < center - head)
        {
            damage = Random.Range(LegsHit, 2 * LegsHit);
            GetComponent<PersonStats>().GetDamage(damage, activeShooter, hit.point);
        }
        if (hitPoint > center - head)
        {
            if (hitPoint > center + chest)
            {
                damage = Random.Range(HeadHit, 2 * HeadHit);
                GetComponent<PersonStats>().GetDamage(damage, activeShooter, hit.point);
            }
            else
            {
                damage = Random.Range(ChestHit, 2 * ChestHit);
                GetComponent<PersonStats>().GetDamage(damage, activeShooter, hit.point);
            }
        }
    }
}
