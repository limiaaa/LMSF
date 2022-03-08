using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;

public class DeviceHelper : Singleton<DeviceHelper>
{
    public static bool IsLowDevice()
    {
        string gpuType = SystemInfo.graphicsDeviceType.ToString();
        if (SystemInfo.graphicsMemorySize <= 1024)
            return true;
        if (SystemInfo.systemMemorySize <= 2048)
            return true;

        return false;
    }
}
