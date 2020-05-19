using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Shooter_Clay_Fragment : MonoBehaviour
{
    List<Rigidbody> m_pList = null;
    List<Transform> m_pList_Transform = null;
    List<Vector3> m_pList_Vector = null;

    public void Enter()
    {
        m_pList = new List<Rigidbody>();
        m_pList_Transform = new List<Transform>();
        m_pList_Vector = new List<Vector3>();

        Transform frags = transform.Find("fragments");
        foreach (Transform obj in frags)
        {
            m_pList_Transform.Add(obj.transform);
            m_pList_Vector.Add(obj.localPosition);
            m_pList.Add(obj.GetComponent<Rigidbody>());
        }
    }

    public void Reset_Fragment()
    {
        for (int index = 0; index < m_pList_Transform.Count; index++)
            m_pList_Transform[index].localPosition = m_pList_Vector[index];
    }

    public void Destroy()
    {
        StopCoroutine("Cor_ForCe");
        if (m_pList!=null)
        {
            m_pList.Clear();
            m_pList = null;
        }
        if (m_pList_Vector != null)
        {
            m_pList_Vector.Clear();
            m_pList_Vector = null;
        }
    }

    public void AddForce(float fPower)
    {
        for(int i=0;i<m_pList.Count;i++)
        {
            m_pList[i].transform.localPosition = m_pList_Vector[i];
            m_pList[i].transform.rotation = Quaternion.Euler(Vector3.zero);
            m_pList[i].gameObject.transform.localScale = Vector3.one;
            m_pList[i].velocity = new Vector3(0.0f, 0.0f, 0.0f);
            m_pList[i].AddForce(Random.Range(-fPower, fPower), Random.Range(-fPower, fPower), Random.Range(-fPower, fPower));
            m_pList[i].AddTorque(Random.Range(-fPower, fPower), Random.Range(-fPower, fPower), Random.Range(-fPower, fPower));
        }

        StartCoroutine("Cor_ForCe");
    }

    IEnumerator Cor_ForCe()
    {
        yield return new WaitForSeconds(1.0f);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime*2.0f)
        {
            for (int i = 0; i < m_pList.Count; i++)
                m_pList[i].gameObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            yield return null;
        }
    }
}