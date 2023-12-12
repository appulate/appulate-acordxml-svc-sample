# ACORD XML Web Service Sample
ACORD XML Web Service Sample is a simple implementation of Web API Service to receive submissions from Appulate with ACORD XML payload.  
This service
* can help you to set up your own service to receive submissions from Appulate
* receives HTTP request with XML content
* generates HTTP response with XML content
* is based on the following documentation
  * [ACORD XML Web Service implementation](https://docs.google.com/document/d/1OsiX_q3pDfWtQ_b40LFgxgm_24EbNn_Pmd5Swh_yX5Y)
  * [ACORD XML documentation](https://docs.google.com/document/d/19TOWxipRPxCP-kW3ZCbXAuSKPUPU70dsRNrFJ-LzwVI)
* is based on ACORD PCS v2.2.0, Worker's Compensation insurance line

#### Repository Structure
* The sources folder contains AcordXmlServiceSample solution folder
* The testing folder contains
  * Postman collection for testing ACORD XML Web Service Sample API
  * Subfolders with ACORD XML payload samples

#### Implemented cases
###### Case1
AcordXmlServiceSample API receives request, reads ACORD XML from request body and creates a text file.  
The text file will contain the following data from request XML:
* Agency Name, Address, Phone and Code
* Producer Name, Phone, Email and Code
* Insured Name, FEIN, Entity, Mailing Address, Date Business Started, Contacts
* Locations (addresses)
* Rating Information
* some Underwriting questions
* WC Insurance and Loss History  
Response will contain ResultUrl which you can use to open the created text file in your browser.

###### Case2
AcordXmlServiceSample API receives the request, reads ACORD XML and verifies the data.
1. The following information is required:
    * Agency Name and Address should be present
    * Insured Name and FEIN should be present
    * at least one Location should be present  
If some required information is missing, AcordXmlServiceSample response will contain an "Error" message. 
2. AcordXmlServiceSample service declines requests with locations in FL state. Response will contain a "Rejected" message.

###### Case3
AcordXmlServiceSample API receives request, reads ACORD XML and verifies the authentication data from XML.

Request ACORD XML XPaths:
* [User login/password](https://docs.google.com/spreadsheets/d/1TPe8oWDJKzcxYNGYiW_pALYlnM9EHWLWthiStBzAh_g/view#gid=0&range=4:5)
* [Insured data](https://docs.google.com/spreadsheets/d/1TPe8oWDJKzcxYNGYiW_pALYlnM9EHWLWthiStBzAh_g/view#gid=171077237)
* [Agency/Producer data](https://docs.google.com/spreadsheets/d/1TPe8oWDJKzcxYNGYiW_pALYlnM9EHWLWthiStBzAh_g/view#gid=788011577)
* for Locations and WC specific information see [ACORD XML Mapping - Workers' Compensation v2.2.0](https://docs.google.com/spreadsheets/d/1sfQJ8P6M7A_XA10c41b6GOTbnaE-TbJUZ8rzcpzu9i0) (sheets for ACORDs 125, 130)

Response XML documentation: [Response XML Mapping v2.2.0](https://docs.google.com/spreadsheets/d/1G_V9ggaR1jcuTEjIOECetucrjY9ZyCzSJxgronn0roU)

*Response XML Statuses - StatusCd, MsgStatusCd values
| Case#     | Sub-case              | Controller                                                                                                                                                                       | Request XML sample                                                                                                       | Response XML Statuses | Response XML structure sample                                             |
| --------- | --------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------ | --------------------- | ------------------------------------------------------------------------- |
| **Case1** | Save data to txt file | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Sources/AcordXmlServiceSample/AcordXmlServiceSample/Controllers/Case1ReadSaveRequestDataController.cs) | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Testing/case1/case1.xml)                       | 0, Success            | [file](https://drive.google.com/file/d/1xmVmtTlvZJuEtpgh1fpQs-Ql6kvmFhie) |
| **Case2** | Not all required data | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Sources/AcordXmlServiceSample/AcordXmlServiceSample/Controllers/Case2ValidateRequestDataController.cs) | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Testing/case2/case2_error.xml)                 | 0, Error              | [file](https://drive.google.com/file/d/1h7G1z7Z8DPppU8dNXzwx4rSyr2s8-Xu5) |
| **Case2** | Unsupported state     | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Sources/AcordXmlServiceSample/AcordXmlServiceSample/Controllers/Case2ValidateRequestDataController.cs) | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Testing/case2/case2_reject.xml)                | 0, Rejected           | [file](https://drive.google.com/file/d/1smwtRlXyoLkcGj6INnHhFrrSGFnOZkYW) |
| **Case2** | Valid data set        | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Sources/AcordXmlServiceSample/AcordXmlServiceSample/Controllers/Case2ValidateRequestDataController.cs) | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Testing/case2/case2_success.xml)               | 0, Success            |                                                                           |
| **Case3** | Invalid credentials   | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Sources/AcordXmlServiceSample/AcordXmlServiceSample/Controllers/Case3ValidateCredentialsController.cs) | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Testing/case3/case3_incorrect_credentials.xml) | 1740, Error           | [file](https://drive.google.com/file/d/135n0kX01VlKzc3Om-A2kk5EGvhXHIcXa) |
| **Case3** | Empty credentials     | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Sources/AcordXmlServiceSample/AcordXmlServiceSample/Controllers/Case3ValidateCredentialsController.cs) | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Testing/case3/case3_without_credentials.xml)   | 1740, Error           | [file](https://drive.google.com/file/d/135n0kX01VlKzc3Om-A2kk5EGvhXHIcXa) |
| **Case3** | Valid credentials     | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Sources/AcordXmlServiceSample/AcordXmlServiceSample/Controllers/Case3ValidateCredentialsController.cs) | [file](https://github.com/appulate/appulate-acordxml-svc-sample/blob/main/Testing/case3/case3_correct_credentials.xml)   | 0, Success            |                                                                           |

#### How to test
You can use Swagger or [Postman](https://www.postman.com/downloads/) for test requests.  
Sample of postman collection is included in the Testing folder.
* If the application is launched over HTTPS, set the Current value of the base_url collection variable to "https:\\\\localhost:7226"
* If the application is launched over HTTP, set the Current value of the base_url collection variable to "http:\\\\localhost:5055" 

#### Built With
* ASP .NET Core Web API
* C#

#### Run application
Clone the repository
   ```sh
   git clone https://github.com/appulate/appulate-acordxml-svc-sample.git
   ```
You can use free [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/) (Windows) to run an app
1. Make sure ASP.NET and web development tools are installed
2. Open AcordXmlServiceSample solution in Visual Studio
3. Start the app without debugging (press Ctrl+F5 to run without the debugger).  
Swagger page will be opened automatically
