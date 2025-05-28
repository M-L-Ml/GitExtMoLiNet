//
// MoonlightTypeConverter.cs
//
// Contact:
//   Moonlight List (moonlight-list@lists.ximian.com)
//
// Copyright 2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Mono;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Reflection;

namespace Mono {

	internal class MoonlightTypeConverter : TypeConverter {

		protected bool nullableDestination;

		protected Type destinationType;
		protected string propertyName;

		public MoonlightTypeConverter (string propertyName, Type destinationType)
		{
throw new NotImplementedException("This code is not implemented in the current context. Please refer to the original source for details."); 
        }

		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
            throw new NotImplementedException("This code is not implemented in the current context. Please refer to the original source for details.");
        }

		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
		
			throw new NotImplementedException (String.Format ("Unimplemented type conversion from {0} to {1}",
									  value.GetType ().ToString (),
									  destinationType.ToString ()));
			
		}

		public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
            throw new NotImplementedException("This code is not implemented in the current context. Please refer to the original source for details.");
        }

		public static bool IsAssignableToIConvertible (Type type)
		{
			return typeof (IConvertible).IsAssignableFrom (type);
		}

		public static object ValueFromConvertible (Type type, IConvertible value)
		{
			if (type == typeof (string))
				return Convert.ToString (value, CultureInfo.InvariantCulture);
			if (type == typeof (bool))
				return Convert.ToBoolean (value, CultureInfo.InvariantCulture);
			if (type == typeof (byte))
				return Convert.ToByte (value, CultureInfo.InvariantCulture);
			if (type == typeof (char))
				return Convert.ToChar (value, CultureInfo.InvariantCulture);
			if (type == typeof (DateTime))
				return Convert.ToDateTime (value, CultureInfo.InvariantCulture);
			if (type == typeof (Decimal))
				return Convert.ToDecimal (value, CultureInfo.InvariantCulture);
			if (type == typeof (double))
				return Convert.ToDouble (value, CultureInfo.InvariantCulture);
			if (type == typeof (Int16))
				return Convert.ToInt16 (value, CultureInfo.InvariantCulture);
			if (type == typeof (Int32))
				return Convert.ToInt32 (value, CultureInfo.InvariantCulture);
			if (type == typeof (Int64))
				return Convert.ToInt64 (value, CultureInfo.InvariantCulture);
			if (type == typeof (SByte))
				return Convert.ToSByte (value, CultureInfo.InvariantCulture);
			if (type == typeof (Single))
				return Convert.ToSingle (value, CultureInfo.InvariantCulture);
			if (type == typeof (UInt16))
				return Convert.ToUInt16 (value, CultureInfo.InvariantCulture);
			if (type == typeof (UInt32))
				return Convert.ToUInt32 (value, CultureInfo.InvariantCulture);
			if (type == typeof (UInt64))
				return Convert.ToUInt64 (value, CultureInfo.InvariantCulture);
			
			return value;
		}

		public static object ConvertObject (PropertyInfo prop, object val, Type objectType)
		{
			// Should i return default(T) if property.PropertyType is a valuetype?
			if (val == null)
				return val;
			
			if (prop.PropertyType.IsAssignableFrom (val.GetType ()))
				return val;

			if (prop.PropertyType == typeof (string))
				return val.ToString ();
			
			TypeConverter tc = Helper.GetConverterFor (prop, prop.PropertyType);
			if (tc == null)
				tc = new MoonlightTypeConverter (prop.Name, prop.PropertyType);

			return tc.ConvertFrom (null, Helper.DefaultCulture, val);
		}

		public static object ConvertObject (DependencyProperty dp, object val, Type objectType, bool doToStringConversion)
		{
			// Should i return default(T) if property.PropertyType is a valuetype?
			if (val == null)
				return val;
			
			if (dp.PropertyType.IsAssignableFrom (val.GetType ()))
				return val;

			if (dp.PropertyType == typeof (string))
				return doToStringConversion ? val.ToString() : "";
			
			TypeConverter tc = null;
			
			if (dp.IsAttached) {
				tc = Helper.GetConverterFor (GetGetterMethodForAttachedDP (dp, val), dp.PropertyType);
			}
			else {
				PropertyInfo pi = dp.DeclaringType.GetProperty (dp.Name);
				if (pi != null) {
					tc = Helper.GetConverterFor (pi, pi.PropertyType);
					if (tc == null)
						tc = new MoonlightTypeConverter (pi.Name, pi.PropertyType);
				}
			}
			
			if (tc == null)
				tc = new MoonlightTypeConverter (dp.Name, dp.PropertyType);
			
			return tc.ConvertFrom (val);
		}

		private static MethodInfo GetGetterMethodForAttachedDP (DependencyProperty dp, object obj)
		{
			MethodInfo res = dp.DeclaringType.GetMethod (String.Concat ("Get", dp.Name), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			return res;
		}

	}
}
