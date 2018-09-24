using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Timer
{
    public abstract bool IsDue();
    public abstract void Reset();
}
/*I dont need this type of manager, i should be able to emulate the functionality of a timer using Coroutines, but i dont have time to fix the TowerComponent now, the only place that uses
  timers*/
public class TimerManager : MonoBehaviour
{
    private static List<TimerImpl> Timers = new List<TimerImpl>();
    private class TimerImpl : Timer
    {
        private float ElapsedTime = 0f;
        private float Treshold = -1f;

        public TimerImpl(float treshold)
        {
            Treshold = treshold;
        }

        public void Update(float dt)
        {
            ElapsedTime += dt;
        }

        override public bool IsDue() { return ElapsedTime >= Treshold; }
        override public void Reset() { ElapsedTime = 0f; }

    };

	// Use this for initialization
	private void OnDestroy ()
    {
        Timers.Clear();
	}

    public static void RemoveTimer(Timer timer)
    {
        int index = Timers.IndexOf(timer as TimerImpl);
        if (index >= 0)
        {
            Timers.RemoveAt(index);
        }
    }

    public static Timer CreateTimer(float treshold)
    {
        TimerImpl t = new TimerImpl(treshold);
        Timers.Add(t);
        return t;
    }

	// Update is called once per frame
	void Update () {
        foreach (TimerImpl t in Timers)
            t.Update(Time.deltaTime);
	}

}
