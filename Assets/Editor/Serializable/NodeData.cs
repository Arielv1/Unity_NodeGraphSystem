using System;
using UnityEngine;

[Serializable]
public class NodeData 
{
    public string NodeGUID;
    public NodeType.Type type;
    public Vector2 Position;

    public NodeData(string NodeGUID, NodeType.Type type, Vector2 Position)
    {
        this.NodeGUID = NodeGUID;
        this.type = type;
        this.Position = Position;
    }

}
