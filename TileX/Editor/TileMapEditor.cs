using UnityEditor;
using UnityEngine;
using System.Linq;

class TileMapEditor: EditorWindow {

	public static int TilesetActionbarHeight = 30;

	public static TileMapEditor Show() {
		return EditorWindow.GetWindow<TileMapEditor>("Tilemap Editor", true);
	}
	public static TileMapEditor Get() {
		return EditorWindow.GetWindow<TileMapEditor>("Tilemap Editor", false);
	}

	public TileMap selectedMap;
	public int     selectedTileSetIndex = -1;
	public int 	   selectedTileX = -1, selectedTileY = -1;
	public bool    isErase = false;

	public Vector2 tileViewScrollPosition;

	float _tileViewHeightRequired;

	public void OnFocus() {
		if(selectedMap == null)
			this.OnSelectionChange();
		_tileViewHeightRequired = this.position.height;
	}

	public void OnGUI() {
		EditorGUIUtility.labelWidth = 80;
		if(selectedMap != null) {
			this.TilesetGUI();
		} else 
			EditorGUILayout.HelpBox("Add Tilemap to start", MessageType.Info);
	}

	public void OnSelectionChange() {
		GameObject selectedObject = Selection.activeObject as GameObject;
		if(selectedObject != null) {
			selectedMap = selectedObject.GetComponent<TileMap>();
		} else
			selectedMap = null;
		if(selectedMap != null) {
			selectedMap.previewTile.sprite = null;
		}

		this.Repaint();
	}

	public void TilesetGUI() {
		GUILayout.BeginArea(new Rect(0, 6, this.position.width, this.position.height), "");
		{
			GUILayout.BeginVertical();
			{
				selectedTileSetIndex = EditorGUILayout.Popup(selectedTileSetIndex, selectedMap.tilesets.Select(o => o.ToString()).ToArray());

				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if(GUILayout.Button ("Add Tileset", GUILayout.Width(120))) {
					Tileset ts = new Tileset();
					selectedMap.tilesets.Add(ts);
					selectedTileSetIndex = selectedMap.tilesets.Count - 1;
					EditorUtility.SetDirty(selectedMap.gameObject);
				}
				if(GUILayout.Button("Remove Tileset", GUILayout.Width(120))) {
					if(selectedTileSetIndex != -1) {
						selectedMap.tilesets.RemoveAt (selectedTileSetIndex);
						if(selectedTileSetIndex >= selectedMap.tilesets.Count)
							selectedTileSetIndex = selectedMap.tilesets.Count - 1;
						EditorUtility.SetDirty(selectedMap.gameObject);
					}
				}
				GUILayout.EndHorizontal();

			}
			EditorGUILayout.Space();
			GUILayout.EndVertical();


			if(selectedTileSetIndex != -1 && selectedTileSetIndex < selectedMap.tilesets.Count) {
				Tileset ts = selectedMap.tilesets[selectedTileSetIndex];

				{
					GUILayout.BeginVertical("box");
					ts.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", ts.texture, typeof(Texture2D), false);

					GUILayout.BeginHorizontal();
					ts.tileWidth = EditorGUILayout.IntField("Tile Width", ts.tileWidth);
					ts.tileHeight = EditorGUILayout.IntField("Tile Height", ts.tileHeight);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					ts.spacing = EditorGUILayout.IntField("Spacing", ts.spacing);
					ts.margin = EditorGUILayout.IntField("Margin", ts.margin);
					GUILayout.EndHorizontal();

					if(GUILayout.Button("Generate Tiles")) {
						ts.CreateTileSprites(selectedMap);
					}
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					isErase = GUILayout.Toggle(isErase, "Eraser",  GUILayout.Width(60));
					GUILayout.EndHorizontal();
					
					GUILayout.EndVertical();
				}

				{
					if(ts.isValidindex(selectedTileX, selectedTileY)) {
						GUILayout.BeginVertical("box");

						TileInfo ti = ts.getTileInfo(selectedTileX, selectedTileY);
						GUILayout.BeginHorizontal();
						ti.editorExpanded = EditorGUILayout.Foldout(ti.editorExpanded, "Tile Attributes");

						ti.isHole = GUILayout.Toggle(ti.isHole, "Is Hole  |", GUILayout.Width(80));
						ti.isBlock = GUILayout.Toggle(ti.isBlock, "Is Block  |", GUILayout.Width(80));
						if(GUILayout.Button("New Attribute", "label", GUILayout.Width(80))) {
							ti.attributes.Add (new TileAttribute("New Attribute", ""));
						}
						GUILayout.EndHorizontal();
						if(ti.editorExpanded) {
							float ewidth = EditorGUIUtility.labelWidth;
							EditorGUIUtility.labelWidth = 60;

							TileAttribute attributeToRemove = null;
							foreach(TileAttribute attribute in ti.attributes) {
								GUILayout.BeginHorizontal();

								attribute.name = EditorGUILayout.TextField("Name", attribute.name);
								attribute.value = EditorGUILayout.TextField("Value", attribute.value);

								if(GUILayout.Button("Remove", "label", GUILayout.Width(60))) {
									attributeToRemove = attribute;
								}

								GUILayout.EndHorizontal();
							}
							if(attributeToRemove != null) {
								ti.attributes.Remove(attributeToRemove);
							}
							EditorGUIUtility.labelWidth = ewidth;
						}

						GUILayout.EndVertical();
					}

				}

				{
					Rect lr = GUILayoutUtility.GetLastRect();
					tileViewScrollPosition = GUI.BeginScrollView(new Rect(0, lr.y + lr.height, this.position.width, this.position.height - (lr.y + lr.height)),
					                                             tileViewScrollPosition,
					                                             new Rect(0, 0, this.position.width - 30f, _tileViewHeightRequired));

					int xc = (int)Mathf.Floor((this.position.width - 10f) / (ts.tileWidth + 6f));
					float startx = _tileViewHeightRequired < (this.position.height - (lr.y + lr.height)) ? (this.position.width - 10f - xc * (ts.tileWidth + 6f)) / 2 : 2f;
					float xf = startx;
					float yf = 10f;
					int cx = 0;

					for(int j=0; j<ts.rows; j++) {
						for(int i=0; i<ts.columns; i++) {
							Rect current_rect = new Rect(xf, yf, ts.tileWidth, ts.tileHeight);
							GUI.DrawTextureWithTexCoords(current_rect,
							                             ts.texture,
							                             ts.getTexRect(i, j));
							
							if(Event.current.type == EventType.MouseDown &&
							   Event.current.button == 0) {
								if(current_rect.Contains(Event.current.mousePosition)) {
									selectedTileX = i;
									selectedTileY = j;
									ts.getTileInfo(i, j).direction = 0;
									this.Repaint();
								}
							}
							if(selectedTileX == i && selectedTileY == j) {
								TileGUIUtility.DrawSceneBezierRect(current_rect, 6f, Color.green);
							}
							
							xf += ts.tileWidth + 10f;
							cx ++;
							if(cx >= xc) {
								cx = 0;
								xf = startx;
								yf += ts.tileHeight + 10f;
							}
						}	
					}

					_tileViewHeightRequired = yf;

					GUI.EndScrollView();
				}
			} else {
				EditorGUILayout.HelpBox("No tileset selected", MessageType.Info);
			}

		}
		GUILayout.EndArea();
	}
	
};