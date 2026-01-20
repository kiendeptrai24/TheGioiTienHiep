// Assets/_Game/WorldMap/Runtime/Travel/DestinationDatabase.cs
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap.Travel
{
    [CreateAssetMenu(menuName = "WorldMap/DestinationDatabase")]
    public class DestinationDatabase : ScriptableObject
    {
        public List<Destination> destinations = new List<Destination>();

        public Destination FindById(string id)
            => destinations.Find(d => d != null && d.id == id);

        public IEnumerable<Destination> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) yield break;
            text = text.ToLowerInvariant();

            foreach (var d in destinations)
            {
                if (d == null) continue;
                if (d.displayName != null && d.displayName.ToLowerInvariant().Contains(text))
                    yield return d;

                if (d.keywords != null)
                {
                    foreach (var k in d.keywords)
                    {
                        if (!string.IsNullOrEmpty(k) && k.ToLowerInvariant().Contains(text))
                        {
                            yield return d;
                            break;
                        }
                    }
                }
            }
        }
    }
}
