using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class DialogueBoxManager : MonoBehaviour
{
    public static DialogueBoxManager Instance { get; private set; }

    [SerializeField]
    private RectTransform _dialogueBoxTrans = null;

    private TMP_Text _text;

    public void Awake()
    {
        // Singleton
        if (Instance != null) Destroy(Instance);
        Instance = this;

        _text = _dialogueBoxTrans.GetComponentInChildren<TMP_Text>(true);
        GHSUtility.CheckNull(_text);
    }

    //////////////////////////////////////////////////
    public void UpdateText(string newText, Vector3 talkerPos, bool playPopAni = true)
    {
        // Show UI
        if (!IsChating())
        {
            SetBoxActive(true);
        }

        // Play animation
        if (playPopAni)
        {
            var ani = this.gameObject.GetComponent<Animator>();
            ani.SetTrigger("In"); 
        }

        // Remove tag
        if (newText.EndsWith("}"))
        {
            newText = newText.Remove(newText.IndexOf('{'));
        }

        /// Replace dialogue box.
        this.transform.position = GHSUtility.WorldToViewportPoint(talkerPos);

        _text.text = newText;
    }

    public void CloseDiaolugeBox()
    {
        SetBoxActive(false);
    }

    //////////////////////////////////////////////////
    private void SetBoxActive(bool activity)
    {
        transform.GetChild(0).gameObject.SetActive(activity);
    }

    public bool IsChating()
    {
        return transform.GetChild(0).gameObject.activeSelf;
    }
}