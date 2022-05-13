using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextRoll : Text
{
    private long m_Cur;
    private long m_Target;
    private float m_Offset;
    private bool m_IsBegin = false;
    private bool m_IsAdd;
    private float CurCount;
    //����һ����ǰ����һ��Ŀ����������ʵ�ִӵ�ǰ���任��Ŀ������Ч��
    public void ChangeText(long cur, long target)
    {
        m_Cur = cur;
        m_Target = target;
        m_Offset = m_Target - m_Cur;
        m_IsBegin = true;
        CurCount = 0;

        if (m_Target < m_Cur)
            m_IsAdd = false;
        else if (m_Target > m_Cur)
            m_IsAdd = true;
    }

    void Update()
    {
        Debug.Log("??");
        if (!m_IsBegin)
            return;

        CurCount += m_Offset * Time.deltaTime * 2;
        if (Mathf.Abs(CurCount) > 1)
        {
            m_Cur += Mathf.FloorToInt(CurCount);
            CurCount -= Mathf.FloorToInt(CurCount);
        }

        if ((m_IsAdd && m_Cur > m_Target)
            || (!m_IsAdd && m_Cur < m_Target))
        {
            m_IsBegin = false;
            m_Cur = m_Target;
        }
        Debug.Log(CurCount + "__" + m_Cur);
        text = m_Cur.ToString();
    }
}
