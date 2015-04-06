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
            test.ShouldThrow<NullReferenceException>(() => {
                object target = null;
                var dummy = target.ToString();
            });

		_ this_is_a_throw_expected_failing_test = test =>
			test.ShouldThrow<NullReferenceException>(() => {
				//it doesn't throws; So it fails.
			});
    }

    class Contest201 {

		//Seguir desde aca:
		//before_each/after_each no esta implementado.
		_ before_each = test => {
			User.Create("pipe");
			User.Create("vilmis");
			User.Create("amiralles");
		};

		_ after_each = test =>
			User.Reset();

		_ find_existing_user_returns_user = assert => 
			assert.IsNotNull(User.Find("pipe"));

		_ find_non_existing_user_returns_null = assert => 
			assert.IsNull(User.Find("not exists"));

		_ create_user_adds_new_user = assert => {
			User.Create("foo");
			assert.Equal(4, User.Count());
		};
    }

	public class User {	
		static readonly List<string> _users = new List<string>();

		public static Action Reset = () => _users.Clear();
			
		public static Action<string> Create = name => _users.Add(name);

		public static Func<int> Count = () => _users.Count;

		public static Func<string, object> Find = name =>
			_users.FirstOrDefault(u => u == name);
	}
}
