using System.Xml.Linq;

namespace AcordXmlServiceSample;
public static class ResponseGenerator {
	public static string CreateResponseXml(ResponseOptions options) {
		var responseFullStr = $@"<ACORD>
									<SignonRs>
										<Status>
											<StatusCd>{options.StatusCd}</StatusCd>
											<StatusDesc>{options.StatusDesc}</StatusDesc>
										</Status>
									</SignonRs>
									<InsuranceSvcRs>
										<WorkCompPolicyQuoteInqRs>
											<MsgStatus>
												<MsgStatusCd>{options.MsgStatusCd}</MsgStatusCd>
												<MsgStatusDesc>{options.MsgStatusDesc}</MsgStatusDesc>
											</MsgStatus>
											<ResponseURL>{options.ResultUrl}</ResponseURL>
											<PolicySummaryInfo>
												<FullTermAmt>
													<Amt>{options.Premium}</Amt>
												</FullTermAmt>
											</PolicySummaryInfo>
											<Policy>
												<QuoteInfo>
													<CompanysQuoteNumber>{options.QuoteNumber}</CompanysQuoteNumber>
												</QuoteInfo>
											</Policy>
										</WorkCompPolicyQuoteInqRs>
									</InsuranceSvcRs>
								</ACORD>";

		XDocument resultXml = XDocument.Parse(responseFullStr);
		XElement root = resultXml.Root;
		List<XElement> emptyElements = (from x in root.Descendants()
										where string.IsNullOrEmpty(x.Value)
										select x).ToList();
		foreach (XElement element in emptyElements) {
			RemoveNode(element);
		}
		return resultXml.ToString();
	}

	private static void RemoveNode(XElement node) {
		XElement? parent = node.Parent;
		if (parent == null) {
			return;
		}
		node.Remove();
		if (!parent.HasElements) {
			RemoveNode(parent);
		}
	}
}