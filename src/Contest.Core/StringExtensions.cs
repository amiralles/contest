
namespace Contest.Core {
    public static class StringExtensions {
		public static string Interpol(this string placeholder, params object[] args){
			return string.Format(placeholder, args);
		}

		///This method IS case sensitive.
        public static bool Match(this string pattern, string str){
            if(string.IsNullOrEmpty(pattern))
                return true;

            foreach(var p in pattern.Split(' ')){
                if(p == "*")
                    return true;

                if(p.EndsWith("*") && p.StartsWith("*"))
                    return str.Contains(p.Replace("*",""));

                if(p.EndsWith("*"))
                    return str.StartsWith(p.Replace("*",""));

                if(p.StartsWith("*"))
                    return str.EndsWith(p.Replace("*",""));

                if(p == str)
                    return true;
            }

            return false;
        }

	}
}
