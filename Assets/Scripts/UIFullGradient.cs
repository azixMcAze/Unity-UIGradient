using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

[AddComponentMenu("UI/Effects/Full Gradient")]
public class UIFullGradient : BaseMeshEffect
{
	public Gradient m_gradient;
	public Gradient m_gradient2;
	public List<float> m_times;
	public Color m_color1 = Color.white;
	public Color m_color2 = Color.white;
	[Range(-180f, 180f)]
	public float m_angle = 0f;
	public bool m_ignoreRatio = true;


	public new void OnValidate()
	{
		base.OnValidate();
		// List<KeyValuePair<float, Color>> keyList = GetCombineGradient(m_gradient);
		// var colorKeys = keyList.Select(kvp => new GradientColorKey(new Color(kvp.Value.r, kvp.Value.g, kvp.Value.b, 1f), kvp.Key)).ToArray();
		// var alphaKeys = keyList.Select(kvp => new GradientAlphaKey(kvp.Value.a, kvp.Key)).ToArray();

		m_times = GetKeyTimes(m_gradient);
		var colorKeys = m_times.Select(t => new GradientColorKey(m_gradient.Evaluate(t), t)).ToArray();
		var alphaKeys = m_times.Select(t => new GradientAlphaKey(m_gradient.Evaluate(t).a, t)).ToArray();
		m_gradient2.SetKeys(colorKeys, alphaKeys);
		m_gradient2.mode = m_gradient.mode;
	}

	static List<float> GetKeyTimes(Gradient gradient)
	{
		List<float> keyList = new List<float>(gradient.alphaKeys.Length + gradient.colorKeys.Length);
		var colorKeys = gradient.colorKeys;
		var alphaKeys = gradient.alphaKeys;
		int nc = colorKeys.Length;
		int na = alphaKeys.Length;
		int ic = 0;
		int ia = 0;
		float t = -1f;
		
		while(ic < nc || ia < na)
		{
			float tk;
			if(ic < nc && ia < na)
			{
				float tc = colorKeys[ic].time;
				float ta = alphaKeys[ia].time;
				if(tc < ta)
				{
					tk = tc;
					ic++;
				}
				else
				{
					tk = ta;
					ia++;
				}
			}
			else if(ic < nc)
			{
				tk = colorKeys[ic].time;
				ic++;
			}
			else //if(ia < nc)
			{
				tk = alphaKeys[ia].time;
				ia++;
			}

			if(tk > t)
			{
				t = tk;
				keyList.Add(tk);
			}
		}
		return keyList;
	}

    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
            Rect rect = graphic.rectTransform.rect;

            float angleRad = m_angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(angleRad);
            float cos = Mathf.Cos(angleRad);

			UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, cos, sin);

            List<float> keyTimes = GetKeyTimes(m_gradient);
            UIVertex v0 = default(UIVertex);
            UIVertex v1 = default(UIVertex);
            UIVertex v2 = default(UIVertex);
            UIVertex v3 = default(UIVertex);
            int vertexCount = vh.currentVertCount;
            for (int i = 0; i < vertexCount; i += 4)
            {
				UIGradientUtils.GetQuad(vh, ref v0, ref v1, ref v2, ref v3, i);

            	Vector2 pos0 = localPositionMatrix * v0.position;
            	Vector2 pos1 = localPositionMatrix * v1.position;
            	Vector2 pos2 = localPositionMatrix * v2.position;
            	Vector2 pos3 = localPositionMatrix * v3.position;

            	int i0 = keyTimes.BinarySearch(pos0.y);
            	int i1 = keyTimes.BinarySearch(pos1.y);
            	int i2 = keyTimes.BinarySearch(pos2.y);
            	int i3 = keyTimes.BinarySearch(pos3.y);

				if(i0 < 0)
					i0 = ~i0;
				if(i1 < 0)
					i1 = ~i1;
				if(i2 < 0)
					i2 = ~i2;
				if(i3 < 0)
					i3 = ~i3;

/*

1 -- 2
|  / |
| /  |
0 -- 3

 */


				// Debug.Log(i + " : " + rp0.y + ":" + i0 + " " + rp1.y + ":" + i1 + " " + rp2.y + ":" + i2 + " " + rp3.y + ":" + i3);
            	if(i0 == i1 && i0 == i2 && i0 == i3)
            	{
            		v0.color *= m_gradient.Evaluate(pos0.y);
            		v1.color *= m_gradient.Evaluate(pos1.y);
            		v2.color *= m_gradient.Evaluate(pos2.y);
            		v3.color *= m_gradient.Evaluate(pos3.y);
            	}

				UIGradientUtils.SetQuad(vh, v0, v1, v2, v3, i);
            }
        }
    }
}
