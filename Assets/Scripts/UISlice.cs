using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlice : BaseMeshEffect {
	[Range(-180f, 180f)]
	public float m_angle = 0f;
	public float m_slice = 0.5f;

    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
			Rect rect = graphic.rectTransform.rect;

			float angleRad = m_angle * Mathf.Deg2Rad;
			float sin = Mathf.Sin(angleRad);
			float cos = Mathf.Cos(angleRad);
			
			UIVertex v0 = default(UIVertex);
			UIVertex v1 = default(UIVertex);
			UIVertex v2 = default(UIVertex);
			UIVertex v3 = default(UIVertex);

			UIVertex v01 = default(UIVertex);
			UIVertex v12 = default(UIVertex);
			UIVertex v23 = default(UIVertex);
			UIVertex v30 = default(UIVertex);

			for (int i = 0; i < vh.currentVertCount; i += 4)
			{
				int i0 = i;
				int i1 = i + 1;
				int i2 = i + 2;
				int i3 = i + 3;

				vh.PopulateUIVertex (ref v0, i0);
				vh.PopulateUIVertex (ref v1, i1);
				vh.PopulateUIVertex (ref v2, i2);
				vh.PopulateUIVertex (ref v3, i3);

				Vector2 pos0 = LocalPosition(v0.position, rect, cos, sin);
				Vector2 pos1 = LocalPosition(v1.position, rect, cos, sin);
				Vector2 pos2 = LocalPosition(v2.position, rect, cos, sin);
				Vector2 pos3 = LocalPosition(v3.position, rect, cos, sin);

				bool side0 = pos0.y < m_slice;
				bool side1 = pos1.y < m_slice;
				bool side2 = pos2.y < m_slice;
				bool side3 = pos3.y < m_slice;

				if(side0 != side1 || side0 != side2 || side0 != side3)
				{
					bool split01 = SplitEdgeIfNeeded(v0, v1, pos0.y, pos1.y, m_slice, side0, side1, ref v01);
					bool split12 = SplitEdgeIfNeeded(v1, v2, pos1.y, pos2.y, m_slice, side1, side2, ref v12);
					bool split23 = SplitEdgeIfNeeded(v2, v3, pos2.y, pos3.y, m_slice, side2, side3, ref v23);
					bool split30 = SplitEdgeIfNeeded(v3, v0, pos3.y, pos0.y, m_slice, side3, side0, ref v30);

					if(split12 && split30)
					{
						// vh.SetUIVertex (v0, i0);
						// vh.SetUIVertex (v1, i1);
						vh.SetUIVertex (v12, i2);
						vh.SetUIVertex (v30, i3);
						int i30 = AddUIVert(vh, v30);
						int i12 = AddUIVert(vh, v12);
						AddUIQuad(vh, i30, i12, i2, i3);
					}
					else if(split01 && split23)
					{
						// vh.SetUIVertex (v0, i0);
						vh.SetUIVertex (v01, i1);
						vh.SetUIVertex (v23, i2);
						// vh.SetUIVertex (v3, i3);
						int i01 = AddUIVert(vh, v01);
						int i23 = AddUIVert(vh, v23);
						AddUIQuad(vh, i01, i1, i2, i23);
					}
					else if(split01 && split12)
					{
						// vh.SetUIVertex (v0, i0);
						vh.SetUIVertex (v12, i1);
						// vh.SetUIVertex (v2, i2);
						// vh.SetUIVertex (v3, i3);
						int i01 = AddUIVert(vh, v01);
						int i12 = AddUIVert(vh, v12);
						AddUIQuad(vh, i0, i01, i1, i12);
					}
					else if(split12 && split23)
					{
						// vh.SetUIVertex (v0, i0);
						// vh.SetUIVertex (v1, i1);
						vh.SetUIVertex (v23, i2);
						// vh.SetUIVertex (v3, i3);
						int i12 = AddUIVert(vh, v12);
						int i23 = AddUIVert(vh, v23);
						AddUIQuad(vh, i3, i12, i2, i23);
					}
					else if(split23 && split30)
					{
						// vh.SetUIVertex (v0, i0);
						// vh.SetUIVertex (v1, i1);
						// vh.SetUIVertex (v2, i2);
						vh.SetUIVertex (v30, i3);
						int i23 = AddUIVert(vh, v23);
						int i30 = AddUIVert(vh, v30);
						AddUIQuad(vh, i30, i2, i23, i3);
					}
					else if(split30 && split01)
					{
						vh.SetUIVertex (v01, i0);
						// vh.SetUIVertex (v1, i1);
						// vh.SetUIVertex (v2, i2);
						// vh.SetUIVertex (v3, i3);
						int i30 = AddUIVert(vh, v30);
						int i01 = AddUIVert(vh, v01);
						AddUIQuad(vh, i0, i01, i3, i30);
					}



					// v0.color = side0 ? Color.red : Color.blue;
					// v1.color = side1 ? Color.red : Color.blue;
					// v2.color = side2 ? Color.red : Color.blue;
					// v3.color = side3 ? Color.red : Color.blue;
				}

				// vh.SetUIVertex (v0, i0);
				// vh.SetUIVertex (v1, i1);
				// vh.SetUIVertex (v2, i2);
				// vh.SetUIVertex (v3, i3);
			}
		}
	}

	static Vector2 LocalPosition(Vector3 position, Rect rect, float cos, float sin)
	{
		Vector2 center = new Vector2 (0.5f, 0.5f);
		Vector2 normalizedPosition = UIGradientUtils.NormalizedPosition(position, rect);
		Vector2 rotatedPosition = UIGradientUtils.Rotate(normalizedPosition - center, cos, sin) + center;
		return rotatedPosition;
	}

	static bool SplitEdgeIfNeeded(UIVertex v0, UIVertex v1, float pos0, float pos1, float posSlice, bool side0, bool side1, ref UIVertex v2)
	{
		if(side0 != side1)
		{
			float tSlice  = UIGradientUtils.InverseLerp(pos0, pos1, posSlice);
			UIGradientUtils.Lerp(v0, v1, tSlice, ref v2);
			return true;
		}
		else
		{
			return false;
		}
	}

	static int AddUIVert(VertexHelper vh, UIVertex v)
	{
		int i = vh.currentVertCount;
		vh.AddVert(v);
		return i;
	}

	static void AddUIQuad(VertexHelper vh, int i0, int i1, int i2, int i3)
	{
		vh.AddTriangle(i0, i1, i2);
		vh.AddTriangle(i2, i3, i0);
	}

}
