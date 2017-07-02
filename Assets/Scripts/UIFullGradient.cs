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
		// Debug.LogFormat("gradient {0} {1} alpha, {2} color", m_gradient.mode, m_gradient.alphaKeys.Length, m_gradient.colorKeys.Length);
		// foreach(var key in m_gradient.alphaKeys)
		// {
		// 	Debug.LogFormat(" alpha {0} {1}", key.alpha, key.time);
		// }
		// foreach(var key in m_gradient.colorKeys)
		// {
		// 	Debug.LogFormat(" color {0} {1}", key.color, key.time);
		// }

		if(enabled)
		{
			// Debug.Log(vh.currentVertCount + "\n" + vh.currentIndexCount);


			// int childs = transform.childCount;
			// for (int i = childs - 1; i >= 0; i--)
			// {
			// 	GameObject.Destroy(transform.GetChild(i).gameObject);
			// }


            // System.Text.StringBuilder sb = new System.Text.StringBuilder();
			// UIVertex vertex = default(UIVertex);
			// for (int i = 0; i < vh.currentVertCount; i++) {
			// 	vh.PopulateUIVertex (ref vertex, i);
			// 	GameObject go = new GameObject(i.ToString());
			// 	go.transform.SetParent(transform, false);
			// 	go.transform.SetAsLastSibling();
			// 	go.transform.localPosition = vertex.position;
			// 	sb.AppendLine(vertex.position.ToString("F7"));
			// }
			// Debug.Log(sb.ToString());



            Rect rect = graphic.rectTransform.rect;
            Vector2 rectMin = rect.min;
            Vector2 rectMax = rect.max;

            float angleRad = m_angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(angleRad);
            float cos = Mathf.Cos(angleRad);
            Vector2 center = new Vector2 (0.5f, 0.5f);

            List<float> keyTimes = GetKeyTimes(m_gradient);
            UIVertex v0 = default(UIVertex);
            UIVertex v1 = default(UIVertex);
            UIVertex v2 = default(UIVertex);
            UIVertex v3 = default(UIVertex);
            int vertexCount = vh.currentVertCount;
            for (int i = 0; i < vertexCount; i += 4)
            {
            	vh.PopulateUIVertex (ref v0, i);
            	vh.PopulateUIVertex (ref v1, i + 1);
            	vh.PopulateUIVertex (ref v2, i + 2);
            	vh.PopulateUIVertex (ref v3, i + 3);

            	Vector2 np0 = UIGradientUtils.NormalizedPosition(v0.position, rectMin, rectMax);
            	Vector2 np1 = UIGradientUtils.NormalizedPosition(v1.position, rectMin, rectMax);
            	Vector2 np2 = UIGradientUtils.NormalizedPosition(v2.position, rectMin, rectMax);
            	Vector2 np3 = UIGradientUtils.NormalizedPosition(v3.position, rectMin, rectMax);
				
            	Vector2 rp0 = UIGradientUtils.Rotate(np0 - center, cos, sin) + center;
            	Vector2 rp1 = UIGradientUtils.Rotate(np1 - center, cos, sin) + center;
            	Vector2 rp2 = UIGradientUtils.Rotate(np2 - center, cos, sin) + center;
            	Vector2 rp3 = UIGradientUtils.Rotate(np3 - center, cos, sin) + center;

            	int i0 = keyTimes.BinarySearch(rp0.y);
            	int i1 = keyTimes.BinarySearch(rp1.y);
            	int i2 = keyTimes.BinarySearch(rp2.y);
            	int i3 = keyTimes.BinarySearch(rp3.y);

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
            	// if(i0 == i1 && i0 == i2 && i0 == i3)
            	{
            		v0.color *= m_gradient.Evaluate(rp0.y);
            		v1.color *= m_gradient.Evaluate(rp1.y);
            		v2.color *= m_gradient.Evaluate(rp2.y);
            		v3.color *= m_gradient.Evaluate(rp3.y);
            	}

            	vh.SetUIVertex (v0, i);
            	vh.SetUIVertex (v1, i + 1);
            	vh.SetUIVertex (v2, i + 2);
            	vh.SetUIVertex (v3, i + 3);
            }
        }
    }
}
