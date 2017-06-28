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

			int n = vh.currentVertCount;
			for (int i = 0; i < n; i += 4)
			{
				UIGradientUtils.GetQuad(vh, ref v0, ref v1, ref v2, ref v3, i);

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
						UIGradientUtils.SetQuad(vh, v0, v1, v12, v30, i);
						UIGradientUtils.AddQuad(vh, v30, v12, v2, v3);
					}
					else if(split01 && split23)
					{
						UIGradientUtils.SetQuad(vh,v0, v01, v23, v3, i);
						UIGradientUtils.AddQuad(vh, v01, v1, v2, v23);
					}
					else if(split01 && split12)
					{
						UIGradientUtils.SetQuad(vh, v0, v12, v2, v3, i);
						UIGradientUtils.AddQuad(vh, v01, v1, v12, v0);
					}
					else if(split12 && split23)
					{
						UIGradientUtils.SetQuad(vh, v0, v1, v23, v3, i);
						UIGradientUtils.AddQuad(vh, v12, v2, v23, v1);
					}
					else if(split23 && split30)
					{
						UIGradientUtils.SetQuad(vh, v0, v1, v2, v30, i);
						UIGradientUtils.AddQuad(vh, v30, v2, v23, v3);
					}
					else if(split30 && split01)
					{
						UIGradientUtils.SetQuad(vh, v01, v1, v2, v3, i);
						UIGradientUtils.AddQuad(vh, v01, v3, v30, v0);
					}
				}
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
}
