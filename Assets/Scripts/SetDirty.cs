using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDirty : MonoBehaviour {
	public Graphic m_graphic;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		m_graphic.SetVerticesDirty();
	}
}
