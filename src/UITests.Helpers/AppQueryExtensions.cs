﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using static UITests.Queries.Helpers;
using static UITests.Helpers.BackdoorInvocationHelper;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using UITests.Helpers;

namespace UITests.Queries
{
	public static partial class AppQueryExtensions
	{
		public static AppQuery XamlParent(this AppQuery query, string controlName)
		{
			return query.Parent(PlatformHelpers.On(iOS: () => controlName.Replace(".", "_"), Android: () => GetAndroidName(controlName)));
		}

		private static string GetAndroidName(string controlName)
		{
			var assembly = App.Invoke("GetTypeAssemblyFullName", controlName)?.ToString();

			if(assembly == null)
			{
				throw new InvalidOperationException($"Unknown type {controlName}");
			}

			var typeName = controlName.Substring(controlName.LastIndexOf('.') + 1);
			var nameSpace = controlName.Substring(0, controlName.LastIndexOf('.'));

			var controlType = "md5" + GetMd5Hash(nameSpace + ":" + assembly) + "." + typeName;

			Console.WriteLine($"{controlName}({typeName} / {nameSpace}): {controlType}");

			return controlType;
		}

		public static AppTypedSelector<object> GetDependencyPropertyValue(this AppQuery query, string dependencyPropertyName)
		{
			return query
				.Invoke(FormatBackdoorMethodName("GetDependencyPropertyValue"), dependencyPropertyName);
		}


		static string GetMd5Hash(string input)
		{
			using (MD5 md5Hash = MD5.Create())
			{
				// Convert the input string to a byte array and compute the hash.
				var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

				// Create a new Stringbuilder to collect the bytes
				// and create a string.
				var sBuilder = new StringBuilder();

				// Loop through each byte of the hashed data 
				// and format each one as a hexadecimal string.
				for (int i = 0; i < data.Length; i++)
				{
					sBuilder.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
				}

				// Return the hexadecimal string.
				return sBuilder.ToString();
			}
		}
	}
}
