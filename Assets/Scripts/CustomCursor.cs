using UnityEngine;

public class CustomCursor : MonoBehaviour
{
	public bool active = false;

	private void Awake()
	{
		Cursor.visible = false;
	}

	private void LateUpdate()
	{
		//if (!active)
		//	return;

		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
	}
}