# PSDLayerName Parser

PSDLayerNameParser is a PSD layer and layer group parser.
It corresponds to the unicode layer name.

## How to get

* [nuget - PSDLayerNameParser](https://www.nuget.org/packages/PSDLayerNameParser/0.1.0)

## How to use

```csharp
// rootElement is a root node. It doesn't have Name.
var rootElement = Parser.Parse("./TestData.psd");

// Get a layers and layer groups tree as a json string.
rootElement.Serialize();  // return {"Children": [{"Children": [{"Children": [],"IsGroup": false,"Name": "Layer1-1"},{"Children": [],"IsGroup": false,"Name": "Layer1-2"}],"IsGroup": true,"Name": "Group1"},{"Children": [],"IsGroup": false,"Name": "日本語のレイヤー"}],"IsGroup": true,"Name": ""}

// Get children of root element or layer group element.
rootElement.GetChildren();  // return LayerElement[]

var firstChild = rootElement.GetChild(0);
// Get layer or layer group name.
firstChild.Name;
// Get a flag indicating whether it is a group layer or not.
firstChild.IsGroup;
```
