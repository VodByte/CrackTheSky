using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialoguePlayableAsset : PlayableAsset
{
    public ExposedReference<GameObject> talker;
    public string sentence;
    public Vector3 posBias;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var behaviour = new DialogueBehaviour
        {
            talker = talker.Resolve(graph.GetResolver()),
            sentence = sentence,
            posBias = posBias
        };
        return ScriptPlayable<DialogueBehaviour>.Create(graph, behaviour);
    }
}