using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class NodeDataContainer : ScriptableObject
{
    public List<NodeData> NodesData = new List<NodeData>();
    public List<NodeConnectionData> NodesConnectionsData = new List<NodeConnectionData>();
}
