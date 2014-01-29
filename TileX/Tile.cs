using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile: MonoBehaviour {

	public TileLayer parentLayer;

	/// <summary>
	/// coordinates of the tile, cannot be changed
	/// </summary>
	[SerializeField]
	int _x, _y, _width, _height;

	public int x {
		get { return _x; }
	}
	public int y {
		get { return _y; }
	}
	public int width {
		get { return _width; }
	}
	public int height {
		get { return _height; }
	}

	/// <summary>
	/// The user data. 
	/// </summary>
	[SerializeField]
	public UnityEngine.Object userData; 

	[SerializeField]
	public List<TileAttribute> attributes = new List<TileAttribute>();

	public bool isBlock = false;
	public bool isFloatable = false;
	public bool isBreakable = false;
	public bool isClimbable = false;
	public bool isMoveable = false;
	public bool isTunnelable = false;

	public int astarScore = 0;
	public Tile astarFrom = null;

	bool _visible;
	public bool visible {
		get {
			return _visible;
		}
		set {
			_visible = value;
			this.gameObject.GetComponent<SpriteRenderer>().enabled = _visible;
		}
	}

	public void Init(int x, int y, TileInfo info, int sortingOrder, int sortingLayer) {
		this._x = x;
		this._y = y;
		this._width = (int)info.sprite.textureRect.width;
		this._height = (int)info.sprite.textureRect.height;

		this.isBlock = info.isBlock;

		SpriteRenderer r = this.gameObject.GetComponent<SpriteRenderer>();
		r.sprite = info.sprite;
		r.sortingOrder = sortingOrder;
		r.sortingLayerID = sortingLayer;

		this.attributes = new List<TileAttribute>();
		foreach(TileAttribute attr in info.attributes) {
			this.attributes.Add(new TileAttribute(attr.name, attr.value));
		}
	}

	public void setTransparency(float a) {
		SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
		sr.color = new Color(255, 255, 255, a);
	}

	public string getAttribute(string key) {
		foreach(TileAttribute attr in this.attributes) {
			if(attr.name == key) {
				return attr.value;
			}
		}
		return "";
	}

}