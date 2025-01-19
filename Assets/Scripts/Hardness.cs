using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hardness : MonoBehaviour
{
    public float hardness = 0.2f;
    public bool mGrabbable = false;
    private bool mPreviouslyHeld = false;
    private GameObject mHandObject;
    private Transform mPreviousTransformParent;
    private Dictionary<FINGER, bool> mTouchStates = new Dictionary<FINGER, bool>();
    public bool mHasParrent = false;
    public bool mSpin = false;
    public bool mSplinter = false;
    public bool mStayKinematic = false;

    List<Vector3> mPreviousPosition;
    List<float> mTimeList;
    const int NumberOfFrames = 15;

    private void Start()
    {
        mTouchStates[FINGER.FINGER_THUMB_A] = false;
        mTouchStates[FINGER.FINGER_THUMB_B] = false;
        mTouchStates[FINGER.FINGER_POINTER] = false;
        mTouchStates[FINGER.FINGER_MIDDLE] = false;
        mTouchStates[FINGER.FINGER_RING] = false;
        mTouchStates[FINGER.FINGER_PINKY] = false;

        mPreviousPosition = new List<Vector3>();
        mTimeList = new List<float>();
    }

    public float GetHardness()
    {
        return hardness;
    }

    //the finger can get this object so we will let them call these functions to set current contact state.
    public void SetTouch(FINGER aFinger, bool aState, GameObject aHandObject)
    {
        if(mHasParrent)
        {
            this.transform.parent.gameObject.GetComponent<Hardness>().SetTouch(aFinger, aState, aHandObject);
            return;
        }
        mTouchStates[aFinger] = aState;
        mHandObject = aHandObject;
    }

    //trigger for pickup
    public void Update()
    {
        //check if object  is set as able to be grabbed. Some object we want to be touchable but not movable.
        if ((!mHasParrent) && mGrabbable)
        {
            //Has to do with velocity computation
            var currentPosition = this.transform.position;
            var currentTime = Time.time;
            mPreviousPosition.Add(currentPosition);
            mTimeList.Add(currentTime);

            var timePassed = Time.deltaTime;
            var currentVelocity = (mPreviousPosition[mPreviousPosition.Count-1] - mPreviousPosition[0]) / (mTimeList[mTimeList.Count-1]-mTimeList[0]);

            if (mPreviousPosition.Count > NumberOfFrames)
            {
                mPreviousPosition.RemoveAt(0);
            }
            if (mTimeList.Count > NumberOfFrames)
            {
                mTimeList.RemoveAt(0);
            }

            if ((mTouchStates[FINGER.FINGER_THUMB_A] || mTouchStates[FINGER.FINGER_THUMB_B]) &&
                ((mTouchStates[FINGER.FINGER_POINTER]) ||
                (mTouchStates[FINGER.FINGER_MIDDLE]) ||
                (mTouchStates[FINGER.FINGER_RING]) ||
                (mTouchStates[FINGER.FINGER_PINKY])))
            {
                //store previous transform for safe keeping
                if (!mPreviouslyHeld)
                {
                    this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    this.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
                    mPreviousTransformParent = this.transform.parent;
                    this.transform.SetParent(mHandObject.transform);
                    this.GetComponent<Rigidbody>().useGravity = false;
                    this.GetComponent<Rigidbody>().isKinematic = true;
                    mPreviouslyHeld = true;
                }
            }
            else if (mPreviouslyHeld)
            {
                //throwing/dropping physics
                mPreviouslyHeld = false;
                //restore previous parent
                this.transform.SetParent(mPreviousTransformParent);
                //turn gravity back on
                this.GetComponent<Rigidbody>().useGravity = true;
                if (!mStayKinematic) 
                { 
                    this.GetComponent<Rigidbody>().isKinematic = false;
                }
                this.GetComponent<Rigidbody>().velocity = currentVelocity;
                if (mSpin)
                {
                    this.GetComponent<Rigidbody>().angularVelocity = new Vector3(2, 0, 0);
                }
                if (mSplinter)
                {
                    foreach (var goFast in this.GetComponentsInChildren<GoFast>())
                    {
                        goFast.TriggerGoFast();
                    }
                }
            }
        }
    }
}
