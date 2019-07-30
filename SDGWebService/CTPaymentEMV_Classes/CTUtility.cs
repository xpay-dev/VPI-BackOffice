namespace SDGWebService.CTPaymentEMV_Classes
{
    public class CTUtility
    {
        private string _messageClassHost = "HH";
        private string _posResCode = "999";
        private string _posStatusIndicator = "000";
        private string _messageVersion = "04";
        private string _posEntryMode = "00";
        private string _operatorId = "   ";
        private string _transactionType = "097";

        public string MessageClassHost
        {
            get { return this._messageClassHost; }
            set { this._messageClassHost = value; }
        }

        public string PosResCode
        {
            get { return this._posResCode; }
            set { this._posResCode = value; }
        }

        public string PosStatusIndicator
        {
            get { return this._posStatusIndicator; }
            set { this._posStatusIndicator = value; }
        }

        public string MessageVersion
        {
            get { return this._messageVersion; }
            set { this._messageVersion = value; }
        }

        public string PosEntryMode
        {
            get { return this._posEntryMode; }
            set { this._posEntryMode = value; }
        }

        public string OperatorId
        {
            get { return this._operatorId; }
            set { this._operatorId = value; }
        }

        public string TransactionType
        {
            get { return this._transactionType; }
            set { this._transactionType = value; }
        }
    }
}