using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGWebService.TLVFunctions
{
    public class TLVTags
    {
        private string _track2 = "57";
        private string _cardNumber = "5a";
        private string _appCryptogram = "9f26";
        private string _appProfile = "82";
        private string _atc = "9f36";
        private string _cia = "9f27";
        private string _issuerData = "9f10";
        private string _terminalCountryCode = "9f1a";
        private string _tvr = "95";
        private string _currencyCode = "5f2a";
        private string _transactionDate = "9a";
        private string _amount = "9f02";
        private string _amountCashback = "9f03";
        private string _transactionType = "9c";
        private string _unpredictableNumber = "9f37";
        private string _panSeqNumber = "5f34";
        private string _appVersion = "9f09";
        private string _cvm = "9f34";
        private string _serialNumber = "9f1e";
        private string _terminalType = "9f35";
        private string _transSeqCounter = "9f41";
        private string _dedicatedFilename = "84";
        private string _posEntryMode = "9f39";
        private string _discretionaryData = "9f1f";
        private string _serviceCode = "5f30";
        private string _expiryDate = "5f24";
        private string _nameOnCard = "5f20";

        public string Track2
        {
            get { return this._track2; }
            set { this._track2 = value; }
        }

        public string CardNumber
        {
            get { return this._cardNumber; }
            set { this._cardNumber = value; }
        }

        public string ApplicationCryptogram
        {
            get { return this._appCryptogram; }
            set { this._appCryptogram = value; }
        }

        public string ApplicationProfile
        {
            get { return this._appProfile; }
            set { this._appProfile = value; }
        }

        public string AppTransactionCounter
        {
            get { return this._atc; }
            set { this._atc = value; }
        }

        public string CryptogramInfoData
        {
            get { return this._cia; }
            set { this._cia = value; }
        }

        public string IssuerAppData
        {
            get { return this._issuerData; }
            set { this._issuerData = value; }
        }

        public string TerminalCountryCode
        {
            get { return this._terminalCountryCode; }
            set { this._terminalCountryCode = value; }
        }

        public string TerminalVerificationResult
        {
            get { return this._tvr; }
            set { this._tvr = value; }
        }

        public string CurrencyCode
        {
            get { return this._currencyCode; }
            set { this._currencyCode = value; }
        }

        public string TransactionDate
        {
            get { return this._transactionDate; }
            set { this._transactionDate = value; }
        }

        public string Amount
        {
            get { return this._amount; }
            set { this._amount = value; }
        }


        public string AmountCashback
        {
            get { return this._amountCashback; }
            set { this._amountCashback = value; }
        }

        public string TransactionType
        {
            get { return this._transactionType; }
            set { this._transactionType = value; }
        }

        public string UnpredictableNumber
        {
            get { return this._unpredictableNumber; }
            set { this._unpredictableNumber = value; }
        }

        public string PanSeqNumber  
        {
            get { return this._panSeqNumber; }
            set { this._panSeqNumber = value; }
        }

        public string AppVersion
        {
            get { return this._appVersion; }
            set { this._appVersion = value; }
        }

        public string CardVerificationMethod
        {
            get { return this._cvm; }
            set { this._cvm = value; }
        }

        public string SerialNumber
        {
            get { return this._serialNumber; }
            set { this._serialNumber = value; }
        }

        public string TerminalType
        {
            get { return this._terminalType; }
            set { this._terminalType = value; }
        }

        public string TransactionSeqCounter
        {
            get { return this._transSeqCounter; }
            set { this._transSeqCounter = value; }
        }

        public string DedicatedFileName
        {
            get { return this._dedicatedFilename; }
            set { this._dedicatedFilename = value; }
        }

        public string PosEntryMode 
        {
            get { return this._posEntryMode; }
            set { this._posEntryMode = value; }
        }

        public string ExpiryDate
        {
            get { return this._expiryDate; }
            set {this._expiryDate = value;}
        }

        public string ServiceCode
        {
            get { return this._serviceCode; }
            set { this._serviceCode = value; }
        }

        public string DiscretionaryData
        {
            get { return this._discretionaryData; }
            set { this._discretionaryData = value; }
        }

        public string NameOnCard
        {
            get { return this._nameOnCard; }
            set { this._nameOnCard = value; }
        }

    }
}
