using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
	public bool isTarget = false;

	public Transform[] positions = null;
	private int target = 0;

	[SerializeField]
	private float friction = 0.3f;
	[SerializeField]
	private float maxSpeed = 0.2f;
	[SerializeField]
	private float speedMultiplier = 0.2f;

	[SerializeField]
	private Sprite targetDeadTexture = null;
	[SerializeField]
	private Sprite bystanderDeadTexture = null;

	private Vector2 velocity = Vector2.zero;

	private SpriteRenderer spriteRenderer = null;

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private bool isAlive = true;

	public void Kill()
	{
		if (isTarget)
			spriteRenderer.sprite = targetDeadTexture;
		else
			spriteRenderer.sprite = bystanderDeadTexture;

		isAlive = false;
	}

	private void FixedUpdate()
	{
		if (positions.Length < 1)
			return;

		if (!isAlive)
			return; 

		Transform targetTransform = positions[target];
		Vector2 delta = (targetTransform.position - transform.position).normalized;

		velocity += delta * speedMultiplier;
		velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

		if (velocity.x > 0.0f)
		{
			velocity.x -= friction * Time.fixedDeltaTime;
			if (velocity.x < 0.0f)
				velocity.x = 0.0f;

			spriteRenderer.flipX = false;
		}
		else if (velocity.x < 0.0f)
		{
			velocity.x += friction * Time.fixedDeltaTime;
			if (velocity.x > 0.0f)
				velocity.x = 0.0f;

			spriteRenderer.flipX = true;
		}

		if (velocity.y > 0.0f)
		{
			velocity.y -= friction * Time.fixedDeltaTime;
			if (velocity.y < 0.0f)
				velocity.y = 0.0f;
		}
		else if (velocity.y < 0.0f)
		{
			velocity.y += friction * Time.fixedDeltaTime;
			if (velocity.y > 0.0f)
				velocity.y = 0.0f;
		}

		Vector2 deltaVelocity = velocity * Time.fixedDeltaTime;
		transform.position += new Vector3(deltaVelocity.x, deltaVelocity.y);

		if (Vector2.Distance(transform.position, targetTransform.position) <= 0.1f)
		{
			target++;
			if (target == positions.Length)
				target = 0;
		}
	}
}
