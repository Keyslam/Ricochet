using System.Collections.Generic;
using UnityEngine;

// Represents a set of bounced rays.
public class BounceRay
{
	// BounceRay result state.

	public List<Vector2> endPoints;
	public List<RaycastHit2D> contacts;
	public RaycastHit2D? hit;
	public bool bounced;
	public Vector2 finalDirection;

	// Returns all contact points from a bouncing ray at the specified position and moving in the specified direction.
	public static BounceRay Cast(Vector2 position, Vector2 direction, float magnitude, int bounceMask, int layerMask)
	{
		// Initialize the return data.
		BounceRay bounceRay = new BounceRay
		{
			contacts = new List<RaycastHit2D>(),
			endPoints = new List<Vector2>(),
			finalDirection = direction.normalized
		};

		// If there is magnitude left...
		if (magnitude > 0)
		{
			// Fire out initial vector.
			RaycastHit2D bounceHit = Physics2D.Raycast(position, direction, magnitude, bounceMask);
			RaycastHit2D hit = Physics2D.Raycast(position, direction, magnitude, layerMask);

			Collider2D hitCollider = hit.collider;

			if (bounceHit.distance > 0 && hit.distance > 0 && bounceHit.distance < hit.distance)
				hitCollider = null;

			if (hitCollider != null)
				bounceRay.hit = hit;

			// Calculate our bounce conditions.
			bool hitSucceeded = bounceHit.collider != null && bounceHit.distance > 0;
			bool magnitudeRemaining = bounceHit.distance < magnitude;

			Vector2 finalPosition = Vector2.zero;
			// Get the final position.
			if (hitCollider != null)
			{
				finalPosition = hit.point;
			}
			else
			{
				finalPosition = hitSucceeded ? bounceHit.point : position + direction.normalized * magnitude;
			}

			// Draw final position.
			Debug.DrawLine(position, finalPosition, Color.green);

			// If the bounce conditions are met, add another bounce.
			if (hitSucceeded && magnitudeRemaining && bounceRay.hit == null)
			{
				// Add the contact and hit point of the raycast to the BounceRay.
				bounceRay.contacts.Add(bounceHit);
				bounceRay.endPoints.Add(bounceHit.point);

				// Reflect the hit.
				Vector2 reflection = Vector2.Reflect((bounceHit.point - position).normalized, bounceHit.normal);

				// Create the reflection vector
				Vector2 reflectionVector = reflection;

				// Bounce the ray.
				BounceRay bounce = Cast(
					bounceHit.point + bounceHit.normal / 1000.0f,
					reflectionVector,
					magnitude - bounceHit.distance,
					bounceMask,
					layerMask);

				// Include the bounce contacts and origins.
				bounceRay.contacts.AddRange(bounce.contacts);
				bounceRay.endPoints.AddRange(bounce.endPoints);

				// Set the final direction to what our BounceRay call returned.
				bounceRay.finalDirection = bounce.finalDirection;

				// We've bounced if we are adding more contact points and origins.
				bounceRay.bounced = true;

				if (hitCollider == null)
				{
					bounceRay.hit = bounce.hit;
				}
			}
			else
			{
				// Add the final position if there is no more magnitude left to cover.
				bounceRay.endPoints.Add(finalPosition);
				bounceRay.finalDirection = direction;
			}
		}

		// Return the current position & direction as final.
		return bounceRay;
	}
}