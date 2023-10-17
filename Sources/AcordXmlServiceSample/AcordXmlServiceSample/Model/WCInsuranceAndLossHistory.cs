using System.Xml.Linq;
using System.Xml.XPath;

namespace AcordXmlServiceSample.Model;
public record WCInsuranceAndLossHistory {
	public string? Year { get; init; }
	public string? Carrier { get; init; } 
	public string? Premium { get; init; } 
	public string? ExpModFactor { get; init; } 
	public string? NumberOfClaims { get; init; } 
	public string? Paid { get; init; } 
	public string? Reserve { get; init; }

	public static WCInsuranceAndLossHistory[] GetInsuranceLossHistory(XDocument xml) {
		//get WC Insurance and Loss History data from repeatable aggregates OtherOrPriorPolicy[PolicyCd = 'Prior' and LOBCd = 'WORK']
		XElement[] historyElms = xml.XPathSelectElements("ACORD/InsuranceSvcRq/*/Policy[@id = 'CommlPolicy']/OtherOrPriorPolicy[PolicyCd = 'Prior' and LOBCd = 'WORK']").ToArray();
		List<WCInsuranceAndLossHistory> insuranceLossHistory = new();
		foreach (XElement element in historyElms) {
			insuranceLossHistory.Add(new WCInsuranceAndLossHistory {
				Year = element.XPathSelectElement("ContractTerm/EffectiveDt")?.Value[..4],
				Carrier = element.XPathSelectElement("InsurerName")?.Value,
				Premium = element.XPathSelectElement("PolicyAmt/Amt")?.Value,
				ExpModFactor = element.XPathSelectElement("RatingFactor")?.Value,
				NumberOfClaims = element.XPathSelectElement("NumLosses")?.Value,
				Paid = element.XPathSelectElement("TotalPaidLossesAmt/Amt")?.Value,
				Reserve = element.XPathSelectElement("ReserveTotalAmt/Amt")?.Value
			});
		}
		return insuranceLossHistory.ToArray();
	}
}