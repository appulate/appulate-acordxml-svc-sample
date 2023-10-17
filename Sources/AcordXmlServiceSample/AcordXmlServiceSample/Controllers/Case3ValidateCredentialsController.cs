using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;

namespace AcordXmlServiceSample.Controllers;
[ApiController]
[Route("api/wc/220/case3")]
public class Case3ValidateCredentialsController : ControllerBase {

	[HttpPost]
	[Consumes(Application.Xml, Text.Xml)]
	public ActionResult Post([FromBody] string requestContent) {
		var requestXml = XDocument.Parse(requestContent);
		ResponseOptions options;
		var login = requestXml.XPathSelectElements("ACORD/SignonRq/SignonPswd/CustId/CustLoginId").SingleOrDefault()?.Value;
		var password = requestXml.XPathSelectElements("ACORD/SignonRq/SignonPswd/CustPswd/Pswd").SingleOrDefault()?.Value;

		bool validUser;
		try {
			validUser = AccountMembership.ValidateUser(login, password);
		}
		catch (AuthenticationException ex) {
			options = new ResponseOptions("1740", "Error", "Authentication Failed", ex.Message);
			return Content(ResponseGenerator.CreateResponseXml(options), Text.Xml);
		}

		if (validUser) {
			options = new ResponseOptions("0", "Success", "Data uploaded successfully", "The user is valid.");
		} else {
			options = new ResponseOptions("1740", "Error", "Authentication Failed", "Incorrect login or password.");
		}
		return Content(ResponseGenerator.CreateResponseXml(options), Text.Xml);
	}
}