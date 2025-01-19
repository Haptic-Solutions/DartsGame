using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetScore : MonoBehaviour
{
    float mTriggerTime = 0.0f;
    bool mHasTriggered = false;
    bool mInactive = false;
    List<int> mScores;
    public GameObject mScoreBoard;
    // Start is called before the first frame update
    void Start()
    {
        mScores = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mHasTriggered && !mInactive)
        {
            if(Time.time > (mTriggerTime + 1.0f))
            {
                ComputeScore();
                mInactive = true; // one score per dart
            }
        }
    }
    void ComputeScore()
    {
        int multiplier = 1;
        int storedScore = 0;
        foreach (var score in mScores)
        {
            if(score == -3)
            {
                multiplier = 3;
            }
            else if (score == -2)
            {
                multiplier = 2;
            }
            else if (score > storedScore)
            {
                storedScore = score;
            }
        }
        var finalScore = storedScore * multiplier;
        //Debug.Log("Final Score is" + finalScore);
        mScoreBoard.GetComponent<IncrementScoreBoard>().IncrementScore(finalScore);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        int score = 0;
        try
        {
            score = other.GetComponent<ScoreTrigger>().GetPointValue();
        }
        catch
        {
            return;
        }
        this.GetComponent<Rigidbody>().isKinematic = true;
        //Debug.Log(score);
        mHasTriggered = true;
        mTriggerTime = Time.time;
        mScores.Add(score);
    }


}
