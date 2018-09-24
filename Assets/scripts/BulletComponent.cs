using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class BulletComponent : MonoBehaviour {
    //public:
    public float Speed = 150f;
    public float Damage = 30f;
    [Tooltip("generate an explosion if greater than 0")]
    public float BlastRadius = 0f;
    [Tooltip("If blast radius > 0 an explosion will be spawned")]
    public GameObject ExplosionPrefab = null;

    public EElementType DamageType = EElementType.None;

    //private:
    private GameObject Target = null; //target to which the dmg is applied 
    private DamageModifierComponent DamageModifierComp = null;
    private MoveModifierComponent MoveModifierComp = null;

    // Use this for initialization
	void Start () {
        Assert.IsNotNull(Target, "BulletComponent.Start(): Target is NULL");
        DamageModifierComp = gameObject.GetComponent<DamageModifierComponent>();
        MoveModifierComp = gameObject.GetComponent<MoveModifierComponent>();

        if (BlastRadius > 0f)
        {
            Assert.IsNotNull(ExplosionPrefab, "BulletComponent.Start: Projectile is set to explode but no prefab is set!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 direction = transform.forward;
        if (Target)
        {
            Vector3 targetPoint = Target.transform.position + Vector3.up; //try to shoot for the head
            direction = targetPoint - transform.position;
            direction.Normalize();
        }

        transform.position = transform.position + (direction * Speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<Collider>().enabled = false;

        if (BlastRadius > 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, BlastRadius, Utils.EnemyLayerMask);
            foreach(Collider collider in hits)
            {
                ApplyDamageAndEffects(collider.gameObject);
            }
            Instantiate(ExplosionPrefab, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
        }
        else if (other.tag == Utils.EnemyTag)
        {
            ApplyDamageAndEffects(other.gameObject);
        }
        
        Destroy(gameObject);
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    private void ApplyDamageAndEffects(GameObject gObject)
    {
        EnemyStatusComponent enemyStatus = gObject.GetComponent<EnemyStatusComponent>();

        if (enemyStatus && !enemyStatus.IsDead())
        {
            enemyStatus.OnDamage(Damage, DamageType);

            if (DamageModifierComp != null)
                DamageModifierComp.ApplyOnTarget(enemyStatus, DamageType);

            EnemyMoveScript enemyMoveComp = gObject.GetComponent<EnemyMoveScript>();
            if (enemyMoveComp && MoveModifierComp != null)
            {
                MoveModifierComp.ApplyOnTarget(enemyMoveComp);
            }
        }
        
    }
}
