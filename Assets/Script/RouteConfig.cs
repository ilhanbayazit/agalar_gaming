using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RouteConfig
{
    public string Ad;
    public Transform MerkezWaypointParent;
    public Transform SpawnPoint;
    public float flyHeight = 1.5f;

    [HideInInspector] public Transform lanesRoot;

    // DEĞİŞTİ: sabit lane0/lane1/lane2 yerine dinamik listeler
    [HideInInspector] public List<List<Transform>> lanes = new List<List<Transform>>();
    [HideInInspector] public List<List<Transform>> lanesFly = new List<List<Transform>>();

    [HideInInspector] public int lastLane = -1;
}

