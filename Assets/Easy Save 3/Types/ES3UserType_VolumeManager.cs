using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("masterSliderObj", "masterVolumeSlider", "masterVolumeValue", "musicVolumeMixer", "musicSliderObj", "musicVolumeSlider", "musicVolumeValue", "effectsVolumeMixer", "effectsSliderObj", "effectsVolumeSlider", "effectsVolumeValue", "screenCheckboxObj", "screenCheckbox", "isFullScreen")]
	public class ES3UserType_VolumeManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_VolumeManager() : base(typeof(VolumeManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (VolumeManager)obj;
			
			writer.WritePrivateFieldByRef("masterSliderObj", instance);
			writer.WritePrivateFieldByRef("masterVolumeSlider", instance);
			writer.WritePrivateField("masterVolumeValue", instance);
			writer.WritePropertyByRef("musicVolumeMixer", instance.musicVolumeMixer);
			writer.WritePrivateFieldByRef("musicSliderObj", instance);
			writer.WritePrivateFieldByRef("musicVolumeSlider", instance);
			writer.WritePrivateField("musicVolumeValue", instance);
			writer.WritePropertyByRef("effectsVolumeMixer", instance.effectsVolumeMixer);
			writer.WritePrivateFieldByRef("effectsSliderObj", instance);
			writer.WritePrivateFieldByRef("effectsVolumeSlider", instance);
			writer.WritePrivateField("effectsVolumeValue", instance);
			writer.WritePrivateFieldByRef("screenCheckboxObj", instance);
			writer.WritePrivateFieldByRef("screenCheckbox", instance);
			writer.WriteProperty("isFullScreen", instance.isFullScreen, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (VolumeManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "masterSliderObj":
					instance = (VolumeManager)reader.SetPrivateField("masterSliderObj", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "masterVolumeSlider":
					instance = (VolumeManager)reader.SetPrivateField("masterVolumeSlider", reader.Read<UnityEngine.UI.Slider>(), instance);
					break;
					case "masterVolumeValue":
					instance = (VolumeManager)reader.SetPrivateField("masterVolumeValue", reader.Read<System.Single>(), instance);
					break;
					case "musicVolumeMixer":
						instance.musicVolumeMixer = reader.Read<UnityEngine.Audio.AudioMixer>();
						break;
					case "musicSliderObj":
					instance = (VolumeManager)reader.SetPrivateField("musicSliderObj", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "musicVolumeSlider":
					instance = (VolumeManager)reader.SetPrivateField("musicVolumeSlider", reader.Read<UnityEngine.UI.Slider>(), instance);
					break;
					case "musicVolumeValue":
					instance = (VolumeManager)reader.SetPrivateField("musicVolumeValue", reader.Read<System.Single>(), instance);
					break;
					case "effectsVolumeMixer":
						instance.effectsVolumeMixer = reader.Read<UnityEngine.Audio.AudioMixer>();
						break;
					case "effectsSliderObj":
					instance = (VolumeManager)reader.SetPrivateField("effectsSliderObj", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "effectsVolumeSlider":
					instance = (VolumeManager)reader.SetPrivateField("effectsVolumeSlider", reader.Read<UnityEngine.UI.Slider>(), instance);
					break;
					case "effectsVolumeValue":
					instance = (VolumeManager)reader.SetPrivateField("effectsVolumeValue", reader.Read<System.Single>(), instance);
					break;
					case "screenCheckboxObj":
					instance = (VolumeManager)reader.SetPrivateField("screenCheckboxObj", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "screenCheckbox":
					instance = (VolumeManager)reader.SetPrivateField("screenCheckbox", reader.Read<UnityEngine.UI.Toggle>(), instance);
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