using System.Xml.Linq;
using System.Xml.XPath;

namespace AcordXmlServiceSample.Model;
public record RatingFactors {
	public string? State { get; init; }
	public string? ExpModFactor { get; init; }
	
	public static RatingFactors[] GetRatingFactors(XDocument xml) {
		XElement[] premiumCovElms = xml.XPathSelectElements("ACORD/InsuranceSvcRq/*/WorkCompLineBusiness/WorkCompRateState").ToArray();
		List<RatingFactors> factors = new();
		//get <Coverage> aggregates under all <WorkCompRateState> aggregates
		//sample includes only Experience Or Merit Modification - Factor
		foreach (XElement element in premiumCovElms) {
			factors.Add(new RatingFactors {
				State = element.XPathSelectElement("StateProvCd")?.Value,
				ExpModFactor = element.XPathSelectElement("Coverage[CoverageCd = 'EXP']/Limit/FormatModFactor")?.Value
			});
		}
		return factors.ToArray();
	}
}