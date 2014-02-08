using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(TileMap))]
class TileMapInspector: Editor {

	public int selectedLayerIndex;

	public override void OnInspectorGUI() {

		TileMap tm = this.target as TileMap;
		EditorGUILayout.BeginVertical();

		EditorGUI.BeginChangeCheck();

		tm.mapWidth = EditorGUILayout.IntField("Map Width", tm.mapWidth);
		tm.mapHeight = EditorGUILayout.IntField("Map Height", tm.mapHeight);
		tm.tileWidth = EditorGUILayout.IntField("Tile Width", tm.tileWidth);
		tm.tileHeight = EditorGUILayout.IntField("Tile Height", tm.tileHeight);
		tm.pixelPerUnit = EditorGUILayout.IntField("Pixel-Per-Unit", tm.pixelPerUnit);

		if(EditorGUI.EndChangeCheck()) {
			SceneView.RepaintAll();
		}

		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Add Tile Layer")) {
			Undo.RegisterCreatedObjectUndo(tm.AddLayer(TileLayerType.TileLayer, "Layer " + tm.layers.Count.ToString()).gameObject, "Layer");
			this.Repaint();
		}
		//GUILayout.Button("Add Object Layer", "button off");
		/*
		if() {
			Undo.RegisterCreatedObjectUndo(tm.AddLayer(TileLayerType.ObjectLayer, "Layer " + tm.layers.Count.ToString()).gameObject, "Tile");
			this.Repaint();
		}
		*/
		GUILayout.EndHorizontal();

