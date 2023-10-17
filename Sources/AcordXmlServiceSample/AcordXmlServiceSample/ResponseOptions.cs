namespace AcordXmlServiceSample;
public record ResponseOptions(
	string StatusCd, string MsgStatusCd,
	string? StatusDesc = null, string? MsgStatusDesc = null,
	string? Premium = null, string? ResultUrl = null, string? QuoteNumber = null);