using System;

[Serializable]
public class NodeConnectionData
{
    public string BaseNodeGUID;
    public string TargetNodeGUID;
    public string OutputPortName;

    public NodeConnectionData(string BaseNodeGUID, string TargetNodeGUID, string OutputPortName)
    {
        this.BaseNodeGUID = BaseNodeGUID;
        this.TargetNodeGUID = TargetNodeGUID;
        this.OutputPortName = OutputPortName;
    }
}
