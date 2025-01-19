#define USE_SERIAL
//#define USE_UDP

using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Text;

public enum FINGER:int { FINGER_MIDDLE = 3, FINGER_RING = 4, FINGER_PINKY = 5, FINGER_POINTER = 0, FINGER_THUMB_A = 1, FINGER_THUMB_B = 2};

public class FingerState
{
    private volatile float mPosition;
    private volatile float mHardness;
    private volatile bool mDirty;
    

    public void SetPosition(float aPosition)
    {
        mPosition = aPosition;
        //Debug.Log("Set Position" + mPosition.ToString());
    }
    public void SetHardness(float aHardness)
    {
        //Debug.Log("setting hardness to " + aHardness);
        mHardness = aHardness;
        if(mHardness < 0.15 || mHardness > 1.0)
        {
            //Debug.Log("invalid hardness");
        }
        mDirty = true;
    }
    public void ClearDirty()
    {
        mDirty = false;
    }
    public float GetPosition()
    {
        //Debug.Log("Get Position" + mPosition.ToString());
        return mPosition;
    }
    public float GetHardness()
    {
        return mHardness;
    }
    public bool GetDirty()
    {
        return mDirty;
    }
    public FingerState(float aPosition, float ahardness)
    {
        mPosition = aPosition;
        mHardness = ahardness;
        mDirty = true;
    }
}

public class AnimateFingers : MonoBehaviour
{
    public GameObject mPinky0;
    public GameObject mPinky1;
    public GameObject mPinky2;

    public GameObject mRing0;
    public GameObject mRing1;
    public GameObject mRing2;

    public GameObject mMiddle0;
    public GameObject mMiddle1;
    public GameObject mMiddle2;

    public GameObject mPointer0;
    public GameObject mPointer1;
    public GameObject mPointer2;

    public GameObject mThumb0;
    public GameObject mThumb1;
    public GameObject mThumb2;

    public volatile Dictionary<int,FingerState> mFingerStates = new Dictionary<int,FingerState>();

    private bool runThread = true;

