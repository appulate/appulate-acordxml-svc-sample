using AcordXmlServiceSample.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AcordXmlServiceSample.Controllers;
[ApiController]
[Route("api/wc/220/case2")]
public class Case2ValidateRequestDataController : ControllerBase {
	
	[HttpPost]
	[Consumes(Application.Xml, Text.Xml)]
	public ActionResult Post([FromBody] string requestContent) {
		var requestXml = XDocument.Parse(requestContent);
		var agency = Agency.GetAgencyAndProducer(requestXml);
		var mainInsured = Insured.GetGeneralInsured(requestXml);

		IList<string> requiredDataAndAbsentInRequest = new List<string>();
		if (string.IsNullOrEmpty(agency.Name)) {
			requiredDataAndAbsentInRequest.Add("Agency name");
		}
		if (string.IsNullOrEmpty(agency.Address?.City) || string.IsNullOrEmpty(agency.Address?.State) || string.IsNullOrEmpty(agency.Address?.PostalCode)) {
			requiredDataAndAbsentInRequest.Add("Agency address (City, State, Postal Code)");
		}
		if (string.IsNullOrEmpty(mainInsured.Name) || string.IsNullOrEmpty(mainInsured.Fein)) {
			requiredDataAndAbsentInRequest.Add("First Named Insured (name, FEIN)");
		}
		if (mainInsured.Locations.Length == 0) {
			requiredDataAndAbsentInRequest.Add("Insured locations");
		}

		string msgStatusCd;
		string msgStatusDesc;
		if (requiredDataAndAbsentInRequest.Count > 0) {
			var msgDescSb = new StringBuilder();
			msgDescSb.AppendJoin(',', requiredDataAndAbsentInRequest.ToArray());
			msgStatusCd = "Error";
			msgStatusDesc = "Next data is required: " + msgDescSb;
		} else if (mainInsured.Locations.Any(loc => loc.Address?.State == "FL")) {
			msgStatusCd = "Rejected";
			msgStatusDesc = "We regret that we are unable to provide you with a quote for Insured locations in the FL state.";
		} else {
			msgStatusCd = "Success";
			msgStatusDesc = "Request XML contains all required data.";
		}
		ResponseOptions options = new("0", msgStatusCd, "Data uploaded successfully", msgStatusDesc);

		string responseString = ResponseGenerator.CreateResponseXml(options);
		return Content(responseString, Text.Xml);
	}
}