namespace AcordXmlServiceSample;
public class AccountMembership {
	private static readonly Dictionary<string, string> Accounts = new(StringComparer.InvariantCultureIgnoreCase) {
		{ "john", "pass123" },
		{ "mike", "Pass456" },
		{ "kelly", "pass_789" }
	};

	public static bool ValidateUser(string? userName, string? password) {
		if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) {
			throw new AuthenticationException("No login or password in the request.");
		}
		return Accounts.ContainsKey(userName) && string.CompareOrdinal(Accounts[userName], password) == 0;
	}
}