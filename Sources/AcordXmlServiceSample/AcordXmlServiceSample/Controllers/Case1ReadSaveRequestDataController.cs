using AcordXmlServiceSample.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;

namespace AcordXmlServiceSample.Controllers;
[ApiController]
public class Case1ReadSaveRequestDataController : ControllerBase {
	private readonly FileService _fileService;

	public Case1ReadSaveRequestDataController(FileService fileService) {
		_fileService = fileService;
	}

	[HttpPost]
	[Route("api/wc/220/case1")]
	[Consumes(Application.Xml, Text.Xml)]
	public async Task<ActionResult> Post([FromBody]string requestContent) {
		var requestXml = XDocument.Parse(requestContent);

		var agency = Agency.GetAgencyAndProducer(requestXml);
		var mainInsured = Insured.GetGeneralInsured(requestXml);
		
		string policyEffDate = requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Policy[@id = 'CommlPolicy']/ContractTerm/EffectiveDt")?.Value ?? "N/A";
		string policyExpDate = requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Policy[@id = 'CommlPolicy']/ContractTerm/ExpirationDt")?.Value ?? "N/A";
		Dictionary<string, string> employersLiabilityLimits = new() {
			{ "Each Accident", requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Policy[@id = 'CommlPolicy']/Coverage[CoverageCd = 'WCEL']/Limit[LimitAppliesToCd = 'PerAcc']/FormatCurrencyAmt/Amt")?.Value ?? "N/A" },
			{ "Disease-Policy Limit", requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Policy[@id = 'CommlPolicy']/Coverage[CoverageCd = 'WCEL']/Limit[LimitAppliesToCd = 'DisPol']/FormatCurrencyAmt/Amt")?.Value ?? "N/A" },
			{ "Disease-Each Employee Limit", requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Policy[@id = 'CommlPolicy']/Coverage[CoverageCd = 'WCEL']/Limit[LimitAppliesToCd = 'DisEachEmpl']/FormatCurrencyAmt/Amt")?.Value ?? "N/A" }
		};

		//get data from repeatable aggregates <LocationUWInfo>
		List<LocationUWInfo> locationsUWInfo = new();
		XElement[] locUWElements = requestXml.XPathSelectElements("ACORD/InsuranceSvcRq/*/LocationUWInfo").ToArray();
		foreach (var locUWElement in locUWElements) {
			locationsUWInfo.Add(new LocationUWInfo(
				locUWElement.XPathSelectElement("BldgOccupancy/AreaOccupied[UnitMeasurementCd = 'FTK']/NumUnits")?.Value,
				locUWElement.Attribute("LocationRef")?.Value, //is used to relate to the Location by Location.Id value
				locUWElement.Attribute("SubLocationRef")?.Value //is used to relate to the Building by Building.Id value
			));
		}

		var ratingInfo = RatingInfo.GetRatingInformation(requestXml);
		var ratingFactors = RatingFactors.GetRatingFactors(requestXml);
		var insuranceLossHistory = WCInsuranceAndLossHistory.GetInsuranceLossHistory(requestXml);

		var resultSb = new StringBuilder(100);
		resultSb.AppendLine("Agency:")
			.AppendLine($"-- Name: {agency.Name ?? "N/A"}")
			.AppendLine("-- Address:")
			.AppendLine($"---- Street: {agency.Address?.Street ?? "N/A"}")
			.AppendLine($"---- City: {agency.Address?.City ?? "N/A"}")
			.AppendLine($"---- State: {agency.Address?.State ?? "N/A"}")
			.AppendLine($"---- ZIP Code: {agency.Address?.PostalCode ?? "N/A"}")
			.AppendLine($"-- Phone #: {agency.Phone ?? "N/A"}")
			.AppendLine($"-- Code: {agency.AgencyCode ?? "N/A"}")
			.AppendLine("-- Producer:")
			.AppendLine($"---- Name: {agency.Producer?.Name ?? "N/A"}")
			.AppendLine($"---- Phone #: {agency.Producer?.Phone ?? "N/A"}")
			.AppendLine($"---- Email: {agency.Producer?.Email ?? "N/A"}")
			.AppendLine($"---- Code: {agency.Producer?.ProducerCode ?? "N/A"}")
			.AppendLine()
			.AppendLine("General Insured:")
			.AppendLine($"-- Name: {mainInsured.Name ?? "N/A"}")
			.AppendLine($"-- FEIN: {mainInsured.Fein ?? "N/A"}")
			.AppendLine($"-- Entity: {mainInsured.LegalEntityCd ?? "N/A"}")
			.AppendLine($"-- Date Business Started: {mainInsured.DateBusinessStarted ?? "N/A"}")
			.AppendLine("-- Mailing Address:")
			.AppendLine($"---- Street: {mainInsured.MailingAddress?.Street ?? "N/A"}")
			.AppendLine($"---- City: {mainInsured.MailingAddress?.City ?? "N/A"}")
			.AppendLine($"---- State: {mainInsured.MailingAddress?.State ?? "N/A"}")
			.AppendLine($"---- ZIP Code: {mainInsured.MailingAddress?.PostalCode ?? "N/A"}")
			.AppendLine();
		if (mainInsured.Contacts.Length > 0) {
			resultSb.AppendLine("Contact Information:");
			foreach (Contact contact in mainInsured.Contacts) {
				resultSb.AppendLine($"-- Name: {contact.Name ?? "N/A"}")
					.AppendLine($"---- Phone #: {contact.Phone ?? "N/A"}")
					.AppendLine($"---- Email: {contact.Email ?? "N/A"}");
				if (contact.RoleCds.Length > 0) {
					resultSb.AppendLine($"---- Role code: {string.Join(',', contact.RoleCds)}");
				} else { resultSb.AppendLine($"---- Role code: N/A"); }
			}
		} else { resultSb.AppendLine("Contact Information: N/A"); }

		resultSb.AppendLine()
			.AppendLine($"Policy Effective Date: {policyEffDate}")
			.AppendLine($"Policy Expiration Date: {policyExpDate}")
			.AppendLine()
			.AppendLine("Employer's Liability Limits: ");
		foreach(var limit in employersLiabilityLimits) {
			resultSb.AppendLine($"-- {limit.Key}: {limit.Value}");
		}

		resultSb.AppendLine();
		if (mainInsured.Locations.Length > 0) {
			foreach (Location location in mainInsured.Locations) {
				resultSb.AppendLine($"Location: {location.LocationNumber ?? "N/A"}, Id - {location.Id ?? "N/A"}")
					.AppendLine($"-- Street: {location.Address?.Street ?? "N/A"}")
					.AppendLine($"-- City: {location.Address?.City ?? "N/A"}")
					.AppendLine($"-- State: {location.Address?.State ?? "N/A"}")
					.AppendLine($"-- ZIP Code: {location.Address?.PostalCode ?? "N/A"}");
				if (location.Buildings.Length == 0) {
					resultSb.AppendLine($"-- Occupied Area: {locationsUWInfo.Where(luw => luw.LocationRef == location.Id)
																.Select(luw => luw.AreaOccupied)
																.FirstOrDefault() ?? "N/A"}"); ;
				} else {
					foreach (Building building in location.Buildings) {
						resultSb.AppendLine($"---- Building: {building.BuildingNumber ?? "N/A"}, Id - {building.Id ?? "N/A"}")
							.AppendLine($"------ Street: {building.Address?.Street ?? "N/A"}")
							.AppendLine($"------ City: {building.Address?.City ?? "N/A"}")
							.AppendLine($"------ State: {building.Address?.State ?? "N/A"}")
							.AppendLine($"------ ZIP Code: {building.Address?.PostalCode ?? "N/A"}")
							.AppendLine($"------ Occupied Area: {locationsUWInfo.Where(luw => luw.SubLocationRef == building.Id)
																	.Select(luw => luw.AreaOccupied)
																	.FirstOrDefault() ?? "N/A"}");
					}
				}
				foreach (RatingInfo rating in ratingInfo.Where(r => r.LocationRef == location.Id)) {
					//Rating Information items that correspond to the Location by LocationRef
					resultSb.AppendLine("Rating Information:");
					resultSb.AppendLine($"-- Location #: {rating.LocationNumber ?? "N/A"}, LocationRef: {rating.LocationRef}, State: {rating.State ?? "N/A"}")
						.AppendLine($"---- Class Code: {rating.ClassCode ?? "N/A"}, Description: {rating.ClassCodeDesc ?? "N/A"}")
						.AppendLine($"------ Number of full time employees: {rating.FullTimeEmpl ?? "N/A"}")
						.AppendLine($"------ Number of part time employees: {rating.PartTimeEmpl ?? "N/A"}")
						.AppendLine($"------ Estimated annual payroll: {rating.EstimatedPayroll ?? "N/A"}");
				}
			}
		} else { resultSb.AppendLine("Locations: N/A"); }

		resultSb.AppendLine();
		if (ratingInfo.Length == 0) {
			resultSb.AppendLine("Rating Information: N/A");
		}
		if (ratingFactors.Length > 0) {
			resultSb.AppendLine("Rating Factors:");
			foreach(RatingFactors factor in ratingFactors) {
				resultSb.AppendLine($"-- State: {factor.State ?? "N/A"}")
					.AppendLine($"---- Experience Or Merit Modification - Factor: {factor.ExpModFactor ?? "N/A"}");
			}
		} else { resultSb.AppendLine("Rating Factors: N/A"); }
		
		resultSb.AppendLine();
		resultSb.AppendLine("Underwriting:")
			.AppendLine($"1. Is a written safety program in operation? - {requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/WorkCompLineBusiness/QuestionAnswer[QuestionCd = 'WORK15']/YesNoCd")?.Value ?? "N/A"}")
			.AppendLine($"2. Are employee health plans provided? - {requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/WorkCompLineBusiness/QuestionAnswer[QuestionCd = 'WORK08']/YesNoCd")?.Value ?? "N/A"}")
			.AppendLine($"3. Does the insured lease employees to or from other employers? - {requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/Policy[@id = 'CommlPolicy']/QuestionAnswer[QuestionCd = 'CGL04']/YesNoCd")?.Value ?? "N/A"}")
			.AppendLine($"4. Any tax liens or bankruptcy within the last 5 years? - {requestXml.XPathSelectElement("ACORD/InsuranceSvcRq/*/WorkCompLineBusiness/QuestionAnswer[QuestionCd = 'GENRL14']/YesNoCd")?.Value ?? "N/A"}");

		resultSb.AppendLine();
		if (insuranceLossHistory.Length > 0) {
			resultSb.AppendLine("WC Insurance and Loss History:");
			foreach(WCInsuranceAndLossHistory ins in insuranceLossHistory) {
				resultSb.AppendLine($"-- Year: {ins.Year ?? "N/A"}")
					.AppendLine($"---- Carrier: {ins.Carrier ?? "N/A"}")
					.AppendLine($"---- Experience modification: {ins.ExpModFactor ?? "N/A"}")
					.AppendLine($"---- Premium: {ins.Premium ?? "N/A"}")
					.AppendLine($"---- # Claims: {ins.NumberOfClaims ?? "N/A"}")
					.AppendLine($"---- Amount paid: {ins.Paid ?? "N/A"}")
					.AppendLine($"---- Reserve: {ins.Reserve ?? "N/A"}");
			}
		} else { resultSb.AppendLine("WC Insurance and Loss History: N/A"); }


		var id = Guid.NewGuid();
		var resultPath = await _fileService.SaveAsync(id, resultSb.ToString());
		ResponseOptions options = new("0", "Success",
			"Data uploaded successfully",
			$"Request XML was successfully processed. New text file was created {resultPath}",
			"1000.00",
			Url.Action(action: "Get", controller: "Files", values: new { id }, protocol: Request.Scheme),
			id.ToString());

		string responseString = ResponseGenerator.CreateResponseXml(options);
		return Content(responseString, Text.Xml);
	}
}