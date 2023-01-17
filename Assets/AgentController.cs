using UnityEngine;
using System.Collections.Generic;

public class AgentController : MonoBehaviour
{
    [field: SerializeField]
    public int Health { get; private set; }

    [field: SerializeField]
    public int MaxHealth { get; private set; } = 100;

    [field: SerializeField]
    public int Ammo { get; private set; }

    [field: SerializeField]
    public int MaxAmmo { get; private set; } = 30;

    public List<AgentController> Agents { get; private set; } = null;

    [field: SerializeField]
    public float ShootJitterAngle { get; private set; } = 15.0f;

    [field: SerializeField]
    public int ShootDamageRange { get; private set; } = 50;

    void Awake()
    {
        if (Agents is null)
            Agents = new List<AgentController>();

        Agents.Add(this);

        Heal();
        RefillAmmo();
    }

    public void Respawn()
    {
        var spawnNode = SpawnNode.GetSpawnNode();
        transform.position = spawnNode.Position;
        
        Heal();
        RefillAmmo();
        
        gameObject.SetActive(true);
    }

    public void Heal() => Health = MaxHealth;

    public void RefillAmmo() => Ammo = MaxAmmo;

    public bool Detect(out List<GameObject> agents)
    {
        if (Agents is null || Agents.Count == 0)
        {
            agents = null;
            return false;
        }

        agents = new List<GameObject>();
        var detected = false;

        foreach (var agent in Agents)
        {
            if (agent == this)
                continue;

            if (Vector3.Dot(transform.forward, agent.transform.forward) > .5f)
            {
                var dir = agent.transform.position - transform.position;
                
                RaycastHit hit;

                if (Physics.Raycast(transform.position, dir, out hit)
                    && hit.collider.CompareTag("Agent"))
                {
                    detected = true;
                    agents.Add(hit.collider.gameObject);
                }
            }
        }

        return detected;
    }

    public AgentController PickRandomEnemy(List<GameObject> enemies)
    {
        if (enemies is null)
            return null;

        var enemyGO = enemies[Random.Range(0, enemies.Count - 1)];

        return enemyGO?.GetComponent<AgentController>();
    }

    public void Shoot(AgentController enemy)
    {
        if (Ammo <= 0)
        {
            Debug.Log($"{gameObject.name} out of ammo!");
            return;
        }

        Ammo -= 1;

        var dir = enemy.transform.position - transform.position;
        
        var jitter = Random.Range(-ShootJitterAngle, ShootJitterAngle);

        dir = Quaternion.Euler(0, 0, jitter) * dir;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit)
            && hit.collider.CompareTag("Agent"))
        {
            var damage = Random.Range(-ShootDamageRange, ShootDamageRange);
            hit.collider?.GetComponent<AgentController>()?.Hit(damage);
        }
    }

    public void Hit(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Debug.Log($"{gameObject.name} died!");
            gameObject.SetActive(false);
        }
    }
}
