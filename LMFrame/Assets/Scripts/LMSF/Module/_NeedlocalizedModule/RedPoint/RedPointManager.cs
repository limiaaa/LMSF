using LMSF.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPointManager : MonoSingleton<RedPointManager>
{

    Transform ThemBtn;
    Transform MilestoneBtn;
    Transform ThemBtn2;
    Transform MilestoneBtn2;
    bool OpenThemRedPoint;
    bool OpenMilestoneRedPoint;

    public void InitMainTransform(Transform them,Transform milestone)
    {
        ThemBtn = them;
        MilestoneBtn = milestone;
    }
    internal void InitMainTransform2(Transform them, Transform milestone)
    {
        ThemBtn2 = them;
        MilestoneBtn2 = milestone;
    }
    public void RefeshThemRedPoint(bool OpenThemRed)
    {
        OpenThemRedPoint = OpenThemRed;
        RefehMainRedState();
    }
    public void RefeshMilestoneRedPoint(bool OpenMilestoneRed)
    {
        OpenMilestoneRedPoint = OpenMilestoneRed;
        RefehMainRedState();
    }
    public void RefehMainRedState()
    {
        if (ThemBtn)
        {
            ThemBtn.Find("RedPoint").gameObject.SetActive(OpenThemRedPoint);
        }
        if (MilestoneBtn)
        {
            MilestoneBtn.Find("RedPoint").gameObject.SetActive(OpenMilestoneRedPoint);
        }
        if (ThemBtn2)
        {
            ThemBtn2.Find("RedPoint").gameObject.SetActive(OpenThemRedPoint);
        }
        if (MilestoneBtn2)
        {
            MilestoneBtn2.Find("RedPoint").gameObject.SetActive(OpenMilestoneRedPoint);
        }
    }


}
