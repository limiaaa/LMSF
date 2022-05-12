using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayTimerHelper
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

    private List<TimeInfo> mList = new List<TimeInfo>();
    private int index = 0;
    public int Delay(float time, Action callback)
    {
        index++;
        mList.Add(new TimeInfo(index, time, 1, callback));
        return index;
    }
    public int DelayLoop(float time, Action callback, int loop_num)
    {
        index++;
        mList.Add(new TimeInfo(index, time, loop_num, callback));
        return index;
    }
    
    public void Stop(int id , bool OnComplate = false)
    {
        if (id == -1) { return; }
        for (int i = 0; i < mList.Count; i++)
        {
            var info = mList[i];
            if (info.id == id)
            {
                info.finished = true;
                if(OnComplate) info.m_callback?.Invoke();
                break;
            }
        }
    }

    public void StopAll(bool OnComplate = false)
    {
        for (int i = 0; i < mList.Count; i++)
        {
            var info = mList[i];
            info.finished = true;
            if(OnComplate) info.m_callback?.Invoke();
        }
    }

    public void Remove(int id)
    {
        for (int i = 0; i < mList.Count; i++)
        {
            var info = mList[i];
            if (info.id == id)
            {
                mList.RemoveAt(id);
                info.m_callback = null;
                info = null;
                break;
            }
        }
    }
    
    public void RemoveAll()
    {
        for (int i = 0; i < mList.Count; i++)
        {
            var info = mList[i];
            info.m_callback = null;
            info = null;
        }

        mList.Clear();
        index = 0;
    }

    private TimeInfo info = null;
    // Update is called once per frame
    public void FixedUpdate()
    {
        if (mList.Count <= 0) { return; }
        for (int i = mList.Count - 1; i >= 0; i--)
        {
            info = mList[i];
            info.left -= Time.fixedDeltaTime;
            if (info.left < 0)
            {
                if (info.reset() == false)
                {
                    info.finished = true;
                }
                else if (info.finished == false)
                {
                    info.m_callback?.Invoke();
                }
            }
           
        }
        for (int i = mList.Count - 1; i >= 0; i--)
        {
            info = mList[i];
            if (info.finished)
            {
                mList.RemoveAt(i);
            }
        }
    }
}
