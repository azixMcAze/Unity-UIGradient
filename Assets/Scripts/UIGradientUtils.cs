using UnityEngine;
using UnityEngine.UI;

public static class UIGradientUtils
{
	public static Color InterpolatedColor(Vector2 normalizedPosition, Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
	{
		Color top = Color.Lerp(topLeft, topRight, normalizedPosition.x);
		Color bottom = Color.Lerp(bottomLeft, bottomRight, normalizedPosition.x);
		return Color.Lerp(bottom, top, normalizedPosition.y);
	}

	public static Vector2 NormalizedPosition(Vector2 position, Rect rect)
	{
		float x = Mathf.InverseLerp (rect.xMin, rect.xMax, position.x);
		float y = Mathf.InverseLerp (rect.yMin, rect.yMax, position.y);
		return new Vector2(x, y);
	}

	public static Vector2 Rotate(Vector2 v, float cos, float sin)
	{
		float x = (cos * v.x) - (sin * v.y);
		float y = (sin * v.x) + (cos * v.y);
		return new Vector2(x, y);
	}

	static Vector2 ms_center = new Vector2 (0.5f, 0.5f);
	public static Vector2 LocalPosition(Vector3 position, Rect rect, float cos, float sin)
	{
		Vector2 normalizedPosition = UIGradientUtils.NormalizedPosition(position, rect);
		Vector2 rotatedPosition = UIGradientUtils.Rotate(normalizedPosition - ms_center, cos, sin) + ms_center;
		return rotatedPosition;
	}

	static Vector2[] ms_verticesPositions = new Vector2[] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };
	public static Vector2[] VerticePositions
	{
		get { return ms_verticesPositions; }
	}

	public static float InverseLerp (float a, float b, float v)
	{
		return a != b ? (v - a) / (b - a) : 0f;
	}

	public static void Lerp(UIVertex a, UIVertex b, float t, ref UIVertex c)
	{
		c.position = Vector3.LerpUnclamped(a.position, b.position, t);
		c.normal = Vector3.LerpUnclamped(a.normal, b.normal, t);
		c.color = Color.LerpUnclamped(a.color, b.color, t);
		c.tangent = Vector3.LerpUnclamped(a.tangent, b.tangent, t);
		c.uv0 = Vector3.LerpUnclamped(a.uv0, b.uv0, t);
		c.uv1 = Vector3.LerpUnclamped(a.uv1, b.uv1, t);
		// c.uv2 = Vector3.LerpUnclamped(a.uv2, b.uv2, t);
		// c.uv3 = Vector3.LerpUnclamped(a.uv3, b.uv3, t);		
	}

	public static int AddVert(VertexHelper vh, UIVertex v)
	{
		int i = vh.currentVertCount;
		vh.AddVert(v);
		return i;
	}

	public static void AddQuad(VertexHelper vh, UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3)
	{
		int i0 = AddVert(vh, v0);
		int i1 = AddVert(vh, v1);
		int i2 = AddVert(vh, v2);
		int i3 = AddVert(vh, v3);
		vh.AddTriangle(i0, i1, i2);
		vh.AddTriangle(i2, i3, i0);
	}

	public static void SetQuad(VertexHelper vh, UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3, int i)
	{
		vh.SetUIVertex (v0, i);
		vh.SetUIVertex (v1, i + 1);
		vh.SetUIVertex (v2, i + 2);
		vh.SetUIVertex (v3, i + 3);
	}

	public static void GetQuad(VertexHelper vh, ref UIVertex v0, ref UIVertex v1, ref UIVertex v2, ref UIVertex v3, int i)
	{
		vh.PopulateUIVertex (ref v0, i);
		vh.PopulateUIVertex (ref v1, i + 1);
		vh.PopulateUIVertex (ref v2, i + 2);
		vh.PopulateUIVertex (ref v3, i + 3);
	}
}
