using System;
using System.Collections.Generic;
using UnityEngine;

namespace FeatureToggles
{
    [CreateAssetMenu(menuName = "FeatureToggles/Feature Config", fileName = "FeatureConfig")]
    public class FeatureConfig : ScriptableObject
    {
        [Serializable]
        public class FeatureDef
        {
            public FeatureId id;
            public string category = "General";
            public string displayName;
            [TextArea] public string description;
            public bool defaultDesired = true;
        }

        public List<FeatureDef> features = new List<FeatureDef>();

        public bool TryGet(FeatureId id, out FeatureDef def)
        {
            foreach (var f in features)
            {
                if (f.id.Equals(id))
                {
                    def = f;
                    return true;
                }
            }
            def = null;
            return false;
        }
    }
}