    Mutex COM_MUTEX;
#if USE_SERIAL
    SerialPort COMX;
#endif
#if USE_UDP
    UdpClient client;
    IPEndPoint gauntl33tEndpoint;
    IPEndPoint receiveEndpoint;
    string gauntl33tIP = "192.168.34.209";
    int gauntl33tPort = 2024;
    int receivePort = 2023;
#endif
    const int NUMBER_OF_MOTORS = 6;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("starting");
        COM_MUTEX = new Mutex();
        for (int i = 0; i < NUMBER_OF_MOTORS; i++)
        {
            mFingerStates[i]=new FingerState( 0.0f, 0.2f);
        }
        Thread launchCilia = new Thread(ReadSerial);
        launchCilia.Start();
        //Application.targetFrameRate = 72;
    }

    void OnApplicationQuit()
    {
        runThread = false;
        
    }

    void ReadSerial()
    {
#if USE_SERIAL
        COMX = new SerialPort();
        COMX.PortName = "COM9" +
            "";
        COMX.BaudRate = 921600;
        COMX.ReadTimeout = 100;
        COMX.WriteTimeout = 100;
        try
        {
            COMX.Open();
        }
        catch
        {
            Debug.Log("failed to open Serial");
        }
#endif
#if USE_UDP
        client = new UdpClient(receivePort);
        try
        {
            gauntl33tEndpoint = new IPEndPoint(IPAddress.Parse(gauntl33tIP), gauntl33tPort);
            receiveEndpoint = new IPEndPoint(IPAddress.Any, 0);
        }
        catch
        {
            Debug.Log("failed to open UDP");
        }
#endif

        while (runThread)
        {
            string updateBuffer = "";
            try
            {
#if USE_UDP
                byte[] data = client.Receive(ref receiveEndpoint);
                updateBuffer = Encoding.UTF8.GetString(data);
#endif
#if USE_SERIAL
                updateBuffer = COMX.ReadLine();
#endif
                //Debug.Log(updateBuffer);
            }
            catch
            {
                //Debug.Log("Failed to read message");
                continue;
            }
            try
            {
                FPJSON loadProfileObject = JsonConvert.DeserializeObject<FPJSON>(updateBuffer);
                if (loadProfileObject != null && loadProfileObject.FP != null)
                {
                    foreach (var fingerPosition in loadProfileObject.FP)
                    {
                        if (fingerPosition.Count > 1)
                        {
                            //Debug.Log("setting " + System.Convert.ToInt32(fingerPosition[0]).ToString() + " to " + System.Convert.ToSingle(fingerPosition[1]).ToString());
                            var rawFingerPos = System.Convert.ToSingle(fingerPosition[1]);
                            var clampedFingerPos = ClampFingerOrientation(rawFingerPos);
                            mFingerStates[System.Convert.ToInt32(fingerPosition[0])].SetPosition(clampedFingerPos);
                        }
                    }
                }
            }
            catch
            {
                continue;
            }
            
            TVJSON setFinger = new TVJSON();
            int count = 0;
            foreach (var fingerStateInst in mFingerStates)
            {
                if (fingerStateInst.Value.GetDirty())
                {
                    count++;
                    fingerStateInst.Value.ClearDirty();
                    setFinger.TV.Add(new List<object> { fingerStateInst.Key, fingerStateInst.Value.GetHardness() });
                }
            }
            if (count > 0)
            {
                var StringToWrite = JsonConvert.SerializeObject(setFinger) + "\n";
                //Debug.Log(StringToWrite);
                COM_MUTEX.WaitOne();
#if USE_UDP
                var buffer = Encoding.UTF8.GetBytes(StringToWrite);
                client.Send(buffer, buffer.Length,gauntl33tEndpoint);
#endif
#if USE_SERIAL
                COMX.Write(StringToWrite);
#endif
                COM_MUTEX.ReleaseMutex();
            }
        }
#if USE_UDP
        client.Close();
#endif
#if USE_SERIAL
        COMX.Close();
#endif
    }

    float ClampFingerOrientation(float aOrientation)
    {
        if (aOrientation < 0.0f)
        {
            aOrientation = 0.0f;
        }
        else if (aOrientation > 1.0f)
        {
            aOrientation = 1.0f;
        }
        return aOrientation;
    }

    // Update is called once per frame
    void Update()
    {
        var pinkyOrientation = mFingerStates[(int)FINGER.FINGER_PINKY].GetPosition();
        mPinky0.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f * pinkyOrientation);
        mPinky1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -60.0f * pinkyOrientation);
        mPinky2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -45.0f * pinkyOrientation);
        var ringOrientation = mFingerStates[(int)FINGER.FINGER_RING].GetPosition();
        mRing0.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f * ringOrientation);
        mRing1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -70.0f * ringOrientation);
        mRing2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -45.0f * ringOrientation);
        var middleOrientation = mFingerStates[(int)FINGER.FINGER_MIDDLE].GetPosition();
        mMiddle0.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f * middleOrientation);
        mMiddle1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -70.0f * middleOrientation);
        mMiddle2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -45.0f * middleOrientation);
        var pointerOrientation = (1.0f - mFingerStates[(int)FINGER.FINGER_POINTER].GetPosition());
        mPointer0.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f * pointerOrientation);
        mPointer1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -70.0f * pointerOrientation);
        mPointer2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -45.0f * pointerOrientation);
        var thumbOrientationA = (1.0f - mFingerStates[(int)FINGER.FINGER_THUMB_A].GetPosition());
        var thumbOrientationB = (1.0f - mFingerStates[(int)FINGER.FINGER_THUMB_B].GetPosition());

        var angle1 = 0.0f;
        if ((thumbOrientationA + thumbOrientationB) != 0.0f)
        {
            angle1 = Mathf.Clamp01(thumbOrientationA / (thumbOrientationA + thumbOrientationB));
        }
        var angle2 = Mathf.Clamp01((thumbOrientationA + thumbOrientationB - 1.0f) / 1.0f);
       

        //angle2 goes from 0 at 90 to 30 at 1


        mThumb0.transform.localRotation = Quaternion.Euler(90.0f * angle1 + 40.0f, -90.0f, 90.0f-(60.0f * angle2));
        mThumb1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -30.0f * angle2);
        mThumb2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f * angle2);
    }

    public void SetHardness(FINGER afingerNumber, float ahardnessVal)
    {
        //Debug.Log("setting hardness 1");
        mFingerStates[(int)afingerNumber].SetHardness(ahardnessVal);
    }
}
