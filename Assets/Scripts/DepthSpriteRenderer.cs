using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DepthSpriteRenderer : MonoBehaviour
{
	[SerializeField]
	private int offset = 0;

	private SpriteRenderer spriteRenderer = null;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void LateUpdate()
	{
		if (transform.hasChanged)
			spriteRenderer.sortingOrder = -(int)Camera.main.WorldToScreenPoint(spriteRenderer.bounds.min).y + offset;
	}
}