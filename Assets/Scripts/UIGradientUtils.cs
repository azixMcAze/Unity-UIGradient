using UnityEngine;
using UnityEngine.UI;

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

	static Vector2[] ms_verticesPositions = new Vector2[] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };
	public static Vector2[] VerticePositions
	{
		get { return ms_verticesPositions; }
	}

	public static void CompensateAspectRatio(Rect rect, float cos, float sin, out float out_cos, out float out_sin)
	{
		float ratio = rect.height / rect.width;
		cos *= ratio;
		float norm = Mathf.Sqrt (cos * cos + sin * sin);
		out_cos = cos / norm;
		out_sin = sin / norm;
	}

	public static float InverseLerp (float a, float b, float v)
	{
		return a != b ? (v - a) / (b - a) : 0f;
	}

	public static Color Bilerp(Color a1, Color a2, Color b1, Color b2, Vector2 t)
	{
		Color a = Color.LerpUnclamped(a1, a2, t.x);
		Color b = Color.LerpUnclamped(b1, b2, t.x);
		return Color.LerpUnclamped(a, b, t.y);
	}

	public static void Lerp(UIVertex a, UIVertex b, float t, ref UIVertex c)
	{
		c.position = Vector3.LerpUnclamped(a.position, b.position, t);
		c.normal = Vector3.LerpUnclamped(a.normal, b.normal, t);
		c.color = Color32.LerpUnclamped(a.color, b.color, t);
		c.tangent = Vector3.LerpUnclamped(a.tangent, b.tangent, t);
		c.uv0 = Vector3.LerpUnclamped(a.uv0, b.uv0, t);
		c.uv1 = Vector3.LerpUnclamped(a.uv1, b.uv1, t);
		// c.uv2 = Vector3.LerpUnclamped(a.uv2, b.uv2, t);
		// c.uv3 = Vector3.LerpUnclamped(a.uv3, b.uv3, t);		
	}
}
