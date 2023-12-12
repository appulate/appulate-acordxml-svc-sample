using System.Xml.Linq;
using System.Xml.XPath;

namespace AcordXmlServiceSample.Model;
public record Insured {
	public string? Name { get; init; }
	public string? Fein { get; init; }
	public string? LegalEntityCd { get; init; }
	public string? DateBusinessStarted { get; init; }
	public Address? MailingAddress { get; init; }
	public Contact[] Contacts { get; init; } = Array.Empty<Contact>();
	public Location[] Locations { get; init; } = Array.Empty<Location>();

	public static Insured GetGeneralInsured(XDocument xml) {
		var locations = Location.GetLocations(xml);
		var contacts = Contact.GetContacts(xml);

		XElement? mainInsuredElement = xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/InsuredOrPrincipal[InsuredOrPrincipalInfo/InsuredOrPrincipalRoleCd = 'FNI']");
		var mainInsured = new Insured {
			Name = mainInsuredElement?.XPathSelectElement("GeneralPartyInfo/NameInfo/CommlName/CommercialName")?.Value,
			LegalEntityCd = mainInsuredElement?.XPathSelectElement("GeneralPartyInfo/NameInfo/LegalEntityCd")?.Value,
			Fein = mainInsuredElement?.XPathSelectElement("GeneralPartyInfo/NameInfo/TaxIdentity[TaxIdTypeCd = 'FEIN']/TaxId")?.Value,
			MailingAddress = new Address(
				mainInsuredElement?.XPathSelectElement("GeneralPartyInfo/Addr[AddrTypeCd = 'MailingAddress']/Addr1")?.Value,
				mainInsuredElement?.XPathSelectElement("GeneralPartyInfo/Addr[AddrTypeCd = 'MailingAddress']/City")?.Value,
				mainInsuredElement?.XPathSelectElement("GeneralPartyInfo/Addr[AddrTypeCd = 'MailingAddress']/StateProvCd")?.Value,
				mainInsuredElement?.XPathSelectElement("GeneralPartyInfo/Addr[AddrTypeCd = 'MailingAddress']/PostalCode")?.Value),
			DateBusinessStarted = mainInsuredElement?.XPathSelectElement("InsuredOrPrincipalInfo/BusinessInfo/BusinessStartDt")?.Value,
			Contacts = contacts,
			Locations = locations
		};
		return mainInsured;
	}
}