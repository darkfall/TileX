using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Reflection;

public class TileGUIUtility {
	
	public static GUIStyle boxStyle;
	public static GUIStyle listEntryNormal;
	public static GUIStyle listEntryFocused;
	public static GUIStyle boxNoBackground;
	public static GUIStyle horizontalLine;
	
	public static bool Initialized = false;
	
	public static void Init() {
		if(!Initialized) {
			boxStyle = new GUIStyle(GUI.skin.box);
			
			listEntryNormal = new GUIStyle(GUI.skin.label);
			listEntryNormal.alignment = TextAnchor.MiddleCenter;
			
			listEntryFocused = new GUIStyle(GUI.skin.label);
			listEntryFocused.alignment = TextAnchor.MiddleCenter;
			SetStyleTextColor(listEntryFocused, new Color(0, 1, 0, 1));
			
			boxNoBackground = new GUIStyle(GUI.skin.box);
			boxNoBackground.normal.background = boxNoBackground.focused.background = boxNoBackground.active.background = null;
			
			horizontalLine = new GUIStyle("box");
			horizontalLine.border.top = horizontalLine.border.bottom = 1;
			horizontalLine.margin.top = horizontalLine.margin.bottom = 1;
			horizontalLine.padding.top = horizontalLine.padding.bottom = 1;
			
		}
		Initialized = true;
	}
	
	public static GUIStyle SetStyleTextColor(GUIStyle style, Color c) {
		style.normal.textColor =
			style.active.textColor =
				style.hover.textColor =
				style.focused.textColor =
				style.onNormal.textColor =
				style.onActive.textColor =
				style.onHover.textColor =
				style.onFocused.textColor = c;
		return style;
	}
	
	public static int MakeSimpleList(int selectedIndex, System.Collections.IList entries, TextAnchor textAlignment = TextAnchor.MiddleCenter) {
		return MakeSimpleList(selectedIndex, entries, () => {}, textAlignment);
	}
	
	public static int MakeSimpleList(int selectedIndex, System.Collections.IList entries, Action onSelectionChange, TextAnchor textAlignment = TextAnchor.MiddleCenter) {
		if(!Initialized) {
			TileGUIUtility.Init();
		}
		if(entries.Count == 0)
			return selectedIndex;
		
		listEntryNormal.alignment = listEntryFocused.alignment = textAlignment;
		
		GUILayout.BeginVertical(boxStyle);
		int newSelectedIndex = -1;
		{
			for(int index=0; index<entries.Count; ++index) {
				if(GUILayout.Button(entries[index].ToString(),
				                    selectedIndex == index ? TileGUIUtility.listEntryFocused : TileGUIUtility.listEntryNormal)) {
					newSelectedIndex = index;
				}
			} 
		}
		
		GUILayout.EndVertical();
		
		int result = newSelectedIndex == -1 ? selectedIndex : newSelectedIndex;
		if(result != selectedIndex)
			onSelectionChange();
		return result;
	}

	public static int MakeSimpleList(int selectedIndex, System.Collections.IList entries, TextAnchor textAlignment, Action onSelectionChange, Func<int, bool, bool> draw) {
		if(!Initialized) {
			TileGUIUtility.Init();
		}
		if(entries.Count == 0)
			return selectedIndex;
		
		listEntryNormal.alignment = listEntryFocused.alignment = textAlignment;
		
		GUILayout.BeginVertical(boxStyle);
		int newSelectedIndex = -1;
		{
			for(int index=0; index<entries.Count; ++index) {
				if(draw(index, selectedIndex == index)) {
					newSelectedIndex = index;
				}
			} 
		}
		
		GUILayout.EndVertical();
		
		int result = newSelectedIndex == -1 ? selectedIndex : newSelectedIndex;
		if(result != selectedIndex)
			onSelectionChange();
		return result;
	}

	public static void DrawSceneRect(Rect r, float z) {
		Handles.DrawLine(new Vector3(r.x, r.y, z) ,
		                 new Vector3(r.x, r.y + r.height, z));
		Handles.DrawLine(new Vector3(r.x, r.y + r.height, z) ,
		                 new Vector3(r.x + r.width, r.y + r.height, z));
		Handles.DrawLine(new Vector3(r.x + r.width, r.y + r.height, z) ,
		                 new Vector3(r.x + r.width, r.y, z));
		Handles.DrawLine(new Vector3(r.x + r.width, r.y, z) ,
		                 new Vector3(r.x, r.y, z));
	}

	public static void DrawSceneBezierRect(Rect r, float width, Color c) {
		Handles.DrawBezier(new Vector3(r.x, r.y) ,
		                   new Vector3(r.x, r.y + r.height),
		                   new Vector3(r.x, r.y) ,
		                   new Vector3(r.x, r.y + r.height),
		                   c,
		                   null,
		                   width);
		Handles.DrawBezier(new Vector3(r.x, r.y + r.height) ,
		                   new Vector3(r.x + r.width, r.y + r.height),
		                   new Vector3(r.x, r.y + r.height) ,
		                   new Vector3(r.x + r.width, r.y + r.height),
		                   c,
		                   null,
		                   width);
		Handles.DrawBezier(new Vector3(r.x + r.width, r.y + r.height) ,
		                   new Vector3(r.x + r.width, r.y),
		                   new Vector3(r.x + r.width, r.y + r.height) ,
		                   new Vector3(r.x + r.width, r.y),
		                   c,
		                   null,
		                   width);
		Handles.DrawBezier(new Vector3(r.x + r.width, r.y) ,
		                   new Vector3(r.x, r.y),
		                   new Vector3(r.x + r.width, r.y) ,
		                   new Vector3(r.x, r.y),
		                   c,
		                   null,
		                   width);
	}

	public static void DrawTextureWithTexCoordsAndColor(Rect dst, Texture tex, Rect coords, Color c) {
		Color gc = GUI.color;
		GUI.color = c;
		GUI.DrawTextureWithTexCoords(dst, 
		                             tex, 
		                             coords,
		                             true);
		GUI.color = gc;
	}
	
	public static void DrawTextureAt(Texture tex, Rect dst, Color c) {
		Color gc = GUI.color;
		GUI.color = c;
		GUI.DrawTexture(dst, tex, ScaleMode.ScaleToFit, true);
		GUI.color = gc;
	}

	public static string[] GetSortingLayerNames() {
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}

	public static string[] GetStringArray(System.Collections.IList entries) {
		System.Collections.Generic.List<string> strs = new System.Collections.Generic.List<string>();
		foreach(object obj in entries)
			strs.Add(obj.ToString());
		return strs.ToArray();
	}
	
};
