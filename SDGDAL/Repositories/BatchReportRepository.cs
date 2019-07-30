using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGDAL.Repositories
{
    public class BatchReportRepository
    {
        #region Debit Batch Reports
        public List<TransactionAttemptDebit> GetDebitReportsByMobileAppId(int paymentId, int mobileAppId, string terminalId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var batchData = CheckBatchCloseByMobileAppIdAndPaymentTypeId(paymentId, mobileAppId);

                    if(batchData == null)
                    {
                        var trans = context.TransactionAttemptDebit
                            .Include("TransactionDebit")
                            .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                            .Include("TransactionDebit.MerchantPOS")
                            .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                            .Include("TransactionDebit.TransactionEntryType")
                            .Include("TransactionDebit.Key")
                            .Include("TransactionDebit.Currency")
                            .Include("TransactionSignature")
                            .Where(t => t.MobileAppId == mobileAppId && t.ReturnCode == "00" && t.TransactionTypeId == 3 && t.DisplayTerminal == terminalId).OrderByDescending(t => t.DateReceived).ToList();

                        return trans.Where(x => x.Amount.ToString() != "0.00").ToList();
                    }
                    else
                    {
                        var trans = context.TransactionAttemptDebit
                            .Include("TransactionDebit")
                            .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                            .Include("TransactionDebit.MerchantPOS")
                            .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                            .Include("TransactionDebit.TransactionEntryType")
                            .Include("TransactionDebit.Key")
                            .Include("TransactionDebit.Currency")
                            .Include("TransactionSignature")
                            .Where(t => t.MobileAppId == mobileAppId && t.ReturnCode == "00" && t.TransactionTypeId == 3 && t.DisplayTerminal == terminalId && t.DateReceived > batchData.BatchDate).OrderByDescending(t => t.DateReceived).ToList();

                        return trans.Where(x => x.Amount.ToString() != "0.00").ToList();
                    }
                    
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Batch InsertBatchClose(Batch batch)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        batch.BatchNumber = batch.BatchNumber;
                        batch.BatchDate = DateTime.Now;
                        var savedBatchClose = context.Batch.Add(batch);
                        context.SaveChanges();

                        trans.Commit();

                        return savedBatchClose;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public string GenerateBatchNumber(int mobileAppId, int paymentTypeId)
        {
            string batchNumber = string.Empty;
            var checkData = CheckBatchHasData(mobileAppId, paymentTypeId);
            if (checkData == null)
            {
                batchNumber = "000001";
            }
            else if(checkData.BatchNumber == "999999")
            {
                batchNumber = "000001";
            }
            else
            {
                batchNumber = Convert.ToString(Convert.ToInt32(checkData.BatchNumber) + 1).PadLeft(6, '0');
            }

            return batchNumber;
        }

        public Batch CheckBatchHasData(int mobileAppId, int paymentTypeId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    return context.Batch.OrderByDescending(b => b.BatchId)
                           .Take(1)
                           .Where(b => b.MobileAppId == mobileAppId && b.PaymentTypeId == paymentTypeId)
                           .ToList().FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Batch CheckBatchCloseByMobileAppIdAndPaymentTypeId(int paymentId, int mobileAppId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var batchData =  context.Batch.Where(b => b.PaymentTypeId == paymentId && b.MobileAppId == mobileAppId)
                                    .OrderByDescending(b => b.BatchId)
                                    .Take(1)
                                    .ToList().FirstOrDefault();

                    return batchData;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public bool IsBatchNumberAvailable(string batchNumber, int paymentTypeId, int mobileAppId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Batch.SingleOrDefault(d => d.BatchNumber == batchNumber && d.PaymentTypeId == paymentTypeId && d.MobileAppId == mobileAppId) == null;
            }
        }

        #endregion
    }
}
