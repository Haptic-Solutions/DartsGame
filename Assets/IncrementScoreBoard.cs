using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IncrementScoreBoard : MonoBehaviour
{
    int mCurrentScore = 0;
    public AudioSource mAudioSource;
    public AudioClip mAudioClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementScore(int aScore)
    {
        mCurrentScore += aScore;
        this.GetComponent<TextMeshPro>().text = "Score: " + mCurrentScore;
        mAudioSource.PlayOneShot(mAudioClip);
    }
}
