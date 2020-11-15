using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Union(0, typeof(Letter))]
[Union(1, typeof(UpdateRotatableAxis))]
public interface IMessageBody { }

[MessagePackObject]
public class Letter : IMessageBody
{
    [Key(0)]
    public string Name { get; set; }
    [Key(1)]
    public string Message { get; set; }
}

public enum RotatableAxis
{
    None = 0,
    X = 1,
    Y = 2,
    XY = 3,
}
[MessagePackObject]
public class UpdateRotatableAxis : IMessageBody
{
    [Key(0)]
    public RotatableAxis Axis { get; set; }
}