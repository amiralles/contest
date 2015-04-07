
namespace Contest.Core {
    using System;
    using System.Collections.Generic;

    public static class StringExtensions {
		public static string Interpol(this string placeholder, params object[] args){
			return string.Format(placeholder, args);
		}
	}
}
