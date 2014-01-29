
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileLayer))]
class TileLayerInspector: Editor {
	
	public override void OnInspectorGUI() {
		TileLayer layer = (TileLayer)this.target;

		GUILayout.Label("Num Tiles = " + layer.tiles.Count);
		DrawDefaultInspector();
	}

}