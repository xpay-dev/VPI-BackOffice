﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GatewayProcessor.VaultPosGateway {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://TPISoft.com/SmartPayments/", ConfigurationName="VaultPosGateway.SmartPaymentsSoap")]
    public interface SmartPaymentsSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessCreditCard", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response ProcessCreditCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Zip, string Street, string CVNum, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessCreditCard", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessCreditCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Zip, string Street, string CVNum, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessDebitCard", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response ProcessDebitCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Pin, string RegisterNum, string SureChargeAmt, string CashBackAmt, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessDebitCard", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessDebitCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Pin, string RegisterNum, string SureChargeAmt, string CashBackAmt, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessCash", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response ProcessCash(string UserName, string Password, string TransType, string Amount, string InvNum, string PNRef, string RegisterNum, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessCash", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessCashAsync(string UserName, string Password, string TransType, string Amount, string InvNum, string PNRef, string RegisterNum, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessEBTCard", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response ProcessEBTCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Pin, string RegisterNum, string SureChargeAmt, string CashBackAmt, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessEBTCard", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessEBTCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Pin, string RegisterNum, string SureChargeAmt, string CashBackAmt, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessLoyaltyCard", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response ProcessLoyaltyCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string Amount, string InvNum, string PNRef, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessLoyaltyCard", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessLoyaltyCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string Amount, string InvNum, string PNRef, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessGiftCard", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response ProcessGiftCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string Amount, string InvNum, string PNRef, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessGiftCard", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessGiftCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string Amount, string InvNum, string PNRef, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessCheck", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response ProcessCheck(string UserName, string Password, string TransType, string CheckNum, string TransitNum, string AccountNum, string Amount, string MICR, string NameOnCheck, string DL, string SS, string DOB, string StateCode, string CheckType, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessCheck", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessCheckAsync(string UserName, string Password, string TransType, string CheckNum, string TransitNum, string AccountNum, string Amount, string MICR, string NameOnCheck, string DL, string SS, string DOB, string StateCode, string CheckType, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/GetInfo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response GetInfo(string UserName, string Password, string TransType, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/GetInfo", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> GetInfoAsync(string UserName, string Password, string TransType, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessSignature", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        GatewayProcessor.VaultPosGateway.Response ProcessSignature(string UserName, string Password, string SignatureType, string SignatureData, string PNRef, string Result, string AuthCode, string ExtData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://TPISoft.com/SmartPayments/ProcessSignature", ReplyAction="*")]
        System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessSignatureAsync(string UserName, string Password, string SignatureType, string SignatureData, string PNRef, string Result, string AuthCode, string ExtData);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://TPISoft.com/SmartPayments/")]
    public partial class Response : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int resultField;
        
        private string respMSGField;
        
        private string messageField;
        
        private string message1Field;
        
        private string message2Field;
        
        private string authCodeField;
        
        private string pNRefField;
        
        private string hostCodeField;
        
        private string hostURLField;
        
        private string receiptURLField;
        
        private string getAVSResultField;
        
        private string getAVSResultTXTField;
        
        private string getStreetMatchTXTField;
        
        private string getZipMatchTXTField;
        
        private string getCVResultField;
        
        private string getCVResultTXTField;
        
        private string getGetOrigResultField;
        
        private string getCommercialCardField;
        
        private string workingKeyField;
        
        private string keyPointerField;
        
        private string extDataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public int Result {
            get {
                return this.resultField;
            }
            set {
                this.resultField = value;
                this.RaisePropertyChanged("Result");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string RespMSG {
            get {
                return this.respMSGField;
            }
            set {
                this.respMSGField = value;
                this.RaisePropertyChanged("RespMSG");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
                this.RaisePropertyChanged("Message");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string Message1 {
            get {
                return this.message1Field;
            }
            set {
                this.message1Field = value;
                this.RaisePropertyChanged("Message1");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string Message2 {
            get {
                return this.message2Field;
            }
            set {
                this.message2Field = value;
                this.RaisePropertyChanged("Message2");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string AuthCode {
            get {
                return this.authCodeField;
            }
            set {
                this.authCodeField = value;
                this.RaisePropertyChanged("AuthCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string PNRef {
            get {
                return this.pNRefField;
            }
            set {
                this.pNRefField = value;
                this.RaisePropertyChanged("PNRef");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public string HostCode {
            get {
                return this.hostCodeField;
            }
            set {
                this.hostCodeField = value;
                this.RaisePropertyChanged("HostCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string HostURL {
            get {
                return this.hostURLField;
            }
            set {
                this.hostURLField = value;
                this.RaisePropertyChanged("HostURL");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public string ReceiptURL {
            get {
                return this.receiptURLField;
            }
            set {
                this.receiptURLField = value;
                this.RaisePropertyChanged("ReceiptURL");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=10)]
        public string GetAVSResult {
            get {
                return this.getAVSResultField;
            }
            set {
                this.getAVSResultField = value;
                this.RaisePropertyChanged("GetAVSResult");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=11)]
        public string GetAVSResultTXT {
            get {
                return this.getAVSResultTXTField;
            }
            set {
                this.getAVSResultTXTField = value;
                this.RaisePropertyChanged("GetAVSResultTXT");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=12)]
        public string GetStreetMatchTXT {
            get {
                return this.getStreetMatchTXTField;
            }
            set {
                this.getStreetMatchTXTField = value;
                this.RaisePropertyChanged("GetStreetMatchTXT");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=13)]
        public string GetZipMatchTXT {
            get {
                return this.getZipMatchTXTField;
            }
            set {
                this.getZipMatchTXTField = value;
                this.RaisePropertyChanged("GetZipMatchTXT");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=14)]
        public string GetCVResult {
            get {
                return this.getCVResultField;
            }
            set {
                this.getCVResultField = value;
                this.RaisePropertyChanged("GetCVResult");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=15)]
        public string GetCVResultTXT {
            get {
                return this.getCVResultTXTField;
            }
            set {
                this.getCVResultTXTField = value;
                this.RaisePropertyChanged("GetCVResultTXT");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=16)]
        public string GetGetOrigResult {
            get {
                return this.getGetOrigResultField;
            }
            set {
                this.getGetOrigResultField = value;
                this.RaisePropertyChanged("GetGetOrigResult");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=17)]
        public string GetCommercialCard {
            get {
                return this.getCommercialCardField;
            }
            set {
                this.getCommercialCardField = value;
                this.RaisePropertyChanged("GetCommercialCard");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=18)]
        public string WorkingKey {
            get {
                return this.workingKeyField;
            }
            set {
                this.workingKeyField = value;
                this.RaisePropertyChanged("WorkingKey");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=19)]
        public string KeyPointer {
            get {
                return this.keyPointerField;
            }
            set {
                this.keyPointerField = value;
                this.RaisePropertyChanged("KeyPointer");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=20)]
        public string ExtData {
            get {
                return this.extDataField;
            }
            set {
                this.extDataField = value;
                this.RaisePropertyChanged("ExtData");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SmartPaymentsSoapChannel : GatewayProcessor.VaultPosGateway.SmartPaymentsSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SmartPaymentsSoapClient : System.ServiceModel.ClientBase<GatewayProcessor.VaultPosGateway.SmartPaymentsSoap>, GatewayProcessor.VaultPosGateway.SmartPaymentsSoap {
        
        public SmartPaymentsSoapClient() {
        }
        
        public SmartPaymentsSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SmartPaymentsSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SmartPaymentsSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SmartPaymentsSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public GatewayProcessor.VaultPosGateway.Response ProcessCreditCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Zip, string Street, string CVNum, string ExtData) {
            return base.Channel.ProcessCreditCard(UserName, Password, TransType, CardNum, ExpDate, MagData, NameOnCard, Amount, InvNum, PNRef, Zip, Street, CVNum, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessCreditCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Zip, string Street, string CVNum, string ExtData) {
            return base.Channel.ProcessCreditCardAsync(UserName, Password, TransType, CardNum, ExpDate, MagData, NameOnCard, Amount, InvNum, PNRef, Zip, Street, CVNum, ExtData);
        }
        
        public GatewayProcessor.VaultPosGateway.Response ProcessDebitCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Pin, string RegisterNum, string SureChargeAmt, string CashBackAmt, string ExtData) {
            return base.Channel.ProcessDebitCard(UserName, Password, TransType, CardNum, ExpDate, MagData, NameOnCard, Amount, InvNum, PNRef, Pin, RegisterNum, SureChargeAmt, CashBackAmt, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessDebitCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Pin, string RegisterNum, string SureChargeAmt, string CashBackAmt, string ExtData) {
            return base.Channel.ProcessDebitCardAsync(UserName, Password, TransType, CardNum, ExpDate, MagData, NameOnCard, Amount, InvNum, PNRef, Pin, RegisterNum, SureChargeAmt, CashBackAmt, ExtData);
        }
        
        public GatewayProcessor.VaultPosGateway.Response ProcessCash(string UserName, string Password, string TransType, string Amount, string InvNum, string PNRef, string RegisterNum, string ExtData) {
            return base.Channel.ProcessCash(UserName, Password, TransType, Amount, InvNum, PNRef, RegisterNum, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessCashAsync(string UserName, string Password, string TransType, string Amount, string InvNum, string PNRef, string RegisterNum, string ExtData) {
            return base.Channel.ProcessCashAsync(UserName, Password, TransType, Amount, InvNum, PNRef, RegisterNum, ExtData);
        }
        
        public GatewayProcessor.VaultPosGateway.Response ProcessEBTCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Pin, string RegisterNum, string SureChargeAmt, string CashBackAmt, string ExtData) {
            return base.Channel.ProcessEBTCard(UserName, Password, TransType, CardNum, ExpDate, MagData, NameOnCard, Amount, InvNum, PNRef, Pin, RegisterNum, SureChargeAmt, CashBackAmt, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessEBTCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string NameOnCard, string Amount, string InvNum, string PNRef, string Pin, string RegisterNum, string SureChargeAmt, string CashBackAmt, string ExtData) {
            return base.Channel.ProcessEBTCardAsync(UserName, Password, TransType, CardNum, ExpDate, MagData, NameOnCard, Amount, InvNum, PNRef, Pin, RegisterNum, SureChargeAmt, CashBackAmt, ExtData);
        }
        
        public GatewayProcessor.VaultPosGateway.Response ProcessLoyaltyCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string Amount, string InvNum, string PNRef, string ExtData) {
            return base.Channel.ProcessLoyaltyCard(UserName, Password, TransType, CardNum, ExpDate, MagData, Amount, InvNum, PNRef, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessLoyaltyCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string Amount, string InvNum, string PNRef, string ExtData) {
            return base.Channel.ProcessLoyaltyCardAsync(UserName, Password, TransType, CardNum, ExpDate, MagData, Amount, InvNum, PNRef, ExtData);
        }
        
        public GatewayProcessor.VaultPosGateway.Response ProcessGiftCard(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string Amount, string InvNum, string PNRef, string ExtData) {
            return base.Channel.ProcessGiftCard(UserName, Password, TransType, CardNum, ExpDate, MagData, Amount, InvNum, PNRef, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessGiftCardAsync(string UserName, string Password, string TransType, string CardNum, string ExpDate, string MagData, string Amount, string InvNum, string PNRef, string ExtData) {
            return base.Channel.ProcessGiftCardAsync(UserName, Password, TransType, CardNum, ExpDate, MagData, Amount, InvNum, PNRef, ExtData);
        }
        
        public GatewayProcessor.VaultPosGateway.Response ProcessCheck(string UserName, string Password, string TransType, string CheckNum, string TransitNum, string AccountNum, string Amount, string MICR, string NameOnCheck, string DL, string SS, string DOB, string StateCode, string CheckType, string ExtData) {
            return base.Channel.ProcessCheck(UserName, Password, TransType, CheckNum, TransitNum, AccountNum, Amount, MICR, NameOnCheck, DL, SS, DOB, StateCode, CheckType, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessCheckAsync(string UserName, string Password, string TransType, string CheckNum, string TransitNum, string AccountNum, string Amount, string MICR, string NameOnCheck, string DL, string SS, string DOB, string StateCode, string CheckType, string ExtData) {
            return base.Channel.ProcessCheckAsync(UserName, Password, TransType, CheckNum, TransitNum, AccountNum, Amount, MICR, NameOnCheck, DL, SS, DOB, StateCode, CheckType, ExtData);
        }
        
        public GatewayProcessor.VaultPosGateway.Response GetInfo(string UserName, string Password, string TransType, string ExtData) {
            return base.Channel.GetInfo(UserName, Password, TransType, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> GetInfoAsync(string UserName, string Password, string TransType, string ExtData) {
            return base.Channel.GetInfoAsync(UserName, Password, TransType, ExtData);
        }
        
        public GatewayProcessor.VaultPosGateway.Response ProcessSignature(string UserName, string Password, string SignatureType, string SignatureData, string PNRef, string Result, string AuthCode, string ExtData) {
            return base.Channel.ProcessSignature(UserName, Password, SignatureType, SignatureData, PNRef, Result, AuthCode, ExtData);
        }
        
        public System.Threading.Tasks.Task<GatewayProcessor.VaultPosGateway.Response> ProcessSignatureAsync(string UserName, string Password, string SignatureType, string SignatureData, string PNRef, string Result, string AuthCode, string ExtData) {
            return base.Channel.ProcessSignatureAsync(UserName, Password, SignatureType, SignatureData, PNRef, Result, AuthCode, ExtData);
        }
    }
}
