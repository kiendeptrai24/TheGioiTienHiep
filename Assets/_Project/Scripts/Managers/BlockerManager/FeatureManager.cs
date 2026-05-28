using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace FeatureToggles
{
    public sealed class FeatureManager : Singleton<FeatureManager>
    {

        [Header("Optional config (default values, names, categories)")]
        [SerializeField] private FeatureConfig config;

        [Header("Persistence")]
        [SerializeField] private bool loadFromPlayerPrefs = true;
        [SerializeField] private bool saveToPlayerPrefs = true;
        [SerializeField] private string playerPrefsPrefix = "FeatureToggle_";

        public event Action<FeatureId, bool> OnFeatureEffectiveChanged;
        public event Action<FeatureId, bool> OnFeatureDesiredChanged;

        private class FeatureState
        {
            public bool desired;
            public bool effective;
            public HashSet<string> blockers = new HashSet<string>();
        }

        private readonly Dictionary<FeatureId, FeatureState> _states = new();

        protected override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            InitAllFeatures();
        }

        private void InitAllFeatures()
        {
            _states.Clear();

            foreach (FeatureId id in Enum.GetValues(typeof(FeatureId)))
            {
                if (!_states.ContainsKey(id))
                    _states[id] = new FeatureState();

                var st = _states[id];

                // default desired từ config nếu có
                bool defaultDesired = true;
                if (config != null && config.TryGet(id, out var def))
                    defaultDesired = def.defaultDesired;

                st.desired = defaultDesired;

                if (loadFromPlayerPrefs)
                {
                    string key = playerPrefsPrefix + id;
                    if (PlayerPrefs.HasKey(key))
                        st.desired = PlayerPrefs.GetInt(key, st.desired ? 1 : 0) == 1;
                }

                Recompute(id, fireEvents: false);
            }
        }

        public bool GetDesired(FeatureId id) => GetState(id).desired;

        public bool IsEnabled(FeatureId id) => GetState(id).effective;

        public IReadOnlyCollection<string> GetBlockers(FeatureId id) => GetState(id).blockers;

        public void SetDesired(FeatureId id, bool desired)
        {
            var st = GetState(id);
            if (st.desired == desired) return;

            st.desired = desired;

            if (saveToPlayerPrefs)
            {
                string key = playerPrefsPrefix + id;
                PlayerPrefs.SetInt(key, desired ? 1 : 0);
                PlayerPrefs.Save();
            }

            OnFeatureDesiredChanged?.Invoke(id, desired);
            Recompute(id, fireEvents: true);
        }

        public void AddBlocker(FeatureId id, string source)
        {
            if (string.IsNullOrWhiteSpace(source)) source = "Unknown";
            var st = GetState(id);
            if (st.blockers.Add(source))
            {
                Recompute(id, fireEvents: true);
            }
        }

        public void RemoveBlocker(FeatureId id, string source)
        {
            var st = GetState(id);
            if (st.blockers.Remove(source))
            {
                Recompute(id, fireEvents: true);
            }
        }

        public void ClearBlockers(FeatureId id)
        {
            var st = GetState(id);
            if (st.blockers.Count == 0) return;
            st.blockers.Clear();
            Recompute(id, fireEvents: true);
        }

        private FeatureState GetState(FeatureId id)
        {
            if (!_states.TryGetValue(id, out var st))
            {
                st = new FeatureState { desired = true };
                _states[id] = st;
                Recompute(id, fireEvents: false);
            }
            return st;
        }
        public int CheckBlock(FeatureId id, string source)
        {
            var st = GetState(id);
            return st.blockers.Count;
        }
        private void Recompute(FeatureId id, bool fireEvents)
        {
            var st = _states[id];
            bool newEffective = st.desired && st.blockers.Count == 0;


            if (st.effective == newEffective) return;
            st.effective = newEffective;
            if (fireEvents)
            {
                OnFeatureEffectiveChanged?.Invoke(id, newEffective);
            }
        }

        // Helper: lấy meta từ config để UI show đẹp
        public bool TryGetDefinition(FeatureId id, out FeatureConfig.FeatureDef def)
        {
            if (config == null)
            {
                def = null;
                return false;
            }
            return config.TryGet(id, out def);
        }
        public void Reset()
        {
            InitAllFeatures();
        }
    }
}