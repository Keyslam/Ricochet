using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float friction = 0.3f;
	[SerializeField]
	private float maxSpeed = 0.3f;
	[SerializeField]
	private float speedMultiplier = 0.2f;

	public Vector2 walkRange = new Vector2(-3.12f, 3.12f);

	private float velocity = 0.0f;

	private SpriteRenderer spriteRenderer = null;

	public bool controllable = false;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void FixedUpdate()
	{
		if (!controllable)
			return;

		float horizontal = Input.GetAxisRaw("Horizontal");

		velocity = Mathf.Clamp(velocity + horizontal * speedMultiplier, -maxSpeed, maxSpeed);
		if (velocity > 0.0f)
		{
			velocity -= friction * Time.fixedDeltaTime;
			if (velocity < 0.0f)
				velocity = 0.0f;

			spriteRenderer.flipX = false;
		}
		else if (velocity < 0.0f)
		{
			velocity += friction * Time.fixedDeltaTime;
			if (velocity > 0.0f)
				velocity = 0.0f;

			spriteRenderer.flipX = true;
		}
		
		transform.position += new Vector3(velocity, 0, 0) * Time.fixedDeltaTime;
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, walkRange.x, walkRange.y), transform.position.y, transform.position.z);
	}
}
