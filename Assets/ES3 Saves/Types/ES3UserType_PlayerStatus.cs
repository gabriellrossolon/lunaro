using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("playerGold")]
	public class ES3UserType_PlayerStatus : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerStatus() : base(typeof(PlayerStatus)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PlayerStatus)obj;
			
			writer.WriteProperty("playerGold", instance.playerGold, ES3Type_float.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PlayerStatus)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "playerGold":
						instance.playerGold = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_PlayerStatusArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PlayerStatusArray() : base(typeof(PlayerStatus[]), ES3UserType_PlayerStatus.Instance)
		{
			Instance = this;
		}
	}
}