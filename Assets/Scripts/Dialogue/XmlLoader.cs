using System.Xml;
using UnityEngine;

public class XmlLoader {

    public XmlNodeList LoadXMLdata() {
        TextAsset xmlData = (TextAsset)Resources.Load(Constants.XmlData);
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlData.text);
        
        return xmlDocument.ChildNodes;
    }
}
