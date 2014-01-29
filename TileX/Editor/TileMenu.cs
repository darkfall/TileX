using UnityEditor;
using UnityEngine;

class TileMenu {

	[MenuItem("TileX/Tile Editor", false, 1)]
	static void OpenTileEditor() {
		TileMapEditor.Show();
	}

	[MenuItem("TileX/Create TileMap", false, 2)]
	static void CreateTileMap() {
		GameObject obj = Selection.activeGameObject;
		if(obj != null) {
			obj.AddComponent<TileMap>();
			TileMapEditor.Get ().OnSelectionChange();
		}
	}
}
