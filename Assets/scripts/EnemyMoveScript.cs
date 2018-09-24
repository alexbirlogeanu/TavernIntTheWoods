using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyMoveScript : MonoBehaviour
{
    public float Speed = 3;
    public AudioClip ReachDestinationAudioClip = null;
    //privates
    private bool ReachedDestination = false;
    private GameObject GoToCheckpoint = null;
    private Vector3 Destination;
    private Dictionary<int, MoveModifier> Modifiers = new Dictionary<int, MoveModifier>();
    // Use this for initialization
    private EnemyStatusComponent StatusComponent = null; //cache
    private Quaternion DesiredRotation;
    private float TurnRate = 15f;
	void Start ()
    {
        StatusComponent = gameObject.GetComponent<EnemyStatusComponent>();
        Assert.IsNotNull(StatusComponent, "EnemyMoveScript.Start EnemyStatusComponent is null. Every enemy should have a status component!");
        ComputeNewMoveParameters();
        transform.LookAt(Destination);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (ReachedDestination)
        {
            StartCoroutine(OnReachDestination());
            return;
        }

        if (StatusComponent.IsDead())
            enabled = false;

        UpdateModifiers();
        Turning();
        Move();
    }

    public void ApplyModifier(MoveModifier modifier) //its called damage modifier but can affect speed too. need a better name
    {
        Assert.IsNotNull(modifier, "EnemyMoveScript.ApplyModifier: Modifier should not be null");
        Modifiers[modifier.Id] = modifier;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Utils.FinishTag)
        {
            ReachedDestination = true;
            ++GameState.EnemiesEscaped;
        }
        if (other.tag == Utils.CheckpointTag)
            ComputeNewMoveParameters();
    }

    private void ComputeNewMoveParameters()
    {
        GoToCheckpoint = CheckpointsManager.GetNextCheckpoint(GoToCheckpoint);
        Assert.IsNotNull(GoToCheckpoint, "EnemyMoveScript.ComputeNewMoveParameters GoToCheckpoint is null");

        Destination = GoToCheckpoint.transform.position;
        

        //transform.LookAt(dest);
    }

    private float ComputeSpeed()
    {
        float factor = 1f;
        foreach (MoveModifier modifier in Modifiers.Values)
            factor *= (1f - modifier.Slow);

        return Speed * factor;
    }

    private void UpdateModifiers()
    {
        List<int> keysToRemove = new List<int>();
        foreach (KeyValuePair<int, MoveModifier> entry in Modifiers)
        {
            entry.Value.StatusDuration -= Time.deltaTime;
            if (entry.Value.StatusDuration < 0f)
                keysToRemove.Add(entry.Key);
        }

        foreach (int key in keysToRemove)
            Modifiers.Remove(key);
    }
    void Move()
    {
        transform.Translate(ComputeSpeed() * Time.deltaTime * transform.forward, Space.World);
    }

    private bool Turning()
    {
        DesiredRotation = Quaternion.LookRotation((Destination - transform.position).normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, DesiredRotation, TurnRate * Time.deltaTime);

        return transform.rotation != DesiredRotation;
    }

    private IEnumerator OnReachDestination()
    {
        AudioSource audioSourceComp = GetComponent<AudioSource>();
        enabled = false; //we dont move anymore
        if (audioSourceComp != null)
        {
            audioSourceComp.clip = ReachDestinationAudioClip;
            audioSourceComp.Play();
            yield return new WaitForSeconds(ReachDestinationAudioClip.length);
        }
        Destroy(gameObject);
        
    }
}
