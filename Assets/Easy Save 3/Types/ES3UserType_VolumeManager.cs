using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("masterVolumeValue", "musicVolumeValue", "effectsVolumeValue", "isFullScreen")]
	public class ES3UserType_VolumeManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_VolumeManager() : base(typeof(VolumeManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (VolumeManager)obj;
			
			writer.WritePrivateField("masterVolumeValue", instance);
			writer.WritePrivateField("musicVolumeValue", instance);
			writer.WritePrivateField("effectsVolumeValue", instance);
			writer.WriteProperty("isFullScreen", instance.isFullScreen, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (VolumeManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "masterVolumeValue":
					instance = (VolumeManager)reader.SetPrivateField("masterVolumeValue", reader.Read<System.Single>(), instance);
					break;
					case "musicVolumeValue":
					instance = (VolumeManager)reader.SetPrivateField("musicVolumeValue", reader.Read<System.Single>(), instance);
					break;
					case "effectsVolumeValue":
					instance = (VolumeManager)reader.SetPrivateField("effectsVolumeValue", reader.Read<System.Single>(), instance);
					break;
					case "isFullScreen":
						instance.isFullScreen = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_VolumeManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_VolumeManagerArray() : base(typeof(VolumeManager[]), ES3UserType_VolumeManager.Instance)
		{
			Instance = this;
		}
	}
}