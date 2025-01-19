using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoFast : MonoBehaviour
{
    bool mHasTriggered = false;
    bool mInactive = false;
    float mTriggerTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mHasTriggered && !mInactive)
        {
            if (Time.time > (mTriggerTime + 1.0f))
            {
                mInactive = true; // one score per dart
                transform.parent = null;
                this.gameObject.AddComponent<Rigidbody>();
                var currentVel = this.GetComponent<Rigidbody>().velocity;
                this.GetComponent<Rigidbody>().velocity = new Vector3(currentVel.x - 10.0f, currentVel.y, currentVel.z);
            }
        }
    }

    public void TriggerGoFast()
    {
        mTriggerTime = Time.time;
        mHasTriggered = true;
    }
}
