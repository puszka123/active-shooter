using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fight : MonoBehaviour
{
    public List<GameObject> ShooterEnemies;
    public bool FightIsStarted;
    float roundTime = 0.5f;
    float time = 0f;
    public const float DropGunProbability = 5f;
    public const float ShootProbability = 85f;

    public float DropGunChance { get { return DropGunProbability * (EnemiesStrength() * EnemiesStrength()); } }
    public float ShootChance { get { return ShootProbability / (EnemiesStrength()); } }

    // Use this for initialization
    void Start()
    {
        FightIsStarted = false;

    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= roundTime && FightIsStarted)
        {
            time = 0f;
            StartRound();
        }
    }

    public void JoinFight(GameObject enemy)
    {
        if (ShooterEnemies == null)
        {
            ShooterEnemies = new List<GameObject> { enemy };
        }
        else if (!AlreadyFighting(enemy))
        {
            ShooterEnemies.Add(enemy);
        }
       // Debug.Log(enemy.name + " joined a fight");
        if (!FightIsStarted)
        {
            StartFight();
        }
    }

    public void StartFight()
    {
        SetVictimSearch(false);
        FightIsStarted = true;
    }

    public bool AlreadyFighting(GameObject enemy)
    {
        if (ShooterEnemies == null) return false;
        return ShooterEnemies.Select(e => e.name).Contains(enemy.name);
    }

    public void SetVictimSearch(bool active)
    {
        GetComponent<Shooting>().enabled = active;
        GetComponent<FollowVictim>().enabled = active;
        GetComponent<ShooterAwarness>().enabled = active;
    }

    public void StartRound()
    {
        RemoveDeadEnemies();
        if (ShooterEnemies.Count == 0)
        {
            Win();
            return;
        }
        if (CanShoot())
        {
            ShooterEnemies[Random.Range(0, ShooterEnemies.Count - 1)].GetComponent<PersonHit>().YouAreHit(gameObject);
        }
        RemoveDeadEnemies();
        if (ShooterEnemies.Count == 0)
        {
            Win();
            return;
        }
        if (DropGun())
        {
            Debug.Log("Active shooter killed by fighters");
            GameObject.FindGameObjectWithTag("SimulationManager").SendMessage("ResetSimulationRequest");
            GetComponent<Fight>().enabled = false;
        }
    }

    private void RemoveDeadEnemies()
    {
        List<string> enemiesToRemove = new List<string>();
        foreach (var enemy in ShooterEnemies)
        {
            if (enemy.GetComponent<PersonStats>().GetHealth() <= 0f)
            {
                enemiesToRemove.Add(enemy.name);
            }
        }
        foreach (var enemy in enemiesToRemove)
        {
            GameObject enemyToRemove = ShooterEnemies.Find(e => e.name == enemy);
            if (enemyToRemove != null)
            {
                ShooterEnemies.Remove(enemyToRemove);
            }
        }
    }

    public void Win()
    {
        FightIsStarted = false;
        SetVictimSearch(true);
    }

    public bool CanShoot()
    {
        return ShootChance >= Random.Range(0f, 100f);
    }

    public bool DropGun()
    {
        return DropGunChance >= Random.Range(0f, 100f);
    }

    public float EnemiesStrength()
    {
        float strengthSum = 0f;
        ShooterEnemies.ForEach(e => strengthSum += e.GetComponent<PersonStats>().Strength);
        if (strengthSum == 0f) return 1f;
        else return strengthSum;
    }
}
