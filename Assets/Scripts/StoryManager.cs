using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public static class StoryManager
{
    public static void Reset()
    {
        _isHoldWaterMagic = true;
        OnGetWaterMagic.RemoveAllListeners();
        _isGetHouseMission = false;
        OnGetHouseMission.RemoveAllListeners();
        IsProcessingHouseMission = false;
        _isHouseMissionBegined = false;
        OnSlovedHouseMission.RemoveAllListeners();
        _isHouseMissionFinished = false;
        _isProcessingGoatEvnet = false;
        _isGetRiverMission = false;
        OnGetRiverMission.RemoveAllListeners();
        IsProcessingRiverEvent = false;
        _isRiverEventBegined = false;
        _isRiverEventFinished = false;
        OnSlovedRiverEvent.RemoveAllListeners();
        _isGetForestlincense = false;
        OnGetForestLincense.RemoveAllListeners();
        _isGetWindMission = false;
        OnGetWindMission.RemoveAllListeners();
        IsProcessingWindEvent = false;
        _isWindMissionBegined = false;
        _isWindMissionFinished = false;
        OnSlovedWindMission.RemoveAllListeners();
    }

    public static bool IsProcessingEvent
    {
        get
        {
            return IsProcessingHouseMission ||
                IsProcessingRiverEvent ||
                IsProcessingGoatEvent;
        }
        private set { }
    }

    #region WaterMagic
    public static UnityEvent OnGetWaterMagic = new UnityEvent();
    private static bool _isHoldWaterMagic = true;
    public static bool IsHoldWaterMagic
    {
        get { return _isHoldWaterMagic; }
        set
        {
            if (value)
            {
                OnGetWaterMagic?.Invoke();
            }
            _isHoldWaterMagic = value;
        }
    }
    #endregion

    #region GetHouseMission
    private static bool _isGetHouseMission = false;
    public static bool IsGetHouseMission
    {
        get { return _isGetHouseMission; }
        set
        {
            if (value)
            {
                OnGetHouseMission?.Invoke();
            }
            _isGetHouseMission = value;
        }
    }
    public static UnityEvent OnGetHouseMission = new UnityEvent();
    #endregion

    #region HouseMission
    public static bool IsProcessingHouseMission { get; private set; }

    private static bool _isHouseMissionBegined = false;
    public static bool IsHouseMissionBegined
    {
        get
        {
            return _isHouseMissionBegined;
        }
        set
        {
            _isHouseMissionBegined = value;
            if (value)
            {
                IsProcessingHouseMission = true;
            }
        }
    }


    public static UnityEvent OnSlovedHouseMission = new UnityEvent();
    private static bool _isHouseMissionFinished = false;
    public static bool IsHouseMissionFinished
    {
        get { return _isHouseMissionFinished; }
        set 
        {
            if (value)
            {
                IsProcessingHouseMission = false;
                OnSlovedHouseMission?.Invoke();
            }
            _isHouseMissionFinished = value;
        }
    }
    #endregion

    #region GoatEvent
    private static bool _isProcessingGoatEvnet = false;
    public static bool IsProcessingGoatEvent
    {
        get
        {
            return _isProcessingGoatEvnet;
        }
        set
        {
            _isProcessingGoatEvnet = value;
        }
    }
    #endregion

    #region GetRiverMission
    private static bool _isGetRiverMission = false;
    public static bool IsGetRiverMission
    {
        get { return _isGetRiverMission; }
        set
        {
            if (value)
            {
                OnGetRiverMission?.Invoke();
            }
            _isGetRiverMission = value;
        }
    }
    public static UnityEvent OnGetRiverMission = new UnityEvent();
    #endregion

    #region RiverEvent
    public static bool IsProcessingRiverEvent { get; private set; }
    private static bool _isRiverEventBegined = false;
    public static bool IsRiverEventBegined
    {
        get
        {
            return _isRiverEventBegined;
        }
        set
        {
            _isRiverEventBegined = value;
            if (value)
            {
                IsProcessingRiverEvent = true;
            }
        }
    }
    private static bool _isRiverEventFinished = false;
    public static bool IsRiverEventFinished
    {
        get { return _isRiverEventFinished; }
        set
        {
            if (value)
            {
                OnSlovedRiverEvent?.Invoke();
                IsProcessingRiverEvent = false;
            }
            _isRiverEventFinished = value;
        }
    }
    public static UnityEvent OnSlovedRiverEvent = new UnityEvent();
    #endregion

    #region GetForestLincese
    private static bool _isGetForestlincense = false;
    public static bool IsGetForestLincense
    {
        get { return _isGetForestlincense; }
        set
        {
            if (value)
            {
                OnGetForestLincense?.Invoke();
            }
            _isGetForestlincense = true;
        }
    }
    public static UnityEvent OnGetForestLincense = new UnityEvent();
    #endregion

    #region GetWindMission
    private static bool _isGetWindMission = false;
    public static bool IsGetWindMission
    {
        get { return _isGetWindMission; }
        set
        {
            if (value)
            {
                OnGetWindMission?.Invoke();
            }
            _isGetWindMission = value;
        }
    }
    public static UnityEvent OnGetWindMission = new UnityEvent();
    #endregion

    #region WindEvent
    public static bool IsProcessingWindEvent { get; private set; }
    private static bool _isWindMissionBegined = false;
    public static bool IsWindEventBegined
    {
        get { return _isWindMissionBegined; }
        set
        {
            _isWindMissionBegined = value;
            if (value)
            {
                IsProcessingWindEvent = true;
            }
        }
    }
    public static UnityEvent OnSlovedWindMission = new UnityEvent();
    private static bool _isWindMissionFinished = false;
    public static bool IsWindMissionFinished
    {
        get { return _isWindMissionFinished; }
        set
        {
            if (value)
            {
                OnSlovedWindMission?.Invoke();
                IsProcessingWindEvent = false;
            }
            _isWindMissionFinished = value;
        }
    }
    #endregion
}