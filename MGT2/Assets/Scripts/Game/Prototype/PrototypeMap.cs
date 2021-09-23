using System;
using System.Xml;
using System.Collections.Generic;
using MFrameWork;
using UnityEngine;

public class PrototypeMap : BasePrototype
{
    public string Path { get; private set; }
    protected override void OnLoadData(XmlNode data)
    {
        Path = Utility.Xml.GetAttribute<string>(data, "Path");
    }
  
}