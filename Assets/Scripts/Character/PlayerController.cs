using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;   //for NavMeshAgent 

public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent; //通过NavMeshAgent的属性来控制角色移动
    Animator anim;  
    private CharacterStats characterStats;
    private GameObject attackTarget;    //攻击目标
    private float lastAttackTime;   // 攻击冷却时间，与所使用的武器相关，在Update中减小
    private bool isDead;
    float stopDistance;
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // 旋转速度

    // 自身变量的获取
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }

    void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }
    void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(this.characterStats);
    }
    void OnDisable()
    {
        if(MouseManager.IsInitialized)
        {
            MouseManager.Instance.OnMouseClicked -= MoveToTarget;
            MouseManager.Instance.OnEnemyClicked -= EventAttack;
        }
    }
    void OnDestroy()
    {
        if(MouseManager.IsInitialized)
        {
            MouseManager.Instance.OnMouseClicked -= MoveToTarget;
            MouseManager.Instance.OnEnemyClicked -= EventAttack;
        }
    }

    void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        if(isDead)
            GameManager.Instance.NotifyObservers();

        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;

        // Move();
    }

    private void SwitchAnimation()
    {

        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);

    }

    // void Move()
    // {
    //     // 1. 获取输入
    //     float h = Input.GetAxisRaw("Horizontal"); // GetAxisRaw 响应更即时，没有平滑阻尼
    //     float v = Input.GetAxisRaw("Vertical");

    //     // 2. 获取摄像机的方向参考
    //     Transform camTransform = Camera.main.transform;
    //     Vector3 forward = camTransform.forward;
    //     Vector3 right = camTransform.right;

    //     // 重要：抹除垂直方向的分量，防止因为相机低头导致玩家想往地底下走
    //     forward.y = 0;
    //     right.y = 0;
    //     forward.Normalize();
    //     right.Normalize();

    //     // 3. 计算基于相机视角的移动向量
    //     Vector3 moveDir = (forward * v + right * h).normalized;

    //     // 4. 执行移动和旋转
    //     if (moveDir.magnitude > 0.1f && !agent.isStopped) 
    //     {
    //         // 旋转：平滑转向移动方向
    //         Quaternion targetRotation = Quaternion.LookRotation(moveDir);
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    //         // 位移：使用 agent.Move 确保受导航网格限制
    //         agent.Move(moveDir * moveSpeed * Time.deltaTime);

    //         anim.SetFloat("Speed", 1.0f);
    //     }
    //     else
    //     {
    //         anim.SetFloat("Speed", 0f);
    //     }
    // }

    public void MoveToTarget(Vector3 destination)
    {
        if (isDead) return;
        //当鼠标在追寻敌人时点击敌人之外的点，需要暂停当前协程（主要是MoveToAttackTarget），开始移动（agent.isStopped = false），然后向目标点移动
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = destination;
        agent.stoppingDistance = stopDistance;
    }    
    public void EventAttack(GameObject target){
        if(isDead) return;
        if(target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;
        // 首先更改player朝向，使其朝向target
        transform.LookAt(attackTarget.transform);

        // 其次检测player与target的距离，如果距离大于于武器攻击距离，逐帧向target移动
        while(Vector3.Distance(transform.position, attackTarget.transform.position) > characterStats.attackData.attackRange)
        {

            agent.destination = attackTarget.transform.position;
            yield return null;  //下一帧再次执行
        }

        // 当player与target距离小于武器攻击距离时，停止player的移动
        agent.isStopped = true;

        // 执行攻击动画
        if(lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    // Animation Event
    private void Hit()
    {
        if(attackTarget == null) return;
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>())
            {
                attackTarget.GetComponent<Rock>().HitByPlayer();
            }
        }
        else if (attackTarget.CompareTag("Enemy"))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamege(characterStats, targetStats);
        }
    }
}
