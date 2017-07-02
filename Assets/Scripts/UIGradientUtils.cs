using UnityEngine;

public static class UIGradientUtils
{
	public struct Matrix2x3
	{
		public float m00, m01, m02, m10, m11, m12;
		public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12)
		{
			this.m00 = m00;
			this.m01 = m01;
			this.m02 = m02;
			this.m10 = m10;
			this.m11 = m11;
			this.m12 = m12;
		}

		public static Vector2 operator*(Matrix2x3 m, Vector2 v)
		{
			float x = (m.m00 * v.x) - (m.m01 * v.y) + m.m02;
			float y = (m.m10 * v.x) + (m.m11 * v.y) + m.m12;
			return new Vector2(x, y);
		}
	}

	public static Matrix2x3 LocalPositionMatrix(Rect rect, float cos, float sin)
	{
		Vector2 rectMin = rect.min;
		Vector2 rectSize = rect.size;
		float c = 0.5f;
		float ax = rectMin.x / rectSize.x + c;
		float ay = rectMin.y / rectSize.y + c;
		float m00 = cos / rectSize.x;
		float m01 = sin / rectSize.y;
		float m02 = -(ax * cos - ay * sin - c);
		float m10 = sin / rectSize.x;
		float m11 = cos / rectSize.y;		
		float m12 = -(ax * sin + ay * cos - c);
		return new Matrix2x3(m00, m01, m02, m10, m11, m12);
	}

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
