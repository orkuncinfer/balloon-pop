using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_DataTest : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DataTest() : base(typeof(DataTest)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (DataTest)obj;
			
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (DataTest)obj;
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
	}


	public class ES3UserType_DataTestArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DataTestArray() : base(typeof(DataTest[]), ES3UserType_DataTest.Instance)
		{
			Instance = this;
		}
	}
}