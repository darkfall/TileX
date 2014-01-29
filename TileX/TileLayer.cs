using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum LayerType {
	TileLayer,
	ObjectLayer,
	ImageLayer,
}

public class TileLayer: MonoBehaviour {

	public LayerType layerType;
	public TileMap   parentMap; 
	public string    name = "New Layer";

	public int sortingOrder = 0;
	public int sortingLayer = 0;

	public int layerGroup = 0;
	public string layerTag = "";

	[SerializeField]
	public List<GameObject> tiles;
	
	bool _visible = true;
	public bool visible {
		get {
			return _visible;
		}
		set {
			_visible = value;
			this._setVisible(_visible);
		}

	}

	public TileLayer() {
	
	}

	public void init(int w, int h) {
		tiles = new List<GameObject>();
		for(int i=0; i<w*h; ++i) {
			tiles.Add(null);
		}
	}

	public void UpdateSorting() {
		foreach(GameObject obj in this.tiles) {
			if(obj != null) {
				SpriteRenderer r = obj.GetComponent<SpriteRenderer>();
				r.sortingLayerID = sortingLayer;
				r.sortingOrder = sortingOrder;
			}
		}
	}

	void _setVisible(bool visible) {
		foreach(GameObject t in this.tiles) {
			if(t != null)
				t.GetComponent<Tile>().visible = visible;
		}
	}

	public Tile getTile(int x, int y) {
		GameObject t = tiles[parentMap.width * y + x];
		if(t != null)
			return t.GetComponent<Tile>();
		return null;
	}

	public Tile addTile(int x, int y, TileInfo ti) {
		this.removeTile(x, y);

		GameObject obj = new GameObject();
		obj.transform.parent = this.gameObject.transform;
		obj.transform.position = new Vector3(x * parentMap.xStep + parentMap.xStep / 2, 
		                                     y * parentMap.yStep + parentMap.yStep / 2) + parentMap.startPoint;
		obj.name = "Tile_"+x.ToString()+"_"+y.ToString();
		obj.transform.rotation = Quaternion.AngleAxis(ti.direction, Vector3.forward);

		Tile t = obj.AddComponent<Tile>();
		t.Init(x, y, ti, sortingOrder, sortingLayer);
		t.parentLayer = this;

		this.tiles[y * parentMap.width + x] = obj;
		return t;
	}

	public void removeTile(int x, int y) {
		GameObject t = this.tiles[y * parentMap.width + x];
		if(t != null) { 
			DestroyImmediate(t);
			this.tiles[y * parentMap.width + x] = null;
		}
	}

	float _transparency = 1.0f;
	public float transparency {
		get {
			return _transparency;
		}
		set {
			_transparency = value;
			foreach(GameObject t in this.tiles) {
				if(t != null) 
					t.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, value);
			}
		}
	}

	public void ResetLayer() {
		Tile[] ptiles = this.gameObject.GetComponentsInChildren<Tile>();
		foreach(Tile t in ptiles) {
			DestroyImmediate(t.gameObject);
		}
		tiles = new List<GameObject>();
		for(int i=0; i<parentMap.width*parentMap.height; ++i) {
			tiles.Add(null);
		}
	}

	public Tile getTileAt(Vector3 pos) {
		return this.getTile((int)Mathf.Floor((pos.x) / parentMap.xStep),
		                    (int)Mathf.Floor((pos.y ) / parentMap.yStep));
	}

	public override string ToString () {
		return this.name;
	}


}