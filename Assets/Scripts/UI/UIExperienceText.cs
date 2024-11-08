using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(TMP_Text))]
	public class UIExperienceText : MonoBehaviour
	{
		private TMP_Text _text;

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
			Player.OnPlayerExperienceChange += UpdateExperienceText;
		}

		private void OnDisable()
		{
			Player.OnPlayerExperienceChange -= UpdateExperienceText;

		}

		void UpdateExperienceText(int exp)
		{
			_text.text = "EXP: "+exp.ToString();
		}
	}
}