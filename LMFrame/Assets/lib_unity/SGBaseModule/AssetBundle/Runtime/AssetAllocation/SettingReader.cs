using UnityEngine;
using System.Collections;

namespace SG
{
    public class SettingReader : IScriptableObjectReader<SettingReader, SettingAsset>
    {
        protected override string SubGameScriptableObjectAssetNameInResources()
        {
            return "SubGameSettingAsset";
        }

        protected override string ScriptableObjectAssetNameInResources()
        {
            return "SettingAsset";
        }
    }
}
