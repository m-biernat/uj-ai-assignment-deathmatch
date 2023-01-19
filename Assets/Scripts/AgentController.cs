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

    public static List<AgentController> Agents { get; private set; } = null;

    [field: SerializeField]
    public float ShootJitterAngle { get; private set; } = 15.0f;

    [field: SerializeField]
    public int ShootDamageRange { get; private set; } = 50;

    [field: SerializeField]
    public float MovementSpeed { get; private set; } = 1.0f;
    
    [field: SerializeField]
    public float RotationSpeed { get; private set; } = 1.0f;

    [field: SerializeField]
    public Node InitialNode { get; private set; }

    [field: SerializeField]
    public Node TargetNode { get; private set; }

    public Node CurrentNode { get; private set; }

    Node _currentTarget;

    List<Node> _path;

    Collider2D _collider;

    void Awake()
    {
        if (Agents is null)
            Agents = new List<AgentController>();

        Agents.Add(this);

        Heal();
        RefillAmmo();

        CurrentNode = InitialNode;

        _collider = GetComponent<Collider2D>();
    }

    void Start() 
    {
        if (InitialNode != TargetNode)
            PathFinder.Instance.EnqueueRequest(this);
        //TargetRandomNode();
    }

    void Update()
    {
        

        Debug.DrawRay(transform.position, transform.up, Color.white);
    }

    public void Respawn()
    {
        var spawnNode = SpawnNode.GetSpawnNode();
        CurrentNode = spawnNode;
        transform.position = spawnNode.Position;
        
        Heal();
        RefillAmmo();
        
        gameObject.SetActive(true);
    }

    public void Heal() => Health = MaxHealth;

    public void RefillAmmo() => Ammo = MaxAmmo;

    public bool Detect(out List<GameObject> agents)
    {
        agents = new List<GameObject>();
        var detected = false;

        _collider.enabled = false;

        foreach (var agent in Agents)
        {
            if (agent == this)
                continue;

            var dir = agent.transform.position - transform.position;
            dir.Normalize();

            if (Vector3.Dot(transform.up, dir) > .5f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);

                if (hit && hit.collider.CompareTag("Agent"))
                {
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
        var minDist = Mathf.Infinity;
        
        if (enemies.Count > 1)
        {
            foreach (var enemy in enemies)
            {
                var dist = Vector3.Distance(transform.position, 
                                            enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    enemyGO = enemy;
                }
            }
        }

        return enemyGO?.GetComponent<AgentController>();
    }

    public void Shoot(AgentController enemy)
    {
        if (Ammo <= 0)
        {
            Debug.Log($"{gameObject.name} is out of ammo!");
            return;
        }

        Ammo -= 1;

        var dir = enemy.transform.position - transform.position;
        
        var jitter = Random.Range(-ShootJitterAngle, ShootJitterAngle);

        dir = Quaternion.Euler(0, 0, jitter) * dir;

        _collider.enabled = false;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        Debug.DrawLine(transform.position, hit.point, Color.red, 2.0f);

        if (hit.collider.CompareTag("Agent"))
        {
            var damage = Random.Range(1, ShootDamageRange);
            hit.collider?.GetComponent<AgentController>()?.Hit(damage);
        }
        
        _collider.enabled = true;
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

    public void FollowPath(System.Action onComplete)
    {
        if (_currentTarget is null)
        {
            if (_path is null)
                return;
            
            _currentTarget = _path[_path.Count - 1];
        }

        var targetPosition = _currentTarget.Position;
        var targetDirection = targetPosition - transform.position;

        var movementStep = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementStep);

        var rotationStep = RotationSpeed * Time.deltaTime;
        transform.up = Vector3.RotateTowards(transform.up, targetDirection, rotationStep, 0.0f);

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
