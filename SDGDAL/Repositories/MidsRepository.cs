using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MidsRepository
    {
        public Mid GetMidByPosIdAndCardTypeId(int posId, int cardTypeId)
        {
            using (DataContext context = new DataContext())
            {
                var mid = context.Mids
                                    .Include("Switch")
                                    .Include("Currency")
                                    .Include("Merchant")
                                    .Include("Merchant.ContactInformation")
                                    .Include("Merchant.ContactInformation.Country")
                                    .SingleOrDefault(m =>
                                        m.CardTypeId == cardTypeId
                                        && m.MerchantBranchPOSs.Any(mbp => mbp.MerchantBranchPOSId == posId
                                                                            && mbp.IsActive
                                                                            && !mbp.IsDeleted));
                return mid;
            }
        }

        public Mid CreateMid(Mid mid)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var savedMid = context.Mids.Add(mid);
                    context.SaveChanges();

                    trans.Commit();
                    return savedMid;
                }
            }
        }

        public List<Mid> GetMidByMerchantId(int merchantID, int cardTypeID, string Action, int midID)
        {
            List<Mid> lMid = new List<Mid>();
            using (DataContext context = new DataContext())
            {
                switch (Action)
                {
                    case "Adding":
                        lMid = context.Mids.Where<Mid>(m => m.MerchantId == merchantID && m.CardTypeId == cardTypeID).ToList();
                        break;

                    case "Updating":
                        lMid = context.Mids.Where<Mid>(m => m.MerchantId == merchantID && m.CardTypeId == cardTypeID && m.MidId != midID).ToList();
                        break;
                }
                return lMid;
            }
        }

        public Mid UpdateMids(Dictionary<string, object> param)
        {
            using (DataContext context = new DataContext())
            {
                List<SqlParameter> ps = new List<SqlParameter>();

                param.Add("@UpdateStatusMessage out", "");

                string sqlQuery = "sp_UpdateMidsInfo ";
                foreach (var key in param.Keys)
                {
                    if (!key.Contains("out"))
                    {
                        sqlQuery += key + ", ";
                        var sqlParam = new SqlParameter(key.Replace("@", ""), param[key]);

                        if (key.EndsWith("Id"))
                        {
                            sqlParam.SqlValue = Convert.ToInt32(sqlParam.Value);
                            sqlParam.SqlDbType = System.Data.SqlDbType.Int;
                        }

                        ps.Add(sqlParam);
                    }
                    else
                    {
                        sqlQuery += key;
                        var newParam = new SqlParameter(key.Replace("@", "").Replace("out", ""), param[key]);
                        newParam.Direction = System.Data.ParameterDirection.Output;
                        ps.Add(newParam);
                    }
                }

                var result = context.Mids.SqlQuery(sqlQuery, ps.ToArray()).SingleOrDefault();

                return result;
            }
        }

        public Mid UpdateMids(Mid mid)
        {
            using (DataContext context = new DataContext())
            {
                //Update Mid
                var um = context.Mids.Attach(mid);

                var entryM = context.Entry(mid);

                //Update Transaction Charges
                var ut = context.TransactionCharges.Attach(mid.TransactionCharges);

                var entryT = context.Entry(mid.TransactionCharges);

                entryM.Property(m => m.MidName).IsModified = true;
                entryM.Property(m => m.IsActive).IsModified = true;
                entryM.Property(m => m.SwitchId).IsModified = true;
                entryM.Property(m => m.CardTypeId).IsModified = true;
                entryM.Property(m => m.CurrencyId).IsModified = true;
                entryM.Property(m => m.Param_1).IsModified = true;
                entryM.Property(m => m.Param_2).IsModified = true;
                entryM.Property(m => m.Param_3).IsModified = true;
                entryM.Property(m => m.Param_4).IsModified = true;
                entryM.Property(m => m.Param_5).IsModified = true;
                entryM.Property(m => m.Param_6).IsModified = true;
                entryM.Property(m => m.Param_7).IsModified = true;
                entryM.Property(m => m.Param_8).IsModified = true;
                entryM.Property(m => m.Param_9).IsModified = true;
                entryM.Property(m => m.Param_10).IsModified = true;
                entryM.Property(m => m.Param_11).IsModified = true;
                entryM.Property(m => m.Param_12).IsModified = true;
                entryM.Property(m => m.Param_13).IsModified = true;
                entryM.Property(m => m.Param_14).IsModified = true;
                entryM.Property(m => m.Param_15).IsModified = true;
                entryM.Property(m => m.Param_16).IsModified = true;
                entryM.Property(m => m.Param_17).IsModified = true;
                entryM.Property(m => m.Param_18).IsModified = true;
                entryM.Property(m => m.Param_19).IsModified = true;
                entryM.Property(m => m.Param_20).IsModified = true;
                entryM.Property(m => m.Param_21).IsModified = true;
                entryM.Property(m => m.Param_22).IsModified = true;
                entryM.Property(m => m.Param_23).IsModified = true;
                entryM.Property(m => m.Param_24).IsModified = true;
                entryM.Property(m => m.SetLikeMerchantId).IsModified = true;
                entryM.Property(m => m.SetLikeTerminalId).IsModified = true;
                entryM.Property(m => m.NeedAddBulk).IsModified = true;
                entryM.Property(m => m.NeedAddTerminal).IsModified = true;
                entryM.Property(m => m.NeedDeleteBulk).IsModified = true;
                entryM.Property(m => m.NeedUpdateBulk).IsModified = true;
                entryM.Property(m => m.AcquiringBin).IsModified = true;
                entryM.Property(m => m.City).IsModified = true;
                entryM.Property(m => m.Country).IsModified = true;

                entryT.Property(t => t.DiscountRate).IsModified = true;
                entryT.Property(t => t.CardNotPresent).IsModified = true;
                entryT.Property(t => t.Declined).IsModified = true;
                entryT.Property(t => t.eCommerce).IsModified = true;
                entryT.Property(t => t.PreAuth).IsModified = true;
                entryT.Property(t => t.Capture).IsModified = true;
                entryT.Property(t => t.Purchased).IsModified = true;
                entryT.Property(t => t.Refund).IsModified = true;
                entryT.Property(t => t.Void).IsModified = true;
                entryT.Property(t => t.CashBack).IsModified = true;
    
                context.SaveChanges();

                return mid;
            }
        }

        public MidsMerchantBranchPOSs UpdateMidsMerchantBranchPOSs(MidsMerchantBranchPOSs mmbPOS)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var mmbp = context.MidsMerchantBranchPOSs.Attach(mmbPOS);

                    var entryMMBP = context.Entry(mmbPOS);

                    entryMMBP.Property(m => m.IsDeleted).IsModified = true;

                    context.SaveChanges();

                    return mmbPOS;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<Mid> GetAllMids(int merchantId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> mid = new List<Entities.Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Currency")
                    .Include("Switch").Where(m => m.MerchantId == merchantId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                mid = q.ToList();
            }

            return mid;
        }

        public List<Mid> GetAllMids(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Mids.OrderByDescending(m => m.MerchantId == merchantId).ToList();
            }
        }

        public List<Mid> GetAllMidsByMerchantId(int merchantId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Mids.Where(m => m.MerchantId == merchantId).ToList();
            }
        }

        public List<Mid> GetAllMidsByMidID(int mId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            using (DataContext context = new DataContext())
            {
                var q = context.Mids.Include("CardType")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs").Where(m => m.MidId == mId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                return q.ToList();
            }
        }

        public bool IsMidsMerchantBranchesExist(int midId)
        {
            using (DataContext context = new DataContext())
            {
                return context.MidsMerchantBranches.SingleOrDefault(mmb => mmb.MidId == midId) == null;
            }
        }

        #region Get Merchant Details

        public Mid GetDetailsbyMidId(int mid)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var mids = context.Mids
                        .Include("Merchant")
                        .Include("Switch")
                        .Include("CardType")
                        .Include("Currency")
                        .Include("TransactionCharges")
                        .SingleOrDefault(u => u.MidId == mid);

                    return mids;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion Get Merchant Details

        #region Assign Mids

        public MidsMerchantBranchPOSs AssignMerchantBranchPOSs(MidsMerchantBranches midsBranch, MidsMerchantBranchPOSs midsPOSs)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    if (midsBranch != null)
                    {
                        var assignMidb = context.MidsMerchantBranches.Add(midsBranch);
                        context.SaveChanges();
                    }

                    var assignMidp = context.MidsMerchantBranchPOSs.Add(midsPOSs);
                    context.SaveChanges();

                    trans.Commit();
                    return assignMidp;
                }
            }
        }

        #endregion Assign Mids

        #region CTPayment MID for Bulk Merchant

        public List<Mid> GetMerchantMidAddedByResellerId(int resellerId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> mid = new List<SDGDAL.Entities.Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Merchant.ContactInformation.Country")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs")
                    .Where(r => r.Merchant.ResellerId == resellerId && r.Switch.SwitchCode == "CTPayment" && (r.NeedAddBulk == true || r.NeedAddTerminal == true || r.Merchant.NeedAddToCT == true)).Distinct();

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                mid = q.ToList();
            }

            return mid;
        }

        public List<Mid> GetMerchantMidUpdatedByResellerIdAndMerchant(int resellerId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> mid = new List<SDGDAL.Entities.Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Merchant.ContactInformation.Country")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs")
                    .Where(r => r.Merchant.ResellerId == resellerId && r.Switch.SwitchCode == "CTPayment" && r.Merchant.NeedUpdateToCT == true && r.Merchant.NeedAddToCT == false);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                mid = q.ToList();
            }

            return mid;
        }

        public List<Mid> GetMerchantMidUpdatedByResellerIdAndTerminal(int resellerId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> mid = new List<SDGDAL.Entities.Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Merchant.ContactInformation.Country")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs")
                    .Where(r => r.Merchant.ResellerId == resellerId && r.Switch.SwitchCode == "CTPayment" && r.NeedUpdateBulk == true && r.NeedAddTerminal == false);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                mid = q.ToList();
            }

            return mid;
        }

        public List<Mid> GetMerchantMidDeletedByResellerIdAndCreditMerchant(int resellerId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> mid = new List<SDGDAL.Entities.Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Merchant.ContactInformation.Country")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs")
                    .Where(r => r.Merchant.ResellerId == resellerId && r.Switch.SwitchCode == "CTPayment" && r.NeedAddBulk == false);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                mid = q.ToList();
            }

            return mid;
        }

        public List<Mid> GetMerchantMidDeletedByResellerIdAndTerminal(int resellerId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> mid = new List<SDGDAL.Entities.Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Merchant.ContactInformation.Country")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs")
                    .Where(r => r.Merchant.ResellerId == resellerId && r.Switch.SwitchCode == "CTPayment" && r.NeedAddTerminal == false);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                mid = q.ToList();
            }

            return mid;
        }

        public List<Mid> GetMerchantMidDeletedByResellerIdAndMerchant(int resellerId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> mid = new List<SDGDAL.Entities.Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Merchant.ContactInformation.Country")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs")
                    .Where(r => r.Merchant.ResellerId == resellerId && r.Switch.SwitchCode == "CTPayment" && r.Merchant.NeedAddToCT == false);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                mid = q.ToList();
            }

            return mid;
        }

        public List<Mid> GetAllListMidsToBeAdded(int merchantId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> merchant = new List<Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids
                    .Include("CardType")
                    .Include("Merchant")
                    .Include("Switch")
                    .Where(m => m.MerchantId == merchantId && m.Merchant.MerchantName.Contains(search)
                                           && m.Switch.SwitchCode == "CTPayment" && (m.NeedAddBulk == true || m.Merchant.NeedAddToCT == true || m.NeedAddTerminal == true)
                                           && m.IsDeleted == false);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                merchant = q.ToList();
            }

            return merchant;
        }

        public List<Mid> GetUpdatedMerchantBulk(int merchantId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> merchant = new List<Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids
                    .Include("CardType")
                    .Include("Merchant")
                    .Include("Switch")
                    .Where(m => m.MerchantId == merchantId && m.Merchant.MerchantName.Contains(search)
                                           && m.Switch.SwitchCode == "CTPayment" && m.IsDeleted == false && (m.Merchant.NeedUpdateToCT == true || m.NeedUpdateBulk == true));

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                merchant = q.ToList();
            }

            return merchant;
        }

        public List<Mid> GetDeletedMerchantBulk(int merchantId, string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Mid> merchant = new List<Mid>();

            using (DataContext context = new DataContext())
            {
                var q = context.Mids
                    .Include("CardType")
                    .Include("Merchant")
                    .Include("Switch")
                    .Where(m => m.MerchantId == merchantId && m.Merchant.MerchantName.Contains(search)
                                           && m.Switch.SwitchCode == "CTPayment" && (m.NeedAddBulk == false || m.Merchant.NeedAddToCT == false || m.NeedAddTerminal == false));

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                merchant = q.ToList();
            }

            return merchant;
        }

        public Mid UpdateMidNeedAddBulk(bool needAddBulk, int merchantId, string cardType)
        {
            using (var db = new DataContext())
            {
                try
                {
                    var mid = db.Mids.Include("CardType").Single(m => m.MerchantId == merchantId && m.CardType.IsoCode == cardType && m.Switch.SwitchCode == "CTPayment");

                    mid.NeedAddBulk = needAddBulk;

                    db.SaveChanges();

                    return mid;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Mid UpdateMidNeedAddTerminal(bool command, int midId, string tid)
        {
            using (var db = new DataContext())
            {
                try
                {
                    var mid = db.Mids.Single(m => m.MidId == midId && m.Param_6 == tid);

                    mid.NeedAddTerminal = command;

                    db.SaveChanges();

                    return mid;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Mid UpdateMidNeedUpdateBulk(bool command, int midId, string terminalId)
        {
            using (var db = new DataContext())
            {
                try
                {
                    var mid = db.Mids.Single(m => m.MidId == midId && m.Param_6 == terminalId);

                    mid.NeedUpdateBulk = command;

                    db.SaveChanges();

                    return mid;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Mid GetDetailsbyParam2AndSwitchCode(string param2)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var mids = context.Mids
                        .Include("Merchant")
                        .Include("Switch")
                        .FirstOrDefault(u => u.Param_2 == param2 && u.Switch.SwitchCode == "CTPayment");

                    return mids;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Mid GetDetailsbyParam2AndTerminalId(string param2, string param6)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var mids = context.Mids
                        .Include("Merchant")
                        .Include("Switch")
                        .FirstOrDefault(u => u.Param_2 == param2 && u.Param_6 == param6 && u.Switch.SwitchCode == "CTPayment");

                    return mids;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion CTPayment MID for Bulk Merchant

        #region CTPayment Send Update Merchant Boarding

        public Mid GetAllMerchantUpdateDetailsToCT(int midId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var merchant = context.Mids.Include("CardType")
                     .Include("Merchant")
                     .Include("Merchant.Reseller")
                     .Include("Merchant.ContactInformation")
                     .Include("Merchant.Reseller.Partner")
                     .Include("Merchant.ContactInformation.Country")
                     .Include("Currency")
                     .Include("Switch")
                     .Include("MerchantBranchPOSs")
                     .FirstOrDefault(r => r.MidId == midId && r.Switch.SwitchCode == "CTPayment" && r.Merchant.NeedUpdateToCT == true && r.Merchant.NeedAddToCT == false);

                    return merchant;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Mid GetAllMerchantTerminalUpdateToCT(int midId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var merchant = context.Mids.Include("CardType")
                     .Include("Merchant")
                     .Include("Merchant.Reseller")
                     .Include("Merchant.ContactInformation")
                     .Include("Merchant.Reseller.Partner")
                     .Include("Merchant.ContactInformation.Country")
                     .Include("Currency")
                     .Include("Switch")
                     .Include("MerchantBranchPOSs")
                     .FirstOrDefault(m => m.MidId == midId && m.Switch.SwitchCode == "CTPayment" && m.NeedUpdateBulk == true && m.NeedAddTerminal == false);

                    return merchant;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        #endregion CTPayment Send Update Merchant Boarding

        #region CTPayment Send Delete Merchant Boarding

        public Mid GetAllCreditCardsToBeDeletedToCT(int midId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var ccard = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Merchant.ContactInformation.Country")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs")
                    .FirstOrDefault(r => r.MidId == midId && r.Switch.SwitchCode == "CTPayment" && r.NeedAddBulk == false);

                    return ccard;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Mid GetAllTerminalsToBeDeletedToCT(int midId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var terminals = context.Mids.Include("CardType")
                    .Include("Merchant")
                    .Include("Merchant.Reseller")
                    .Include("Merchant.ContactInformation")
                    .Include("Merchant.Reseller.Partner")
                    .Include("Merchant.ContactInformation.Country")
                    .Include("Currency")
                    .Include("Switch")
                    .Include("MerchantBranchPOSs")
                    .FirstOrDefault(r => r.MidId == midId && r.Switch.SwitchCode == "CTPayment" && r.NeedAddTerminal == false);

                    return terminals;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Mid GetAllMerchantsToBeDeletedToCT(int midId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var merchant = context.Mids.Include("CardType")
                     .Include("Merchant")
                     .Include("Merchant.Reseller")
                     .Include("Merchant.ContactInformation")
                     .Include("Merchant.Reseller.Partner")
                     .Include("Merchant.ContactInformation.Country")
                     .Include("Currency")
                     .Include("Switch")
                     .Include("MerchantBranchPOSs")
                     .FirstOrDefault(r => r.MidId == midId && r.Switch.SwitchCode == "CTPayment" && r.Merchant.NeedAddToCT == false);
                    return merchant;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        #endregion CTPayment Send Delete Merchant Boarding

        public bool IsSameCardType(string mid, int cardTypeId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Mids.FirstOrDefault(m => m.Param_2 == mid && m.SwitchId == 22 && m.CardTypeId == cardTypeId) == null;
            }
        }

        public List<Mid> CheckCardType(int mId, int cardTypeId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Mids.Where(m => m.MerchantId == mId && m.SwitchId == 22
                    && m.CardTypeId != cardTypeId).ToList();
            }
        }

        public List<Mid> GetMidsBySwitchIdAndMerchantIdAndCardTypeId(int merchantId, int sId, int cTypeId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Mids.Where(m => (m.SwitchId == sId) && (m.MerchantId == merchantId) && (m.CardTypeId != cTypeId)).ToList();
            }
        }

        public List<Mid> GetMidsByMerchId(int merchantId, int cardTypeId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Mids.Where(m => m.MerchantId == merchantId && m.CardTypeId != cardTypeId).ToList();
            }
        }

        public bool CheckTerminalIdIsExist(int tId)
        {
            string terminalId = Convert.ToString(tId);

            using (DataContext context = new DataContext())
            {
                return context.Mids.SingleOrDefault(m => m.Param_6 == terminalId) == null;
            }
        }
    }
}