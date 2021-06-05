using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class GoatHousePlayableAsset : PlayableAsset
{
    public ExposedReference<GameObject> Goat;
    public ExposedReference<GameObject> House;
    public ExposedReference<GameObject> HouseTrigger;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var behaviour = new GoatHouseBehaviour
        {
            House = House.Resolve(graph.GetResolver()),
            Goat = Goat.Resolve(graph.GetResolver()),
            HouseTrigger = HouseTrigger.Resolve(graph.GetResolver())
        };
        return ScriptPlayable<GoatHouseBehaviour>.Create(graph, behaviour);
    }
}