using UnityEngine;

public static class UIGradientUtils
{
	public static Color InterpolatedColor(Vector2 normalizedPosition, Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
	{
		Color top = Color.Lerp(topLeft, topRight, normalizedPosition.x);
		Color bottom = Color.Lerp(bottomLeft, bottomRight, normalizedPosition.x);
		return Color.Lerp(bottom, top, normalizedPosition.y);
	}

	public static Vector2 NormalizedPosition(Vector2 position, Vector2 rectMin, Vector2 rectInvSize)
	{
		float x = (position.x - rectMin.x) * rectInvSize.x;
		float y = (position.y - rectMin.y) * rectInvSize.y;
		return new Vector2(x, y);
	}

	public static Vector2 Rotate(Vector2 v, float cos, float sin)
	{
		float x = (cos * v.x) - (sin * v.y);
		float y = (sin * v.x) + (cos * v.y);
		return new Vector2(x, y);
	}

	static Vector2 ms_center = new Vector2 (0.5f, 0.5f);
	public static Vector2 LocalPosition(Vector3 position, Vector2 rectMin, Vector2 rectInvSize, float cos, float sin)
	{
		Vector2 normalizedPosition = UIGradientUtils.NormalizedPosition(position, rectMin, rectInvSize);
		Vector2 rotatedPosition = UIGradientUtils.Rotate(normalizedPosition - ms_center, cos, sin) + ms_center;
		return rotatedPosition;
	}

	static Vector2[] ms_verticesPositions = new Vector2[] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };
	public static Vector2[] VerticePositions
	{
		get { return ms_verticesPositions; }
	}
}
