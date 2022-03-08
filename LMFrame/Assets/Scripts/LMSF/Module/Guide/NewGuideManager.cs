using LMSF.Utils;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IListenerGuide
{
    void OnGuideStart(GuideSteps step);
    void OnGuideEnd(GuideSteps step);
}
public enum GuideSteps
{
    Guide_0 = 0,//没有新手引导
    Guide_1 = 1,//点第一个方块
    Guide_2 = 2,//点第二个方块
    Guide_3 = 3,//点第三个方块
    Guide_4 = 4,//旋转
    Guide_5 = 5,//第一关完了
    Guide_6 = 6,//提示点击加号
    Guide_7 = 7,//提示胜利界面点击奖杯
    Guide_8 = 8,//引导点击里程碑界面领取奖励按钮
    Guide_9 = 9,//引导点击里程碑界面返回
    Guide_Undo = 100,//撤回
    Guide_Tips = 101,//提示
}

public class NewGuideManager : MonoSingleton<NewGuideManager>
{
    GuideSteps m_currentGuideStep=GuideSteps.Guide_0;
    bool IsOnGuide=false;
    UINewGuidePage UIGuide;
    public void InitGuideManager()
    {
        //UIGuide = UIManager.Instance.OpenPage<UINewGuidePage>();
        UIGuide.gameObject.SetActive(false);
    }
   public void StartGuide(GuideSteps Step,IListenerGuide IGuide)
    {
        if (IsOnGuide)
        {
            DebugUtils.Log("正在进行新手引导：" + m_currentGuideStep);
        }
        //DebugUtils.Log("StartGuide_" + Step);
        //IsOnGuide = true;
        //m_currentGuideStep = Step;
        //UIGuide?.InitGuideUI();
        ////SendAct.ActGuide("start", Step.ToString(), (int)Step);
        //IGuide.OnGuideStart(Step);
        //UIGuide?.gameObject.SetActive(true);
    }
    public void EndGuide(GuideSteps Step, IListenerGuide IGuide)
    {
        if (GetCurrentGuideStep() != Step)
        {
            DebugUtils.Log("想结束的新手引导与正在进行的新手引导不一致：" + Step + "--" + GetCurrentGuideStep());
            return;
        }
        DebugUtils.Log("EndGuide_" + Step);
        SetGuideStepOver(Step);
        IsOnGuide = false;
        m_currentGuideStep = GuideSteps.Guide_0;
        UIGuide?.gameObject.SetActive(false);
        //SendAct.ActGuide("complete", Step.ToString(), (int)Step);
        IGuide.OnGuideEnd(Step);
    }
    //状态检查*******************************************************
    //获取当前正在进行的新手引导
    public GuideSteps GetCurrentGuideStep()
    {
        if (IsOnGuide)
        {
            return m_currentGuideStep;
        }
        else
        {
            return GuideSteps.Guide_0;
        }
    }
    //是否正在新手引导中
    public bool GetIsOnGuide()
    {
        return IsOnGuide;
    }
    //检查某一步新手引导是否已经执行
    public bool CheckGuideStepIsOver(GuideSteps step)
    {
        return false;
        //需要本地储存判断
        //if (LocalDataMgr.Instance.mGameData.OverGuideList.Contains((int)step))
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }
    //结束一步新手引导
    public void SetGuideStepOver(GuideSteps step)
    {
        //需要本地储存
        //if (!LocalDataMgr.Instance.mGameData.OverGuideList.Contains((int)step))
        //{
        //    LocalDataMgr.Instance.mGameData.OverGuideList.Add((int)step);
        //    LocalDataMgr.Instance.SaveAll();
        //}
    }
    //控制UI*******************************************************
    public void ShowText(int guideId, float PosY, int DistanceState)
    {
        //GuideConfig config = GuideConfig.GetData(guideId);

        //UIGuide?.ShowText(config.IntroduceKey, PosY, DistanceState);
    }
    //设置手指
    public void ShowFinger(int FingerType, Vector3 Pos,float OffisePosY, Vector3 Rotation)
    {
        //Pos.y = Pos.y+OffisePosY;
        //Pos.z = -2;
        //UIGuide?.ShowFinger(FingerType, Pos, Rotation);
    }
    public void ShowMaskBtn(Action p)
    {
        //UIGuide?.ShowMaskBtn(p);
    }
    public void SaveDragFunc(GuideSteps step, Action<PointerEventData> Draging, Action EndDrag)
    {
        //UIGuide?.SaveDragFunc(step, Draging, EndDrag);
    }
    //打开旋转提示
    public void ShowTip()
    {
        //UIGuide?.ShowTip();
    }
    public void ShowMask3D(Vector3 pos)
    {
        //UIGuide?.ShowMask3D(pos);
    }
    public void ShowCamera()
    {
        //UIGuide?.ShowCamera();
    }
    
}
