using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class DetectJoint : MonoBehaviour
{
    public BodySourceManager m_pBodySrcMng = null;
    public JointType TrackedJoint;

    Body[] Bodys = null;

    public float multip = 10.0f;

    internal bool m_bDetect = false;

    public bool IsDetect() { return m_bDetect; }

	// Use this for initialization
	void Start () {
        m_bDetect = false;
    }

    private void OnDestroy()
    {
        m_pBodySrcMng = null;
        Bodys = null;
    }

    // Update is called once per frame
    void Update () {
        m_bDetect = false;

        if (m_pBodySrcMng==null)
        {
            return;
        }
        if (m_pBodySrcMng.gameObject.activeSelf == false) return;

        //Bodys = m_pBodySrcMng.GetData();

        if (Bodys == null) return;

        for(int i=0;i< Bodys.Length;i++)
        {
            if (Bodys[i] == null) continue;
            if(Bodys[i].IsTracked==true)
            {
                CameraSpacePoint cp=  Bodys[i].Joints[TrackedJoint].Position;
                transform.localPosition = new Vector3(cp.X*multip, cp.Y * multip, 0.0f);
                m_bDetect = true;
            }
        }
    }
}
