using System.Xml.Linq;
using System.Xml.XPath;

namespace AcordXmlServiceSample.Model;
public record Contact {
	public string[] RoleCds { get; init; } = Array.Empty<string>();
	public string? Name { get; init; }
	public string? Phone { get; init; }
	public string? Email { get; init; }

	public static Contact[] GetContacts(XDocument xml) {
		List<Contact> contacts = new();
		XElement[] contactElements = xml.XPathSelectElements("ACORD/InsuranceSvcRq/*/Policy[@id = 'CommlPolicy']/MiscParty").ToArray();
		foreach (XElement element in contactElements) {
			string? firstName = element.XPathSelectElement("GeneralPartyInfo/NameInfo/PersonName/GivenName")?.Value;
			string? lastName = element.XPathSelectElement("GeneralPartyInfo/NameInfo/PersonName/Surname")?.Value;
			string? contactName = string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) ? null : string.Join(" ", firstName, lastName);
			contacts.Add(new Contact {
				RoleCds = element.XPathSelectElements("MiscPartyInfo/MiscPartyRoleCd")?.Select(r => r.Value).ToArray() ?? Array.Empty<string>(),
				Name = contactName,
				Phone = element.XPathSelectElement("GeneralPartyInfo/Communications/PhoneInfo[PhoneTypeCd = 'Phone' and CommunicationUseCd = 'Business']/PhoneNumber")?.Value
					?? element.XPathSelectElement("GeneralPartyInfo/Communications/PhoneInfo[PhoneTypeCd = 'Cell']/PhoneNumber")?.Value
					?? element.XPathSelectElement("GeneralPartyInfo/Communications/PhoneInfo[PhoneTypeCd = 'Phone' and CommunicationUseCd = 'Home']/PhoneNumber")?.Value,
				Email = element.XPathSelectElement("GeneralPartyInfo/Communications/EmailInfo[CommunicationUseCd = 'Business']/EmailAddr")?.Value
					?? element.XPathSelectElement("GeneralPartyInfo/Communications/EmailInfo[CommunicationUseCd = 'Alternate']/EmailAddr")?.Value
			});
		}
		return contacts.ToArray();
	}
}