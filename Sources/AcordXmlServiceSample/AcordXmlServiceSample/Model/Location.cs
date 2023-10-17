using System.Xml.Linq;
using System.Xml.XPath;

namespace AcordXmlServiceSample.Model;
public record Location {
	public string? Id { get; init; }
	public string? LocationNumber { get; init; }
	public Address? Address { get; init; }
	public Building[] Buildings { get; init; } = Array.Empty<Building>();
	public static Location[] GetLocations(XDocument xml) {
		//get data from repeatable aggregates <Location>
		XElement[] locationElements = xml.XPathSelectElements("ACORD/InsuranceSvcRq/*/Location").ToArray();
		List<Location> locations = new();
		foreach (XElement location in locationElements) {
			//get data from repeatable aggregates <SubLocation> under each <Location>
			XElement[] sublocationElements = location.XPathSelectElements("SubLocation").ToArray();
			List<Building> buildings = new();
			foreach (XElement sublocation in sublocationElements) {
				buildings.Add(new Building (
					sublocation.Attribute("id")?.Value,
					sublocation.XPathSelectElement("ItemIdInfo/AgencyId")?.Value,
					new Address(
						sublocation.XPathSelectElement("Addr/Addr1")?.Value,
						sublocation.XPathSelectElement("Addr/City")?.Value,
						sublocation.XPathSelectElement("Addr/StateProvCd")?.Value,
						sublocation.XPathSelectElement("Addr/PostalCode")?.Value)
				));
			}
			locations.Add(new Location {
				Id = location.Attribute("id")?.Value,
				LocationNumber = location.XPathSelectElement("ItemIdInfo/AgencyId")?.Value,
				Address = new Address(
					location.XPathSelectElement("Addr/Addr1")?.Value,
					location.XPathSelectElement("Addr/City")?.Value,
					location.XPathSelectElement("Addr/StateProvCd")?.Value,
					location.XPathSelectElement("Addr/PostalCode")?.Value),
				Buildings = buildings.ToArray()
			});
		}
		return locations.ToArray();
	}
}