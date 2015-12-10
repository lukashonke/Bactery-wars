using Assets.scripts.Mono.ObjectData;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.scripts.Mono
{
	public sealed class PlayerUI : MonoBehaviour
	{
		private bool mouseOverUi = false;
		public bool MouseOverUI
		{
			get { return mouseOverUi; }
		}

		public Button skill1;
		public Button skill2;
		public Button skill3;
		public Button skill4;

		public Text hp;

		private PlayerData data;

		// Use this for initialization
		void Start()
		{
			data = GetComponent<PlayerData>();
			//hp = FindO
		}

		// Update is called once per frame
		void Update()
		{
			//hp.text = "HP " + data.visibleHp; //TODO fix
		}

		public void MenuClick()
		{
			Debug.Log("clicked ");
		}

		public void Skill(int order)
		{
			data.LaunchSkill(order);
		}

		public void Skill1()
		{
			data.LaunchSkill(1);
		}

		public void Skill2()
		{
			data.LaunchSkill(2);
		}

		public void Skill3()
		{
			data.LaunchSkill(3);
		}

		public void Skill4()
		{
			data.LaunchSkill(4);
		}

		public void Skill5()
		{
			data.LaunchSkill(5);
		}

		public void SetMouseOverUi()
		{
			mouseOverUi = true;
		}

		public void SetMouseNotOverUi()
		{
			mouseOverUi = false;
		}
	}
}