		selectedLayerIndex = TileGUIUtility.MakeSimpleList(
			selectedLayerIndex, 
			TileGUIUtility.GetStringArray(tm.layers), 
			TextAnchor.MiddleLeft,
			() => {
				
			},
			(index, is_selection) => {
				return DrawLayerInspector(index, is_selection);
			}

		);
		EditorGUILayout.EndVertical();
	}

	bool DrawLayerInspector(int index, bool is_selection) {
		TileMap tm = this.target as TileMap;
		TileLayer layer = tm.layers[index];
		
		GUILayout.BeginHorizontal();
		layer.editorExpanded = EditorGUILayout.Toggle(layer.editorExpanded, "foldout", GUILayout.Width(16));

		layer.visible = GUILayout.Toggle(layer.visible, "", GUILayout.Width(12));
		bool selection = GUILayout.Button(layer.name + " (" + layer.layerType.ToString() + ")",
		                                  is_selection ? TileGUIUtility.listEntryFocused : TileGUIUtility.listEntryNormal,
		                                  GUILayout.Width(Screen.width - 90));

		if(GUILayout.Button(new GUIContent(Resources.LoadAssetAtPath<Texture2D>("Assets/Scripts/TileX/Editor/Resources/Icons/settings.png")),
		              		"label",
		                    GUILayout.Height(16))) {
			ShowLayerContextMenu(layer);
		}

		GUILayout.EndHorizontal();

		if(layer.editorExpanded) {
			layer.transparency = EditorGUILayout.Slider("Transparency", layer.transparency, 0f, 1f);

			EditorGUI.BeginChangeCheck();
			layer.sortingLayer = EditorGUILayout.Popup("Sorting Layer", layer.sortingLayer, TileGUIUtility.GetSortingLayerNames());
			layer.sortingOrder = EditorGUILayout.IntField("Sorting Order", layer.sortingOrder);
			if(EditorGUI.EndChangeCheck()) {
				layer.ApplySorting();
			}

			EditorGUILayout.Space();
			layer.layerGroup = EditorGUILayout.IntField("Group Id", layer.layerGroup);
			layer.layerTag = EditorGUILayout.TextField("Layer Tag", layer.layerTag);

		}

		Rect r = GUILayoutUtility.GetLastRect();
		EditorGUILayout.Space();
		GUI.Box (new Rect((Screen.width - r.width) / 2 + 1, r.y + r.height + 8f, r.width + 5f, 1), "");
		
		return selection;
	}

	public void OnSceneGUI() {
		TileMap tm = this.target as TileMap;

		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

		Vector3 pos = tm.startPoint;
		int rows = tm.mapHeight / tm.tileHeight;
		int columns = tm.mapWidth / tm.tileWidth;

		for(int i=0; i<=columns; ++i) {
			Handles.DrawLine(new Vector3((float)i * tm.xStep, 0) + pos,
			                 new Vector3((float)i * tm.xStep, (float)tm.worldHeight) + pos);
		}
		for(int i=0; i<=rows; ++i) {
			Handles.DrawLine(new Vector3(0, (float)i * tm.yStep) + pos,
			                 new Vector3((float)tm.worldWidth, (float)i * tm.yStep) + pos);
		}


		Event evt = Event.current;
		Camera sceneCam = SceneView.currentDrawingSceneView.camera;
		Vector2 mousePos = Event.current.mousePosition;
		mousePos.y = sceneCam.pixelHeight - mousePos.y;
		Vector3 p = sceneCam.ScreenPointToRay(mousePos).origin;
		Vector3 off = (p - pos) * tm.pixelPerUnit ;
		int x = (int)Mathf.Floor(off.x / tm.tileWidth);
		int y = (int)Mathf.Floor(off.y / tm.tileHeight);

		TileMapEditor editor = TileMapEditor.Get ();
		SpriteRenderer previewRenderer = tm.previewTile;
		if(x >= 0 && x < columns &&
		   y >= 0 && y < rows) {
			if(editor.selectedTileSetIndex != -1) {
				Tileset ts = tm.tilesets[editor.selectedTileSetIndex];
				TileInfo ti = ts.GetTileInfo(editor.selectedTileX, editor.selectedTileY);
				previewRenderer.sprite = ti.sprite;
				previewRenderer.color = new Color(255, 255, 255, 0.5f);
				previewRenderer.gameObject.transform.rotation = Quaternion.AngleAxis(ti.direction, Vector3.forward);
				previewRenderer.gameObject.transform.position = new Vector3(pos.x + x * tm.xStep + tm.xStep / 2, 
				                                                            pos.y + y * tm.yStep + tm.yStep / 2, 
				                                                            tm.gameObject.transform.position.z);

				if(editor.isErase)
					TileGUIUtility.DrawSceneBezierRect(new Rect(pos.x + x * tm.xStep, pos.y + y * tm.yStep, tm.xStep, tm.yStep), 6, Color.red);
				else
					TileGUIUtility.DrawSceneBezierRect(new Rect(pos.x + x * tm.xStep, pos.y + y * tm.yStep, tm.xStep, tm.yStep), 6, Color.green);

				if(evt.type == EventType.MouseDown) {
					if(evt.button == 0) {
						TileLayer layer;
						if(tm.layers.Count == 0)
							layer = tm.AddLayer(TileLayerType.TileLayer, "Layer 0");
						else
							layer = tm.layers[selectedLayerIndex];
						if(layer != null) {
							if(editor.isErase) {
								layer.RemoveTile(x, y);
							} else {
								Tile t = layer.AddTile(x, y, ti);
								Undo.RegisterCreatedObjectUndo(t.gameObject, "Tile");
							}
						}
					} else {
						ti.direction = (ti.direction + 90) % 360;
					}

				}
			} 

			if(evt.type == EventType.mouseDrag)
				evt.Use();
		}else {
			previewRenderer.sprite = null;
		}
	}

	#region layer context menu

	
	void OnLayerMenuResetClicked(object userData) {
		TileLayer layer = userData as TileLayer;
		layer.ResetLayer();
	}
	
	void OnLayerMenuCopyAsNewClicked(object userData) {
		TileLayer layer = userData as TileLayer;
		
		TileMap tm = this.target as TileMap;
		TileLayer newlayer = tm.AddLayer(TileLayerType.TileLayer, layer.name + " Copied");
		for(int y=0; y<tm.height; y++) {
			for(int x=0; x<tm.width; x++) {
				Tile t = layer.GetTile(x, y);
				TileInfo ti = new TileInfo();
				ti.sprite = t.gameObject.GetComponent<SpriteRenderer>().sprite;
				ti.attributes = t.attributes;
				
				float angle;
				Vector3 axis;
				t.gameObject.transform.rotation.ToAngleAxis(out angle, out axis);
				ti.direction = angle;
				ti.isBlock = t.isBlock;
				ti.editorExpanded = false;
				
				newlayer.AddTile(x, y, ti);	
			}
		}

	}
	
	void OnLayerMenuRemoveClicked(object userData) {
		TileLayer layer = userData as TileLayer;
		TileMap tm = this.target as TileMap;
		tm.RemoveLayer(layer);
	}

	void OnLayerMenuFillWithCurrentTileClicked(object userData) {
		TileLayer layer = userData as TileLayer;
		TileMap tm = this.target as TileMap;
		
		TileMapEditor editor = TileMapEditor.Get ();
		if(editor.selectedTileSetIndex != -1 &&
		   editor.selectedTileX != -1 && editor.selectedTileY != -1) {
			Tileset ts = tm.tilesets[editor.selectedTileSetIndex];
			TileInfo ti = ts.GetTileInfo(editor.selectedTileX, editor.selectedTileY);

			if(ti != null) {
				int rows = tm.mapHeight / tm.tileHeight;
				int columns = tm.mapWidth / tm.tileWidth;
				for(int i=0; i<rows; ++i) {
					for(int j=0; j<columns; ++j) {
						layer.AddTile(j, i, ti);
					}
				}
			}
		}
	}

	void ShowLayerContextMenu(TileLayer layer) {
		GenericMenu menu = new GenericMenu();
		
		menu.AddItem(new GUIContent("Reset"), false, OnLayerMenuResetClicked, layer);

		TileMapEditor editor = TileMapEditor.Get ();
		if(editor.selectedTileSetIndex != -1 &&
		   editor.selectedTileX != -1 && editor.selectedTileY != -1) 
			menu.AddItem(new GUIContent("Fill with Current Tile"), false, OnLayerMenuFillWithCurrentTileClicked, layer);
		else
			menu.AddDisabledItem(new GUIContent("Fill with Current Tile"));
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Copy as New"), false, OnLayerMenuCopyAsNewClicked, layer);
		menu.AddSeparator("");
		menu.AddItem(new GUIContent("Remove"), false, OnLayerMenuRemoveClicked, layer);
		menu.ShowAsContext();
	}

	#endregion

}
