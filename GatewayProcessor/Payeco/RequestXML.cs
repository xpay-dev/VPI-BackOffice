using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatewayProcessor.Payeco
{
    public sealed class RequestXML
    {
        public static string GenerateXML(TransactionData trans, string MAC)
        {
            StringBuilder sb = new StringBuilder();

            string srcXml = string.Empty;
                 
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>").Append("\n")
            .Append("<x:NetworkRequest xmlns:x=\"http://www.payeco.com\" xmlns:xsi=\"http://www.w3.org\">").Append("\n\t")
            .Append("<Version>").Append(trans.Version).Append("</Version>").Append("\n\t")
            .Append("<ProcCode>").Append(trans.ProcCode).Append("</ProcCode>").Append("\n\t")
            .Append("<ProcessCode>").Append(trans.ProcessCode).Append("</ProcessCode>").Append("\n\t")
            .Append("<AccountNo>").Append(trans.Card).Append("</AccountNo>").Append("\n\t")
            .Append("<AccountType>").Append("").Append("</AccountType>").Append("\n\t")
            .Append("<MobileNo>").Append(trans.Phone).Append("</MobileNo>").Append("\n\t")
            .Append("<Amount>").Append(trans.Amount).Append("</Amount>").Append("\n\t")
            .Append("<Currency>").Append(trans.Currency).Append("</Currency>").Append("\n\t")
            .Append("<SynAddress>").Append(trans.SynAddress).Append("</SynAddress>").Append("\n\t")
            .Append("<AsynAddress>").Append(trans.AsynAddress).Append("</AsynAddress>").Append("\n\t")
            .Append("<Remark>").Append(trans.Remark).Append("</Remark>").Append("\n\t")
            .Append("<TerminalNo>").Append(trans.TerminalId).Append("</TerminalNo>").Append("\n\t")
            .Append("<MerchantNo>").Append(trans.MerchantId).Append("</MerchantNo>").Append("\n\t")
            .Append("<MerchantOrderNo>").Append(trans.MerchantOrderNo).Append("</MerchantOrderNo>").Append("\n\t")
            .Append("<OrderNo>").Append(trans.OrderNo).Append("</OrderNo>").Append("\n\t")
            .Append("<OrderFrom>").Append("02").Append("</OrderFrom>").Append("\n\t")
            .Append("<Language>").Append("02").Append("</Language>").Append("\n\t")
            .Append("<Description>").Append(trans.Description).Append("</Description>").Append("\n\t")
            .Append("<OrderType>").Append("00").Append("</OrderType>").Append("\n\t")
            .Append("<AcqSsn>").Append(trans.AcqSsn).Append("</AcqSsn>").Append("\n\t")
            .Append("<Reference>").Append(trans.Reference).Append("</Reference>").Append("\n\t")
            .Append("<TransDatetime>").Append(trans.Datetime).Append("</TransDatetime>").Append("\n\t")
            .Append("<MerchantName>").Append(trans.MerchantName).Append("</MerchantName>").Append("\n\t")
            .Append("<TransData>").Append(trans.TransData).Append("</TransData>").Append("\n\t")
            .Append("<IDCardName>").Append(trans.Name).Append("</IDCardName>").Append("\n\t")
            .Append("<IDCardNo>").Append(trans.IdCard).Append("</IDCardNo>").Append("\n\t")
            .Append("<BankAddress>").Append(trans.BankAddress).Append("</BankAddress>").Append("\n\t")
            .Append("<BankName>").Append(trans.BankName).Append("</BankName>").Append("\n\t")
            .Append("<IDCardType>").Append(trans.IdCardType).Append("</IDCardType>").Append("\n\t")
            .Append("<BeneficiaryName>").Append(trans.Beneficiary).Append("</BeneficiaryName>").Append("\n\t")
            .Append("<BeneficiaryMobileNo>").Append(trans.Phone).Append("</BeneficiaryMobileNo>").Append("\n\t")
            .Append("<DeliveryAddress>").Append(trans.DeliveryAddress).Append("</DeliveryAddress>").Append("\n\t")
            .Append("<IpAddress>").Append(trans.IpAddress).Append("</IpAddress>").Append("\n\t")
            .Append("<Location>").Append("").Append("</Location>").Append("\n\t")
            .Append("<UserFlag>").Append("").Append("</UserFlag>").Append("\n\t")
            .Append("<MAC>").Append(MAC).Append("</MAC>").Append("\n")
            .Append("</x:NetworkRequest>").Append("\n");

            srcXml = sb.ToString();

            return srcXml;
        }
    }
}
