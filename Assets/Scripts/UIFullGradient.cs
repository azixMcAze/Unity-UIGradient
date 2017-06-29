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
	public float[] m_times;
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

	// static List<KeyValuePair<float, Color>> GetCombineGradient(Gradient gradient)
	// {
	// 	List<KeyValuePair<float, Color>> keyList = new List<KeyValuePair<float, Color>>();
	// 	var colorKeys = gradient.colorKeys;
	// 	var alphaKeys = gradient.alphaKeys;
	// 	int nc = colorKeys.Length;
	// 	int na = alphaKeys.Length;
	// 	int ai = -1;
	// 	int ci = -1;
	// 	float t = 0f;

	// 	while(ci < nc && ai < na)
	// 	{
	// 		Color ck, ck1;
	// 		if(ci == -1)
	// 		{

	// 			ck = colorKeys[0].color;
	// 			ck1 = colorKeys[0].color;
	// 		}
	// 		else if(ci == nc)
	// 		{
	// 			ck = colorKeys[nc - 1].color;
	// 			ck1 = colorKeys[nc - 1].color;
	// 		}
	// 		else
	// 		{
	// 			ck = colorKeys[ci].color;
	// 			ck1 = colorKeys[ci + 1].color;
	// 		}

	// 		var ak = alphaKeys[ai];

	// 		t = Mathf.Min(ck.time, ak.time);

	// 		var ck1 = colorKeys[ci + 1];
	// 		var ak1 = alphaKeys[ai + 1];

	// 		// if(ci == 0 && ai == 0)
	// 		// {
	// 		// 	if(ck.time > 0f || ak.time > 0f)
	// 		// 	{
	// 		// 		Color c = new Color(ck.color.r, ck.color.g, ck.color.b, ak.alpha);
	// 		// 		// float t = 0f;
	// 		// 		keyList.Add(new KeyValuePair<float, Color>(t, c));
	// 		// 	}
	// 		// }

	// 		if(ck.time == t && ak.time == t)
	// 		{
	// 			Color c = ck.color;
	// 			float a = ak.alpha;
	// 			Color ca = new Color(c.r, c.g, c.b, a);
	// 			keyList.Add(new KeyValuePair<float, Color>(t, ca));

	// 			ai++;
	// 			ci++;					
	// 		}
	// 		else if(ck.time == t)
	// 		{
	// 			Color c = ck.color;
	// 			float at = (t - ak.time) / (ak1.time - ak.time);
	// 			float a = Mathf.Lerp(ak.alpha, ak1.alpha, at);
	// 			Color ca = new Color(c.r, c.g, c.b, a);
	// 			keyList.Add(new KeyValuePair<float, Color>(t, ca));

	// 			ci++;
	// 		}
	// 		else if(ak.time == t)
	// 		{
	// 			float ct = (t - ck.time) / (ck1.time - ck.time);
	// 			Color c = Color.Lerp(ck.color, ck1.color, ct);
	// 			float a = ak.alpha;
	// 			Color ca = new Color(c.r, c.g, c.b, a);
	// 			keyList.Add(new KeyValuePair<float, Color>(t, ca));

	// 			ai++;
	// 		}


	// 		// if(ci == colorKeys.Length - 1 && ai == alphaKeys.Length - 1)
	// 		// {
	// 		// 	if(ck.time < 1f || ak.time < 1f)
	// 		// 	{
	// 		// 		Color c = new Color(ck.color.r, ck.color.g, ck.color.b, ak.alpha);
	// 		// 		// float t = 1f;
	// 		// 		keyList.Add(new KeyValuePair<float, Color>(t, c));
	// 		// 	}
	// 		// }
	// 	}

	// 	return keyList;

	// }

	static float[] GetKeyTimes(Gradient gradient)
	{
		float[] keyTimes = new float[] {0f, 1f};
		var colorKeysTimes = gradient.colorKeys.Select(k => k.time);
		var alphaKeysTimes = gradient.alphaKeys.Select(k => k.time);
		var uniqueTimes = keyTimes.Concat(colorKeysTimes).Concat(alphaKeysTimes).Distinct().OrderBy(t => t);
		return uniqueTimes.ToArray();
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

            float angleRad = m_angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(angleRad);
            float cos = Mathf.Cos(angleRad);
            Vector2 center = new Vector2 (0.5f, 0.5f);

            float[] keyTimes = GetKeyTimes(m_gradient);
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

            	Vector2 np0 = UIGradientUtils.NormalizedPosition(v0.position, rect);
            	Vector2 np1 = UIGradientUtils.NormalizedPosition(v1.position, rect);
            	Vector2 np2 = UIGradientUtils.NormalizedPosition(v2.position, rect);
            	Vector2 np3 = UIGradientUtils.NormalizedPosition(v3.position, rect);
				
            	Vector2 rp0 = UIGradientUtils.Rotate(np0 - center, cos, sin) + center;
            	Vector2 rp1 = UIGradientUtils.Rotate(np1 - center, cos, sin) + center;
            	Vector2 rp2 = UIGradientUtils.Rotate(np2 - center, cos, sin) + center;
            	Vector2 rp3 = UIGradientUtils.Rotate(np3 - center, cos, sin) + center;

            	int i0 = Array.BinarySearch(keyTimes, rp0.y);
            	int i1 = Array.BinarySearch(keyTimes, rp1.y);
            	int i2 = Array.BinarySearch(keyTimes, rp2.y);
            	int i3 = Array.BinarySearch(keyTimes, rp3.y);

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
