using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;

public class DelayTimeHelper : MonoBehaviour
{
    private class TimeInfo
    {
        public int id;
        //剩下时间
        public float left;
        public float time;
        // public bool repeat = false;
        public int loop_num;
        public bool finished;
        public int timeSpace;
        public Action m_callback;
        public TimeInfo(int _id, float _time, int _loop_num, Action callback)
        {
            id = _id;
            left = time = _time;
            loop_num = _loop_num;
            m_callback = callback;
            timeSpace = 1;
            finished = false;
        }

        public void update(float t)
        {
            left = left - t;
        }

        public bool reset()
        {
            left = time;
            if (loop_num == -1) { return true; }
            if (loop_num > 0) { loop_num--; return true; }
            return false;
        }
    }
    private ArrayList mList = new ArrayList();
    private int index = 0;
    public int delay_time_run(float time, Action callback)
    {
        index++;
        mList.Add(new TimeInfo(index, time, 1, callback));
        return index;
    }
    public int delay_time_run_loop(float time, Action callback, int loop_num)
    {
        index++;
        mList.Add(new TimeInfo(index, time, loop_num, callback));
        return index;
    }
    public void cancel_delay_run(int id)
    {
        if (id == -1) { return; }
        foreach (TimeInfo info in mList)
        {
            if (info.id == id)
            {
                info.finished = true;
                break;
            }
        }
    }
    public void Clear()
    {
        mList.Clear();
        index = 0;
    }
    private TimeInfo info = null;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (mList.Count <= 0) { return; }
        for (int i = mList.Count - 1; i >= 0; i--)
        {
            info = mList[i] as TimeInfo;
            info.left -= Time.fixedDeltaTime;
            if (info.left < 0)
            {
                if (info.reset() == false)
                {
                    info.finished = true;
                }
                else if (info.finished == false)
                {
                    info.m_callback();
                }
            }
           
        }
        for (int i = mList.Count - 1; i >= 0; i--)
        {
            info = mList[i] as TimeInfo;
            if (info.finished)
            {
                mList.RemoveAt(i);
            }
        }
    }
}
