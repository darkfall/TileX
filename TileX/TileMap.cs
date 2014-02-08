using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TileMap: MonoBehaviour {
	
	[SerializeField]
	public List<TileLayer> layers = new List<TileLayer>();
	[SerializeField]
	public List<Tileset> tilesets = new List<Tileset>();

	public int mapWidth = 256;
	public int mapHeight = 256;
	public int tileWidth = 32;
	public int tileHeight = 32;
	public int pixelPerUnit = 100;

	public TileLayer AddLayer(TileLayerType t, string name) {
		GameObject obj = new GameObject();
		obj.transform.parent = this.gameObject.transform;
		obj.name = name;

		TileLayer l = obj.AddComponent<TileLayer>();
		l.parentMap = this;
		l.layerType = t;
		l.name = name;
		l.sortingOrder = this.layers.Count;
		l.Init(this.width, this.height);

		this.layers.Add(l);
		return l;
	}

	public void RemoveLayer(TileLayer t) {
		if(t != null) {
			this.layers.Remove(t);
			DestroyImmediate(t.gameObject);
		}
	}

	public Vector3 startPoint {
		get {
			return this.gameObject.transform.position - new Vector3((float)this.mapWidth / this.pixelPerUnit / 2,
			                                                        (float)this.mapHeight / this.pixelPerUnit / 2);
		}
	}

	public Vector3 endPoint {
		get {
			return this.startPoint + new Vector3(this.worldWidth, this.worldHeight);
		}
	}

	public float xStep {
		get {
			return (float)this.tileWidth / this.pixelPerUnit; 
		}
	}

	public float yStep {
		get {
			return (float)this.tileHeight / this.pixelPerUnit;
		}
	}

	public float worldWidth {
		get {
			return (float)this.mapWidth / this.pixelPerUnit;
		}
	}

	public float worldHeight {
		get {
			return (float)this.mapHeight / this.pixelPerUnit;
		}
	}

	public Rect worldMapRect {
		get {
			Vector3 sp = this.startPoint;
			return new Rect(sp.x, sp.y, this.worldWidth, this.worldHeight);
		}
	}

	public int width {
		get {
			return this.mapWidth / this.tileWidth;
		}
	}

	public int height {
		get {
			return this.mapHeight / this.tileHeight;
		}
	}

	public Tile GetTileAt(Vector3 pos, int layerId) {
		List<TileLayer> layers = this.layers;
		if(layerId >= 0 && layerId < layers.Count) {
			return layers[layerId].GetTileAt(pos - this.startPoint);
		}
		return null;
	}

	public Tile GetTile(int x, int y, int layerId) {
		List<TileLayer> layers = this.layers;
		if(layerId >= 0 && layerId < layers.Count) {
			return layers[layerId].GetTile(x, y);
		}
		return null;
	}

	public void GetTileCoordinateAt(Vector3 pos, ref int x, ref int y) {
		Vector3 dis = pos - this.startPoint;
		x = (int)Mathf.Floor((dis.x) / this.xStep);
		y = (int)Mathf.Floor((dis.y) / this.yStep);
	}
	
	public SpriteRenderer previewTile {
		get {
			TilePreview tp = this.gameObject.GetComponentInChildren<TilePreview>();
			if(tp == null) {
				GameObject previewObj = new GameObject();
				previewObj.transform.parent = this.gameObject.transform;
				previewObj.name = "PreviewTile";
				tp = previewObj.AddComponent<TilePreview>();
				SpriteRenderer sr = previewObj.AddComponent<SpriteRenderer>();
				sr.sortingOrder = 20;
			}
			return tp.gameObject.GetComponent<SpriteRenderer>();
		}
	}

	// shows layers with tag = tag in a group, hides all others
	public void ShowLayerInGroupWithTag(string tag, int groupId, bool mainCamera = true) {
		List<TileLayer> layers = this.layers;
		foreach(TileLayer layer in layers) {
			if(layer.layerGroup == groupId) {
			    if(layer.layerTag == tag) {
					layer.visible = true;
				} else {
					layer.visible = false;
				}
			}
		}
	}

	public string GetAttributeAt(int x, int y, string key) {
		foreach(TileLayer layer in this.layers) {
			Tile t = layer.GetTile(x, y);
			string v = t.GetAttribute(key);
			if(v.Length > 0)
				return v;
		}
		return "";
	}

	public bool IsBlockAt(int x, int y) {
		foreach(TileLayer layer in this.layers) {
			if(layer.visible && layer.enabled) {
				Tile t = layer.GetTile(x, y);
				if(t.isBlock)
					return true;
			}
		}
		return false;
	}

}