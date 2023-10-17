using System.Xml.Linq;
using System.Xml.XPath;

namespace AcordXmlServiceSample.Model;
public record RatingInfo {
	//LocationRef corresponds to LocationRef attribute from Request XML
	//LocationRef value is used to relate to the Location by Location.Id value
	public string? LocationRef { get; init; }
	public string? LocationNumber { get; init; }
	public string? State { get; init; }
	public string? ClassCode { get; init; }
	public string? ClassCodeDesc { get; init; }
	public string? FullTimeEmpl { get; init; }
	public string? PartTimeEmpl { get; init; }
	public string? EstimatedPayroll { get; init; }
	
	public static RatingInfo[] GetRatingInformation(XDocument xml) {
		//get data from repeatable aggreages <WorkCompLocInfo> under all <WorkCompRateState>
		XElement[] wcRatingElms = xml.XPathSelectElements("ACORD/InsuranceSvcRq/*/WorkCompLineBusiness/WorkCompRateState/WorkCompLocInfo").ToArray();
		List<RatingInfo> ratingInfos = new();
		foreach (XElement element in wcRatingElms) {
			ratingInfos.Add(new RatingInfo {
				State = element.XPathSelectElement("../StateProvCd")?.Value,
				LocationRef = element.Attribute("LocationRef")?.Value,
				LocationNumber = element.XPathSelectElement("WorkCompRateClass/ItemIdInfo/OtherIdentifier[OtherIdTypeCd = 'Location']/OtherId")?.Value,
				ClassCode = element.XPathSelectElement("WorkCompRateClass/RatingClassificationCd")?.Value,
				ClassCodeDesc = element.XPathSelectElement("WorkCompRateClass/RatingClassificationDesc")?.Value,
				FullTimeEmpl = element.XPathSelectElement("WorkCompRateClass/NumEmployeesFullTime")?.Value,
				PartTimeEmpl = element.XPathSelectElement("WorkCompRateClass/NumEmployeesPartTime")?.Value,
				EstimatedPayroll = element.XPathSelectElement("WorkCompRateClass/ExposureInfo/Exposure")?.Value
			});
		}
		return ratingInfos.ToArray();
	}
}