using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


public enum EnemyState { GUARD, PATROL, CHASE, DEAD };

[RequireComponent(typeof(NavMeshAgent))]    //使得绑定这一脚本的对象都具有NavMeshAgent，没有就帮他生成一个

public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private NavMeshAgent agent;
    private Collider coll;
    private EnemyState enemyState;  
    private Animator anim;  
    public CharacterStats characterStats;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;    
    private float speed;
    protected GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

    bool isWalk;
    bool isChase;
    bool Chasing;
    bool isDead;
    bool playerDead;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 patrolPoint;
    private Vector3 guardPos;   //站桩位置
    private Quaternion guardRotation;  // 初始朝向
    void Awake()
    {
        //获取组件
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        //属性初始化
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }
    void Start()
    {
        // start when run the project
        if(isGuard == true)
        {
            enemyState = EnemyState.GUARD;
        }
        else
        {
            enemyState = EnemyState.PATROL;
            GetPatrolPoint();
        }
        //FIXME:场景切换后修改掉
        GameManager.Instance.AddObserver(this);
    }
        //切换场景时启用
    // void OnEnable()
    // {
    //     if(!GameManager.IsInitialized) Debug.Log("GameManager is not initialized.");
    //     GameManager.Instance.AddObserver(this);
    // }

    void OnDisable()
    {
        // OnDestroy() -> Destroying... -> Destroyed -> OnDisable()
        if(!GameManager.IsInitialized) return ;
        GameManager.Instance.RemoveObserver(this);
    }
    void Update()
    {
        if(!playerDead)
        {
            if(characterStats.CurrentHealth == 0) 
                isDead = true;
            
            SwitchStates();
            SwitchAnimation();
            

            lastAttackTime -= Time.deltaTime;            
        }

    }
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Chasing", Chasing);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }
    void SwitchStates()
    {
        if(isDead)
        {
            enemyState = EnemyState.DEAD;
        }
        else if(FoundPlayer())
        {
            enemyState = EnemyState.CHASE;
        }


        switch (enemyState)
        {
            case EnemyState.GUARD:
                OnStateGuard();
                break;
            case EnemyState.PATROL:
                OnStatePatrol();
                break;
            case EnemyState.CHASE:
                OnStateChase();
                break;
            case EnemyState.DEAD:
                OnStateDead();
                break;

        }            
        


    }

    private void OnStateDead()
    {
        // 播放死亡动画，敌人消失
        coll.enabled = false;
        // agent.enabled = false;
        agent.radius = 0f;

        Destroy(gameObject, 2f);

    }

    private void OnStateGuard()
    {
        isChase = false;
        
        if (Vector3.Distance(transform.position, guardPos) > 0.1f)
        {
            // Debug.Log("guardPos: " + guardPos+", transform.position: " + transform.position+", Vector3.Distance(transform.position, guardPos): " + Vector3.Distance(transform.position, guardPos));
            isWalk = true;
            agent.isStopped = false;
            agent.destination = guardPos;

            if(Vector3.SqrMagnitude(guardPos - transform.position) < agent.stoppingDistance)
            {
                isWalk = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
            }
        }
    }

    private void OnStatePatrol()
    {
        
        // 判断是否到达了巡逻点
        if(Vector3.Distance(transform.position, patrolPoint) <= agent.stoppingDistance)
        {
            // animate control
            isWalk = false;

            // stop and look around
            if(remainLookAtTime > 0)
            {
                remainLookAtTime -= Time.deltaTime;
            }
            else 
            {
                // when the look time is over, patrol to a new point and reset the look time;
                GetPatrolPoint();
                remainLookAtTime = lookAtTime;
            }

        }
        else
        {
            // keep walking, update the desitination;
            isWalk = true;
            agent.destination = patrolPoint;
        }
    }

    bool FoundPlayer()
    {
        // Physics.OverlapSphere(position, radius), 返回球体内所有碰撞体
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach(var collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                attackTarget = collider.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    
    private void OnStateChase(){
        // Debug.Log("OnStateChase");
        // chage the attributes in Animator
        isWalk = false;
        isChase = true;

        // change the speed
        agent.speed = speed;

        // if player is out of vision
        if(!FoundPlayer()){

            // when the player is out of view, stop and look for a while 
            Chasing = false;
            isChase = false;

            if(remainLookAtTime > 0){
                remainLookAtTime -= remainLookAtTime;
                agent.destination = transform.position;
            }
            // after that, back to original position and original state
            else if (isGuard) 
                enemyState = EnemyState.GUARD;
            else 
                enemyState = EnemyState.PATROL;
            
            agent.speed = speed*0.5f;

        }
        // if player is in vision
        else 
        {
            agent.isStopped = false;

            Chasing = true;
            agent.destination = attackTarget.transform.position;
        }
       
        if(TargetInAttackRange() || TargetInSkillRange())
        {
            Chasing = false;
            agent.isStopped = true;

            // time for last attack
            if(lastAttackTime < 0)
            {
                lastAttackTime = characterStats.attackData.coolDown;

                // 暴击判断
                characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                // 执行攻击
                Attack();
            }
        }
       
    }

    private void Attack()
    {
        // look at atttack Target
        transform.LookAt(attackTarget.transform);
        if(TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        if(TargetInSkillRange())
        {
            //远程攻击动画
            anim.SetTrigger("Skill");
        }
    }

    bool TargetInAttackRange()
    {
        if(attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else 
            return false;
    }
    bool TargetInSkillRange()
    {
        if(attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else 
            return false;

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }
    void GetPatrolPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x+randomX, transform.position.y, guardPos.z+randomZ);
        
        // 根据NavMesh确定当前点是否可到达
        NavMeshHit hit;
        patrolPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)? hit.position:transform.position;   //return bool

    }

    //Animation Event
    void Hit()
    {
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            // Debug.Log("Hit");
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamege(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        // 当游戏结束（Player死亡）时，播放获胜动画
        // 停止所有移动
        // 停止Agent
        isChase = false;
        isWalk = false;
        attackTarget = null;

        playerDead = true;

        anim.SetBool("Win", true);
        SwitchAnimation();
        
    }


}
