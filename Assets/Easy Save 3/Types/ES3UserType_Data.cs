using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_Data : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Data() : base(typeof(Data)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Data)obj;
			
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Data)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Data();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_DataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DataArray() : base(typeof(Data[]), ES3UserType_Data.Instance)
		{
			Instance = this;
		}
	}
}