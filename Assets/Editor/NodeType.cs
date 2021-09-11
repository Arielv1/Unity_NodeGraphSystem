using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class NodeType : Node
{
    public enum Type
    {
        TypeA,
        TypeB,
        TypeC,
        Bridge
    }

    public string GUID;
    public Type type;

    public NodeType(string GUID, Type type)
    {
        this.GUID = GUID;
        this.type = type;
    }

        

}
