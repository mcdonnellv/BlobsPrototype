﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ColorDefines {

	static public Dictionary<string,string> blobColorSet01 = new Dictionary<string,string>();
	static public Color defaultBlobColor = new Color(0.529f, 0.506f, 0.459f, 1f);
	public static Color maleColor { get{ return new Color(0.259f, 0.753f, 0.984f, 1f);} }
	public static Color femaleColor { get{ return new Color(0.984f, 0.537f, 0.659f, 1f);} }


	public static void BuildColorDefines() {
		//blobColorSet01.Add("Pastel Brown", "836953");
		blobColorSet01.Add("Pastel Purple", "B39EB5");
		blobColorSet01.Add("Pastel Yellow", "FDFD96");
		blobColorSet01.Add("Pastel Violet", "CB99C9");
		blobColorSet01.Add("Pastel Red", "FF6961");
		blobColorSet01.Add("Pastel Pink 2", "FFD1DC");
		blobColorSet01.Add("Pastel Pink", "DEA5A4");
		blobColorSet01.Add("Pastel Orange", "FFB347");
		blobColorSet01.Add("Pastel Magenta", "F49AC2");
		blobColorSet01.Add("Pastel Green", "77DD77");
		blobColorSet01.Add("Pastel Gray", "CFCFC4");
		blobColorSet01.Add("Pastel Blue", "AEC6CF");
		blobColorSet01.Add("Light Pastel Purple", "B19CD9");
		blobColorSet01.Add("Dark Pastel Red", "C23B22");
		blobColorSet01.Add("Dark Pastel Purple", "966FD6");
		blobColorSet01.Add("Dark Pastel Green", "03C03C");
		blobColorSet01.Add("Dark Pastel Blue", "779ECB");
	}


	public static string ColorToHexString(Color c){
		return string.Format("[{0}{1}{2}]",
		                     ((int)(c.r * 255)).ToString("X2"),
		                     ((int)(c.g * 255)).ToString("X2"),
		                     ((int)(c.b * 255)).ToString("X2"));
	}

	public static Color HexStringToColor(string s){
		int hexVal = Convert.ToInt32(s, 16);
		Color c = new Color( ((hexVal >> 16) & 0xFF) / 255f, ((hexVal >> 8) & 0xFF) / 255f, ((hexVal) & 0xFF) / 255f, 1f);
		return c;
	}

	public static Color ColorForQuality(Quality q) {
		switch (q) {
		case Quality.Standard:  return new Color(.7f, .7f, .7f, 1f);
		case Quality.Rare:      return new Color(0.255f, 0.616f, 1f, .5f);
		case Quality.Epic:      return new Color(0.957f, 0.294f, 1f, .5f);
		case Quality.Legendary: return new Color(1f, 0.773f, 0.082f, .5f);
		}
		return Color.white;
	}
}
