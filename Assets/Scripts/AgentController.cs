using UnityEngine;
using System.Collections.Generic;

public class AgentController : MonoBehaviour
{
    public static List<AgentController> Agents { get; private set; } = null;

    [field: SerializeField]
    public int Health { get; private set; }

    [field: SerializeField]
    public int MaxHealth { get; private set; } = 100;

    [field: SerializeField]
    public int Ammo { get; private set; }

    [field: SerializeField]
    public int MaxAmmo { get; private set; } = 30;

    [field: SerializeField, Space]
    public float MovementSpeed { get; private set; } = 1.0f;
    
    [field: SerializeField]
    public float RotationSpeed { get; private set; } = 1.0f;

    [field: SerializeField, Space]
    public float ShootAngleJitter { get; private set; } = 15.0f;

    [field: SerializeField]
    public int ShootDamageJitter { get; private set; } = 50;

    [field: SerializeField, Space]
    public float ShootCooldown { get; private set; } = 0.03f;

    [field: SerializeField]
    public float RespawnCooldown { get; private set; } = 5.0f;

    [field: SerializeField, Space]
    public Node InitialNode { get; private set; }

    [field: SerializeField]
    public Node TargetNode { get; private set; }

    public Node CurrentNode { get; private set; }

    [field: SerializeField, Space]
    public int Kills { get; private set; } = 0;

    [field: SerializeField]
    public int Deaths { get; private set; } = 0;

    Node _currentTarget;

    List<Node> _path;

    Collider2D _collider;

    SpriteRenderer _renderer;

    void Awake()
    {
        if (Agents is null)
            Agents = new List<AgentController>();

        Agents.Add(this);

        Heal();
        RefillAmmo();

        CurrentNode = InitialNode;

        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update() 
    {
        Debug.DrawRay(transform.position, transform.up, Color.white);
    }

    public void Respawn()
    {
        Node spawnNode = SpawnNode.GetSpawnNode();
        CurrentNode = spawnNode;
        transform.position = spawnNode.Position;
        
        _currentTarget = null;
        _path = null;

        Heal();
        RefillAmmo();
        
        _collider.enabled = true;
        _renderer.enabled = true;
    }

    public void Heal() { Health = MaxHealth; }

    public void RefillAmmo() { Ammo = MaxAmmo; }

    public bool Detect(out List<GameObject> agents)
    {
        agents = new List<GameObject>();
        bool detected = false;

        _collider.enabled = false;

        foreach (var agent in Agents)
        {
            if (agent == this)
                continue;

            Vector3 dir = agent.transform.position - transform.position;
            dir.Normalize();

            if (Vector3.Dot(transform.up, dir) > .5f) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);

                if (hit && hit.collider.CompareTag("Agent")) {
                    detected = true;
                    agents.Add(hit.collider.gameObject);
                }
            }
        }

        _collider.enabled = true;

        return detected;
    }

    public AgentController PickRandomEnemy(List<GameObject> enemies)
    {
        if (enemies is null)
            return null;

        GameObject enemyGO;
        
        if (enemies.Count == 1)
            enemyGO = enemies[0];
        else
            enemyGO = enemies[Random.Range(0, enemies.Count - 1)];

        return enemyGO?.GetComponent<AgentController>();
    }

    public AgentController PickClosestEnemy(List<GameObject> enemies)
    {
        if (enemies is null)
            return null;

        GameObject enemyGO = enemies[0];
        float minDist = Mathf.Infinity;
        
        if (enemies.Count > 1) {
            foreach (var enemy in enemies)
            {
                float dist = Vector3.Distance(transform.position, 
                                              enemy.transform.position);
                if (dist < minDist) {
                    minDist = dist;
                    enemyGO = enemy;
                }
            }
        }

        return enemyGO?.GetComponent<AgentController>();
    }

    public void Shoot(AgentController enemy, System.Action OutOfAmmo)
    {
        if (Ammo <= 0)
            return;

        Ammo -= 1;

        Vector3 dir = enemy.transform.position - transform.position;
        
        float jitter = Random.Range(-ShootAngleJitter, ShootAngleJitter);

        dir = Quaternion.Euler(0, 0, jitter) * dir;

        _collider.enabled = false;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        Debug.DrawLine(transform.position, hit.point, _renderer.color, 2.0f);

        if (hit.collider.CompareTag("Agent")) {
            int damage = Random.Range(1, ShootDamageJitter);
            hit.collider?.GetComponent<AgentController>()?.Hit(damage, this);
        }
        
        _collider.enabled = true;

        if (Ammo == 0) {
            Debug.Log($"{transform.name} is out of ammo!");
            OutOfAmmo?.Invoke();
        }
    }

    public void Hit(int damage, AgentController enemy)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Debug.Log($"{gameObject.name} died!");
            
            _collider.enabled = false;
            _renderer.enabled = false;

            var sm = GetComponent<AgentBehaviourSM>();
            sm.ChangeState(sm.DeathState);

            Deaths++;
            enemy.Kills++;
        }
    }

    public void TargetClosestHeal()
    {
        TargetNode = HealNode.GetClosestNode(transform.position);
        PathFinder.Instance.EnqueueRequest(this);
    }

    public void TargetClosestAmmo()
    {
        TargetNode = AmmoNode.GetClosestNode(transform.position);
        PathFinder.Instance.EnqueueRequest(this);
    }

    public void TargetRandomNode()
    {
        TargetNode = Node.GetRandomNodeOfType(CurrentNode, NodeType.Node);
        PathFinder.Instance.EnqueueRequest(this);
    }

    public void SetPath(List<Node> path)
    {
        _path = path;
        _currentTarget = null;
    }

    public bool Pathless() { return _path is null; }

    public void FollowPath(System.Action onComplete)
    {
        if (_currentTarget is null)
        {
            if (_path is null || _path.Count == 0) 
                return;
            
            _currentTarget = _path[_path.Count - 1];
        }

        Vector3 targetPosition = _currentTarget.Position;
        Vector3 targetDirection = targetPosition - transform.position;

        float movementStep = MovementSpeed * Time.deltaTime;
        Vector3 movement = Vector3.MoveTowards(transform.position, targetPosition, movementStep);

        float rotationStep = RotationSpeed * Time.deltaTime;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
        
        transform.SetPositionAndRotation(movement, rotation);

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            CurrentNode = _currentTarget;

            if (_path.Count > 1)
            {
                _path.RemoveAt(_path.Count - 1);
                _currentTarget = _path[_path.Count - 1];
            }
            else
            {
                _currentTarget = null;
                _path = null;
                onComplete?.Invoke();
            }
        }
    }
}
