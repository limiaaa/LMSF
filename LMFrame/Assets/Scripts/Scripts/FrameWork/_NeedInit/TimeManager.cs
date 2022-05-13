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
    public long ContinuousSignDay = 1;
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
    public void InitTime()
    {
        RefeshTimeLocalData();
        //获取离明天零点还有多少秒
        RefeshNextDayZeroTime();
        //检查是不是新的一天
        RefeshNewDayData();
        LogTimeData();
        StartTimeMachine();
    }
    public void LogTimeData()
    {
        DebugUtils.Log("--当前记录时间戳(上次退出)--" + NowTime + "--" + "\n"
                    + "--当前时间戳--" + GetTimeOffsetUtc() + "--" + "\n"
                    + "--这次登录的时间--" + ThisLoginTime + "--" + "\n"
                    + "--今天剩余秒数--" + ToDayRemindTime + "--" + "\n"
                    + "--从1970到现在是第几天--" + TotalDayFrom1970 + "--" + "\n"
                    + "--这次登录距离上次退出用了多久--" + ThisLoginAndLastExit + "--" + "\n"
                    + "--IsNewDay--" + IsNewDay + "--" + "\n"
                    + "--这是第几天登录--" + SortLoginDay + "--" + "\n"
                    + "--AllLoginDay--" + AllLoginDay + "--" + "\n"
                    + "--上次退出的天数--" + LastExitDay + "--" + "\n"
                    + "--连续登录天数(中断为1)--" + GetTimeDataByKey("ContinuousSignDay") + "--" + "\n"
                    + "--Utc时间偏移值(分)--" + localZone.BaseUtcOffset.TotalSeconds / 60 + "--" + "\n"
                    + "--连续登录天数--" + ContinuousSignDay + "--" + "\n"
                    );
    }
    //获取本地数据
    void RefeshTimeLocalData()
    {
        ContinuousSignDay = GetTimeDataByKey("ContinuousSignDay","1");
        //获取现在的时间，是0代表第一次登录
        NowTime = GetTimeDataByKey("NowTime");
        //获取上次退出时间
        LastExitTime = GetTimeDataByKey("LastExitTime");
        //获取从上次退出到这次登录用了多久
        ThisLoginAndLastExit = NowTime == 0 ? 0 : GetTimeOffsetUtc() - LastExitTime;
    }
    //获取离明天零点还有多少秒
    void RefeshNextDayZeroTime()
    {
        ToDayRemindTime = 86400 - (GetTimeOffsetUtc() % 86400);
    }
    //是不是新的一天
    void RefeshNewDayData()
    {
        //这次登录时间
        ThisLoginTime = GetTimeOffsetUtc();
        //获取上次登录的时间
        LastLoginTime = GetTimeDataByKey("LastLoginTime");
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
        DebugUtils.Log(TotalDayFrom1970 + "--" + LastExitDay);
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
        //记录连续登录的时间
        if (TotalDayFrom1970 - LastExitDay > 1)
        {
            ContinuousSignDay = 1;
        }
        else if (TotalDayFrom1970 - LastExitDay == 1)
        {
            ContinuousSignDay++;
        }
        SetTimeDataByKey("ContinuousSignDay", ContinuousSignDay);
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
                return DelyTimerManager.Instance.DelayLoop(timeSpace,
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
            ContinuousSignDay++;
            SetTimeDataByKey("ContinuousSignDay", ContinuousSignDay);
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
    //获取当前时间戳(不会加上UTC偏移时间)
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
        return long.Parse(LocalDataUtils.GetLocalData(key, defaultNumber));
    }
    void SetTimeDataByKey(string key,long number)
    {
        LocalDataUtils.SetLocalData(key, number.ToString());
    }
    
    //外部调用*****************************************************************
    //传入分钟数，获取结束时间戳
    public long GetTimeByMinute(long Minute)
    {
        return GetTimeOffsetUtc() + Minute * 60;
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
    //是否能正常获取时间
    public bool IsCanGetNormalTime = true;

}
