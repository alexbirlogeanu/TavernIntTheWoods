using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TowerComponent : MonoBehaviour
{
    //public:
    public float RateOfFire = 0.3f;
    public float RangeOfFire = 5f;
    public GameObject BulletPrefab = null;
    public int Cost = 35;

    //private:
    private Timer ShootTimer = null;
    private GameObject Target = null;
    private Vector3 ShootStartPoint;
   
    // Use this for initialization
    void Start ()
    {
        Assert.IsNotNull(BulletPrefab, "TowerComponent: Prefab for bullet is not set");

        ShootTimer = TimerManager.CreateTimer(RateOfFire);
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
            if (child.name == "Shoot")
                ShootStartPoint = child.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        ValidateTarget();

        if (Target != null)
            Shoot();
        else
        {
            if (AcquireTarget())
               Shoot();
        }

	}

    private void OnDestroy()
    {
        //TimerManager.RemoveTimer(ShootTimer);
    }

    private bool AcquireTarget()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, RangeOfFire, Utils.EnemyLayerMask);
        Collider closest = null;
        float sqrtMinDist = float.MaxValue;
        foreach (Collider col in enemiesInRange)
        {
            //first check the enemy health status
            if (IsTargetDead(col.gameObject))
                continue;

            float sqrDist = (transform.position - col.transform.position).sqrMagnitude;
            if (sqrDist < sqrtMinDist)
            {
                sqrtMinDist = sqrDist;
                closest = col;
            }
        }

        if (closest != null)
            Target = closest.gameObject;

        return Target != null;
    }

    private void ValidateTarget()
    {
        if (Target != null)
        {
            float sqrDist = (transform.position - Target.transform.position).sqrMagnitude;

            if (sqrDist > RangeOfFire * RangeOfFire || IsTargetDead(Target))
                Target = null; //clear target
        }

    }

    void  Shoot()
    {
        if (!ShootTimer.IsDue())
            return;

        Assert.IsNotNull(Target, "TowerComponent.Shoot call this method only on a valid target");

        Vector3 relDir = (Target.transform.position - ShootStartPoint);
        relDir.Normalize();
        Object bullet = Instantiate(BulletPrefab, ShootStartPoint, Quaternion.LookRotation(relDir));


        BulletComponent bComponent = (bullet as GameObject).GetComponent<BulletComponent>();
        Assert.IsNotNull(bComponent, "TowerComponent.Shoot: Bullet is not created properly");
        bComponent.SetTarget(Target);

        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.Play();
        }

        ShootTimer.Reset();
    }

    private bool IsTargetDead(GameObject target)
    {
        EnemyStatusComponent statusComp = target.GetComponent<EnemyStatusComponent>();
        return statusComp == null || statusComp.IsDead();
    }

}
