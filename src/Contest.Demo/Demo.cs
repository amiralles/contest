namespace Contest.Demo {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using _  = System.Action<Contest.Core.Runner>;

    class Contest101 {

		_ this_is_a_passing_test = assert => 
			assert.Equal(4, 2 + 2);

		_ this_is_a_failing_test = assert =>
			assert.Equal(5, 2 + 2);

		_ this_is_a_throw_expected_passing_test = test =>
			test.ShouldThrow(() => 1/0);

		_ this_is_a_throw_expected_failing_test = test =>
			test.ShouldThrow(() => 1/1);
    }

    class Contest201 {

		_ before_each = test => {
			Users.Create("pipe");
			Users.Create("vilmis");
			Users.Create("amiralles");
		};

		_ after_each = test =>
			Users.Reset();

		_ find_existing_user_returns_user = assert => 
			assert.IsNotNull(FindUsr("pipe"));

		_ find_non_existing_user_returns_null = assert => 
			assert.IsNull(FindUsr("not exists"));

		_ create_user_adds_new_user = assert => {
			User.Create("foo");
			assert.Equal(4, User.Count);
		}
    }

	class User {	
		readonly static List<string> _users = new List<string>;

		public Action Reset = () => _users.Clear();
			
		public Action<string> Create = name =>
			_users.Add(name);

		public static Func<int> Count = _users.Count;

		public static Func<string, object> FindUsr = name =>
			Users.FirstOrDefault(u => u == name);
	}
}
