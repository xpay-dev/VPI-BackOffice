using SDGDAL;
using SDGDAL.Repositories;
using SDGUtil;
using SDGWebService.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDGWebService.WebserviceFunctions
{
    public class WebserviceFunctionsCash
    {
        private MobileAppRepository _mAppRepo = new MobileAppRepository();
        private TransactionRepository _transRepo = new TransactionRepository();
        private MidsRepository _midsRepo = new MidsRepository();
        private UserRepository _userRepo = new UserRepository();

        private Functions.MobileAppFunctions mobileAppFunctions = new Functions.MobileAppFunctions();

        public PurchaseResponse TransactionPurchaseCash(CashTransactionRequest request)
        {
            string action = string.Empty;
            PurchaseResponse response = new PurchaseResponse();
            response.POSWSResponse = new POSWSResponse();

            object obj = response.POSWSResponse;
            if (mobileAppFunctions.CheckDetails(Functions.Enums.METHODS.TRANSACTION_PURCHASE_CASH, (object)request, out obj))
            {
                response.POSWSResponse = (POSWSResponse)obj;
                return response;
            }

            response.POSWSResponse.ErrNumber = "0";

            try
            {
                if (request.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();

                    action = "checking mobile app availability.";
                    response.POSWSResponse = mobileAppFunctions.CheckStatus(request.POSWSRequest.RToken, out mobileApp, out account);

                    if (response.POSWSResponse.Status == "Declined")
                    {
                        return response;
                    }

                    action = "logging mobile app action.";
                    MobileAppMethods mobileAppMethods = new MobileAppMethods();
                    mobileAppFunctions.LogMobileAppAction("Purchase Cash Transaction", mobileApp.MobileAppId, account.AccountId, request.POSWSRequest.GPSLat, request.POSWSRequest.GPSLong);

                    if (!mobileApp.MobileAppFeatures.CashTransaction)
                    {
                        response.POSWSResponse.ErrNumber = "2900.1";
                        response.POSWSResponse.Message = "Cash transactions are currently disabled on this device";
                        response.POSWSResponse.Status = "Declined";
                        return response;
                    }

                    action = "checking mid status before setting up transaction entry.";
                    var mid = new SDGDAL.Entities.Mid();

                    mid = _midsRepo.GetMidByPosIdAndCardTypeId(mobileApp.MerchantBranchPOSId, Convert.ToInt32(Enums.CardTypes.Cash));

                    if (mid == null)
                    {
                        response.POSWSResponse.ErrNumber = "2900.2";
                        response.POSWSResponse.Message = "Mid not found.";
                        response.POSWSResponse.Status = "Declined";

                        return response;
                    }
                    else
                    {
                        if (!mid.IsActive || mid.IsDeleted)
                        {
                            response.POSWSResponse.ErrNumber = "2900.3";
                            response.POSWSResponse.Message = "Mid is Inactive.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }

                        if (!mid.Switch.IsActive)
                        {
                            response.POSWSResponse.ErrNumber = "2900.3";
                            response.POSWSResponse.Message = "Switch is Inactive.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }
                    }

                    action = "getting transaction  and transactionattempt cash details.";
                    var transactionCash = new SDGDAL.Entities.TransactionCash();
                    var transactionAttemptCash = new SDGDAL.Entities.TransactionAttemptCash();

                    try
                    {
                        transactionCash.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(request.Currency);
                    }
                    catch
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid currency.";
                        response.POSWSResponse.ErrNumber = "2900.4";
                        response.TransactionNumber = "0";
                        return response;
                    }

                    transactionCash.MerchantPOSId = mobileApp.MerchantBranchPOSId;
                    transactionCash.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.Cash);
                    transactionCash.Notes = string.Empty;
                    transactionCash.RefNumSales = request.RefNumberSale;
                    transactionCash.RefNumApp = request.RefNumberApp;
                    transactionCash.OriginalAmount = request.Amount;
                    transactionCash.TaxAmount = 0;
                    transactionCash.MidId = mid.MidId;
                    transactionCash.FinalAmount = request.Amount;
                    transactionCash.DateCreated = DateTime.Now;

                    transactionAttemptCash.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale);
                    transactionAttemptCash.AccountId = account.AccountId;
                    transactionAttemptCash.MobileAppId = mobileApp.MobileAppId;
                    transactionAttemptCash.GPSLat = request.POSWSRequest.GPSLat;
                    transactionAttemptCash.GPSLong = request.POSWSRequest.GPSLong;
                    transactionAttemptCash.Amount = transactionCash.OriginalAmount;
                    transactionAttemptCash.TransactionChargesId = mid.TransactionChargesId;
                    transactionAttemptCash.Notes = string.Empty;
                    transactionAttemptCash.DateSent = DateTime.Now;
                    transactionAttemptCash.DateReceived = Convert.ToDateTime("1900/01/01");
                    transactionAttemptCash.DepositDate = Convert.ToDateTime("1900/01/01");

                    action = "saving transaction details to database.";
                    var nTransactionCash = new SDGDAL.Entities.TransactionCash();
                    nTransactionCash.CopyProperties(transactionCash);

                    var savedTransactionCash = _transRepo.CreateCashTransaction(nTransactionCash, transactionAttemptCash);

                    if (savedTransactionCash.TransactionCashId > 0)
                    {
                        action = "processing transaction for api integration. Transaction was successfully saved.";

                        #region Offline Switch
                        if (mid.Switch.SwitchCode == "Offline")
                        {
                            transactionAttemptCash.TransactionAttemptCashId = transactionAttemptCash.TransactionAttemptCashId;
                            transactionAttemptCash.AuthNumber = DateTime.Now.ToString("hhmmss");
                            transactionAttemptCash.ReturnCode = "00";
                            transactionAttemptCash.SeqNumber = DateTime.Now.ToString("yyyyMMdd");
                            transactionAttemptCash.TransNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                            transactionAttemptCash.BatchNumber = DateTime.Now.ToString("yyyyMMdd");
                            transactionAttemptCash.DisplayReceipt = "";
                            transactionAttemptCash.DisplayTerminal = "";
                            transactionAttemptCash.DateReceived = DateTime.Now;
                            transactionAttemptCash.DepositDate = DateTime.Now.AddYears(-100);
                            transactionAttemptCash.Notes = " API Offline Demo Purchase Approved";

                            response.CardType = Convert.ToString(SDGDAL.Enums.CardTypes.Cash);
                            response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptCash.TransactionTypeId);
                            response.POSWSResponse.ErrNumber = "0";
                            response.POSWSResponse.Message = "Transaction Successful.";
                            response.POSWSResponse.Status = "Approved";
                        }
                        #endregion
                    }

                    action = "updating Transaction Attempt Cash details from gateway.";

                    var nTransactionAttempt = _transRepo.UpdateTransactionAttemptCash(transactionAttemptCash);

                    response.TransactionNumber = Convert.ToString(savedTransactionCash.TransactionCashId) + "-" + Convert.ToString(transactionAttemptCash.TransactionAttemptCashId);
                    response.AuthNumber = transactionAttemptCash.AuthNumber;
                    response.TransNumber = transactionAttemptCash.TransNumber;
                    response.SequenceNumber = transactionAttemptCash.SeqNumber;
                    response.BatchNumber = transactionAttemptCash.BatchNumber;
                    response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttemptCash.DateReceived);
                    response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transactionCash.TransactionEntryTypeId);
                    response.Total = Convert.ToDecimal(transactionAttemptCash.Amount.ToString("N2"));
                }
                else
                {
                    response.POSWSResponse.ErrNumber = "2900.5";
                    response.POSWSResponse.Status = "Declined";
                    response.POSWSResponse.Message = "Invalid System Mode.";
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "CashPurchaseTransaction", ex.StackTrace);

                response.POSWSResponse.ErrNumber = "2900.0";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }

        public CashVoidRefundResponse CashTransactionVoidRefund(TransactionVoidRefundRequest VoidRequest)
        {
            string action = string.Empty;

            CashVoidRefundResponse response = new CashVoidRefundResponse();

            response.POSWSResponse = new POSWSResponse();

            if (string.IsNullOrEmpty(VoidRequest.POSWSRequest.SystemMode)
                    || string.IsNullOrEmpty(VoidRequest.TransactionNumber)
                    || VoidRequest.Amount <= 0
                    || VoidRequest.VoidRefundReasonId <= 0)
            {
                response.POSWSResponse.ErrNumber = "1001.1";
                response.POSWSResponse.Message = "Missing input";
                response.POSWSResponse.Status = "DECLINED";
                return response;
            }

            response.POSWSResponse.ErrNumber = "0";

            try
            {
                if (VoidRequest.POSWSRequest.SystemMode.ToUpper().Equals("LIVE"))
                {
                    bool refundExists = false;
                    decimal totalAmount = VoidRequest.Amount;

                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();

                    action = "checking mobile app availability.";

                    if (!String.IsNullOrEmpty(VoidRequest.POSWSRequest.RToken))
                    {
                        response.POSWSResponse = mobileAppFunctions.CheckStatus(VoidRequest.POSWSRequest.RToken, out mobileApp, out account);

                        if (response.POSWSResponse.Status == "Declined")
                        {
                            return response;
                        }
                    }
                    else
                    {
                        action = "retrieving mobileApp Details using Activation Code";
                        mobileApp = _mAppRepo.GetMobileAppFullDetailsByActivationCode(VoidRequest.POSWSRequest.ActivationKey);

                        if (mobileApp == null)
                        {
                            response.POSWSResponse.ErrNumber = "2001.1";
                            response.POSWSResponse.Message = "No record found.";
                            response.POSWSResponse.Status = "Declined";
                            return response;
                        }
                        else
                        {
                            if (mobileApp.IsDeleted)
                            {
                                response.POSWSResponse.ErrNumber = "2001.1";
                                response.POSWSResponse.Message = "No record found.";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }

                            if (!mobileApp.IsActive)
                            {
                                response.POSWSResponse.ErrNumber = "2001.2";
                                response.POSWSResponse.Message = "Activation Code is not yet activated.";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }
                        }

                        action = "retrieving Account Details of Merchant";
                        //Get the MerchantID
                        var merchantInfo = _mAppRepo.GetMobileAppDetailsByMobileAppId(mobileApp.MobileAppId);

                        if (merchantInfo != null)
                        {
                            var accountInfo = _userRepo.GetDetailsbyParentIdAndParentTypeId(Convert.ToInt32(SDGDAL.Enums.ParentType.Merchant), merchantInfo.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId);
                            account.AccountId = accountInfo.AccountId;
                        }
                    }

                    MobileAppMethods mobileAppMethods = new MobileAppMethods();

                    action = "logging mobile app action.";
                    mobileAppFunctions.LogMobileAppAction("Credit Transaction Void TransactionNumber:" + VoidRequest.TransactionNumber, mobileApp.MobileAppId, account.AccountId, VoidRequest.POSWSRequest.GPSLat, VoidRequest.POSWSRequest.GPSLong);

                    int hyphen;
                    int transAttId;
                    int transId;

                    try
                    {
                        hyphen = VoidRequest.TransactionNumber.IndexOf('-');
                        transAttId = Convert.ToInt32(VoidRequest.TransactionNumber.Substring(hyphen + 1));
                        transId = Convert.ToInt32(VoidRequest.TransactionNumber.Substring(0, hyphen));
                    }
                    catch
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.ErrNumber = "2100.2";
                        response.POSWSResponse.Message = "Invalid transaction number";
                        return response;
                    }

                    action = "getting transaction by transaction id from database.";
                    var transactionCash = _transRepo.GetCashTransaction(transId);

                    if (mobileApp.MerchantBranchPOS.MerchantBranch.MerchantId != transactionCash.MerchantPOS.MerchantBranch.MerchantId)
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.ErrNumber = "2100.3";
                        response.POSWSResponse.Message = "Merchant Mismatch";
                        return response;
                    }

                    action = "retrieving cash transaction attempt by transactionAttempt id from database.";
                    var transactionAttemptCash = _transRepo.GetTransactionAttemptCash(transAttId);

                    if (transactionAttemptCash == null)
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.ErrNumber = "2100.4";
                        response.POSWSResponse.Message = "Transaction not found.";
                        return response;
                    }

                    if (string.IsNullOrEmpty(transactionAttemptCash.AuthNumber))
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.ErrNumber = "2100.5";
                        response.POSWSResponse.Message = "No previous completed sale transaction found.";
                        return response;
                    }
                    else
                    {
                        if (!_transRepo.IsCashTransactionAlreadyVoid(transId))
                        {
                            response.POSWSResponse.Status = "Declined";
                            response.POSWSResponse.ErrNumber = "2100.6";
                            response.POSWSResponse.Message = "Transaction already void.";
                            return response;
                        }

                        if (transactionAttemptCash.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Refund))
                        {
                            refundExists = true;
                            totalAmount += transactionAttemptCash.Amount;
                        }
                    }

                    var nTransactionAttemptCash = new SDGDAL.Entities.TransactionAttemptCash();

                    action = "verifying transaction status.";
                    if (!refundExists)
                    {
                        bool hasDeposited = (transactionAttemptCash.DepositDate < DateTime.Now
                                            && transactionAttemptCash.DepositDate > transactionAttemptCash.DateReceived);

                        if (hasDeposited)
                        {
                            if (transactionCash.FinalAmount >= VoidRequest.Amount)
                            {
                                nTransactionAttemptCash.Amount = VoidRequest.Amount;
                                nTransactionAttemptCash.DateSent = DateTime.Now;
                                nTransactionAttemptCash.DateReceived = DateTime.Now.AddYears(-100);
                                nTransactionAttemptCash.DepositDate = DateTime.Now.AddYears(-100);

                                nTransactionAttemptCash.Notes = "";
                                nTransactionAttemptCash.TransactionCashId = transactionCash.TransactionCashId;
                                nTransactionAttemptCash.TransactionChargesId = transactionAttemptCash.TransactionChargesId;
                                nTransactionAttemptCash.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Refund);
                                nTransactionAttemptCash.AuthNumber = transactionAttemptCash.AuthNumber;
                                nTransactionAttemptCash.BatchNumber = "";
                                nTransactionAttemptCash.DisplayReceipt = "";
                                nTransactionAttemptCash.DisplayTerminal = "";
                                nTransactionAttemptCash.ReturnCode = "";
                                nTransactionAttemptCash.SeqNumber = "";
                                nTransactionAttemptCash.TransNumber = transactionAttemptCash.TransNumber;
                            }
                            else
                            {
                                response.POSWSResponse.ErrNumber = "2100.7";
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "Transaction exceeded total amount of transaction.";
                                return response;
                            }
                        }
                        else
                        {
                            if (transactionCash.FinalAmount == VoidRequest.Amount)
                            {
                                // void
                                nTransactionAttemptCash.Amount = transactionCash.FinalAmount;
                                nTransactionAttemptCash.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Void);
                            }
                            else if (transactionCash.FinalAmount > VoidRequest.Amount)
                            {
                                // refund
                                nTransactionAttemptCash.Amount = VoidRequest.Amount;
                                nTransactionAttemptCash.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Refund);
                            }
                            else
                            {
                                response.POSWSResponse.ErrNumber = "2100.8";
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "Transaction exceeded total amount of transaction.";
                                return response;
                            }

                            nTransactionAttemptCash.DateSent = DateTime.Now;
                            nTransactionAttemptCash.DateReceived = DateTime.Now.AddYears(-100);
                            nTransactionAttemptCash.DepositDate = DateTime.Now.AddYears(-100);
                            nTransactionAttemptCash.Notes = "";
                            nTransactionAttemptCash.TransactionCashId = transactionAttemptCash.TransactionCashId;
                            nTransactionAttemptCash.TransactionChargesId = transactionAttemptCash.TransactionChargesId;
                            nTransactionAttemptCash.AuthNumber = "";
                            nTransactionAttemptCash.BatchNumber = "";
                            nTransactionAttemptCash.DisplayReceipt = "";
                            nTransactionAttemptCash.DisplayTerminal = "";
                            nTransactionAttemptCash.ReturnCode = "";
                            nTransactionAttemptCash.SeqNumber = "";
                            nTransactionAttemptCash.TransNumber = transactionAttemptCash.TransNumber;
                        }
                    }
                    else
                    {
                        // Refund exists
                        if (transactionCash.FinalAmount >= totalAmount)
                        {
                            nTransactionAttemptCash.Amount = VoidRequest.Amount;
                            nTransactionAttemptCash.DateSent = DateTime.Now;
                            nTransactionAttemptCash.DateReceived = DateTime.Now.AddYears(-100);
                            nTransactionAttemptCash.DepositDate = DateTime.Now.AddYears(-100);
                            nTransactionAttemptCash.Notes = "";
                            nTransactionAttemptCash.TransactionCashId = transactionAttemptCash.TransactionCashId;
                            nTransactionAttemptCash.TransactionChargesId = transactionAttemptCash.TransactionChargesId;
                            nTransactionAttemptCash.AuthNumber = "";
                            nTransactionAttemptCash.BatchNumber = "";
                            nTransactionAttemptCash.DisplayReceipt = "";
                            nTransactionAttemptCash.DisplayTerminal = "";
                            nTransactionAttemptCash.ReturnCode = "";
                            nTransactionAttemptCash.SeqNumber = "";
                            nTransactionAttemptCash.TransNumber = transactionAttemptCash.TransNumber;
                        }
                        else
                        {
                            response.POSWSResponse.ErrNumber = "2100.9";
                            response.POSWSResponse.Status = "Declined";
                            response.POSWSResponse.Message = "Transaction exceeded total amount of transaction.";
                            return response;
                        }
                    }

                    nTransactionAttemptCash.MobileAppId = mobileApp.MobileAppId;
                    nTransactionAttemptCash.AccountId = account.AccountId;
                    nTransactionAttemptCash.GPSLat = VoidRequest.POSWSRequest.GPSLat;
                    nTransactionAttemptCash.GPSLong = VoidRequest.POSWSRequest.GPSLong;
                    nTransactionAttemptCash.TransactionSignatureId = transactionAttemptCash.TransactionSignatureId;
                    nTransactionAttemptCash.TransactionVoidReasonId = VoidRequest.VoidRefundReasonId;
                    nTransactionAttemptCash.TransactionVoidNote = VoidRequest.VoidRefundNote;
                    nTransactionAttemptCash.PosEntryMode = transactionAttemptCash.PosEntryMode;
                    nTransactionAttemptCash.BatchNumber = transactionAttemptCash.BatchNumber;
                    nTransactionAttemptCash.ReturnCode = null;

                    #region Handle Transaction response

                    action = "setting up transaction entry for database.";

                    try
                    {
                        action = "checking mid status before setting up transaction entry.";
                        var mid = new SDGDAL.Entities.Mid();

                        mid = _midsRepo.GetMidByPosIdAndCardTypeId(transactionCash.MerchantPOSId, Convert.ToInt32(SDGDAL.Enums.CardTypes.Cash));

                        if (mid == null)
                        {
                            response.POSWSResponse.ErrNumber = "2001.8";
                            response.POSWSResponse.Message = "Mid not found.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }
                        else
                        {
                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                response.POSWSResponse.ErrNumber = "2101.5";
                                response.POSWSResponse.Message = "Mid is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }

                            if (!mid.Switch.IsActive)
                            {
                                response.POSWSResponse.ErrNumber = "2101.6";
                                response.POSWSResponse.Message = "Switch is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }
                        }

                        action = "checking if address is required. ";
                        if (mid.Switch.IsAddressRequired)
                        {
                            action = "saving temp transaction because switch requires address.";
                            var tempTransaction = new SDGDAL.Entities.TempTransaction();

                            tempTransaction.CopyProperties(transactionCash);

                            var nTempTransaction = _transRepo.CreateTempTransaction(tempTransaction);

                            if (nTempTransaction.TransactionId > 0)
                            {
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "This transaction require customer to enter their billing address.";
                                response.POSWSResponse.ErrNumber = "2111.1";
                                return response;
                            }
                            else
                            {
                                throw new Exception(action);
                            }
                        }

                        action = "checking if switch is active. ";
                        if (!mid.Switch.IsActive)
                        {
                            nTransactionAttemptCash.DateSent = DateTime.Now;
                            nTransactionAttemptCash.DateReceived = DateTime.Now;
                            nTransactionAttemptCash.DepositDate = DateTime.Now;
                            nTransactionAttemptCash.Notes = ((SDGDAL.Enums.TransactionType)transactionAttemptCash.TransactionTypeId).ToString() + " Declined. Switch inactive.";
                            nTransactionAttemptCash.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                        }
                        else
                        {
                            nTransactionAttemptCash.DateSent = DateTime.Now;
                            nTransactionAttemptCash.DateReceived = Convert.ToDateTime("1900/01/01");
                            nTransactionAttemptCash.DepositDate = Convert.ToDateTime("1900/01/01");

                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                nTransactionAttemptCash.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                            }
                        }

                        action = "saving void transaction to database";

                        var nnTransactionAttemptCash = _transRepo.CreateCashTransactionAttempt(nTransactionAttemptCash);

                        if (nnTransactionAttemptCash.TransactionAttemptCashId > 0)
                        {
                            action = "processing transaction for api integration. Transaction was successfully saved.";

                            if (((SDGDAL.Enums.TransactionType)nnTransactionAttemptCash.TransactionTypeId) == Enums.TransactionType.Declined)
                            {
                                response.POSWSResponse.Message = "Transaction failed. Please contact Support.";
                                response.POSWSResponse.ErrNumber = "2111.2";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }
                            else
                            {
                                #region Offline

                                if (mid.Switch.SwitchCode == "Offline")
                                {
                                    if (nnTransactionAttemptCash.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Void))
                                    {
                                        decimal orgAmount = nnTransactionAttemptCash.Amount;
                                        decimal finalAmount = orgAmount * 100;

                                        nnTransactionAttemptCash.AuthNumber = DateTime.Now.ToString("hhmmss");
                                        nnTransactionAttemptCash.ReturnCode = "00";
                                        nnTransactionAttemptCash.SeqNumber = DateTime.Now.ToString("yyyyMMdd");
                                        nnTransactionAttemptCash.TransNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                                        nnTransactionAttemptCash.BatchNumber = DateTime.Now.ToString("yyyyMMdd");
                                        nnTransactionAttemptCash.DisplayReceipt = "";
                                        nnTransactionAttemptCash.DisplayTerminal = "";
                                        nnTransactionAttemptCash.DateReceived = DateTime.Now;
                                        nnTransactionAttemptCash.DepositDate = DateTime.Now.AddYears(-100);
                                        nnTransactionAttemptCash.TransactionTypeId = Convert.ToInt32((SDGDAL.Enums.TransactionType.Void));
                                        nnTransactionAttemptCash.Notes = " API Void Approved";

                                        response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType.Cash));
                                        response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType.Void));
                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Transaction Successful.";
                                        response.POSWSResponse.Status = "Approved";
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptCash.TransactionTypeId);

                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }

                                #endregion Offline

                                else
                                {
                                    // Invalid switch
                                    response.POSWSResponse.Message = "Transaction failed. Switch not yet supported. Please contact Support.";
                                    response.POSWSResponse.ErrNumber = "2111.4";
                                    response.POSWSResponse.Status = "Declined";
                                    return response;
                                }

                                action = "updating transaction attempt data";
                                var voidTransactionAttempt = _transRepo.UpdateTransactionAttemptCash(nnTransactionAttemptCash);

                                action = "updating the amount of previous transaction";
                                transactionAttemptCash.Amount = (transactionAttemptCash.Amount - nnTransactionAttemptCash.Amount);
                                var updateTransactionAttempt = _transRepo.UpdateTransactionAttemptCash(transactionAttemptCash);

                                response.Date = Convert.ToString(nnTransactionAttemptCash.DateReceived);
                                response.Amount = Convert.ToString(nnTransactionAttemptCash.Amount);
                                response.TransactionNumber = nnTransactionAttemptCash.TransactionCashId + "-" + nnTransactionAttemptCash.TransactionAttemptCashId;
                                response.AuthNumber = nnTransactionAttemptCash.AuthNumber;
                                response.CardType = Convert.ToString((SDGDAL.Enums.MobileAppCardType.Cash));
                                response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType.Void));
                                response.Currency = transactionCash.Currency.CurrencyCode;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error please
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "CreditTransactionRequest", ex.StackTrace);

                        response.POSWSResponse.ErrNumber = "1001.2";
                        response.POSWSResponse.Message = "Unknown error, please contact support. Ref: " + errRefNumber;
                        response.POSWSResponse.Status = "Declined";
                    }

                    #endregion Handle Transaction response
                }
                else if (VoidRequest.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.POSWSResponse.ErrNumber = "0";
                    response.POSWSResponse.Status = "Approved";
                    response.POSWSResponse.Message = "";
                }
                else
                {
                    response.POSWSResponse.ErrNumber = "2100.13";
                    response.POSWSResponse.Status = "Declined";
                    response.POSWSResponse.Message = "";
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "CreditTransactionVoid", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "CreditTransactionVoid  ErrRef:" + errRefNumber, ex.InnerException.StackTrace);
                }

                response.POSWSResponse.ErrNumber = "2101.11";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + " " + action;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }
    }
}