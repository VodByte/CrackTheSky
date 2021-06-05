using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum HintType
{
    Movemment,
    Talk,
    Magic
}

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [SerializeField]
    private Color inActiveTextColor = new Color();
    [SerializeField]
    private Color activeTextColor = new Color();

    [Space][SerializeField]
    private Image _movementImage = null;
    [SerializeField]
    private TMP_Text _movementText = null;
    [Space][SerializeField]
    private Image _talkImage = null;
    [SerializeField]
    private TMP_Text _talkText = null;
    [Space][SerializeField]
    private Image _maigcImage = null;
    [SerializeField]
    private TMP_Text _magicText = null;

    [Space][SerializeField]
    private Sprite _windMagicSprite = null;
    [SerializeField]
    private Sprite _waterMagicSprite = null;
    [SerializeField]
    private Image _holdingMagicImg = null;
    [SerializeField]
    private ParticleSystemRenderer _magicInHand = null;

    private Texture _windMagicTexutre = null;
    private Texture _waterMagicTexutre = null;

    private void Awake()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;
    }

    private void Start()
    {
        _windMagicTexutre = GHSUtility.TextureFromSprite(_windMagicSprite);
        _waterMagicTexutre = GHSUtility.TextureFromSprite(_waterMagicSprite);

        Player.Instance.OnChangedMagic.AddListener(()=>
        {
            switch (Player.Instance.CurMagic)
            {
                case MagicType.Wind:
                    _holdingMagicImg.sprite = _windMagicSprite;
                    _magicInHand.material.SetTexture("_BaseMap", _windMagicTexutre);
                    break;
                case MagicType.Water:
                    _holdingMagicImg.sprite = _waterMagicSprite;
                    _magicInHand.material.SetTexture("_BaseMap", _waterMagicTexutre);
                    break;
            };
        });
    }

    private void Update()
    {
        if (Player.Instance == null) return;

        // Right up HUD
        var playerMark = Player.Instance.GetComponent<InteractiveableMark>();
        switch (playerMark.CurType)
        {
            case InteractiveType.Talkable:
                SetHintAcitivty(HintType.Talk, true);
                break;
            case InteractiveType.Magicable:
                SetHintAcitivty(HintType.Magic, true);
                break;
            case InteractiveType.None:
                SetHintAcitivty(HintType.Talk, false);
                SetHintAcitivty(HintType.Magic, false);
                break;
        }

        if (StoryManager.IsProcessingEvent || InventoryManager.Instance.IsOpening())
        {
            SetHintAcitivty(HintType.Movemment, false);
        }
        else
        {
            SetHintAcitivty(HintType.Movemment, true);
        }
    }

    public void SetHintAcitivty(HintType type, bool activity)
    {
        Image tarImg = null;
        TMP_Text tarText = null;
        switch (type)
        {
            case HintType.Movemment:
                tarImg = _movementImage;
                tarText = _movementText;
                break;
            case HintType.Talk:
                tarImg = _talkImage;
                tarText = _talkText;
                break;
            case HintType.Magic:
                tarImg = _maigcImage;
                tarText = _magicText;
                break;
        }

        if (activity)
        {
            tarImg.color = Color.white;
            tarText.color = activeTextColor;
        }
        else
        {
            tarImg.color = new Color(1, 1, 1, 0.6f);
            tarText.color = inActiveTextColor;
        }
    }
}