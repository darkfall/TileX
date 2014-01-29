using UnityEngine;

class TileCamera: MonoBehaviour {

	public TileMap tileMap;
	public Camera  camera;
	public PlayerController player;
	public Vector2 playerSize = new Vector2(0.32f, 0.32f);

	public void LateUpdate() {
		if(player != null && camera != null && tileMap != null) {
			Vector3 pos = player.gameObject.transform.position;
			Rect tileRect = tileMap.worldMapRect;

			float orthoHeight = camera.orthographicSize;
			float orthoWidth = orthoHeight * camera.aspect;

			
			tileRect.x += orthoWidth;
			tileRect.y += orthoHeight;
			tileRect.width -= orthoWidth * 2;
			tileRect.height -= orthoHeight * 2;

			if(pos.x < tileRect.x) {
				pos.x = tileRect.x;
			}
			else if(pos.x > tileRect.x + tileRect.width) {
				pos.x = tileRect.x + tileRect.width;
			}

			if(pos.y < tileRect.y) {
				pos.y = tileRect.y;
			}
			else if(pos.y > tileRect.y + tileRect.height) {
				pos.y = tileRect.y + tileRect.height;
			}

			camera.transform.position = new Vector3(pos.x, pos.y, camera.transform.position.z);
		}

	}

}
