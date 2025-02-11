using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("inventoryItems", "stashItems", "equippedWeapon", "equippedSecondary", "equippedNecklace", "equippedRing", "equippedAmulet", "equippedPotion")]
	public class ES3UserType_InventoryManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_InventoryManager() : base(typeof(InventoryManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (InventoryManager)obj;
			
			writer.WriteProperty("inventoryItems", instance.inventoryItems, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<ItemBase>)));
			writer.WriteProperty("stashItems", instance.stashItems, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<ItemBase>)));
			writer.WritePropertyByRef("equippedWeapon", instance.equippedWeapon);
			writer.WritePropertyByRef("equippedSecondary", instance.equippedSecondary);
			writer.WritePrivateFieldByRef("equippedNecklace", instance);
			writer.WritePrivateFieldByRef("equippedRing", instance);
			writer.WritePrivateFieldByRef("equippedAmulet", instance);
			writer.WritePropertyByRef("equippedPotion", instance.equippedPotion);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (InventoryManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "inventoryItems":
						instance.inventoryItems = reader.Read<System.Collections.Generic.List<ItemBase>>();
						break;
					case "stashItems":
						instance.stashItems = reader.Read<System.Collections.Generic.List<ItemBase>>();
						break;
					case "equippedWeapon":
						instance.equippedWeapon = reader.Read<Weapon>();
						break;
					case "equippedSecondary":
						instance.equippedSecondary = reader.Read<Secondary>();
						break;
					case "equippedNecklace":
					instance = (InventoryManager)reader.SetPrivateField("equippedNecklace", reader.Read<Jewel>(), instance);
					break;
					case "equippedRing":
					instance = (InventoryManager)reader.SetPrivateField("equippedRing", reader.Read<Jewel>(), instance);
					break;
					case "equippedAmulet":
					instance = (InventoryManager)reader.SetPrivateField("equippedAmulet", reader.Read<Jewel>(), instance);
					break;
					case "equippedPotion":
						instance.equippedPotion = reader.Read<Potion>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_InventoryManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_InventoryManagerArray() : base(typeof(InventoryManager[]), ES3UserType_InventoryManager.Instance)
		{
			Instance = this;
		}
	}
}