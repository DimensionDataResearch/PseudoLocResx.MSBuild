// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CultureInfoExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The culture info extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoLocResx.MSBuild
{
	using System;
	using System.Globalization;

	/// <summary>
	/// The culture info extensions.
	/// </summary>
	public static class CultureInfoExtensions
	{
		/// <summary>
		/// The try parse.
		/// </summary>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <param name="cultureInfo">
		/// The culture info.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public static bool TryParse(this string name, out CultureInfo cultureInfo)
		{
			var result = false;
			CultureInfo ci = null;
			try
			{
				ci = new CultureInfo(name);
				result = true;
			}
			catch (ArgumentException)
			{
			}

			cultureInfo = ci;

			return result;
		}
	}
}
