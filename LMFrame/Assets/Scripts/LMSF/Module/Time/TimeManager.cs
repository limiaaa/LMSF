using LMSF.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>
{
    //上次登录时间
    public long LastLoginTime = 0;
    //到现在是第几天（从1970年开始算）
    public long TotalDayFrom1970 = 0;
    //这次登录的时间
    public long ThisLoginTime = 0;
    //今天剩余秒数
    public long ToDayRemindTime = 0;
    //是否是新的一天
    public bool IsNewDay = false;
    //现在的时间戳
    public long NowTime = 0;
    //总登录天数(可能中途没有登录)
    public long AllLoginDay = 0;
    //上次退出时间
    public long LastExitTime = 0;
    public long LastExitDay = 0;
    //这次登录距离上次退出用了多久
    public long ThisLoginAndLastExit = 0;
    //这是第几天登录
    public long SortLoginDay = 1;
    //连续登录天数，如果中断，重置为1;
    public int ContinuousSignDay = 1;
    //刷新列表，零点时调用
    private List<Action> RefeshFuncList = new List<Action>();
    //秒执行列表，每一秒调用
    private Dictionary<string, Action<long>> TimeFuncList = new Dictionary<string, Action<long>>();
    //UTC时间偏移
    TimeZoneInfo localZone = TimeZoneInfo.Local;
    public void InitGameFunc()
    {
        InitTime();
    }

    void LogTimeDataBefore()
    {
    }
    public void LogTimeDataAfter()
    {
        DebugUtils.Log("当前记录时间戳(上次退出)--" + NowTime);
        DebugUtils.Log("当前时间戳--" + GetTimeOffsetUtc());
        DebugUtils.Log("这次登录的时间--" + ThisLoginTime);
        DebugUtils.Log("今天剩余秒数--" + ToDayRemindTime);
        DebugUtils.Log("从1970到现在是第几天--" + TotalDayFrom1970);
        DebugUtils.Log("这次登录距离上次退出用了多久--" + ThisLoginAndLastExit);
        DebugUtils.Log("IsNewDay--" + IsNewDay);
        DebugUtils.Log("这是第几天登录--" + SortLoginDay);
        DebugUtils.Log("AllLoginDay--" + AllLoginDay);
        DebugUtils.Log("上次退出的天数--" + LastExitDay);
        DebugUtils.Log("连续登录天数(中断为1)--" + GetTimeDataByKey("ContinuousSignDay"));
        DebugUtils.Log("Utc时间偏移值(分)--" + localZone.BaseUtcOffset.TotalSeconds/ 60);
        
    }
    public void InitTime()
    {
        RefeshTimeLocalData();
        LogTimeDataBefore();
        //获取当前的时间
        RefeshNowTime();
        //获取离明天零点还有多少秒
        RefeshNextDayZeroTime();
        //检查是不是新的一天
        RefeshNewDayData();
        LogTimeDataAfter();
        StartTimeMachine();
    }
    //获取本地数据
    void RefeshTimeLocalData()
    {
    }
    //处理第几次登录

    //获取当前的时间
    void RefeshNowTime()
    {
        //获取上一次游戏时最后的时间戳
        NowTime = GetTimeDataByKey("NowTime");
        if (NowTime <= 0)
        {
            LastExitTime = 0;
            //计算这次登录距离上次退出用了多少时间
            ThisLoginAndLastExit = 0;
        }
        else
        {
            LastExitTime = GetTimeDataByKey("LastExitTime");
            //计算这次登录距离上次退出用了多少时间
            ThisLoginAndLastExit = NowTime == 0 ? 0 : GetTimeOffsetUtc() - LastExitTime;
        }
    }
    //获取离明天零点还有多少秒
    void RefeshNextDayZeroTime()
    {
        ToDayRemindTime = 86400 - (GetTimeOffsetUtc() % 86400);
    }
    //是不是新的一天
    void RefeshNewDayData()
    {
        ThisLoginTime = GetTimeOffsetUtc();
        //获取上次登录的时间
        LastLoginTime = GetTimeDataByKey("LastLoginTime");
        DebugUtils.Log("上次登录时间--" + GetTimeDataByKey("LastLoginTime"));
        //获取今天登录离1970年多久了
        TotalDayFrom1970 = (long)Mathf.Floor((ThisLoginTime / 86400));
        //上次退出是在哪一天
        LastExitDay= (long)Mathf.Floor((LastExitTime / 86400));
        //记录下离第一次登录有几天
        if (GetTimeDataByKey("TotalDayFrom1970") == 0)
        {
            SetTimeDataByKey("SortLoginDay", this.TotalDayFrom1970);
            SortLoginDay = 1;
        }
        else
        {
            long time = GetTimeDataByKey("SortLoginDay") == 0 ? TotalDayFrom1970 : GetTimeDataByKey("SortLoginDay");
            SortLoginDay = TotalDayFrom1970 - time + 1;
        }
    
        //计算现在的值和储存的值是不是有区别，有就是新的一天,并记录进总天数
        DebugUtils.Log(TotalDayFrom1970 + "-X-X-" + LastExitDay);
        if (TotalDayFrom1970 > LastExitDay)
        {
            IsNewDay = true;
            SetTimeDataByKey("TotalDayFrom1970", TotalDayFrom1970);
            AllLoginDay = GetTimeDataByKey("AllLoginDay");
            AllLoginDay++;
            SetTimeDataByKey("AllLoginDay", AllLoginDay);
        }
        else
        {
            IsNewDay = false;
            AllLoginDay= GetTimeDataByKey("AllLoginDay");
        }
        //如果这次登录距离上次登录天数大于1,

        //刷新最后一次登录时间
        LastLoginTime = ThisLoginTime;
        SetTimeDataByKey("LastLoginTime", LastLoginTime);
    }
    //刷新本地时间戳
    void InitNowTime()
    {
        NowTime = GetTimeOffsetUtc();
        SetTimeDataByKey("NowTime", NowTime);
        SetTimeDataByKey("LastExitTime",NowTime);
    }
    //通过结束时间戳去循环执行方法块(暂时不对外开放)
    int ObserveLeftWithTimeStamp(long endTimeTs, Action<long> onTick, Action<long> onEnd, int timeSpace = 1)
    {
        long left = endTimeTs - GetTimeOffsetUtc();
        Action<long> onend = onEnd;
        if (left > 0)
        {
            onTick(left);
            if (endTimeTs <= GetTimeOffsetUtc())
            {
                Debug.Log("调用循环执行失败--" + endTimeTs + "-小于当前时间戳-" + GetTimeOffsetUtc() + "--执行结束回调");
                onEnd(left);
                return 0;
            }
            else
            {
                return DelayTimeManager.delay_time_run_loop(timeSpace,
                () =>
                {
                    left = endTimeTs - GetTimeOffsetUtc();
                    if (left > 0)
                    {
                        onTick(left);
                    }
                    else
                    {
                        if (onend != null)
                        {
                            onend(left);
                            onend = null;
                        }
                    }
                },
                (int)left);
            }
        }
        else
        {
            DebugUtils.Log("调用循环执行失败--" + endTimeTs + "-小于当前时间戳-" + GetTimeOffsetUtc() + "--执行结束回调");
            onEnd(left);
            return 0;
        }
    }
    //传入分钟数，获取结束时间戳
    public long GetTimeByMinute(long Minute)
    {
        return GetTimeOffsetUtc() + Minute * 60;
    }
    //打开一个一直执行的倒计时，执行到今天晚上零点，零点后重启刷新
    void StartTimeMachine()
    {
        ObserveLeftWithTimeStamp((long)GetTimeOffsetUtc() + ToDayRemindTime, (long time) => {
            RefeshNextDayZeroTime();
            GoAllFuncToTime(ToDayRemindTime);
        }, 
        (long time) =>{
            DebugUtils.Log("TodayIsOver*********");
            ToDayRemindTime = 86400;
            IsNewDay = true;
            AllLoginDay++;
            //ContinuousSignDay++;
            //SetTimeDataByKey("ContinuousSignDay", ContinuousSignDay);

            TotalDayFrom1970++;
            SetTimeDataByKey("AllLoginDay", AllLoginDay);
            SetTimeDataByKey("TotalDayFrom1970", TotalDayFrom1970);
            RefeshFuncByDay();
            StartTimeMachine();
        });
    }
    void RefeshFuncByDay()
    {
        foreach(var item in RefeshFuncList)
        {
            item();
        }
    }
    void GoAllFuncToTime(long toDayRemindTime)
    {
        foreach (var func in TimeFuncList)
        {
            if (func.Value != null)
            {
                func.Value(toDayRemindTime);
            }
        }
        InitNowTime();
    }
    //添加一个外用接口，将所有要隔天刷新的方法写入
    public void AddFuncToRefeshByDay(Action refeshfunc)
    {
        RefeshFuncList.Add(refeshfunc);
    }
    //添加方法入循环迭代器
    public void AddFuncToTime(string key, Action<long> timeFunc)
    {
        if (TimeFuncList.ContainsKey(key))
        {
            TimeFuncList[key] = timeFunc;
        }
        else
        {
            timeFunc(ToDayRemindTime);
            TimeFuncList.Add(key, timeFunc);
        }
    }
    //删除循环迭代器方法
    public void RemoveFuncToTime(string key)
    {
        if (TimeFuncList.ContainsKey(key))
        {
            TimeFuncList.Remove(key);
        }

    }
    //传入秒数，展示倒计时;
    public List<string> FormatTime(long second)
    {
        long hour = second / 3600;
        second = second % 3600;
        long minute = second / 60;
        second = second % 60;
        List<string> li = new List<string>();
        li.Add(string.Format("{0:D2}",second));
        li.Add(string.Format("{0:D2}", minute));
        li.Add(string.Format("{0:D2}", hour));
        return li;
    }
    public List<string> FormatTimeOnlyMin(long second)
    {
        long hour = second / 3600;
        second = second % 3600;
        long minute = second / 60+ hour*60;
        second = second % 60;
        List<string> li = new List<string>();
        li.Add(string.Format("{0:D2}", second));
        li.Add(string.Format("{0:D2}", minute));
        li.Add(string.Format("{0:D2}", 0));
        return li;
    }



    //获取当前时间戳(不会加上UTC偏移时间)
    //是否能正常获取时间
    public bool IsCanGetNormalTime = true;
    long GetTimeOffsetUtc()
    {
        //获取当前时间戳
        TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        //Debug.Log(localZone.IsDaylightSavingTime());
        return (long)Math.Floor((ts.TotalSeconds+ localZone.BaseUtcOffset.TotalSeconds ));
    }
    //获取当前时间戳(会加上UTC偏移时间)
    long GetLocalTime()
    {
        //获取当前时间戳
        TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        return (long)Math.Floor((ts.TotalSeconds));
    }
    long GetTimeDataByKey(string key,string defaultNumber="0")
    {
        return long.Parse(LocalDataManager.GetLocalData(key, defaultNumber));
    }
    void SetTimeDataByKey(string key,long number)
    {
        LocalDataManager.SetLocalData(key, number.ToString());
    }

}
