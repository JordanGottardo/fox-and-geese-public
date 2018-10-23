using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Utils {

	public static void PrintDictionary<T>(Dictionary<T, T> dict) {
		var str = new StringBuilder();
		str.Append("{");
		foreach (var pair in dict) {
			str.Append(String.Format(" {0}={1} ", pair.Key, pair.Value));
		}
		str.Append("}");
		Debug.Log(str.ToString());
	}

}
