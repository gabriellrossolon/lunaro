using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("currentItem")]
	public class ES3UserType_InventorySlot : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_InventorySlot() : base(typeof(InventorySlot)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (InventorySlot)obj;
			
			writer.WritePrivateFieldByRef("currentItem", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (InventorySlot)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "currentItem":
					instance = (InventorySlot)reader.SetPrivateField("currentItem", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_InventorySlotArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_InventorySlotArray() : base(typeof(InventorySlot[]), ES3UserType_InventorySlot.Instance)
		{
			Instance = this;
		}
	}
}