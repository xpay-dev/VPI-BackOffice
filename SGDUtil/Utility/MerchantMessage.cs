using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SDGUtil.Utility
{
    public class MerchantMessage
    {
        private static long serialVersionUID = 8769203519691723123L;
        private String version = "";//版本号
        private String procCode = "";//消息类型
        private String processCode = "";//处理码
        private String accountNo = "";//卡号
        private String amount = "";//金额
        private String currency = "";//币种

        private String remark = "";//备注
        private String terminalNo = "";//终端号
        private String merchantNo = "";//商户号
        private String merchantOrderNo = "";//商户订单号
        private String orderFrom = "";//订单来源
        private String language = "";//语种
        private String description = "";//订单描述
        private String orderType = "";//下单类型
        private String acqSsn = "";//系统跟踪号
        private String reference = "";//系统参考
        private String transDatetime = "";//传输时间
        private String uiLanguage = "";//UI语言
        private String transData = "";//其他数据
        private String synAddress = "";//同步地址
        private String asynAddress = "";//异步地址

        private String respCode = "";//响应码
        private String orderState = "";//订单状态
        private String upsNo = "";//易联流水号
        private String tsNo = "";//易联终端流水号
        private String orderNo = "";//下单返回的订单号

        private String mac = "";//校验码

        public String getVersion()
        {
            return version;
        }

        public void setVersion(String version)
        {
            if (validLength(version, "Version", 0, 99))
            {
                this.version = version;
            }
        }
        public String getProcCode()
        {
            return procCode;
        }

        public void setProcCode(String procCode)
        {
            if (validLength(procCode, "ProcCode", 4, 4))
            {
                this.procCode = procCode;
            }
        }
        public String getProcessCode()
        {
            return processCode;
        }

        public void setProcessCode(String processCode)
        {
            if (validLength(processCode, "ProcessCode", 6, 6))
            {
                this.processCode = processCode;
            }
        }

        public String getAccountNo()
        {
            return accountNo;
        }

        public void setAccountNo(String accountNo)
        {
            if (validLength(accountNo, "AccountNo", 0, 99))
            {
                this.accountNo = accountNo;
            }
        }

        public String getAmount()
        {
            return amount;
        }

        public void setAmount(String amount)
        {
            if (validLength(amount, "Amount", 0, 12))
            {
                this.amount = amount;
            }
        }
        public String getCurrency()
        {
            return currency;
        }

        public void setCurrency(String currency)
        {
            if (validLength(currency, "Currency", 3, 3))
            {
                this.currency = currency;
            }
        }
        public String getCurCode()
        {
            return currency;
        }
        public String getSynAddress()
        {
            return synAddress;
        }

        public void setSynAddress(String synAddress)
        {
            if (validLength(synAddress, "SynAddress", 0, 99))
            {
                this.synAddress = synAddress;
            }
        }
        public String getAsynAddress()
        {
            return asynAddress;
        }

        public void setAsynAddress(String asynAddress)
        {
            if (validLength(synAddress, "SynAddress", 0, 99))
            {
                this.asynAddress = asynAddress;
            }
        }
        public String getRemark()
        {
            return remark;
        }

        public void setRemark(String remark)
        {
            if (validLength(remark, "Remark", 0, 99))
            {
                this.remark = remark;
            }
        }
        public String getTerminalNo()
        {
            return terminalNo;
        }

        public void setTerminalNo(String terminalNo)
        {
            if (validLength(terminalNo, "TerminalNo", 0, 99))
            {
                this.terminalNo = terminalNo;
            }
        }
        public String getMerchantNo()
        {
            return merchantNo;
        }

        public void setMerchantNo(String merchantNo)
        {
            if (validLength(merchantNo, "MerchantNo", 0, 99))
            {
                this.merchantNo = merchantNo;
            }
        }
        public String getMerchantOrderNo()
        {
            return merchantOrderNo;
        }

        public void setMerchantOrderNo(String merchantOrderNo)
        {
            if (validLength(merchantOrderNo, "MerchantOrderNo", 0, 99))
            {
                this.merchantOrderNo = merchantOrderNo;
            }
        }
        public String getOrderFrom()
        {
            return orderFrom;
        }

        public void setOrderFrom(String orderFrom)
        {
            if (validLength(orderFrom, "OrderFrom", 2, 2))
            {
                this.orderFrom = orderFrom;
            }
        }
        public String getLanguage()
        {
            return language;
        }

        public void setLanguage(String language)
        {
            if (validLength(language, "Language", 2, 2))
            {
                this.language = language;
            }
        }
        public String getDescription()
        {
            return description;
        }

        public void setDescription(String description)
        {
            this.description = description;
        }
        public String getOrderType()
        {
            return orderType;
        }

        public void setOrderType(String orderType)
        {
            if (validLength(orderType, "OrderType", 2, 2))
            {
                this.orderType = orderType;
            }
        }
        public String getAcqSsn()
        {
            return acqSsn;
        }

        public void setAcqSsn(String acqSsn)
        {
            if (validLength(acqSsn, "AcqSsn", 6, 6))
            {
                this.acqSsn = acqSsn;
            }
        }
        public String getReference()
        {
            return reference;
        }

        public void setReference(String reference)
        {
            if (validLength(reference, "Reference", 0, 99))
            {
                this.reference = reference;
            }
        }
        public String getTransDatetime()
        {
            return transDatetime;
        }

        public void setTransDatetime(String transDatetime)
        {
            //if(validLength(transDatetime, "TransDatetime", 10, 10)){
            this.transDatetime = transDatetime;
            //}
        }
        public String getMac()
        {
            return mac;
        }

        public void setMac(String mac)
        {
            if (validLength(mac, "Mac", 32, 32))
            {
                this.mac = mac;
            }
        }

        public String computeMac(String pw)
        {
            string data = getMacString() + " " + pw;

            StringBuilder hashOutput = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytesHash = md5provider.ComputeHash(new UTF8Encoding().GetBytes(data));

            for (int i = 0; i < bytesHash.Length; i++)
            {
                hashOutput.Append(bytesHash[i].ToString("x2"));
            }

            return hashOutput.ToString();
        }

        public String getTransData()
        {
            return transData;
        }

        public void setTransData(String transData)
        {
            if (validLength(transData, "TransData", 0, 999))
            {
                this.transData = transData;
            }
        }

        public String getRespCode()
        {
            return this.respCode;
        }
        public void setRespCode(String respCode)
        {
            if (validLength(respCode, "RespCode", 4, 4))
            {
                this.respCode = respCode;
            }
        }
        public String getOrderState()
        {
            return orderState;
        }
        public void setOrderState(String orderState)
        {
            if (validLength(orderState, "OrderState", 2, 2))
            {
                this.orderState = orderState;
            }
        }
        public String getUpsNo()
        {
            return upsNo;
        }
        public void setUpsNo(String upsNo)
        {
            this.upsNo = upsNo;
        }
        public String getTsNo()
        {
            return tsNo;
        }
        public void setTsNo(String tsNo)
        {
            this.tsNo = tsNo;
        }

        private bool validLength(String src, String name, int start, int end)
        {
            if (!isNullOrEmpty(src))
            {
                if (src.Length < start || src.Length > end)
                {
                    return false;
                }
            }
            return true;
        }

        public String getOrderNo()
        {
            return orderNo;
        }
        public void setOrderNo(String orderNo)
        {
            this.orderNo = orderNo;
        }

        public bool verifyMac(String pass)
        {
            return getMac().Equals(computeMac(pass));
        }

        private bool isNullOrEmpty(String src)
        {
            return src == null || "".Equals(src.Trim());
        }

        public String getMacString()
        {
            String src = "";
            src = getProcCode()
                    + getString(getAccountNo())
                    + getString(getProcessCode())
                    + getString(getAmount())
                    + getString(getTransDatetime())
                    + getString(getAcqSsn())
                    + getString(getOrderNo())
                    + getString(getTransData())
                    + getString(getReference())
                    + getString(getRespCode())
                    + getString(getTerminalNo())
                    + getString(getMerchantNo())
                    + getString(getMerchantOrderNo())
                    + getString(getOrderState());
            return src;
        }

        public String getString(String src)
        {
            return (String.IsNullOrEmpty(src) ? "" : (" " + src.Trim()));
        }

        public MerchantMessage() { }

        public String getUiLanguage()
        {
            return uiLanguage;
        }

        public void setUiLanguage(String uiLanguage)
        {
            this.uiLanguage = uiLanguage;
        }

        public String ToXML()
        {
            string srcXml = string.Empty;
            XNamespace ns1 = "http://www.payeco.com";
            XNamespace ns2 = "http://www.w3.org";
            string prefix1 = "x";
            string prefix2 = "xsi";

            var xmlRequest = new XDocument(new XDeclaration("1.0", "UTF-8", ""),
            new XElement("NetworkRequest", new XAttribute(XNamespace.Xmlns + prefix1, ns1), new XAttribute(XNamespace.Xmlns + prefix2, ns2),
            new XElement("Version", getVersion()),
            new XElement("ProcCode", getProcCode()),
            new XElement("ProcessCode", getProcessCode()),
            new XElement("AccountNo", getAccountNo()),
            new XElement("Amount", getAmount()),
            new XElement("Currency", getCurrency()),
            new XElement("TerminalNo", getTerminalNo()),
            new XElement("MerchantNo", getMerchantNo()),
            new XElement("MerchantOrderNo", getMerchantOrderNo()),
            new XElement("OrderNo", getOrderNo()),
            new XElement("UILanguage", getLanguage()),
            new XElement("Description", getDescription()),
            new XElement("AcqSsn", getAcqSsn()),
            new XElement("TransDatetime", getTransDatetime()),
            new XElement("TransData", getTransData()),
            new XElement("Reference", getReference()),
            new XElement("Remark", getRemark()),
            new XElement("SynAddress", getSynAddress()),
            new XElement("AsynAddress", getAsynAddress()),
            new XElement("RespCode", getRespCode()),
            new XElement("OrderState", getOrderState()),
            new XElement("UpsNo", getUpsNo()),
            new XElement("TsNo", getTsNo()),
            new XElement("TsNo", getTsNo()),
            new XElement("Mac", getMac())));

            return xmlRequest.ToString();
        }

        private void createElement(XmlDocument doc, XmlElement root, String value, String tag)
        {
            if (!String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(tag))
            {
                XmlElement element = doc.CreateElement(tag);
                element.AppendChild(doc.CreateTextNode(value));
                root.AppendChild(element);
            }
        }

        //public static String getStringFromDocument(XmlDocument doc)
        //{
        //    String result = null;

        //    if (doc != null)
        //    {
        //        try
        //        {
        //            StringWriter writer = new StringWriter();
        //            StreamResult strResult = new StreamResult();

        //            StreamResult result = new StreamResult(writer);
        //            TransformerFactory tFactory = TransformerFactory.newInstance();
        //            Transformer transformer = tFactory.newTransformer();
        //            transformer.transform(source, result);
        //            String strResult = writer.toString();
        //        }
        //        catch (Exception e)
        //        {
        //            e.printStackTrace();
        //        }

        //        result = strResult.getWriter().toString();
        //    }

        //    return result;
        //}
    }
}
