using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class TileAttribute {
	public string name;
	public string value;

	public TileAttribute() {
		name = value = "";
	}

	public TileAttribute(string n, string v) {
		name = n;
		value = v;
	}
}

[Serializable]
public class TileInfo {
	public Sprite sprite;
	public float  direction = 0;
	public bool   isBlock = false;
	public bool   isHole = false;
	public List<TileAttribute> attributes = new List<TileAttribute>();

#region editor
	public bool editorExpanded = false;
#endregion;
}

[Serializable]
public class Tileset {

	public Texture2D texture;

	public int tileWidth = 32;
	public int tileHeight = 32;
	public int spacing = 0;
	public int margin = 0;
	public int rows = 0;
	public int columns = 0;

	[SerializeField]
	public List<TileInfo> tileInfos = new List<TileInfo>();
	
	public void CreateTileSprites(TileMap tm) {
		this.tileInfos = new List<TileInfo>();

		int x = margin;
		int y = margin;
		int xc = 0;
		int yc = 0;
		while(y <= texture.height) {
			x += tileWidth + spacing;
			xc += 1;
			if(x >= texture.width) {
				x = margin;
				y += tileHeight + spacing;

				if(y <= texture.height) {
					columns = xc;
					xc = 0;
					yc += 1;
				}
			}
		}
		rows = yc;

		x = margin;
		y = margin;
		for(int j=0; j<rows; ++j) {
			for(int i=0; i<columns; ++i) {
				TileInfo ti = new TileInfo();
				ti.sprite = Sprite.Create(texture, new Rect(x, y, tileWidth, tileHeight), new Vector2(0.5f, 0.5f), tm.pixelPerUnit);
				ti.attributes = new List<TileAttribute>();
				this.tileInfos.Add (ti);

				x += tileWidth + spacing;
				if(x >= texture.width) {
					x = margin;
					y += tileHeight + spacing;
				}
			}
		}
	}

	public bool isValidindex(int x, int y) {
		return x >= 0 && x < this.columns &&
			y >= 0 && y < this.rows;

	}

	public TileInfo getTileInfo(int x, int y) {
		int idx = y * this.columns + x;
		if(idx < this.tileInfos.Count)
			return this.tileInfos[idx];
		return null;
	}

	public Rect getTexRect(int x, int y) {
		return new Rect((float)x * this.tileWidth / this.texture.width,
		                (float)y * this.tileHeight / this.texture.height,
		                (float)(this.tileWidth) / this.texture.width,
		                (float)(this.tileHeight) / this.texture.height);
	}
	
	public override string ToString ()
	{
		if(this.texture != null)
			return this.texture.name;
		return "Empty Tileset " + this.GetHashCode();
	}
}
