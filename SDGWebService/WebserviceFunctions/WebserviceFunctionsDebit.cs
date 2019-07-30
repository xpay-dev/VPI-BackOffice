using HPA_ISO8583;
using SDGDAL;
using SDGDAL.Repositories;
using SDGUtil;
using SDGWebService.Classes;
using SDGWebService.TLVFunctions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SDGWebService.WebserviceFunctions
{
    public class WebserviceFunctionsDebit
    {
        private Functions.MobileAppFunctions mobileAppFunctions = new Functions.MobileAppFunctions();
        private TransactionRepository _transRepo = new TransactionRepository();
        private MidsRepository _midsRepo = new MidsRepository();
        private DebitSystemTraceNumRepository _traceNumRepo = new DebitSystemTraceNumRepository();
        private BatchReportRepository _batchRepo = new BatchReportRepository();
        private MobileAppRepository _mAppRepo = new MobileAppRepository();
      
        public PurchaseResponse TransactionPurchaseDebit(TransactionRequest request)
        {
            string action = string.Empty;
            PurchaseResponse response = new PurchaseResponse();

            response.POSWSResponse = new POSWSResponse();

            if (string.IsNullOrEmpty(request.POSWSRequest.RToken)
                || string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                || request.Device <= 0
                || string.IsNullOrEmpty(request.LanguageUser)
                || string.IsNullOrEmpty(request.CardDetails.Currency)
                || string.IsNullOrEmpty(request.CardDetails.RefNumberApp)
                || request.CardDetails.Amount <= 0
                || request.DeviceId <= 0)
            {
                response.POSWSResponse.ErrNumber = "1001.1";
                response.POSWSResponse.Message = "Missing input";
                response.POSWSResponse.Status = "DECLINED";
                return response;
            }

            var transactionAttemptDebitId = 0;

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

                    MobileAppMethods mobileAppMethods = new MobileAppMethods();

                    string track1 = "";
                    string track2 = "";
                    string cardNumber = "";
                    string nameOnCard = "";
                    string expDate = "";
                    string expYear = "";
                    string expMonth = "";

                    SDGDAL.Enums.SwipeDevice sd = (SDGDAL.Enums.SwipeDevice)request.Device;
                    SDGDAL.Enums.CardAction cardAction = (SDGDAL.Enums.CardAction)request.Action;

                    ClassTLV emvDataResult = new ClassTLV();

                    response.POSWSResponse = GetCardDetails(request.CardDetails, sd, cardAction, out track1, out track2,
                        out cardNumber, out nameOnCard, out expDate, out expYear, out expMonth, out emvDataResult);
                    if (response.POSWSResponse.Status == "Declined")
                    {
                        return response;
                    }

                    action = "logging mobile app action.";
                    mobileAppFunctions.LogMobileAppAction("Purchase Debit Transaction Swipe", mobileApp.MobileAppId, account.AccountId, request.POSWSRequest.GPSLat, request.POSWSRequest.GPSLong);

                    if (!mobileApp.MobileAppFeatures.DebitTransaction)
                    {
                        response.POSWSResponse.ErrNumber = "2000.1";
                        response.POSWSResponse.Message = "Debit transactions are currently disabled on this device";
                        response.POSWSResponse.Status = "Declined";
                        return response;
                    }

                    #region Transaction

                    bool updatePending = mobileApp.UpdatePending;

                    var transactionDebit = new SDGDAL.Entities.TransactionDebit();
                    var transactionAttemptDebit = new SDGDAL.Entities.TransactionAttemptDebit();

                    action = "setting up transaction and transactionattempt details.";
                    int posId = mobileApp.MerchantBranchPOSId;

                    //Validate Account Type
                    if (request.AccountTypeId != 0)
                    {
                        transactionDebit.AccountTypeId = request.AccountTypeId;
                    }
                    else
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid account type";
                        response.POSWSResponse.ErrNumber = "2000.2";
                        response.POSWSResponse.UpdatePending = updatePending;
                        response.TransactionNumber = "0";
                        return response;
                    }

                    transactionDebit.CardNumber = cardNumber;
                    transactionDebit.NameOnCard = request.CardDetails.NameOnCard;
                    transactionDebit.ExpMonth = request.CardDetails.ExpMonth;
                    transactionDebit.ExpYear = request.CardDetails.ExpYear;
                    transactionDebit.OriginalAmount = request.CardDetails.Amount;

                    transactionDebit.TaxAmount = 0;

                    transactionDebit.FinalAmount = request.CardDetails.Amount;

                    try
                    {
                        transactionDebit.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(request.CardDetails.Currency);
                    }
                    catch
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid currency.";
                        response.POSWSResponse.ErrNumber = "2000.3";
                        response.POSWSResponse.UpdatePending = updatePending;
                        response.TransactionNumber = "0";
                        return response;
                    }

                    transactionDebit.DateCreated = DateTime.Now;
                    transactionDebit.RefNumApp = request.CardDetails.RefNumberApp;
                    transactionDebit.RefNumSales = request.CardDetails.RefNumberSale;
                    transactionDebit.Notes = request.Device.ToString();
                    transactionDebit.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.Debit);
                    transactionDebit.MerchantPOSId = mobileApp.MerchantBranchPOSId;
                    transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale);
                    transactionAttemptDebit.AccountId = account.AccountId;
                    transactionAttemptDebit.MobileAppId = mobileApp.MobileAppId;
                    transactionAttemptDebit.DeviceId = request.Device;
                    transactionAttemptDebit.GPSLat = request.POSWSRequest.GPSLat;
                    transactionAttemptDebit.GPSLong = request.POSWSRequest.GPSLong;
                    transactionAttemptDebit.Amount = transactionDebit.OriginalAmount;
                    transactionAttemptDebit.TraceNumber = SDGUtil.Functions.GenerateSystemTraceAudit();

                    transactionAttemptDebit.Notes = request.Device.ToString();

                    #region Handle Transaction response

                    action = "setting up transaction entry for database.";

                    try
                    {
                        action = "checking mid status before setting up transaction entry.";
                        var mid = new SDGDAL.Entities.Mid();

                        mid = _midsRepo.GetMidByPosIdAndCardTypeId(transactionDebit.MerchantPOSId, Convert.ToInt32(SDGDAL.Enums.CardTypes.Debit));

                        if (mid == null)
                        {
                            response.POSWSResponse.ErrNumber = "2000.4";
                            response.POSWSResponse.Message = "Mid not found.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }
                        else
                        {
                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                response.POSWSResponse.ErrNumber = "2000.5";
                                response.POSWSResponse.Message = "Mid is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }

                            if (!mid.Switch.IsActive)
                            {
                                response.POSWSResponse.ErrNumber = "2000.6";
                                response.POSWSResponse.Message = "Switch is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }
                        }

                        action = "trying to encrypt card data.";

                        #region Encrypt Card Data

                        //ENCRYPT CARD DATA
                        string NE_CARD = transactionDebit.CardNumber;
                        string NE_EMONTH = transactionDebit.ExpMonth;
                        string NE_EYEAR = transactionDebit.ExpYear;

                        string E_CARD;
                        string E_EMONTH = null, E_EYEAR = null;
                        //string E_CSC;
                        byte[] desKey;
                        byte[] desIV;

                        //card number masking
                        string s = NE_CARD.Substring(NE_CARD.Length - 4);
                        string r = new string('*', NE_CARD.Length);
                        string MASK_CARD = r + s;
                        //CSC masking
                        //string MASK_CSC = new string('*', NE_CSC.Length);

                        E_CARD = Utility.GenerateSymmetricKeyAndEcryptData(MASK_CARD, out desKey, out desIV);

                        transactionDebit.Key = new SDGDAL.Entities._Key();
                        transactionDebit.Key.Key = Convert.ToBase64String(desKey);
                        transactionDebit.Key.IV = Convert.ToBase64String(desIV);

                        if (NE_EMONTH != null || NE_EYEAR != null)
                        {
                            string MASK_EMONTH = new string('*', NE_EMONTH.Length);
                            string MASK_EYEAR = new string('*', NE_EYEAR.Length);
                            E_EMONTH = Utility.EncryptDataWithExistingKey(NE_EMONTH, desKey, desIV);
                            E_EYEAR = Utility.EncryptDataWithExistingKey(NE_EYEAR, desKey, desIV);
                        }

                        #endregion Encrypt Card Data

                        action = "checking if address is required. ";
                        if (mid.Switch.IsAddressRequired)
                        {
                            action = "saving temp transaction because switch requires address.";
                            var tempTransaction = new SDGDAL.Entities.TempTransaction();

                            tempTransaction.CopyProperties(transactionDebit);

                            var nTempTransaction = _transRepo.CreateTempTransaction(tempTransaction);

                            if (nTempTransaction.TransactionId > 0)
                            {
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "This transaction require customer to enter their billing address.";
                                response.POSWSResponse.ErrNumber = "2000.7";
                                return response;
                            }
                            else
                            {
                                throw new Exception(action);
                            }
                        }

                        transactionAttemptDebit.TransactionChargesId = mid.TransactionChargesId;

                        action = "checking if switch is active. ";
                        if (!mid.Switch.IsActive)
                        {
                            transactionAttemptDebit.DateSent = DateTime.Now;
                            transactionAttemptDebit.DateReceived = DateTime.Now;
                            transactionAttemptDebit.DepositDate = DateTime.Now;
                            transactionAttemptDebit.Notes = ((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId).ToString() + " Declined. Switch inactive.";
                            transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                        }
                        else
                        {
                            transactionAttemptDebit.DateSent = DateTime.Now;
                            transactionAttemptDebit.DateReceived = Convert.ToDateTime("1900/01/01");
                            transactionAttemptDebit.DepositDate = Convert.ToDateTime("1900/01/01");

                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                            }

                            if (mid.Switch.SwitchName == "Transax")
                            {
                                // TODO: Correct?
                                if (((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId) == Enums.TransactionType.PreAuth)
                                {
                                    transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(Enums.TransactionType.Sale);
                                }
                            }
                        }

                        action = "saving transaction details to database.";
                        var nTransaction = new SDGDAL.Entities.TransactionDebit();
                        nTransaction.CopyProperties(transactionDebit);

                        nTransaction.CardNumber = E_CARD;
                        nTransaction.ExpMonth = E_EMONTH;
                        nTransaction.ExpYear = E_EYEAR;
                        nTransaction.CurrencyId = mid.CurrencyId;
                        nTransaction.MidId = mid.MidId;

                        var rTransaction = _transRepo.CreateTransactionDebit(nTransaction, transactionAttemptDebit);

                        if (rTransaction.TransactionDebitId > 0)
                        {
                            action = "processing transaction for api integration. Transaction was successfully saved.";
                            transactionDebit.TransactionDebitId = rTransaction.TransactionDebitId;

                            if (((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId) == Enums.TransactionType.Declined)
                            {
                                response.POSWSResponse.Message = "Transaction failed. Please contact Support.";
                                response.POSWSResponse.ErrNumber = "2000.9";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }
                            else
                            {
                                transactionAttemptDebitId = transactionAttemptDebit.TransactionAttemptDebitId;

                                #region MaxBank debit

                                if (mid.Switch.SwitchCode == "Maxbank")
                                {
                                    if (transactionAttemptDebit.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale))
                                    {
                                        DE_ISO8583 de = new DE_ISO8583();
                                        GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                        GatewayProcessor.VeritasPayment.CardDetails transData = new GatewayProcessor.VeritasPayment.CardDetails();
                                        action = "processing debit card.";

                                        transData.PrivateAdditionalData = Convert.ToString(request.CardDetails.EpbKsn.Length).PadLeft(4, '0') + request.CardDetails.EpbKsn + Convert.ToString(cardNumber.Length).PadLeft(4, '0') + cardNumber;
                                        transData.MerchantID = mid.Param_2;
                                        transData.TerminalID = mid.Param_6;

                                        decimal orgAmount = transactionAttemptDebit.Amount;
                                        decimal finalAmount = orgAmount * 100;

                                        try
                                        {
                                            transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                        }
                                        catch
                                        {
                                            transData.Amount = finalAmount.ToString();
                                        }

                                        transData.SystemTraceAudit = transactionAttemptDebit.TraceNumber;
                                        transData.CardNumber = cardNumber;
                                        transData.NameOnCard = request.CardDetails.NameOnCard;
                                        transData.Track2Data = track2.Replace('D', '=').TrimStart(';').TrimEnd('F').TrimEnd('?');
                                        transData.Track1Data = null;
                                        transData.ExpirationDate = (transactionDebit.ExpMonth == null || transactionDebit.ExpYear == null) ? null : transactionDebit.ExpYear + transactionDebit.ExpMonth;
                                        transData.PinBlock = request.CardDetails.Epb;
                                        transData.CurrencyCode = mid.Currency.IsoCode;

                                        //Fees
                                        transData.AmountTransactionFee = "00000000";
                                        transData.AccountType = Convert.ToString(rTransaction.AccountTypeId);
                                        transData.ChipCardData = emvDataResult.EmvIccData;

                                        action = "processing transaction for debit api integration. Transaction was successfully saved.";

                                        try
                                        {
                                            #region Thread
                                            //Thread myThread = new Thread(() =>
                                            //{
                                            //    int x = 1;
                                            //    while (x < 20)
                                            //    {
                                            //        transData.SystemTraceAudit = SDGUtil.Functions.GenerateSystemTraceAudit();
                                            //        var apiResponse2 = gateway.ProcessPurchaseMaxbankGateway(transData, "PURCHASE");
                                            //        x++;
                                            //        Thread.Sleep(1000);
                                            //    }
                                            //});
                                            //myThread.Start();
                                            #endregion

                                            var apiResponse = gateway.ProcessPurchaseMaxbankGateway(transData, "PURCHASE");

                                            if (apiResponse.Result.Status == "Approved")
                                            {
                                                transactionAttemptDebit.TransactionAttemptDebitId = transactionAttemptDebitId;
                                                transactionAttemptDebit.AuthNumber = apiResponse.Result.AuthorizationID;
                                                transactionAttemptDebit.ReturnCode = apiResponse.Result.ReturnCode;
                                                transactionAttemptDebit.TraceNumber = apiResponse.Result.SytemTraceAudit;
                                                transactionAttemptDebit.ReferenceNumber = apiResponse.Result.Reference;
                                                transactionAttemptDebit.RetrievalRefNumber = apiResponse.Result.Reference;
                                                transactionAttemptDebit.InvoiceNumber = DateTime.Now.ToString("YYYYMMdd");
                                                transactionAttemptDebit.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Debit));
                                                transactionAttemptDebit.DisplayReceipt = transData.MerchantID;
                                                transactionAttemptDebit.DisplayTerminal = transData.TerminalID;
                                                transactionAttemptDebit.DateReceived = DateTime.Now;
                                                transactionAttemptDebit.DepositDate = DateTime.Now.AddYears(-100);
                                                transactionAttemptDebit.Notes = "API Debit Purchase Approved";
                                                response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId);

                                                response.POSWSResponse.ErrNumber = "0";
                                                response.POSWSResponse.Message = "Transaction Successful.";
                                                response.POSWSResponse.Status = "Approved";
                                            }
                                            else if (apiResponse.Result.Status == "Declined")
                                            {
                                                transactionAttemptDebit.TransactionAttemptDebitId = transactionAttemptDebitId;
                                                transactionAttemptDebit.AuthNumber = apiResponse.Result.AuthorizationID;
                                                transactionAttemptDebit.ReturnCode = apiResponse.Result.ReturnCode;
                                                transactionAttemptDebit.TraceNumber = transData.SystemTraceAudit;
                                                transactionAttemptDebit.ReferenceNumber = apiResponse.Result.Reference;
                                                transactionAttemptDebit.DisplayReceipt = transData.MerchantID;
                                                transactionAttemptDebit.DisplayTerminal = transData.TerminalID;
                                                transactionAttemptDebit.DateReceived = DateTime.Now;
                                                transactionAttemptDebit.DepositDate = DateTime.Now.AddYears(-100);
                                                transactionAttemptDebit.Notes = "API Debit Purchase Declined. " + apiResponse.Result.ErrNumber + " " + apiResponse.Result.Message;
                                                transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                                response.POSWSResponse.ErrNumber = apiResponse.Result.ErrNumber;
                                                response.POSWSResponse.Message = apiResponse.Result.Message;
                                                response.POSWSResponse.Status = "Declined";
                                            }
                                            else
                                            {
                                                response.POSWSResponse.Message = "Transaction failed. Please contact Support.";
                                                response.POSWSResponse.ErrNumber = "2000.10";
                                                response.POSWSResponse.Status = "Declined";
                                                return response;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            var errorOnAction = "Error while " + action;
                                            var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction, "TransactionPurchaseDebit", "", "");

                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + "Declined" + " " + errorOnAction + " " + ex.Message;
                                            response.POSWSResponse.ErrNumber = "2000.10";
                                            response.POSWSResponse.Status = "Declined";
                                            return response;
                                        }
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId);

                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        response.POSWSResponse.ErrNumber = "2000.11";
                                        return response;
                                    }
                                }

                                #endregion MaxBank debit

                                #region OFFLINE debit

                                else if (mid.Switch.SwitchCode == "Offline")
                                {
                                    if (transactionAttemptDebit.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale))
                                    {
                                        action = "processing debit card.";

                                        transactionAttemptDebit.AuthNumber = DateTime.Now.ToString("hhmmss");
                                        transactionAttemptDebit.ReturnCode = "00";
                                        transactionAttemptDebit.SeqNumber = DateTime.Now.ToString("yyyyMMdd");
                                        transactionAttemptDebit.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Debit)); ;
                                        transactionAttemptDebit.TransactionAttemptDebitId = transactionAttemptDebitId;
                                        transactionAttemptDebit.TraceNumber = "000000";
                                        transactionAttemptDebit.ReferenceNumber = "00";
                                        transactionAttemptDebit.InvoiceNumber = DateTime.Now.ToString("yyyyMMddhhmmss");
                                        transactionAttemptDebit.DisplayReceipt = mid.Param_2;
                                        transactionAttemptDebit.DisplayTerminal = mid.Param_6;

                                        transactionAttemptDebit.DateReceived = DateTime.Now;
                                        transactionAttemptDebit.DepositDate = DateTime.Now.AddYears(-100);
                                        transactionAttemptDebit.Notes = "Offline Demo Successful";
                                        response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId);

                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Transaction Successful - Offline SWITCH";
                                        response.POSWSResponse.Status = "Approved";
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId);

                                        response.POSWSResponse.ErrNumber = "2000.12";
                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }
                                else
                                {
                                    response.POSWSResponse.Message = "Transaction failed. Switch not yet supported. Please contact Support.";
                                    response.POSWSResponse.ErrNumber = "2000.13";
                                    response.POSWSResponse.Status = "Declined";
                                    return response;
                                }

                                #endregion OFFLINE debit

                                #region Decrypt Card Number

                                string Key = transactionAttemptDebit.TransactionDebit.Key.Key;
                                string KeyIV = transactionAttemptDebit.TransactionDebit.Key.IV;
                                string E_Card = transactionAttemptDebit.TransactionDebit.CardNumber;

                                String hashCardNumber = Utility.DecryptEncDataWithKeyAndIV(E_Card, Key, KeyIV);

                                transactionDebit.CardNumber = SDGUtil.Functions.HashCardNumber(hashCardNumber);

                                #endregion Decrypt Card Number

                                var nTransactionAttempt = _transRepo.UpdateTransactionAttemptDebit(transactionAttemptDebit);

                                response.MerchantId = transactionAttemptDebit.DisplayReceipt;
                                response.TerminalId = transactionAttemptDebit.DisplayTerminal;
                                response.Currency = mid.Currency.CurrencyCode;
                                response.TransactionNumber = Convert.ToString(transactionDebit.TransactionDebitId) + "-" + Convert.ToString(transactionAttemptDebit.TransactionAttemptDebitId);
                                response.AuthNumber = transactionAttemptDebit.AuthNumber;
                                response.TraceNumber = transactionAttemptDebit.TraceNumber;
                                response.SequenceNumber = transactionAttemptDebit.SeqNumber;
                                response.BatchNumber = transactionAttemptDebit.BatchNumber;
                                response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttemptDebit.DateReceived);
                                response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transactionDebit.TransactionEntryTypeId);
                                response.Total = Convert.ToDecimal(transactionAttemptDebit.Amount.ToString("N2"));
                                response.CardNumber = SDGUtil.Functions.HashCardNumber(transactionDebit.CardNumber);
                                response.CardType = Convert.ToString(SDGDAL.Enums.CardTypes.Debit);
                            }
                        }
                        else
                        {
                            throw new Exception(action);
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "TransactionPurchaseDebit", ex.StackTrace);

                        response.POSWSResponse.ErrNumber = "2000.14";
                        response.POSWSResponse.Message = "Unknown error, please contact support. Ref: " + errRefNumber + action;
                        response.POSWSResponse.Status = "Declined";
                    }

                    return response;

                    #endregion Handle Transaction response

                    #endregion Transaction
                }
                else if (request.POSWSRequest.SystemMode.ToUpper().Equals("TESTAPPROVED"))
                {
                    response.POSWSResponse.Status = "Approved";
                    response.POSWSResponse.Message = "";
                    response.POSWSResponse.ErrNumber = "0";
                    response.POSWSResponse.UpdatePending = true;

                    response.AuthNumber = "5042158454";
                    response.SequenceNumber = "0015248456";
                    response.TraceNumber = "0015248456";
                    response.Timestamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                    response.TransactionNumber = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                    response.SubTotal = 0;
                    response.Tax1Amount = 0;
                    response.Tax1Name = "";
                    response.Tax1Rate = 0;
                    response.Tax2Amount = 0;
                    response.Tax2Name = "";
                    response.Tax2Rate = 0;
                    response.Total = request.CardDetails.Amount;
                    response.Tips = request.Tips;
                }
                else
                {
                    response.POSWSResponse.ErrNumber = "2000.15";
                    response.POSWSResponse.Status = "Declined";
                    response.POSWSResponse.Message = "Invalid System Mode.";
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "TransactionPurchaseDebit", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "TransactionPurchaseDebit  ErrRef:" + errRefNumber, ex.InnerException.StackTrace);
                }

                response.POSWSResponse.ErrNumber = "2000.16";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.POSWSResponse.Status = "Declined";
            }

            return response;
        }

        public DebitBalanceInquiryResponse DebitBalanceInquiry(TransactionRequest request)
        {
            string action = string.Empty;

            DebitBalanceInquiryResponse response = new DebitBalanceInquiryResponse();
            response.POSWSResponse = new POSWSResponse();

            var transactionAttemptDebitId = 0;
            response.POSWSResponse.ErrNumber = "0";

            if (string.IsNullOrEmpty(request.POSWSRequest.RToken)
                || string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                || request.Device <= 0
                || string.IsNullOrEmpty(request.LanguageUser)
                || string.IsNullOrEmpty(request.CardDetails.Currency)
                || string.IsNullOrEmpty(request.CardDetails.RefNumberApp)
                || request.AccountTypeId <= 0
                || request.DeviceId <= 0)
            {
                response.POSWSResponse.ErrNumber = "1001.1";
                response.POSWSResponse.Message = "Missing input";
                response.POSWSResponse.Status = "DECLINED";
                return response;
            }

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

                    MobileAppMethods mobileAppMethods = new MobileAppMethods();

                    string track1 = "";
                    string track2 = "";
                    string cardNumber = "";
                    string nameOnCard = "";
                    string expDate = "";
                    string expYear = "";
                    string expMonth = "";

                    SDGDAL.Enums.SwipeDevice sd = (SDGDAL.Enums.SwipeDevice)request.Device;
                    SDGDAL.Enums.CardAction cardAction = (SDGDAL.Enums.CardAction)request.Action;

                    ClassTLV emvDataResult = new ClassTLV();

                    response.POSWSResponse = GetCardDetails(request.CardDetails, sd, cardAction, out track1, out track2,
                        out cardNumber, out nameOnCard, out expDate, out expYear, out expMonth, out emvDataResult);
                    if (response.POSWSResponse.Status == "Declined")
                    {
                        return response;
                    }

                    action = "logging mobile app action.";
                    mobileAppFunctions.LogMobileAppAction("Purchase Debit Transaction Swipe", mobileApp.MobileAppId, account.AccountId, request.POSWSRequest.GPSLat, request.POSWSRequest.GPSLong);

                    if (!mobileApp.MobileAppFeatures.BalanceInquiry)
                    {
                        response.POSWSResponse.ErrNumber = "2200.1";
                        response.POSWSResponse.Message = "Balance Inquiry is currently disabled on this device";
                        response.POSWSResponse.Status = "Declined";
                        return response;
                    }

                    #region Transaction

                    bool updatePending = mobileApp.UpdatePending;

                    var transactionDebit = new SDGDAL.Entities.TransactionDebit();
                    var transactionAttemptDebit = new SDGDAL.Entities.TransactionAttemptDebit();

                    action = "setting up transaction and transactionattempt details.";
                    int posId = mobileApp.MerchantBranchPOSId;

                    ///Validate Account Type
                    if (request.AccountTypeId != 0)
                    {
                        transactionDebit.AccountTypeId = request.AccountTypeId;
                    }
                    else
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid account type";
                        response.POSWSResponse.ErrNumber = "2200.2";
                        response.POSWSResponse.UpdatePending = updatePending;
                        return response;
                    }

                    transactionDebit.CardNumber = cardNumber;
                    transactionDebit.NameOnCard = request.CardDetails.NameOnCard;
                    transactionDebit.ExpMonth = request.CardDetails.ExpMonth;
                    transactionDebit.ExpYear = request.CardDetails.ExpYear;
                    transactionDebit.OriginalAmount = request.CardDetails.Amount;

                    transactionDebit.TaxAmount = 0;

                    transactionDebit.FinalAmount = request.CardDetails.Amount;

                    try
                    {
                        transactionDebit.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(request.CardDetails.Currency);
                    }
                    catch
                    {
                        response.POSWSResponse.Status = "Declined";
                        response.POSWSResponse.Message = "Invalid currency.";
                        response.POSWSResponse.ErrNumber = "2200.3";
                        response.POSWSResponse.UpdatePending = updatePending;
                        return response;
                    }

                    transactionDebit.DateCreated = DateTime.Now;
                    transactionDebit.RefNumApp = request.CardDetails.RefNumberApp;
                    transactionDebit.RefNumSales = request.CardDetails.RefNumberSale;
                    transactionDebit.Notes = request.Device.ToString();
                    transactionDebit.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.Debit);
                    transactionDebit.MerchantPOSId = mobileApp.MerchantBranchPOSId;
                    transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.BalanceInquiry);
                    transactionAttemptDebit.AccountId = account.AccountId;
                    transactionAttemptDebit.MobileAppId = mobileApp.MobileAppId;
                    transactionAttemptDebit.DeviceId = request.Device;
                    transactionAttemptDebit.GPSLat = request.POSWSRequest.GPSLat;
                    transactionAttemptDebit.GPSLong = request.POSWSRequest.GPSLong;
                    transactionAttemptDebit.Amount = transactionDebit.OriginalAmount;

                    transactionAttemptDebit.Notes = request.Device.ToString();

                    #region Handle Transaction response

                    action = "setting up transaction entry for database.";

                    try
                    {
                        action = "checking mid status before setting up transaction entry.";
                        var mid = new SDGDAL.Entities.Mid();

                        mid = _midsRepo.GetMidByPosIdAndCardTypeId(transactionDebit.MerchantPOSId, Convert.ToInt32(SDGDAL.Enums.CardTypes.Debit)); //to change to debit

                        if (mid == null)
                        {
                            response.POSWSResponse.ErrNumber = "2200.4";
                            response.POSWSResponse.Message = "Mid not found.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }
                        else
                        {
                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                response.POSWSResponse.ErrNumber = "2200.5";
                                response.POSWSResponse.Message = "Mid is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }

                            if (!mid.Switch.IsActive)
                            {
                                response.POSWSResponse.ErrNumber = "2200.6";
                                response.POSWSResponse.Message = "Switch is Inactive.";
                                response.POSWSResponse.Status = "Declined";

                                return response;
                            }
                        }

                        action = "trying to encrypt card data.";

                        #region Encrypt Card Data

                        //ENCRYPT CARD DATA
                        string NE_CARD = transactionDebit.CardNumber;
                        string NE_EMONTH = transactionDebit.ExpMonth;
                        string NE_EYEAR = transactionDebit.ExpYear;

                        string E_CARD;
                        string E_EMONTH = null, E_EYEAR = null;
                        //string E_CSC;
                        byte[] desKey;
                        byte[] desIV;

                        //card number masking
                        string s = NE_CARD.Substring(NE_CARD.Length - 4);
                        string r = new string('*', NE_CARD.Length);
                        string MASK_CARD = r + s;
                        //CSC masking
                        //string MASK_CSC = new string('*', NE_CSC.Length);

                        E_CARD = Utility.GenerateSymmetricKeyAndEcryptData(MASK_CARD, out desKey, out desIV);

                        transactionDebit.Key = new SDGDAL.Entities._Key();
                        transactionDebit.Key.Key = Convert.ToBase64String(desKey);
                        transactionDebit.Key.IV = Convert.ToBase64String(desIV);

                        if (NE_EMONTH != null || NE_EYEAR != null)
                        {
                            string MASK_EMONTH = new string('*', NE_EMONTH.Length);
                            string MASK_EYEAR = new string('*', NE_EYEAR.Length);
                            E_EMONTH = Utility.EncryptDataWithExistingKey(NE_EMONTH, desKey, desIV);
                            E_EYEAR = Utility.EncryptDataWithExistingKey(NE_EYEAR, desKey, desIV);
                        }

                        #endregion Encrypt Card Data

                        action = "checking if address is required. ";
                        if (mid.Switch.IsAddressRequired)
                        {
                            action = "saving temp transaction because switch requires address.";
                            var tempTransaction = new SDGDAL.Entities.TempTransaction();

                            tempTransaction.CopyProperties(transactionDebit);

                            var nTempTransaction = _transRepo.CreateTempTransaction(tempTransaction);

                            if (nTempTransaction.TransactionId > 0)
                            {
                                response.POSWSResponse.Status = "Declined";
                                response.POSWSResponse.Message = "This transaction require customer to enter their billing address.";
                                response.POSWSResponse.ErrNumber = "2200.7";
                                return response;
                            }
                            else
                            {
                                throw new Exception(action);
                            }
                        }

                        transactionAttemptDebit.TransactionChargesId = mid.TransactionChargesId;

                        action = "checking if switch is active. ";
                        if (!mid.Switch.IsActive)
                        {
                            transactionAttemptDebit.DateSent = DateTime.Now;
                            transactionAttemptDebit.DateReceived = DateTime.Now;
                            transactionAttemptDebit.DepositDate = DateTime.Now;
                            transactionAttemptDebit.Notes = ((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId).ToString() + " Declined. Switch inactive.";
                            transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                        }
                        else
                        {
                            transactionAttemptDebit.DateSent = DateTime.Now;
                            transactionAttemptDebit.DateReceived = Convert.ToDateTime("1900/01/01");
                            transactionAttemptDebit.DepositDate = Convert.ToDateTime("1900/01/01");

                            if (!mid.IsActive || mid.IsDeleted)
                            {
                                transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                            }

                            if (mid.Switch.SwitchName == "Transax")
                            {
                                // TODO: Correct?
                                if (((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId) == Enums.TransactionType.PreAuth)
                                {
                                    transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(Enums.TransactionType.Sale);
                                }
                            }
                        }

                        action = "saving transaction details to database.";
                        var nTransaction = new SDGDAL.Entities.TransactionDebit();
                        nTransaction.CopyProperties(transactionDebit);

                        nTransaction.CardNumber = E_CARD;
                        nTransaction.ExpMonth = E_EMONTH;
                        nTransaction.ExpYear = E_EYEAR;
                        nTransaction.CurrencyId = mid.CurrencyId;
                        nTransaction.MidId = mid.MidId;

                        var rTransaction = _transRepo.CreateTransactionDebit(nTransaction, transactionAttemptDebit);

                        if (rTransaction.TransactionDebitId > 0)
                        {
                            action = "processing transaction for api integration. Transaction was successfully saved.";
                            transactionDebit.TransactionDebitId = rTransaction.TransactionDebitId;

                            if (((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId) == Enums.TransactionType.Declined)
                            {
                                response.POSWSResponse.Message = "Transaction failed. Please contact Support.";
                                response.POSWSResponse.ErrNumber = "2200.8";
                                response.POSWSResponse.Status = "Declined";
                                return response;
                            }
                            else
                            {
                                transactionAttemptDebitId = transactionAttemptDebit.TransactionAttemptDebitId;

                                #region MaxBank debit

                                if (mid.Switch.SwitchCode == "Maxbank")
                                {
                                    if (transactionAttemptDebit.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.BalanceInquiry))
                                    {
                                        action = "processing debit card.";

                                        DE_ISO8583 de = new DE_ISO8583();
                                        GatewayProcessor.Gateways gateway = new GatewayProcessor.Gateways();
                                        GatewayProcessor.VeritasPayment.CardDetails transData = new GatewayProcessor.VeritasPayment.CardDetails();

                                        transData.PrivateAdditionalData = Convert.ToString(request.CardDetails.EpbKsn.Length).PadLeft(4, '0') + request.CardDetails.EpbKsn + Convert.ToString(cardNumber.Length).PadLeft(4, '0') + cardNumber;
                                        transData.MerchantID = mid.Param_2;
                                        transData.TerminalID = mid.Param_6;

                                        decimal orgAmount = transactionAttemptDebit.Amount;
                                        decimal finalAmount = orgAmount * 100;

                                        try
                                        {
                                            transData.Amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                                        }
                                        catch
                                        {
                                            transData.Amount = finalAmount.ToString();
                                        }

                                        #region Generate System Trace Audit

                                        transData.SystemTraceAudit = SDGUtil.Functions.GenerateSystemTraceAudit();

                                        #endregion Generate System Trace Audit

                                        transData.CurrencyCode = mid.Currency.IsoCode;
                                        transData.CardNumber = cardNumber;
                                        transData.NameOnCard = request.CardDetails.NameOnCard;
                                        transData.Track2Data = track2.Replace('D', '=').TrimStart(';').TrimEnd('F').TrimEnd('?');
                                        transData.Track1Data = null;
                                        transData.ExpirationDate = (transactionDebit.ExpMonth == null || transactionDebit.ExpYear == null) ? null : transactionDebit.ExpYear + transactionDebit.ExpMonth;
                                        transData.PinBlock = request.CardDetails.Epb;

                                        //Fees
                                        transData.AmountTransactionFee = "00000000";
                                        transData.AccountType = Convert.ToString(rTransaction.AccountTypeId);

                                        action = "processing transaction for debit api integration. Transaction was successfully saved.";

                                        #region Test Response

                                        //transactionAttempt.TransactionAttemptDebitId = transactionAttemptId;
                                        //transactionAttempt.AuthNumber = "0011";
                                        //transactionAttempt.ReturnCode = "1111";
                                        //transactionAttempt.TraceNumber = transData.SystemTraceAudit;
                                        //transactionAttempt.ReferenceNumber = "00";
                                        //transactionAttempt.InvoiceNumber = "";
                                        //transactionAttempt.DisplayReceipt = "";
                                        //transactionAttempt.DisplayTerminal = "";
                                        //transactionAttempt.DateReceived = DateTime.Now;
                                        //transactionAttempt.DepositDate = DateTime.Now.AddYears(-100);
                                        //transactionAttempt.Notes += "API Debit Purchase Declined" + "Test Response";
                                        //response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttempt.TransactionTypeId);

                                        //response.POSWSResponse.ErrNumber = "";
                                        //response.POSWSResponse.Message = "TEST RESPONSE- NO SWITCH";
                                        //response.POSWSResponse.Status = "Declined";

                                        #endregion Test Response

                                        try
                                        {
                                            var apiResponse = gateway.ProcessPurchaseMaxbankGateway(transData, "INQUIRY");

                                            if (apiResponse.Result.Status == "Approved")
                                            {
                                                transactionAttemptDebit.TransactionAttemptDebitId = transactionAttemptDebitId;
                                                transactionAttemptDebit.AuthNumber = apiResponse.Result.AuthorizationID;
                                                transactionAttemptDebit.ReturnCode = apiResponse.Result.ReturnCode;
                                                transactionAttemptDebit.TraceNumber = apiResponse.Result.SytemTraceAudit;
                                                transactionAttemptDebit.RetrievalRefNumber = apiResponse.Result.Reference;
                                                transactionAttemptDebit.InvoiceNumber = DateTime.Now.ToString("YYYYMMdd");
                                                transactionAttemptDebit.DisplayReceipt = "";
                                                transactionAttemptDebit.DisplayTerminal = transData.TerminalID;
                                                transactionAttemptDebit.DateReceived = DateTime.Now;
                                                transactionAttemptDebit.DepositDate = DateTime.Now.AddYears(-100);
                                                transactionAttemptDebit.Notes = "API Debit Balance Approved";

                                                response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId);
                                                response.POSWSResponse.ErrNumber = "0";
                                                response.POSWSResponse.Message = "Transaction Successful.";
                                                response.POSWSResponse.Status = "Approved";

                                                try
                                                {
                                                    var resBalance = Functions.ParseBalance.ParseBalanceInquiry(apiResponse.Result.BalanceInqData);

                                                    response.CurrencyAvailableBalance = resBalance.CurrentCurrCode;
                                                    response.AvailableBalance = resBalance.AvailableBalance;
                                                    response.CurrencyCurrentBalance = resBalance.CurrentCurrCode;
                                                    response.CurrentBalance = resBalance.CurrentBalance;
                                                }
                                                catch
                                                {
                                                    response.AvailableBalance = "0.00";
                                                    response.CurrentBalance = "0.00";
                                                }
                                            }
                                            else
                                            {
                                                transactionAttemptDebit.TransactionAttemptDebitId = transactionAttemptDebitId;
                                                transactionAttemptDebit.AuthNumber = apiResponse.Result.AuthorizationID;
                                                transactionAttemptDebit.ReturnCode = apiResponse.Result.ReturnCode;
                                                transactionAttemptDebit.TraceNumber = apiResponse.Result.SytemTraceAudit;
                                                transactionAttemptDebit.ReferenceNumber = apiResponse.Result.Reference;
                                                transactionAttemptDebit.InvoiceNumber = "";
                                                transactionAttemptDebit.DisplayReceipt = "";
                                                transactionAttemptDebit.DisplayTerminal = "";
                                                transactionAttemptDebit.DateReceived = DateTime.Now;
                                                transactionAttemptDebit.DepositDate = DateTime.Now.AddYears(-100);
                                                transactionAttemptDebit.Notes = "API Debit Purchase Declined" + apiResponse.Result.Message;
                                                transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);

                                                response.POSWSResponse.ErrNumber = apiResponse.Result.ErrNumber;
                                                response.POSWSResponse.Message = apiResponse.Result.Message;
                                                response.POSWSResponse.Status = "Declined";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            // Log error
                                            var errorOnAction = "Error while " + action;
                                            var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction, "TransactionPurchaseDebit", "", "");

                                            response.POSWSResponse.Message = "Transaction failed. Please contact Support. Details:" + "Declined" + " " + errorOnAction + " " + ex.Message;
                                            response.POSWSResponse.ErrNumber = "2200.9";
                                            response.POSWSResponse.Status = "Declined";
                                            return response;
                                        }
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId);

                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        response.POSWSResponse.ErrNumber = "2200.11";
                                        return response;
                                    }
                                }

                                #endregion MaxBank debit

                                #region OFFLINE debit

                                else if (mid.Switch.SwitchCode == "Offline")
                                {

                                    if (transactionAttemptDebit.TransactionTypeId == Convert.ToInt32(SDGDAL.Enums.TransactionType.BalanceInquiry))
                                    {
                                        action = "processing debit card.";

                                        transactionAttemptDebit.TransactionAttemptDebitId = transactionAttemptDebitId;
                                        transactionAttemptDebit.AuthNumber = "0011";
                                        transactionAttemptDebit.ReturnCode = "00";
                                        transactionAttemptDebit.TraceNumber = "0000111";
                                        transactionAttemptDebit.ReferenceNumber = "00";
                                        transactionAttemptDebit.InvoiceNumber = "00000001";
                                        transactionAttemptDebit.DisplayReceipt = mid.Param_2;
                                        transactionAttemptDebit.DisplayTerminal = mid.Param_6;
                                        transactionAttemptDebit.DateReceived = DateTime.Now;
                                        transactionAttemptDebit.DepositDate = DateTime.Now.AddYears(-100);
                                        transactionAttemptDebit.Notes = "Offline Demo Successful";
                                        response.TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId);

                                        response.CurrencyAvailableBalance = "Php";
                                        response.AvailableBalance = "1780500.00";
                                        response.CurrencyCurrentBalance = "Php";
                                        response.CurrentBalance = "2000.50";
                                        response.POSWSResponse.ErrNumber = "0";
                                        response.POSWSResponse.Message = "Balance Inquiry - Offline SWITCH";
                                        response.POSWSResponse.Status = "Successful";
                                    }
                                    else
                                    {
                                        string type = Convert.ToString((SDGDAL.Enums.TransactionType)transactionAttemptDebit.TransactionTypeId);

                                        response.POSWSResponse.ErrNumber = "2200.9";
                                        response.POSWSResponse.Message = type + " Transaction not supported, please contact support.";
                                        response.POSWSResponse.Status = "Declined";
                                        return response;
                                    }
                                }
                                else
                                {
                                    // Invalid switch
                                    response.POSWSResponse.Message = "Transaction failed. Switch not yet supported. Please contact Support.";
                                    response.POSWSResponse.ErrNumber = "2200.10";
                                    response.POSWSResponse.Status = "Declined";
                                    return response;
                                }

                                #endregion OFFLINE debit

                                #region Decrypt Card Number

                                string Key = transactionAttemptDebit.TransactionDebit.Key.Key;
                                string KeyIV = transactionAttemptDebit.TransactionDebit.Key.IV;
                                string E_Card = transactionAttemptDebit.TransactionDebit.CardNumber;

                                String hashCardNumber = Utility.DecryptEncDataWithKeyAndIV(E_Card, Key, KeyIV);

                                transactionDebit.CardNumber = SDGUtil.Functions.HashCardNumber(hashCardNumber);

                                #endregion Decrypt Card Number

                                var nTransactionAttempt = _transRepo.UpdateTransactionAttemptDebit(transactionAttemptDebit);

                                response.RetrievalRefNumber = transactionAttemptDebit.ReferenceNumber;
                                response.TraceNumber = transactionAttemptDebit.TraceNumber;
                                response.MerchantId = transactionAttemptDebit.DisplayReceipt;
                                response.TerminalId = transactionAttemptDebit.DisplayTerminal;
                                response.Date = SDGUtil.Functions.Format_Datetime(transactionAttemptDebit.DateReceived);
                                response.Amount = Convert.ToString(transactionAttemptDebit.Amount);
                                response.AuthNumber = transactionAttemptDebit.AuthNumber;
                                response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transactionDebit.TransactionEntryTypeId);
                                response.AccountType = Convert.ToString((SDGDAL.Enums.AccountType)transactionDebit.AccountTypeId);
                                response.CardNumber = SDGUtil.Functions.HashCardNumber(transactionDebit.CardNumber);
                            }
                        }
                        else
                        {
                            response.POSWSResponse.ErrNumber = "2200.11";
                            response.POSWSResponse.Message = "Unknown error, please contact support while " + action;
                            response.POSWSResponse.Status = "Declined";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error please
                        var errorOnAction = "Error while " + action;

                        var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "VoidDebitTransaction", ex.StackTrace);

                        response.POSWSResponse.ErrNumber = "2200.12";
                        response.POSWSResponse.Message = "Unknown error, please contact support. Ref: " + errRefNumber + action;
                        response.POSWSResponse.Status = "Declined";
                    }

                    return response;

                    #endregion Handle Transaction response

                    #endregion Transaction
                }
                else
                {
                    response.POSWSResponse.ErrNumber = "2200.13";
                    response.POSWSResponse.Message = "Invalid System Mode.";
                    response.POSWSResponse.Status = "DECLINED";
                    return response;
                }
            }
            catch (Exception ex)
            {
                // Log error please
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "DebitBalanceInquiry", ex.Message);

                if (ex.InnerException != null)
                {
                    ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "DebitBalanceInquiry  ErrRef:" + errRefNumber, ex.Message);
                }

                response.POSWSResponse.ErrNumber = "2200.14";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }

        public BatchResponse BatchReport(BatchReportsRequest request)
        {
            string action = string.Empty;
            decimal sumTotal = 0;

            BatchResponse response = new BatchResponse();
            response.POSWSResponse = new POSWSResponse();

            action = "checking request";

            if (request.MobileAppTransType <= 0
                || string.IsNullOrEmpty(request.POSWSRequest.ActivationKey)
                || string.IsNullOrEmpty(request.POSWSRequest.RToken)
                || string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                || request.ReportsCriteria <= 0)
            {
                response.POSWSResponse.ErrNumber = "1001.1";
                response.POSWSResponse.Message = "Missing input";
                response.POSWSResponse.Status = "DECLINED";
                return response;
            }

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

                    action = "checking mid status before setting up transaction entry.";
                    var mid = new SDGDAL.Entities.Mid();

                    mid = _midsRepo.GetMidByPosIdAndCardTypeId(mobileApp.MobileAppId, Convert.ToInt32(Enums.CardTypes.Debit));

                    if (mid == null)
                    {
                        response.POSWSResponse.ErrNumber = "2300.1";
                        response.POSWSResponse.Message = "Mid not found.";
                        response.POSWSResponse.Status = "Declined";

                        return response;
                    }
                    else
                    {
                        if (!mid.IsActive || mid.IsDeleted)
                        {
                            response.POSWSResponse.ErrNumber = "2300.2";
                            response.POSWSResponse.Message = "Mid is Inactive.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }

                        if (!mid.Switch.IsActive)
                        {
                            response.POSWSResponse.ErrNumber = "2300.3";
                            response.POSWSResponse.Message = "Switch is Inactive.";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }
                    }

                    action = "retrieving transaction using MobileApp Id";

                    #region Debit

                    if (request.MobileAppTransType == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransType.Debit))
                    {
                        var transDebit = _batchRepo.GetDebitReportsByMobileAppId(request.MobileAppTransType, mobileApp.MobileAppId, mid.Param_6);

                        response.MerchantId = mid.Param_2;
                        response.TerminalId = mid.Param_6;
                        response.Host = "MAXBANK";
                        response.DateTime = Convert.ToString(DateTime.Now);

                        if (request.ReportsCriteria == Convert.ToDecimal(SDGDAL.Enums.ReportsCriteria.SummaryReport))
                        {
                            Sales sales = new Sales();
                            Classes.Void voidTrans = new Classes.Void();
                            Reversed reverse = new Reversed();

                            if (transDebit != null & transDebit.Count > 0)
                            {
                                sales.Total = transDebit.Where(d => d.TransactionTypeId == (int)SDGDAL.Enums.TransactionType.Sale).Sum(d => d.TransactionDebit.OriginalAmount);
                                sales.Count = transDebit.Where(d => d.TransactionTypeId == (int)SDGDAL.Enums.TransactionType.Sale).Count();
                                sales.Currency = mid.Currency.CurrencyCode;

                                voidTrans.Total = transDebit.Where(d => d.TransactionTypeId == (int)SDGDAL.Enums.TransactionType.Void).Sum(d => d.TransactionDebit.OriginalAmount);
                                voidTrans.Count = transDebit.Where(d => d.TransactionTypeId == (int)SDGDAL.Enums.TransactionType.Void).Count();
                                voidTrans.Currency = mid.Currency.CurrencyCode;

                                reverse.Total = transDebit.Where(d => d.TransactionTypeId == (int)SDGDAL.Enums.TransactionType.Reversed).Sum(d => d.TransactionDebit.OriginalAmount);
                                reverse.Count = transDebit.Where(d => d.TransactionTypeId == (int)SDGDAL.Enums.TransactionType.Reversed).Count();
                                reverse.Currency = mid.Currency.CurrencyCode;

                                sumTotal = transDebit.Sum(ts => ts.Amount);
                                response.BatchNumber = transDebit[0].BatchNumber;
                            }
                            else
                            {
                                sumTotal = 0;
                            }

                            response.Sales = sales;
                            response.Void = voidTrans;
                            response.Reversed = reverse;
                            response.Currency = mid.Currency.CurrencyCode;
                            response.TotalAmount = Convert.ToString(sumTotal);
                            response.TotalCount = Convert.ToString(transDebit.Count);
                            response.CardType = Convert.ToString(SDGDAL.Enums.CardTypes.Debit);
                            response.XmlDetailedReport = string.Empty;
                        }
                        else if (request.ReportsCriteria == Convert.ToDecimal(SDGDAL.Enums.ReportsCriteria.DetailedReport) || request.ReportsCriteria == Convert.ToDecimal(SDGDAL.Enums.ReportsCriteria.EndOfTheDay))
                        {
                            if (transDebit != null & transDebit.Count > 0)
                            {
                                response.BatchNumber = transDebit[0].BatchNumber;
                                var resultsTable = transDebit.Select(r => new
                                {
                                    TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)r.TransactionDebit.TransactionEntryTypeId),
                                    TransactionType = Convert.ToString((SDGDAL.Enums.TransactionType)r.TransactionTypeId),
                                    TransactionNumber = r.TransactionDebitId + "-" + r.TransactionAttemptDebitId,
                                    DateReceived = r.DateReceived,
                                    TraceNumber = r.TraceNumber,
                                    AuthNumber = r.AuthNumber,
                                    Amount = r.Amount,
                                    CardNumber = Utility.DecryptEncDataWithKeyAndIV(r.TransactionDebit.CardNumber, r.TransactionDebit.Key.Key, r.TransactionDebit.Key.IV)
                                }).OrderByDescending(x => x.DateReceived);

                                var xElement = new XElement("Reports",
                                               resultsTable.Select(
                                               trans => new XElement("Transactions",
                                               new XElement("CardType", Convert.ToString(SDGDAL.Enums.CardTypes.Debit)),
                                               new XElement("TransactionType", trans.TransactionType),
                                               new XElement("CardNumber", SDGUtil.Functions.HashCardNumber(trans.CardNumber)),
                                               new XElement("TransactionEntryType", trans.TransactionEntryType),
                                               new XElement("TransactionNumber", trans.TransactionNumber),
                                               new XElement("DateReceived", trans.DateReceived),
                                               new XElement("TraceNumber", trans.TraceNumber),
                                               new XElement("AuthNumber", trans.AuthNumber),
                                               new XElement("Amount", trans.Amount)
                                )));

                                sumTotal = resultsTable.Sum(ts => ts.Amount);
                                response.XmlDetailedReport = xElement.ToString();
                            }
                            else
                            {
                                sumTotal = 0;
                                response.XmlDetailedReport = string.Empty;
                            }

                            response.Currency = mid.Currency.CurrencyCode;
                            response.TotalAmount = Convert.ToString(sumTotal);
                            response.TotalCount = Convert.ToString(transDebit.Count);
                        }
                        else
                        {
                            response.POSWSResponse.ErrNumber = "2300.4";
                            response.POSWSResponse.Message = "Invalid Reports Criteria";
                            response.POSWSResponse.Status = "Declined";

                            return response;
                        }

                        response.POSWSResponse.ErrNumber = "0";
                        response.POSWSResponse.Message = "SUMMARY REPORT";
                        response.POSWSResponse.Status = "Approved";
                    }
                    #endregion

                    else
                    {
                        response.POSWSResponse.ErrNumber = "2300.5";
                        response.POSWSResponse.Message = "Invalid Transaction Type";
                        response.POSWSResponse.Status = "Declined";

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "BatchReport", ex.StackTrace);

                response.POSWSResponse.ErrNumber = "2300.3";
                response.POSWSResponse.Message = "Unknown error, please contact support. Ref: " + errRefNumber + " " + action + ex.Message + " " + ex.HResult;
                response.POSWSResponse.Status = "Declined";
            }

            return response;
        }

        public POSWSResponse BatchClose(BatchCloseRequest request)
        {
            string action = string.Empty;
            POSWSResponse response = new POSWSResponse();

            action = "checking request";

            if ( string.IsNullOrEmpty(request.BatchNumber)
                ||request.MobileAppTransType <= 0
                || string.IsNullOrEmpty(request.POSWSRequest.ActivationKey)
                || string.IsNullOrEmpty(request.POSWSRequest.RToken)
                || string.IsNullOrEmpty(request.POSWSRequest.SystemMode)
                || request.TotalCount <= 0)
            {
                response.ErrNumber = "1001.1";
                response.Message = "Missing Input";
                response.Status = "Declined";
            }

            try
            {
                if(request.POSWSRequest.SystemMode.ToUpper() == "LIVE")
                {
                    var mobileApp = new SDGDAL.Entities.MobileApp();
                    var account = new SDGDAL.Entities.Account();

                    action = "checking mobile app availability.";

                    response = mobileAppFunctions.CheckStatus(request.POSWSRequest.RToken, out mobileApp, out account);

                    if (response.Status == "Declined")
                    {
                        return response;
                    }

                    action = "checking batch number if already exists";
                    if (!_batchRepo.IsBatchNumberAvailable(request.BatchNumber, request.MobileAppTransType, mobileApp.MobileAppId))
                    {
                        response.ErrNumber = "2400.1";
                        response.Message = "Batch Close does not successfully saved.";
                        response.Status = "Declined";
                        return response;
                    }

                    if (request.MobileAppTransType == Convert.ToDecimal(SDGDAL.Enums.MobileAppTransType.Debit))
                    {
                        var batchData = new SDGDAL.Entities.Batch();
                        batchData.MerchantId = mobileApp.MerchantBranchPOS.MerchantBranch.Merchant.MerchantId;
                        batchData.MobileAppId = mobileApp.MobileAppId;
                        batchData.Currency = request.Currency;
                        batchData.TotalAmount = request.TotalAmount;
                        batchData.TotalCount = request.TotalCount;
                        batchData.PaymentTypeId = request.MobileAppTransType;
                        batchData.BatchNumber = request.BatchNumber;

                        var savedRespose = _batchRepo.InsertBatchClose(batchData);

                        if (savedRespose.BatchId > 0)
                        {
                            response.ErrNumber = "0";
                            response.Message = "Batch Close successfully saved.";
                            response.Status = "Approved";
                        }
                        else
                        {
                            response.ErrNumber = "2400.2";
                            response.Message = "Batch Close does not successfully saved.";
                            response.Status = "Declined";
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2400.3";
                        response.Message = "Mobile Transaction type not supported.";
                        response.Status = "Declined";
                    }
                }
                else
                {
                    response.ErrNumber = "2400.4";
                    response.Message = "System Mode is not supported.";
                    response.Status = "Declined";
                }
            }
            catch(Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "BatchClose", ex.StackTrace);

                response.ErrNumber = "2400.5";
                response.Message = "Unknown error, please contact support. Ref: " + errRefNumber + " " + action + ex.Message;
                response.Status = "Declined";
            }

            return response;
        }

        public ReversalResponse ReversalTransactionDebit(ReversalRequest request)
        {
            string action = string.Empty;

            ReversalResponse response = new ReversalResponse();

            response.POSWSResponse = new POSWSResponse();

            if (string.IsNullOrEmpty(request.SystemTraceNumber))
            {
                response.POSWSResponse.ErrNumber = "1001.1";
                response.POSWSResponse.Message = "Missing input";
                response.POSWSResponse.Status = "DECLINED";
                return response;
            }

            response.POSWSResponse.ErrNumber = "0";

            try
            {
                var mobileApp = new SDGDAL.Entities.MobileApp();
                var account = new SDGDAL.Entities.Account();
                var transactionAttemptDebit = new SDGDAL.Entities.TransactionAttemptDebit();

                action = "checking trace number is existing";
                transactionAttemptDebit = _transRepo.GetDebitTransactionByTraceNumber(request.SystemTraceNumber);

                if (transactionAttemptDebit != null)
                {
                    action = "retrieving mobile App details using Activation Code";
                    mobileApp = _mAppRepo.GetMobileAppFullDetailsByActivationCode(transactionAttemptDebit.MobileApp.ActivationCode.Replace("-", ""));

                    MobileAppMethods mobileAppMethods = new MobileAppMethods();

                    response.POSWSResponse.Status = "Reversed";
                    transactionAttemptDebit.ReturnCode = request.ReturnCode;
                    transactionAttemptDebit.Notes = "Reversed " + request.Message;
                    transactionAttemptDebit.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Reversed);
                    transactionAttemptDebit.DateReceived = DateTime.Now;
                    transactionAttemptDebit.DisplayReceipt = request.MerchantId;
                    transactionAttemptDebit.DisplayTerminal = request.TerminalId;
                    transactionAttemptDebit.ReturnCode = request.ReturnCode;
                    transactionAttemptDebit.AuthNumber = request.AuthorizationID;
                    transactionAttemptDebit.RetrievalRefNumber = request.RetrievalReferenceNumber;
                    transactionAttemptDebit.BatchNumber = _batchRepo.GenerateBatchNumber(mobileApp.MobileAppId, Convert.ToInt32(SDGDAL.Enums.Ref_PaymentType.Debit));

                    var nTransactionAttempt = _transRepo.UpdateTransactionAttemptDebit(transactionAttemptDebit);

                    response.MerchantId = transactionAttemptDebit.DisplayReceipt;
                    response.TerminalId = transactionAttemptDebit.DisplayTerminal;
                    response.Currency = transactionAttemptDebit.TransactionDebit.Currency.CurrencyCode;
                    response.TransactionNumber = Convert.ToString(transactionAttemptDebit.TransactionDebitId) + "-" + Convert.ToString(transactionAttemptDebit.TransactionAttemptDebitId);
                    response.AuthNumber = transactionAttemptDebit.AuthNumber;
                    response.TraceNumber = transactionAttemptDebit.TraceNumber;
                    response.SequenceNumber = transactionAttemptDebit.SeqNumber;
                    response.BatchNumber = transactionAttemptDebit.BatchNumber;
                    response.Timestamp = SDGUtil.Functions.Format_Datetime(transactionAttemptDebit.DateReceived);
                    response.TransactionEntryType = Convert.ToString((SDGDAL.Enums.TransactionEntryType)transactionAttemptDebit.TransactionDebit.TransactionEntryTypeId);
                    response.Total = Convert.ToDecimal(transactionAttemptDebit.Amount.ToString("N2"));
                }
                else
                {
                    response.POSWSResponse.Status = "Declined";
                    response.POSWSResponse.ErrNumber = "2500.1";
                    response.POSWSResponse.Message = "Invalid Trace Number.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;
                var errRefNumber = ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.Message, "DebitReversalTransaction", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    ApplicationLog.LogError("SDGWebService", errorOnAction + "\n" + ex.InnerException.Message, "DebitReversalTransaction  ErrRef:" + errRefNumber, ex.InnerException.StackTrace);
                }

                response.POSWSResponse.ErrNumber = "2500.2";
                response.POSWSResponse.Message = "Unknown error, please contact support. Reference Number: " + errRefNumber + " " + action;
                response.POSWSResponse.Status = "DECLINED";
            }

            return response;
        }

        private POSWSResponse GetCardDetails(CardDetails cd, SDGDAL.Enums.SwipeDevice swipeDevice, SDGDAL.Enums.CardAction cardAction,
           out string track1, out string track2, out string cardNumber, out string nameOnCard,
           out string expDate, out string expYear, out string expMonth, out ClassTLV emvDataResult)
        {
            POSWSResponse response = new POSWSResponse();
            WisepadLayer.WisePad wisepad = new WisepadLayer.WisePad();
            RamblerLayer1.Class1 rambler1 = new RamblerLayer1.Class1();
            MobileAppMethods mobileAppMethods = new MobileAppMethods();
            TLVParser tlvParser = new TLVParser();
            emvDataResult = new ClassTLV();
            track1 = "";
            track2 = "";
            cardNumber = "";
            nameOnCard = "";
            expDate = "";
            expYear = "";
            expMonth = "";
            string bdk = System.Configuration.ConfigurationManager.AppSettings["BDK"].ToString();
            switch (swipeDevice)
            {
                #region WisePad1 DeviceId = 3
                case SDGDAL.Enums.SwipeDevice.WisePad1:
                    string wisepadKey = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1"].ToString();
                    string wisepadKeyTlvEmv = System.Configuration.ConfigurationManager.AppSettings["DKWisepad1Emv"].ToString();

                    if (cardAction == Enums.CardAction.EMV)
                    {
                        string decryptEmvIcc = wisepad.DecryptC5EmvData(cd.EmvICCData, wisepadKeyTlvEmv);

                        var resultTlv = TLVParser.DecodeTLV(decryptEmvIcc);
                        emvDataResult.EmvIccData = resultTlv.EmvIccData.ToUpper();

                        emvDataResult.SubField1Data = resultTlv.SubField1Data;
                        emvDataResult.SubField2Data = resultTlv.SubField2Data;
                        emvDataResult.TrackData = resultTlv.TrackData;
                        track2 = resultTlv.Track2;
                        cardNumber = resultTlv.CardNumber;
                        expYear = resultTlv.ExpiryYear;
                        expYear = resultTlv.ExpiryMonth;
                    }
                    else if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        string keyWisepad2 = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);
                        string decryptedTrack2 = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, keyWisepad2);

                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(decryptedTrack2);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.1";
                        response.Message = "Invalid Swipe device";
                        response.Status = "Declined";
                        return response;
                    }

                    if (string.IsNullOrEmpty(track2))
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "No track data found on the card.";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region WisePad2 DeviceId = 4
                case SDGDAL.Enums.SwipeDevice.WisePad2:
                    string key = WisepadLayer.DUKPTServer.GetDataKey(cd.Ksn, bdk);

                    if (cardAction == Enums.CardAction.EMV)
                    {
                        string decryptedTlv = WisepadLayer.TripleDES.decrypt_CBC(cd.EmvICCData, key);

                        var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                        emvDataResult.EmvIccData = resultTlv.EmvIccData;
                        emvDataResult.SubField1Data = resultTlv.SubField1Data;
                        emvDataResult.SubField2Data = resultTlv.SubField2Data;
                        emvDataResult.TrackData = resultTlv.TrackData;
                        track2 = resultTlv.Track2;
                        cardNumber = resultTlv.CardNumber;
                        expYear = resultTlv.ExpiryYear;
                        expMonth = resultTlv.ExpiryMonth;
                    }
                    else if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        string decryptedTrack2 = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, key);
                        var resWisepadTrack2 = TLVParser.ParseWisePadTrack2(decryptedTrack2);

                        if (resWisepadTrack2.Track2 != null)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.1";
                        response.Message = "Invalid Swipe device";
                        response.Status = "Declined";
                        return response;
                    }

                    if (string.IsNullOrEmpty(track2))
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "No track data found on the card.";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                #region QPOS DeviceId = 11
                case SDGDAL.Enums.SwipeDevice.QPosMini:
                    string iPek = ConfigurationManager.AppSettings["IPEK"].ToString();
                    string keyIpek = DSPREADQPosLayer.EMVSwipeTLV.DUKPTServer.GetDataKeyFromIPEK(cd.Ksn, iPek);

                    if (cardAction == Enums.CardAction.EMV)
                    {
                        string decryptedTlv = DSPREADQPosLayer.EMVSwipeTLV.TripleDES.decrypt_CBC(cd.EmvICCData, keyIpek);

                        var resultTlv = TLVParser.DecodeTLV(decryptedTlv);
                        emvDataResult.EmvIccData = decryptedTlv;
                        emvDataResult.SubField1Data = resultTlv.SubField1Data;
                        emvDataResult.SubField2Data = resultTlv.SubField2Data;
                        emvDataResult.TrackData = resultTlv.TrackData;
                        track2 = resultTlv.Track2;
                        cardNumber = resultTlv.CardNumber;
                        expYear = resultTlv.ExpiryYear;
                        expMonth = resultTlv.ExpiryMonth;
                    }
                    else if (cardAction == Enums.CardAction.SwipeCard)
                    {
                        string decryptedTrack2 = WisepadLayer.TripleDES.decrypt_CBC(cd.Track2, keyIpek);

                        var resWisepadTrack2 = TLVParser.ParseQPOSTrack2(decryptedTrack2);

                        if (resWisepadTrack2.Track2 != null && resWisepadTrack2.CardNumber.Length > 11)
                        {
                            cardNumber = resWisepadTrack2.CardNumber;
                            track2 = resWisepadTrack2.Track2;
                        }
                        else
                        {
                            response.ErrNumber = "2100.1";
                            response.Message = "Cannot read Track Data.";
                            response.Status = "Declined";
                            return response;
                        }
                    }
                    else
                    {
                        response.ErrNumber = "2100.1";
                        response.Message = "Invalid Swipe device";
                        response.Status = "Declined";
                        return response;
                    }

                    if (string.IsNullOrEmpty(track2))
                    {
                        response.ErrNumber = "2100.2";
                        response.Message = "No track data found on the card.";
                        response.Status = "Declined";
                        return response;
                    }

                    break;
                #endregion

                default:
                    response.ErrNumber = "2100.1";
                    response.Message = "Invalid Device";
                    response.Status = "Declined";
                    return response;
            }

            return response;
        }
    }


}