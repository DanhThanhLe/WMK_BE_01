using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.Helpers
{
	public static class HashHelper
	{
		public static string GetSignature256(String text)
		{
			using(SHA256 sha256 = SHA256.Create())
			{
				// Convert the input string to a byte array and compute the hash.
				byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

				// Convert the byte array to a string representation.
				StringBuilder builder = new StringBuilder();
				for ( int i = 0; i < bytes.Length; i++ )
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}
		public static String GetSignature256(String text , String key)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();

			Byte[] textBytes = encoding.GetBytes(text);
			Byte[] keyBytes = encoding.GetBytes(key);

			Byte[] hashBytes;

			using ( HMACSHA256 hash = new HMACSHA256(keyBytes) )
				hashBytes = hash.ComputeHash(textBytes);

			return BitConverter.ToString(hashBytes).Replace("-" , "").ToLower();
		}
		public static string CreateHMACSHA256Signature(string data , string key)
		{
			using ( var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)) )
			{
				var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
				return BitConverter.ToString(hash).Replace("-" , "").ToLower();
			}
		}

		public static bool VerifyHash(string text , string hash)
		{
			// Hash the input text and compare it to the provided hash.
			string hashOfInput = GetSignature256(text);
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;
			return comparer.Compare(hashOfInput , hash) == 0;
		}
	}
}
