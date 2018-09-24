using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UITimer : MonoBehaviour {
    public float TimeToWait = 0f;
    private UnityEngine.UI.Text TimeText = null;
	// Use this for initialization
	void Start ()
    {
        Transform child = transform.Find("TimerText");
        Assert.IsNotNull(child, "UITimer.Start: No child with name TimerText!");
        TimeText = child.GetComponent<UnityEngine.UI.Text>();
        Assert.IsNotNull(child, "UITimer.Start: Child doesnt have a text component");
    }

    // Update is called once per frame
    void Update () {
        TimeToWait -= Time.deltaTime;
        TimeText.text = ((int)Mathf.Max(TimeToWait, 0f)).ToString();

        if (TimeToWait <= 0f)
            gameObject.SetActive(false);
    }
}
