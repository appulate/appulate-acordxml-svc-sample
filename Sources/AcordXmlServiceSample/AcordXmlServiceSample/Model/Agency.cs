using System.Xml.Linq;
using System.Xml.XPath;

namespace AcordXmlServiceSample.Model;
public record Agency {
	public string? Name { get; init; }
	public string? Phone { get; init; }
	public string? AgencyCode { get; init; }
	public Address? Address { get; init; }
	public Producer? Producer { get; init; }

	public static Agency GetAgencyAndProducer(XDocument xml) {
		string? producerFirstName = xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/NameInfo[@id = 'ProducerName']/PersonName/GivenName")?.Value;
		string? producerLastName = xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/NameInfo[@id = 'ProducerName']/PersonName/Surname")?.Value;
		string? producerName = string.IsNullOrEmpty(producerFirstName) && string.IsNullOrEmpty(producerLastName) ? null : string.Join(" ", producerFirstName, producerLastName);
		var agency = new Agency {
			Name = xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/NameInfo/CommlName/CommercialName")?.Value,
			Address = new Address(
				xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/Addr[AddrTypeCd = 'StreetAddress']/Addr1")?.Value,
				xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/Addr[AddrTypeCd = 'StreetAddress']/City")?.Value,
				xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/Addr[AddrTypeCd = 'StreetAddress']/StateProvCd")?.Value,
				xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/Addr[AddrTypeCd = 'StreetAddress']/PostalCode")?.Value),
			Phone = xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/Communications/PhoneInfo[PhoneTypeCd = 'Phone' and count(CommunicationUseCd)=0]/PhoneNumber")?.Value,
			Producer = new Producer(
				producerName,
				xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/Communications/PhoneInfo[PhoneTypeCd = 'Phone' and CommunicationUseCd = 'Alternate']/PhoneNumber")?.Value
					?? xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/Communications/PhoneInfo[PhoneTypeCd = 'Cell']/PhoneNumber")?.Value,
				xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/GeneralPartyInfo/Communications/EmailInfo/EmailAddr")?.Value,
				xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/ProducerInfo[@id = 'Producer']/ProducerSubCode")?.Value),
			AgencyCode = xml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Producer/ProducerInfo[@id = 'Producer']/ContractNumber")?.Value
		};
		return agency;
	}
}