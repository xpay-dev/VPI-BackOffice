using Dapper;
using SDGDAL;
using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SDGDAL.Repositories {
     public class TransactionRepository {
          #region GetAllTransactionAttempts Credit Transactions
          public List<TransactionAttempt> GetTransactionsByPOS(int mposId, int transTypeId, int actionId, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    if (transTypeId == 0 && actionId == 0) {
                         q = context.TransactionAttempts.Include("Transaction")
                             .Include("Transaction.Key")
                            .Include("Transaction.TransactionEntryType")
                            .Include("Transaction.CardType")
                            .Include("Transaction.Currency")
                            .Include("Account")
                            .Include("Device")
                            .Include("MobileApp")
                            .Include("TransactionCharges")
                            .Where(t => t.Transaction.MerchantPOSId == mposId);// && t.TransactionTypeId == actionId && t.Transaction.TransactionEntryTypeId == transTypeId);
                    } else if (transTypeId == 0) {
                         q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(t => t.Transaction.MerchantPOSId == mposId && t.TransactionTypeId == actionId);
                    } else if (actionId == 0) {
                         q = context.TransactionAttempts.Include("Transaction")
                                     .Include("Transaction.Key")
                                     .Include("Transaction.TransactionEntryType")
                                     .Include("Transaction.CardType")
                                     .Include("Transaction.Currency")
                                     .Include("Account")
                                     .Include("Device")
                                     .Include("MobileApp")
                                     .Include("TransactionCharges")
                                     .Where(t => t.Transaction.MerchantPOSId == mposId && t.Transaction.TransactionEntryTypeId == transTypeId);
                    } else {
                         q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(t => t.Transaction.MerchantPOSId == mposId && t.TransactionTypeId == actionId && t.Transaction.TransactionEntryTypeId == transTypeId);
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    transactions = q.ToList();
               }
               return transactions;
          }

          public List<TransactionAttempt> GetTransactionsByPOS(int transTypeId, int actionId, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    if (transTypeId == 0) {
                         q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(t => t.TransactionTypeId == actionId);
                    } else if (actionId == 0) {
                         q = context.TransactionAttempts.Include("Transaction")
                                     .Include("Transaction.Key")
                                     .Include("Transaction.TransactionEntryType")
                                     .Include("Transaction.CardType")
                                     .Include("Transaction.Currency")
                                     .Include("Account")
                                     .Include("Device")
                                     .Include("MobileApp")
                                     .Include("TransactionCharges")
                                     .Where(t => t.Transaction.TransactionEntryTypeId == transTypeId);
                    } else {
                         q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(t => t.TransactionTypeId == actionId && t.Transaction.TransactionEntryTypeId == transTypeId);
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    transactions = q.ToList();
               }
               return transactions;
          }

          public List<TransactionAttempt> GetAllTransactionAttemptsbyPosId(int transTypeId, int actionid, int posId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Where(item => item.Transaction.MerchantPOSId == posId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOSId == posId && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOSId == posId && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOSId == posId && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.Transaction.MerchantPOSId == posId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOSId == posId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOSId == posId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOSId == posId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                          || (item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionAttemptsbyMerchantId(int transTypeId, int actionid, int merchantId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                          && (!String.IsNullOrEmpty(item.ReturnCode))
                                          && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && item.Transaction.TransactionEntryTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == merchantId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }


          public List<TransactionAttempt> GetAllTransactionAttemptsbyResellerId(int transTypeId, int actionid, int resellerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                          && (!String.IsNullOrEmpty(item.ReturnCode))
                                          && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.TransactionEntryTypeId == transTypeId
                                              && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionAttemptsbyPartnerId(int transTypeId, int actionid, int partnerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date)));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end)));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end)));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.TransactionEntryTypeId == transTypeId
                                              && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionAttemptsbyBranchId(int transTypeId, int actionid, int branchId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                              && item.Transaction.TransactionEntryTypeId == transTypeId
                                              && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.TransactionEntryTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          #endregion GetAllTransactionAttempts

          #region GetAllTransaction

          public List<TransactionAttempt> GetAllTransactionbyPosId(int cardTypeId, int transTypeId, int actionid, string currencyName, int posId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                          || (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                    }
                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public TransactionVoidReason CreateVoidTransaction(TransactionVoidReason voidTransaction) {
               using (DataContext context = new DataContext()) {
                    using (var trans = context.Database.BeginTransaction()) {
                         try {
                              var transVoid = context.TransactionVoidReason.Add(voidTransaction);
                              context.SaveChanges();

                              trans.Commit();

                              return transVoid;
                         } catch (Exception ex) {
                              trans.Rollback();
                              throw ex;
                         }
                    }
               };
          }

          public List<TransactionAttempt> GetAllTransactionbyMerchantId(int cardTypeId, int transTypeId, int actionid, string currencyName, int merchantId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionbyResellerId(int cardTypeId, int transTypeId, int actionid, string currencyName, int resellerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionbyPartnerId(int cardTypeId, int transTypeId, int actionid, string currencyName, int partnerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date)));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)))
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionbyBranchId(int cardTypeId, int transTypeId, int actionid, string currencyName, int branchId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.Amount > 0 && (item.Transaction.CardTypeId == cardTypeId && item.Transaction.Currency.CurrencyName == currencyName && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.Transaction.TransactionEntryTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranchId == branchId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranchId == branchId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Amount > 0 && item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranchId == branchId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          #endregion GetAllTransaction

          #region GetAllTransaction2

          public List<TransactionAttempt> GetAllTransactionbyPosId2(int cardTypeId, int transTypeId, int actionid, string currencyName, int posId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                          && (!String.IsNullOrEmpty(item.ReturnCode))
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                          && (!String.IsNullOrEmpty(item.ReturnCode))
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOSId == posId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                          && (!String.IsNullOrEmpty(item.ReturnCode))
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionbyMerchantId2(int cardTypeId, int transTypeId, int actionid, string currencyName, int merchantId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantId == merchantId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionbyResellerId2(int cardTypeId, int transTypeId, int actionid, string currencyName, int resellerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                           && (!String.IsNullOrEmpty(item.ReturnCode))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionbyPartnerId2(int cardTypeId, int transTypeId, int actionid, string currencyName, int partnerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date)));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)))
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode))));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionbyBranchId2(int cardTypeId, int transTypeId, int actionid, string currencyName, int branchId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttempt> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttempt> transactions = new List<Entities.TransactionAttempt>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                      .Include("Transaction.Key")
                                      .Include("Transaction.MerchantPOS.MerchantBranch")
                                      .Include("Transaction.MerchantPOS")
                                      .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("Transaction.TransactionEntryType")
                                      .Include("Transaction.CardType")
                                      .Include("Transaction.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                       .Include("Transaction.Key")
                                       .Include("Transaction.MerchantPOS.MerchantBranch")
                                       .Include("Transaction.MerchantPOS")
                                       .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("Transaction.TransactionEntryType")
                                       .Include("Transaction.CardType")
                                       .Include("Transaction.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName
                                       && (!String.IsNullOrEmpty(item.ReturnCode))
                                       && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                           .Include("Transaction.Key")
                                           .Include("Transaction.MerchantPOS.MerchantBranch")
                                           .Include("Transaction.MerchantPOS")
                                           .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("Transaction.TransactionEntryType")
                                           .Include("Transaction.CardType")
                                           .Include("Transaction.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.Transaction.CardTypeId == cardTypeId && (item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)))
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId && (!String.IsNullOrEmpty(item.ReturnCode)));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttempts.Include("Transaction")
                                               .Include("Transaction.Key")
                                               .Include("Transaction.MerchantPOS.MerchantBranch")
                                               .Include("Transaction.MerchantPOS")
                                               .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("Transaction.TransactionEntryType")
                                               .Include("Transaction.CardType")
                                               .Include("Transaction.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.Transaction.Currency.CurrencyName == currencyName && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == transTypeId
                                               && (!String.IsNullOrEmpty(item.ReturnCode))
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttempts.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid && (!String.IsNullOrEmpty(item.ReturnCode)));
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranchId == branchId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranchId == branchId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.Transaction.CardTypeId == cardTypeId && item.Transaction.MerchantPOS.MerchantBranchId == branchId && (item.Transaction.TransactionEntryTypeId == 9 || item.Transaction.TransactionEntryTypeId == 5 || item.Transaction.TransactionEntryTypeId == 3 || item.Transaction.TransactionEntryTypeId == 1) && item.Transaction.Currency.CurrencyName == currencyName && item.TransactionTypeId == actionid
                                              && (!String.IsNullOrEmpty(item.ReturnCode))
                                              && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          #endregion GetAllTransaction

          #region GetAllDebitTransactionAttempts

          public List<TransactionAttemptDebit> GetAllDebitTransactionAttemptsbyPosId(int transTypeId, int actionid, int posId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")

                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Where(item => item.TransactionDebit.MerchantPOSId == posId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")

                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                          || (item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionAttemptsbyMerchantId(int transTypeId, int actionid, int merchantId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")

                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")

                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                          || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionAttemptsbyResellerId(int transTypeId, int actionid, int resellerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")

                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")

                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                          || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionAttemptsbyPartnerId(int transTypeId, int actionid, int partnerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                      || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                               || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")

                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                              || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                               || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                          || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")

                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                          || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionAttemptsbyBranchId(int transTypeId, int actionid, int branchId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")

                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")

                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                  || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")

                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")

                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId != 0 && actionid != 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")

                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                          || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")

                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid && item.TransactionDebit.TransactionEntryTypeId == 4)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          #endregion GetAllDebitTransactionAttempts

          #region GetDebitAllTransaction

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyPosId(int transTypeId, int actionid, int posId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")
                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.TransactionDebit.MerchantPOSId == posId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyMerchantId(int transTypeId, int actionid, int merchantId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")
                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyResellerId(int transTypeId, int actionid, int resellerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")
                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyPartnerId(int transTypeId, int actionid, int partnerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                      || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                               || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                              || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                               || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                          || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyBranchId(int transTypeId, int actionid, int branchId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")
                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                  || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    return q.ToList();
               }
          }

          #endregion GetDebitAllTransaction

          #region GetDebitAllTransaction2

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyPosId2(int cardTypeId, int transTypeId, int actionid, int posId, DateTime? startDate, DateTime? endDate, string search,
            int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")
                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.TransactionDebit.MerchantPOSId == posId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOSId == posId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOSId == posId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyMerchantId2(int cardTypeId, int transTypeId, int actionid, int merchantId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")
                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == merchantId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyResellerId2(int cardTypeId, int transTypeId, int actionid, int resellerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")
                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionTypeId == actionid
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                          .Include("Transaction.Key")
                                          .Include("Transaction.MerchantPOS.MerchantBranch")
                                          .Include("Transaction.MerchantPOS")
                                          .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("Transaction.TransactionEntryType")
                                          .Include("Transaction.CardType")
                                          .Include("Transaction.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == resellerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyPartnerId2(int cardTypeId, int transTypeId, int actionid, int partnerId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                      || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                               || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                              || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId
                                               || item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId)
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                          || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.PartnerId == partnerId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == partnerId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitTransactionbyBranchId2(int cardTypeId, int transTypeId, int actionid, int branchId, DateTime? startDate, DateTime? endDate, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               using (DataContext context = new DataContext()) {
                    IQueryable<TransactionAttemptDebit> q = null;

                    DateTime end = DateTime.MinValue;
                    DateTime start = startDate.Value;

                    if (endDate != DateTime.MinValue) {
                         end = endDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }

                    DateTime date = DateTime.MinValue;

                    List<TransactionAttemptDebit> transactions = new List<Entities.TransactionAttemptDebit>();
                    if (transTypeId == 0 && actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                      .Include("TransactionDebit.Key")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                      .Include("TransactionDebit.MerchantPOS")
                                      .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                      .Include("TransactionDebit.TransactionEntryType")
                                      .Include("TransactionDebit.Currency")
                                      .Include("Account")
                                      .Include("Device")
                                      .Include("MobileApp")
                                      .Include("TransactionCharges")
                                      .Include("TransactionSignature")
                                      .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                              && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                       .Include("TransactionDebit.Key")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                       .Include("TransactionDebit.MerchantPOS")
                                       .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                       .Include("TransactionDebit.TransactionEntryType")
                                       .Include("TransactionDebit.Currency")
                                       .Include("Account")
                                       .Include("Device")
                                       .Include("MobileApp")
                                       .Include("TransactionCharges")
                                       .Include("TransactionSignature")
                                       .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                          .Include("TransactionDebit.Key")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                          .Include("TransactionDebit.MerchantPOS")
                                          .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                          .Include("TransactionDebit.TransactionEntryType")
                                          .Include("TransactionDebit.Currency")
                                          .Include("Account")
                                          .Include("Device")
                                          .Include("MobileApp")
                                          .Include("TransactionCharges")
                                          .Include("TransactionSignature")
                                          .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                  || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid));
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                           .Include("TransactionDebit.Key")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                           .Include("TransactionDebit.MerchantPOS")
                                           .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                           .Include("TransactionDebit.TransactionEntryType")
                                           .Include("TransactionDebit.Currency")
                                           .Include("Account")
                                           .Include("Device")
                                           .Include("MobileApp")
                                           .Include("TransactionCharges")
                                           .Include("TransactionSignature")
                                           .Where(item => (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           || (item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionTypeId == actionid)
                                           && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionid == 0) {
                         if (start == date && end == date) {
                              q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                              .Include("TransactionDebit.Key")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                              .Include("TransactionDebit.MerchantPOS")
                                              .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("TransactionDebit.TransactionEntryType")
                                              .Include("TransactionDebit.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId);
                         } else {
                              if (start != date && end == date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date));
                              } else if (start == date && end != date) {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived <= end));
                              } else {
                                   q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                               .Include("TransactionDebit.Key")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                               .Include("TransactionDebit.MerchantPOS")
                                               .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                               .Include("TransactionDebit.TransactionEntryType")
                                               .Include("TransactionDebit.Currency")
                                               .Include("Account")
                                               .Include("Device")
                                               .Include("MobileApp")
                                               .Include("TransactionCharges")
                                               .Include("TransactionSignature")
                                               .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == transTypeId
                                               && (item.DateReceived >= start.Date && item.DateReceived <= end));
                              }
                         }
                         totalRecords = q.Count();

                         return q.ToList();
                    }

                    if (start == date && end == date) {
                         q = context.TransactionAttemptDebit.Include("Transaction")
                                         .Include("Transaction.Key")
                                         .Include("Transaction.MerchantPOS.MerchantBranch")
                                         .Include("Transaction.MerchantPOS")
                                         .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                         .Include("Transaction.TransactionEntryType")
                                         .Include("Transaction.CardType")
                                         .Include("Transaction.Currency")
                                         .Include("Account")
                                         .Include("Device")
                                         .Include("MobileApp")
                                         .Include("TransactionCharges")
                                         .Include("TransactionSignature")
                                         .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid);
                    } else {
                         if (start != date && end == date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date));
                         } else if (start == date && end != date) {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived <= end));
                         } else {
                              q = context.TransactionAttemptDebit.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Include("TransactionSignature")
                                              .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranchId == branchId && item.TransactionDebit.TransactionEntryTypeId == 4 && item.TransactionTypeId == actionid
                                          && (item.DateReceived >= start.Date && item.DateReceived <= end));
                         }
                    }

                    totalRecords = q.Count();

                    return q.ToList();
               }
          }

          #endregion GetDebitAllTransaction2

          #region TransactionAttemptsforGraph

          public List<TransactionAttempt> GetAllTransactionAttemptsforGraph(DateTime date, int uId, int parentId) {
               try {
                    using (DataContext context = new DataContext()) {
                         DateTime end = date.AddDays(7);
                         if (uId == 1) {
                              var q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == parentId
                                              && (item.DateReceived >= date && item.DateReceived <= end));

                              return q.ToList();
                         } else if (uId == 2) {
                              var q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == parentId
                                              && (item.DateReceived >= date && item.DateReceived <= end));

                              return q.ToList();
                         } else if (uId == 3) {
                              var q = context.TransactionAttempts.Include("Transaction")
                                              .Include("Transaction.Key")
                                              .Include("Transaction.MerchantPOS.MerchantBranch")
                                              .Include("Transaction.MerchantPOS")
                                              .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                              .Include("Transaction.TransactionEntryType")
                                              .Include("Transaction.CardType")
                                              .Include("Transaction.Currency")
                                              .Include("Account")
                                              .Include("Device")
                                              .Include("MobileApp")
                                              .Include("TransactionCharges")
                                              .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == parentId
                                              && (item.DateReceived >= date && item.DateReceived <= end));

                              return q.ToList();
                         }
                    }
               } catch (Exception ex) {
                    throw ex;
               }

               return null;
          }

          public List<TransactionAttempt> GetAllPOSTransactionAttemptsforGraph(DateTime date, int p) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttempts.Include("Transaction")
                                    .Include("Transaction.Key")
                                    .Include("Transaction.MerchantPOS.MerchantBranch")
                                    .Include("Transaction.MerchantPOS")
                                    .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("Transaction.TransactionEntryType")
                                    .Include("Transaction.CardType")
                                    .Include("Transaction.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Where(item => item.Transaction.MerchantPOSId == p
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllBranchTransactionAttemptsforGraph(DateTime date, int b) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttempts.Include("Transaction")
                                    .Include("Transaction.Key")
                                    .Include("Transaction.MerchantPOS.MerchantBranch")
                                    .Include("Transaction.MerchantPOS")
                                    .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("Transaction.TransactionEntryType")
                                    .Include("Transaction.CardType")
                                    .Include("Transaction.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Where(item => item.Transaction.MerchantPOS.MerchantBranchId == b
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllMerchantTransactionAttemptsforGraph(DateTime date, int m) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttempts.Include("Transaction")
                                    .Include("Transaction.Key")
                                    .Include("Transaction.MerchantPOS.MerchantBranch")
                                    .Include("Transaction.MerchantPOS")
                                    .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("Transaction.TransactionEntryType")
                                    .Include("Transaction.CardType")
                                    .Include("Transaction.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Where(item => item.Transaction.MerchantPOS.MerchantBranch.MerchantId == m
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllResellerTransactionAttemptsforGraph(DateTime date, int r) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttempts.Include("Transaction")
                                    .Include("Transaction.Key")
                                    .Include("Transaction.MerchantPOS.MerchantBranch")
                                    .Include("Transaction.MerchantPOS")
                                    .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("Transaction.TransactionEntryType")
                                    .Include("Transaction.CardType")
                                    .Include("Transaction.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == r
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllPOSSalesTransactionAttemptsforGraph(DateTime date, int r = 0) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttempts.Include("Transaction")
                                    .Include("Transaction.Key")
                                    .Include("Transaction.MerchantPOS.MerchantBranch")
                                    .Include("Transaction.MerchantPOS")
                                    .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("Transaction.TransactionEntryType")
                                    .Include("Transaction.CardType")
                                    .Include("Transaction.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == r
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttempt> GetAllPartnerTransactionAttemptsforGraph(DateTime date, int pt) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttempts.Include("Transaction")
                                    .Include("Transaction.Key")
                                    .Include("Transaction.MerchantPOS.MerchantBranch")
                                    .Include("Transaction.MerchantPOS")
                                    .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("Transaction.TransactionEntryType")
                                    .Include("Transaction.CardType")
                                    .Include("Transaction.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Where(item => item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == pt
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          #endregion TransactionAttemptsforGraph

          #region TransactionAttemptsforDebitGraph

          public List<TransactionAttemptDebit> GetAllDebitTransactionAttemptsforGraph(DateTime date, int r = 0) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                    .Include("TransactionDebit.Key")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                    .Include("TransactionDebit.MerchantPOS")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("TransactionDebit.TransactionEntryType")
                                    .Include("TransactionDebit.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Include("TransactionSignature")
                                    .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == r
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitPOSTransactionAttemptsforGraph(DateTime date, int p) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                    .Include("TransactionDebit.Key")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                    .Include("TransactionDebit.MerchantPOS")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("TransactionDebit.TransactionEntryType")
                                    .Include("TransactionDebit.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Include("TransactionSignature")
                                    .Where(item => item.TransactionDebit.MerchantPOSId == p
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitBranchTransactionAttemptsforGraph(DateTime date, int b) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                    .Include("TransactionDebit.Key")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                    .Include("TransactionDebit.MerchantPOS")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("TransactionDebit.TransactionEntryType")
                                    .Include("TransactionDebit.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Include("TransactionSignature")
                                    .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranchId == b
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitMerchantTransactionAttemptsforGraph(DateTime date, int m) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                    .Include("TransactionDebit.Key")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                    .Include("TransactionDebit.MerchantPOS")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("TransactionDebit.TransactionEntryType")
                                    .Include("TransactionDebit.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Include("TransactionSignature")
                                    .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.MerchantId == m
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitResellerTransactionAttemptsforGraph(DateTime date, int r) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                    .Include("TransactionDebit.Key")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                    .Include("TransactionDebit.MerchantPOS")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("TransactionDebit.TransactionEntryType")
                                    .Include("TransactionDebit.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Include("TransactionSignature")
                                    .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == r
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitPOSSalesTransactionAttemptsforGraph(DateTime date, int r = 0) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                    .Include("TransactionDebit.Key")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                    .Include("TransactionDebit.MerchantPOS")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("TransactionDebit.TransactionEntryType")
                                    .Include("TransactionDebit.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Include("TransactionSignature")
                                    .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.ResellerId == r
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          public List<TransactionAttemptDebit> GetAllDebitPartnerTransactionAttemptsforGraph(DateTime date, int pt) {
               using (DataContext context = new DataContext()) {
                    DateTime end = date.AddDays(7);

                    var q = context.TransactionAttemptDebit.Include("TransactionDebit")
                                    .Include("TransactionDebit.Key")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                    .Include("TransactionDebit.MerchantPOS")
                                    .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                    .Include("TransactionDebit.TransactionEntryType")
                                    .Include("TransactionDebit.Currency")
                                    .Include("Account")
                                    .Include("Device")
                                    .Include("MobileApp")
                                    .Include("TransactionCharges")
                                    .Include("TransactionSignature")
                                    .Where(item => item.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.Reseller.PartnerId == pt
                                    && (item.DateReceived >= date && item.DateReceived <= end));

                    return q.ToList();
               }
          }

          #endregion TransactionAttemptsforDebitGraph

          #region For backOfficce Transaction Reports
          public List<TransactionAttempt> GetAllTransactionAttempts(int transTypeId, int actionId, List<int> transIds, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               List<TransactionAttempt> transAttempt = new List<Entities.TransactionAttempt>();
               IOrderedQueryable<TransactionAttempt> x = null;
               using (DataContext context = new DataContext()) {
                    if ((transTypeId == 0) && (actionId == 0)) {
                         var q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.MerchantPOS.MerchantBranch")
                                 .Include("Transaction.MerchantPOS")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(item => transIds.Contains(item.TransactionId));

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         var q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.MerchantPOS.MerchantBranch")
                                 .Include("Transaction.MerchantPOS")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(t => t.TransactionTypeId == actionId);

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionId == 0) {
                         var q = context.TransactionAttempts.Include("Transaction")
                                     .Include("Transaction.Key")
                                     .Include("Transaction.MerchantPOS.MerchantBranch")
                                     .Include("Transaction.MerchantPOS")
                                     .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                     .Include("Transaction.TransactionEntryType")
                                     .Include("Transaction.CardType")
                                     .Include("Transaction.Currency")
                                     .Include("Account")
                                     .Include("Device")
                                     .Include("MobileApp")
                                     .Include("TransactionCharges")
                                     .Where(t => t.Transaction.TransactionEntryTypeId == transTypeId);

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    } else {
                         var q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.MerchantPOS.MerchantBranch")
                                 .Include("Transaction.MerchantPOS")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(t => t.TransactionTypeId == actionId && t.Transaction.TransactionEntryTypeId == transTypeId);

                         if (sortOrder.ToUpper() == "DESC") {
                              q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                         } else {
                              q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                         }

                         totalRecords = q.Count();

                         return q.ToList();
                    }
               }
          }

          public List<TransactionAttempt> GetAllTransactionAttempts(int transTypeId, int actionId, string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               List<TransactionAttempt> transAttempt = new List<Entities.TransactionAttempt>();
               IOrderedQueryable<TransactionAttempt> x = null;
               using (DataContext context = new DataContext()) {
                    if ((transTypeId == 0) && (actionId == 0)) {
                         var q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.MerchantPOS.MerchantBranch")
                                 .Include("Transaction.MerchantPOS")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(item => item.TransactionAttemptId > 0);

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (transTypeId == 0) {
                         var q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.MerchantPOS.MerchantBranch")
                                 .Include("Transaction.MerchantPOS")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(t => t.TransactionTypeId == actionId);

                         totalRecords = q.Count();

                         return q.ToList();
                    } else if (actionId == 0) {
                         var q = context.TransactionAttempts.Include("Transaction")
                                     .Include("Transaction.Key")
                                     .Include("Transaction.MerchantPOS.MerchantBranch")
                                     .Include("Transaction.MerchantPOS")
                                     .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                     .Include("Transaction.TransactionEntryType")
                                     .Include("Transaction.CardType")
                                     .Include("Transaction.Currency")
                                     .Include("Account")
                                     .Include("Device")
                                     .Include("MobileApp")
                                     .Include("TransactionCharges")
                                     .Where(t => t.Transaction.TransactionEntryTypeId == transTypeId);

                         totalRecords = q.Count();

                         return q.ToList();
                    } else {
                         var q = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.MerchantPOS.MerchantBranch")
                                 .Include("Transaction.MerchantPOS")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Currency")
                                 .Include("Account")
                                 .Include("Device")
                                 .Include("MobileApp")
                                 .Include("TransactionCharges")
                                 .Where(t => t.TransactionTypeId == actionId && t.Transaction.TransactionEntryTypeId == transTypeId);

                         totalRecords = q.Count();

                         return q.ToList();
                    }
               }
          }

          public List<Transaction> GetAllTransactions(string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               List<Transaction> transAttempt = new List<Entities.Transaction>();
               using (DataContext context = new DataContext()) {
                    IQueryable<Transaction> q = null;

                    q = context.Transactions.Include("MerchantPOS.MerchantBranch")
                            .Include("MerchantPOS")
                            .Include("MerchantPOS.MerchantBranch.Merchant")
                            .Include("MerchantPOS.MerchantBranch.Merchant.Reseller")
                            .Include("MerchantPOS.MerchantBranch.Merchant.Reseller.Partner")
                            .Include("TransactionEntryType")
                            .Include("CardType")
                            .Include("Currency").OrderBy(tr => tr.FinalAmount);

                    totalRecords = q.Count();

                    transAttempt = q.ToList();
               }

               return transAttempt;
          }

          public List<TransactionAttempt> GetAllTransactionAttempts() {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttempts.Include("Transaction")
                                .Include("Transaction.Key")
                                .Include("Transaction.MerchantPOS.MerchantBranch")
                                .Include("Transaction.MerchantPOS")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                .Include("Transaction.TransactionEntryType")
                                .Include("Transaction.CardType")
                                .Include("Transaction.Currency")
                                .Include("Account")
                                .Include("Device")
                                .Include("MobileApp")
                                .Include("TransactionCharges").OrderByDescending(u => u.TransactionId).ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionAttempts(string search,
             int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords) {
               List<TransactionAttempt> transAttempt = new List<Entities.TransactionAttempt>();
               using (DataContext context = new DataContext()) {
                    var q = context.TransactionAttempts.Include("Transaction")
                                .Include("Transaction.Key")
                                .Include("Transaction.MerchantPOS.MerchantBranch")
                                .Include("Transaction.MerchantPOS")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner")
                                .Include("Transaction.TransactionEntryType")
                                .Include("Transaction.CardType")
                                .Include("Transaction.Currency")
                                .Include("Account")
                                .Include("Device")
                                .Include("MobileApp")
                                .Include("TransactionCharges").Where(u => u.TransactionId > 0);

                    totalRecords = q.Count();

                    if (sortOrder.ToUpper() == "DESC") {
                         q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                    } else {
                         q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                    }

                    transAttempt = q.ToList();
               }
               return transAttempt;
          }

          public IEnumerable<TransactionAttempt> GetAllTransactionAttemptsForTopMerchants(string cString) {
               using (var con = new SqlConnection(cString)) {
                    #region Old Code

                    //return context.TransactionAttempts.AsNoTracking().Include("Transaction")
                    //        .Include("Transaction.Key")
                    //        .Include("Transaction.MerchantPOS.MerchantBranch")
                    //        .Include("Transaction.MerchantPOS")
                    //        .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                    //        .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller")
                    //        .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner")
                    //        .Include("Transaction.TransactionEntryType")
                    //        .Include("Transaction.CardType")
                    //        .Include("Transaction.Currency")
                    //        .Include("Account")
                    //        .Include("Device")
                    //        .Include("MobileApp")
                    //        .Include("TransactionSignature")
                    //        .Include("TransactionCharges")
                    //        .Where(u => u.TransactionId > 0 && u.TransactionTypeId == 3)
                    //        .ToList();

                    #endregion Old Code

                    string query = "select * from TransactionAttempts ta"
                    + " inner join (select TransactionId, MerchantPOSId, CurrencyId from Transactions) t on ta.TransactionId = t.TransactionId"
                    + " inner join (select CurrencyId, CurrencyName from Currencies) c on t.CurrencyId = c.CurrencyId"
                    + " inner join (select MerchantBranchPOSId from MobileApps) ma on t.MerchantPOSId = ma.MerchantBranchPOSId"
                    + " inner join (select MerchantPOSId, MerchantBranchId from MerchantBranchPOSs) mbPOS on ma.MerchantBranchPOSId = mbPOS.MerchantPOSId"
                    + " inner join (select MerchantBranchId, MerchantId from MerchantBranches) mb on mbPOS.MerchantBranchId = mb.MerchantBranchId"
                    + " inner join (select MerchantId, MerchantName, ResellerId from Merchants) m on mb.MerchantId = m.MerchantId"
                    + " inner join (select ResellerId, PartnerId from Resellers) r on m.ResellerId = r.ResellerId"
                    + " inner join (select PartnerId, CompanyName from Partners) p on r.PartnerId = p.PartnerId"
                    + " where ta.TransactionId > 0 and (ta.TransactionTypeId = 3 or ta.TransactionTypeId = 4) and m.MerchantName like '%%'";

                    var data = con.Query<TransactionAttempt, Transaction, MerchantBranchPOS, MerchantBranch, Merchant, Reseller, Partner, TransactionAttempt>(
                                    query,
                                    (transAttempt, transaction, mbPOS, mb, m, r, p) => {
                                         transAttempt.Transaction = transaction;
                                         transaction.MerchantPOS = mbPOS;
                                         mbPOS.MerchantBranch = mb;
                                         mb.Merchant = m;
                                         m.Reseller = r;
                                         r.Partner = p;
                                         return transAttempt;
                                    },
                        splitOn: "TransactionAttemptId, TransactionId, MerchantPOSId, MerchantBranchId, MerchantId, ResellerId, PartnerId"
                                );

                    return data;
               }
          }

          public IEnumerable<TransactionAttemptDebit> GetAllDebitTransactionAttemptsForTopMerchants(string cString) {
               using (var con = new SqlConnection(cString)) {
                    string query = "select * from TransactionAttemptDebit ta"
                    + " inner join (select TransactionDebitId, MerchantPOSId, CurrencyId from TransactionDebit) t on ta.TransactionDebitId = t.TransactionDebitId"
                    + " inner join (select CurrencyId, CurrencyName from Currencies) c on t.CurrencyId = c.CurrencyId"
                    + " inner join (select MerchantBranchPOSId from MobileApps) ma on t.MerchantPOSId = ma.MerchantBranchPOSId"
                    + " inner join (select MerchantPOSId, MerchantBranchId from MerchantBranchPOSs) mbPOS on ma.MerchantBranchPOSId = mbPOS.MerchantPOSId"
                    + " inner join (select MerchantBranchId, MerchantId from MerchantBranches) mb on mbPOS.MerchantBranchId = mb.MerchantBranchId"
                    + " inner join (select MerchantId, MerchantName, ResellerId from Merchants) m on mb.MerchantId = m.MerchantId"
                    + " inner join (select ResellerId, PartnerId from Resellers) r on m.ResellerId = r.ResellerId"
                    + " inner join (select PartnerId, CompanyName from Partners) p on r.PartnerId = p.PartnerId"
                    + " where ta.TransactionDebitId > 0 and (ta.TransactionTypeId = 3 or ta.TransactionTypeId = 4) and m.MerchantName like '%%'";

                    var data = con.Query<TransactionAttemptDebit, TransactionDebit, MerchantBranchPOS, MerchantBranch, Merchant, Reseller, Partner, TransactionAttemptDebit>(
                                    query,
                                    (transAttemptDebit, transactionDebit, mbPOS, mb, m, r, p) => {
                                         transAttemptDebit.TransactionDebit = transactionDebit;
                                         transactionDebit.MerchantPOS = mbPOS;
                                         mbPOS.MerchantBranch = mb;
                                         mb.Merchant = m;
                                         m.Reseller = r;
                                         r.Partner = p;
                                         return transAttemptDebit;
                                    },
                        splitOn: "TransactionAttemptDebitId, TransactionDebitId, MerchantPOSId, MerchantBranchId, MerchantId, ResellerId, PartnerId"
                                );

                    return data;
               }
          }

          public IEnumerable<TransactionAttempt> GetAllTransactionAttemptsForTopMerchantsByPartner(string cString, int pId) {
               using (var con = new SqlConnection(cString)) {
                    #region Old Code

                    //return context.TransactionAttempts.Include("Transaction")
                    //            .Include("Transaction.Key")
                    //            .Include("Transaction.MerchantPOS.MerchantBranch")
                    //            .Include("Transaction.MerchantPOS")
                    //            .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                    //            .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller")
                    //            .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner")
                    //            .Include("Transaction.TransactionEntryType")
                    //            .Include("Transaction.CardType")
                    //            .Include("Transaction.Currency")
                    //            .Include("Account")
                    //            .Include("Device")
                    //            .Include("MobileApp")
                    //            .Include("TransactionCharges").Where(u => u.TransactionId > 0 && u.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.PartnerId == pId && u.TransactionTypeId == 3).ToList();

                    #endregion Old Code

                    string query = "select * from TransactionAttempts ta"
                    + " inner join (select TransactionId, MerchantPOSId from Transactions) t on ta.TransactionId = t.TransactionId"
                    + " inner join (select MerchantBranchPOSId from MobileApps) ma on t.MerchantPOSId = ma.MerchantBranchPOSId"
                    + " inner join (select MerchantPOSId, MerchantBranchId from MerchantBranchPOSs) mbPOS on ma.MerchantBranchPOSId = mbPOS.MerchantPOSId"
                    + " inner join (select MerchantBranchId, MerchantId from MerchantBranches) mb on mbPOS.MerchantBranchId = mb.MerchantBranchId"
                    + " inner join (select MerchantId, MerchantName, ResellerId from Merchants) m on mb.MerchantId = m.MerchantId"
                    + " inner join (select ResellerId, PartnerId from Resellers) r on m.ResellerId = r.ResellerId"
                    + " inner join (select PartnerId, CompanyName from Partners) p on r.PartnerId = p.PartnerId"
                    + " where ta.TransactionId > 0 and (ta.TransactionTypeId = 3 or ta.TransactionTypeId = 4) and p.PartnerId =" + pId + " and m.MerchantName like '%%'";

                    var data = con.Query<TransactionAttempt, Transaction, MerchantBranchPOS, MerchantBranch, Merchant, Reseller, Partner, TransactionAttempt>(
                                    query,
                                    (transAttempt, transaction, mbPOS, mb, m, r, p) => {
                                         transAttempt.Transaction = transaction;
                                         transaction.MerchantPOS = mbPOS;
                                         mbPOS.MerchantBranch = mb;
                                         mb.Merchant = m;
                                         m.Reseller = r;
                                         r.Partner = p;
                                         return transAttempt;
                                    },
                        splitOn: "TransactionAttemptId, TransactionId, MerchantPOSId, MerchantBranchId, MerchantId, ResellerId, PartnerId"
                                );

                    return data;
               }
          }

          public List<TransactionAttempt> GetAllTransactionAttemptsForTopMerchantsByReseller(int rId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttempts.Include("Transaction")
                                .Include("Transaction.Key")
                                .Include("Transaction.MerchantPOS.MerchantBranch")
                                .Include("Transaction.MerchantPOS")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner")
                                .Include("Transaction.TransactionEntryType")
                                .Include("Transaction.CardType")
                                .Include("Transaction.Currency")
                                .Include("Account")
                                .Include("Device")
                                .Include("MobileApp")
                                .Include("TransactionCharges").Where(u => u.TransactionId > 0 && u.Transaction.MerchantPOS.MerchantBranch.Merchant.ResellerId == rId && (u.TransactionTypeId == 3 || u.TransactionTypeId == 4)).ToList();
               }
          }

          public List<TransactionAttempt> GetAllTransactionAttemptsForTopMerchantsByMerchant(int mId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttempts.Include("Transaction")
                                .Include("Transaction.Key")
                                .Include("Transaction.MerchantPOS.MerchantBranch")
                                .Include("Transaction.MerchantPOS")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller")
                                .Include("Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner")
                                .Include("Transaction.TransactionEntryType")
                                .Include("Transaction.CardType")
                                .Include("Transaction.Currency")
                                .Include("Account")
                                .Include("Device")
                                .Include("MobileApp")
                                .Include("TransactionCharges").Where(u => u.TransactionId > 0 && u.Transaction.MerchantPOS.MerchantBranch.MerchantId == mId && (u.TransactionTypeId == 3 || u.TransactionTypeId == 4)).ToList();
               }
          }

          public List<TransactionType> GetTransactionTypes() {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionTypes.OrderBy(t => t.Name).ToList();
                    } catch {
                         return null;
                    }
               }
          }


          #endregion

          #region Credit Transactions For Mobile App

          public int GetCurrencyIdByCurrencyName(string currencyName) {
               using (DataContext context = new DataContext()) {
                    var currency = context.Currencies.SingleOrDefault(c => c.CurrencyCode == currencyName || c.CurrencyName == currencyName);

                    return currency.CurrencyId;
               }
          }

          public TransactionAttempt CreateTransactionAttempt(TransactionAttempt transactionAttempt) {
               using (DataContext context = new DataContext()) {
                    try {
                         var trans = context.TransactionAttempts.Add(transactionAttempt);

                         context.SaveChanges();

                         if (trans.TransactionAttemptId > 0) {
                              transactionAttempt.TransactionAttemptId = trans.TransactionAttemptId;
                         }

                         return trans;
                    } catch (System.Data.Entity.Validation.DbEntityValidationException dbEx) {
                         Exception raise = dbEx;

                         foreach (var validationErrors in dbEx.EntityValidationErrors) {
                              foreach (var validationError in validationErrors.ValidationErrors) {
                                   string msg = string.Format("{0} : {1}", validationErrors.Entry.Entity.ToString(),
                                       validationError.ErrorMessage);
                                   raise = new InvalidOperationException(msg, raise);
                              }
                         }
                         throw raise;
                    }
               }
          }

          public Transaction CreateTransaction(Transaction transaction, TransactionAttempt transactionAttempt) {
               using (DataContext context = new DataContext()) {
                    try {
                         var trans = context.Transactions.Add(transaction);

                         context.SaveChanges();

                         if (trans.TransactionId > 0) {
                              transactionAttempt.TransactionId = trans.TransactionId;

                              var transAttempt = context.TransactionAttempts.Add(transactionAttempt);

                              context.SaveChanges();

                              transactionAttempt.TransactionAttemptId = transAttempt.TransactionAttemptId;
                         }

                         return trans;
                    } catch (System.Data.Entity.Validation.DbEntityValidationException dbEx) {
                         Exception raise = dbEx;

                         foreach (var validationErrors in dbEx.EntityValidationErrors) {
                              foreach (var validationError in validationErrors.ValidationErrors) {
                                   string msg = string.Format("{0} : {1}", validationErrors.Entry.Entity.ToString(),
                                       validationError.ErrorMessage);
                                   raise = new InvalidOperationException(msg, raise);
                              }
                         }
                         throw raise;
                    }
               }
          }

          public TempTransaction CreateTempTransaction(TempTransaction transaction) {
               using (DataContext context = new DataContext()) {
                    try {
                         var trans = context.TempTransactions.Add(transaction);

                         context.SaveChanges();

                         return trans;
                    } catch (System.Data.Entity.Validation.DbEntityValidationException dbEx) {
                         Exception raise = dbEx;

                         foreach (var validationErrors in dbEx.EntityValidationErrors) {
                              foreach (var validationError in validationErrors.ValidationErrors) {
                                   string msg = string.Format("{0} : {1}", validationErrors.Entry.Entity.ToString(),
                                       validationError.ErrorMessage);
                                   raise = new InvalidOperationException(msg, raise);
                              }
                         }
                         throw raise;
                    }
               }
          }

          public TransactionAttempt UpdateTransactionAttempt(TransactionAttempt transaction) {
               using (DataContext context = new DataContext()) {
                    var ut = context.TransactionAttempts.Attach(transaction);
                    var entryT = context.Entry(transaction);

                    entryT.Property(e => e.AuthNumber).IsModified = true;
                    entryT.Property(e => e.ReturnCode).IsModified = true;
                    entryT.Property(e => e.Reference).IsModified = true;
                    entryT.Property(e => e.SeqNumber).IsModified = true;
                    entryT.Property(e => e.TransNumber).IsModified = true;
                    entryT.Property(e => e.BatchNumber).IsModified = true;
                    entryT.Property(e => e.DisplayReceipt).IsModified = true;
                    entryT.Property(e => e.DisplayTerminal).IsModified = true;
                    entryT.Property(e => e.DateReceived).IsModified = true;
                    entryT.Property(e => e.DepositDate).IsModified = true;
                    entryT.Property(e => e.Notes).IsModified = true;
                    entryT.Property(e => e.TransactionTypeId).IsModified = true;
                    entryT.Property(e => e.PosEntryMode).IsModified = true;
                    entryT.Property(e => e.TransactionSignatureId).IsModified = true;
                    entryT.Property(e => e.Amount).IsModified = true;

                    context.SaveChanges();

                    return entryT.Entity;
               }
          }

          public TransactionAttempt UpdateTransactionAttemptStatus(TransactionAttempt transaction) {
               using (DataContext context = new DataContext()) {
                    var ut = context.TransactionAttempts.Attach(transaction);
                    var entryT = context.Entry(transaction);

                    entryT.Property(e => e.TransactionTypeId).IsModified = true;

                    context.SaveChanges();

                    return entryT.Entity;
               }
          }

          public TransactionAttempt GetTransactionAttempt(int transactionAttemptId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttempts
                                  .Include("Transaction")
                                  .Include("TransactionSignature")
                                  .SingleOrDefault(t => t.TransactionAttemptId == transactionAttemptId);
               }
          }

          public Transaction GetTransaction(int transactionId) {
               using (DataContext context = new DataContext()) {
                    return context.Transactions
                                  .Include("TransactionAttempts")
                                  .Include("MerchantPOS")
                                  .Include("MerchantPOS.MerchantBranch")
                                  .Include("MerchantPOS.MerchantBranch.Merchant")
                                  .Include("Key")
                                  .Include("Mid")
                                  .Include("Currency")
                                  .SingleOrDefault(t => t.TransactionId == transactionId);
               }
          }

          public List<Transaction> GetAllTransactions() {
               using (DataContext context = new DataContext()) {
                    return context.Transactions.OrderByDescending(u => u.MerchantPOSId).ToList();
               }
          }

          public List<TransactionAttempt> GetCreditTransactionByMobileAppId(int mobileAppId) {
               using (DataContext context = new DataContext()) {
                    try {
                         var trans = context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.MerchantPOS.MerchantBranch")
                                 .Include("Transaction.MerchantPOS")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.Currency")
                                 .Include("TransactionSignature")
                                 .Where(t => t.MobileAppId == mobileAppId && t.ReturnCode != null && t.DateReceived >= DateTime.Today).OrderByDescending(t => t.DateReceived).Take(10).ToList();

                         return trans.Where(x => x.Amount.ToString() != "0.00").ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttempt> GetTransactionAttemptByRefNumApp(string appRefNum) {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionAttempts.Include("Transaction")
                                 .Include("Transaction.MerchantPOS.MerchantBranch")
                                 .Include("Transaction.MerchantPOS")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.Mid.Switch")
                                 .Include("Transaction.Currency")
                                 .Include("TransactionSignature")
                                 .Where(t => t.Transaction.RefNumApp == appRefNum).ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttempt> GetCreditTransactionAttemptByTransNumber(int transId, int transAttId) {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionAttempts
                                 .Include("Transaction")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.Currency")
                                 .Include("TransactionSignature")
                                 .Where(t => t.TransactionId == transId && t.TransactionAttemptId == transAttId && t.ReturnCode != null).ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public TransactionAttempt GetAllCreditTransactionAttemptByTransNumber(int transId, int transAttId) {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionAttempts
                                 .Include("Transaction")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Key")
                                 .Include("TransactionSignature")
                                 .Include("Transaction.Currency")
                                 .SingleOrDefault(t => t.TransactionId == transId && t.TransactionAttemptId == transAttId);
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttempt> GetCreditTransactionAllByMobileAppId(int mobileAppId, string searchTrans, out int totalrecords) {
               using (DataContext context = new DataContext()) {
                    try {
                         var qry = context.TransactionAttempts
                                 .Include("Transaction")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("Transaction.TransactionEntryType")
                                 .Include("Transaction.CardType")
                                 .Include("Transaction.Key")
                                 .Include("Transaction.Currency")
                                 .Include("TransactionSignature")
                                 .Where(t => (t.ReturnCode != null && t.MobileAppId == mobileAppId) && (t.TransactionId.ToString().Contains(searchTrans) || t.TransactionAttemptId.ToString().Contains(searchTrans))).ToList();

                         totalrecords = qry.Count();

                         return qry.Where(x => x.Amount.ToString() != "0.00").ToList();
                    } catch (Exception ex) {
                         throw ex;
                    }
               }
          }

          public TransactionAttempt GetCreditTransactionByTraceNumber(string traceNumber) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttempts
                            .Include("Transaction")
                            .Include("Transaction.MerchantPOS")
                            .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                            .Include("TransactionSignature")
                            .Include("Transaction.Currency")
                            .SingleOrDefault(t => t.TransNumber == traceNumber);
               }
          }

          #endregion

          #region Signature
          public TransactionSignature CreateTransactionSignature(TransactionSignature transSignature, TransactionAttempt transAttempt) {
               using (DataContext context = new DataContext()) {
                    using (var trans = context.Database.BeginTransaction()) {
                         try {
                              var transSign = context.TransactionSignature.Add(transSignature);
                              context.SaveChanges();

                              var transAtt = context.TransactionAttempts.SingleOrDefault(t => t.TransactionAttemptId == transAttempt.TransactionAttemptId && t.TransactionId == transAttempt.TransactionId);
                              transAtt.TransactionSignatureId = transSign.TransactionSignatureId;
                              context.SaveChanges();

                              trans.Commit();

                              return transSign;
                         } catch (Exception ex) {
                              trans.Rollback();
                              throw ex;
                         }
                    }
               }
          }

          public TransactionSignature CreateTransactionSignatureDebit(TransactionSignature transSignature, TransactionAttemptDebit transAttemptDebit) {
               using (DataContext context = new DataContext()) {
                    using (var trans = context.Database.BeginTransaction()) {
                         try {
                              var transSign = context.TransactionSignature.Add(transSignature);
                              context.SaveChanges();

                              var transAtt = context.TransactionAttemptDebit.SingleOrDefault(t => t.TransactionAttemptDebitId == transAttemptDebit.TransactionAttemptDebitId && t.TransactionDebitId == transAttemptDebit.TransactionDebitId);
                              transAtt.TransactionSignatureId = transSign.TransactionSignatureId;
                              context.SaveChanges();

                              trans.Commit();

                              return transSign;
                         } catch (Exception ex) {
                              trans.Rollback();
                              throw ex;
                         }
                    }
               }
          }

          public TransactionSignature CreateTransactionSignatureCash(TransactionSignature transSignature, TransactionAttemptCash transAttempt) {
               using (DataContext context = new DataContext()) {
                    using (var trans = context.Database.BeginTransaction()) {
                         try {
                              var transSign = context.TransactionSignature.Add(transSignature);
                              context.SaveChanges();

                              var transAtt = context.TransactionAttemptCash.SingleOrDefault(t => t.TransactionAttemptCashId == transAttempt.TransactionAttemptCashId && t.TransactionCashId == transAttempt.TransactionCashId);
                              transAtt.TransactionSignatureId = transSign.TransactionSignatureId;
                              context.SaveChanges();

                              trans.Commit();

                              return transSign;
                         } catch (Exception ex) {
                              trans.Rollback();
                              throw ex;
                         }
                    }
               }
          }

          #endregion

          #region Debit Transaction

          public TransactionDebit GetDebitTransaction(int transactionDebitId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionDebit
                                  .Include("TransactionAttemptDebit")
                                  .Include("MerchantPOS")
                                  .Include("MerchantPOS.MerchantBranch")
                                  .Include("MerchantPOS.MerchantBranch.Merchant")
                                  .Include("Key")
                                  .SingleOrDefault(t => t.TransactionDebitId == transactionDebitId);
               }
          }

          public TransactionAttemptDebit GetDebitTransactionByTraceNumber(string traceNumber) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttemptDebit
                            .Include("TransactionDebit")
                            .Include("TransactionDebit.MerchantPOS")
                            .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                            .Include("TransactionSignature")
                            .Include("TransactionDebit.Currency")
                            .SingleOrDefault(t => t.TraceNumber == traceNumber);
               }
          }

          public TransactionAttemptDebit GetDebitTransactionAttempt(int transactionAttemptId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttemptDebit
                                  .Include("TransactionDebit")
                                  .SingleOrDefault(t => t.TransactionAttemptDebitId == transactionAttemptId);
               }
          }

          public TransactionDebit CreateTransactionDebit(TransactionDebit transaction, TransactionAttemptDebit transactionAttemptDebit) {
               using (DataContext context = new DataContext()) {
                    var trans = context.TransactionDebit.Add(transaction);

                    context.SaveChanges();

                    if (trans.TransactionDebitId > 0) {
                         transactionAttemptDebit.TransactionDebitId = trans.TransactionDebitId;

                         var transAttempt = context.TransactionAttemptDebit.Add(transactionAttemptDebit);

                         context.SaveChanges();

                         transactionAttemptDebit.TransactionAttemptDebitId = transAttempt.TransactionAttemptDebitId;
                    }

                    return trans;
               }
          }

          public TransactionAttemptDebit CreateTransactionAttemptDebit(TransactionAttemptDebit transactionAttemptDebit) {
               using (DataContext context = new DataContext()) {
                    var trans = context.TransactionAttemptDebit.Add(transactionAttemptDebit);

                    context.SaveChanges();

                    return trans;
               }
          }

          public TransactionAttemptDebit UpdateTransactionAttemptDebit(TransactionAttemptDebit transaction) {
               using (DataContext context = new DataContext()) {
                    var ut = context.TransactionAttemptDebit.Attach(transaction);
                    var entryT = context.Entry(transaction);

                    entryT.Property(e => e.Amount).IsModified = true;
                    entryT.Property(e => e.AuthNumber).IsModified = true;
                    entryT.Property(e => e.ReturnCode).IsModified = true;
                    entryT.Property(e => e.ReferenceNumber).IsModified = true;
                    entryT.Property(e => e.SeqNumber).IsModified = true;
                    entryT.Property(e => e.TraceNumber).IsModified = true;
                    entryT.Property(e => e.RetrievalRefNumber).IsModified = true;
                    entryT.Property(e => e.BatchNumber).IsModified = true;
                    entryT.Property(e => e.DisplayReceipt).IsModified = true;
                    entryT.Property(e => e.DisplayTerminal).IsModified = true;
                    entryT.Property(e => e.DateReceived).IsModified = true;
                    entryT.Property(e => e.DepositDate).IsModified = true;
                    entryT.Property(e => e.Notes).IsModified = true;
                    entryT.Property(e => e.TransactionTypeId).IsModified = true;

                    context.SaveChanges();

                    return entryT.Entity;
               }
          }

          public List<TransactionAttemptDebit> GetDebitTransactionAttemptByRefNumApp(string appRefNum) {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionAttemptDebit.Include("TransactionDebit")
                                 .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                 .Include("TransactionDebit.MerchantPOS")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("TransactionDebit.TransactionEntryType")
                                 .Include("TransactionDebit.CardType")
                                 .Include("TransactionDebit.Key")
                                 .Include("TransactionDebit.Mid.Switch")
                                 .Include("TransactionSignature")
                                 .Where(t => t.TransactionDebit.RefNumApp == appRefNum).ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttemptDebit> GetDebitTransactionAttemptByTransNumber(int transId, int transAttId) {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionAttemptDebit
                                 .Include("TransactionDebit")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("TransactionDebit.TransactionEntryType")
                                 .Include("TransactionDebit.Key")
                                 .Include("TransactionSignature")
                                 .Include("TransactionDebit.Currency")
                                 .Where(t => t.TransactionDebitId == transId && t.TransactionAttemptDebitId == transAttId && t.ReturnCode != null).ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttemptDebit> GetDebitTransactionByMobileAppId(int mobileAppId) {
               using (DataContext context = new DataContext()) {
                    try {
                         var trans = context.TransactionAttemptDebit
                                 .Include("TransactionDebit")
                                 .Include("TransactionDebit.MerchantPOS.MerchantBranch")
                                 .Include("TransactionDebit.MerchantPOS")
                                 .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("TransactionDebit.TransactionEntryType")
                                 .Include("TransactionDebit.Key")
                                 .Include("TransactionDebit.Currency")
                                 .Include("TransactionSignature")
                                 .Where(t => t.MobileAppId == mobileAppId && t.ReturnCode != null && t.DateReceived >= DateTime.Today).OrderByDescending(t => t.DateReceived).Take(10).ToList();

                         return trans.Where(x => x.Amount.ToString() != "0.00").ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttemptDebit> GetDebitTransactionAllByMobileAppId(int mobileAppId, string searchTrans, out int totalrecords) {
               using (DataContext context = new DataContext()) {
                    try {
                         var qry = context.TransactionAttemptDebit
                                 .Include("TransactionDebit")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("TransactionDebit.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("TransactionDebit.TransactionEntryType")
                                 .Include("TransactionDebit.Key")
                                 .Include("TransactionDebit.Currency")
                                 .Include("TransactionSignature")
                                 .Where(t => (t.ReturnCode != null && t.MobileAppId == mobileAppId) && (t.TransactionDebitId.ToString().Contains(searchTrans) || t.TransactionAttemptDebitId.ToString().Contains(searchTrans))).ToList();

                         totalrecords = qry.Count();

                         return qry.Where(x => x.Amount.ToString() != "0.00").ToList();
                    } catch (Exception ex) {
                         throw ex;
                    }
               }
          }

          #endregion Debit Transaction

          #region Void Transaction
          public List<TransactionVoidReason> GetAllTransactionVoidReason() {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionVoidReason.ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public TransactionVoidReason GetTransactionVoidReasonById(int id) {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionVoidReason.SingleOrDefault(v => v.TransactionVoidReasonId == id);
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public bool IsCreditTransactionAlreadyVoid(int transId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttempts.SingleOrDefault(t => t.TransactionId == transId && t.TransactionTypeId == 4 && t.ReturnCode == "00") == null;
               }
          }

          public bool IsDebitTransactionAlreadyVoid(int transId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttemptDebit.SingleOrDefault(t => t.TransactionDebitId == transId && t.TransactionTypeId == 4 && t.ReturnCode == "00") == null;
               }
          }
          #endregion

          #region Cash Transaction
          public TransactionCash CreateCashTransaction(TransactionCash transactionCash, TransactionAttemptCash transactionAttemptCash) {
               using (DataContext context = new DataContext()) {
                    try {
                         var trans = context.TransactionCash.Add(transactionCash);
                         context.SaveChanges();

                         if (trans.TransactionCashId > 0) {
                              transactionAttemptCash.TransactionCashId = trans.TransactionCashId;

                              var transAttempt = context.TransactionAttemptCash.Add(transactionAttemptCash);

                              context.SaveChanges();

                              transactionAttemptCash.TransactionAttemptCashId = transAttempt.TransactionAttemptCashId;
                         }

                         return trans;
                    } catch (System.Data.Entity.Validation.DbEntityValidationException dbEx) {
                         Exception raise = dbEx;

                         foreach (var validationErrors in dbEx.EntityValidationErrors) {
                              foreach (var validationError in validationErrors.ValidationErrors) {
                                   string msg = string.Format("{0} : {1}", validationErrors.Entry.Entity.ToString(),
                                       validationError.ErrorMessage);
                                   raise = new InvalidOperationException(msg, raise);
                              }
                         }
                         throw raise;
                    }
               }
          }

          public TransactionAttemptCash CreateCashTransactionAttempt(TransactionAttemptCash transactionAttemptCash) {
               using (DataContext context = new DataContext()) {
                    try {
                         var trans = context.TransactionAttemptCash.Add(transactionAttemptCash);

                         context.SaveChanges();

                         return trans;
                    } catch (System.Data.Entity.Validation.DbEntityValidationException dbEx) {
                         Exception raise = dbEx;

                         foreach (var validationErrors in dbEx.EntityValidationErrors) {
                              foreach (var validationError in validationErrors.ValidationErrors) {
                                   string msg = string.Format("{0} : {1}", validationErrors.Entry.Entity.ToString(),
                                       validationError.ErrorMessage);
                                   raise = new InvalidOperationException(msg, raise);
                              }
                         }
                         throw raise;
                    }
               }
          }

          public TransactionAttemptCash UpdateTransactionAttemptCash(TransactionAttemptCash transaction) {
               using (DataContext context = new DataContext()) {
                    var ut = context.TransactionAttemptCash.Attach(transaction);
                    var entryT = context.Entry(transaction);

                    entryT.Property(e => e.ReturnCode).IsModified = true;
                    entryT.Property(e => e.AuthNumber).IsModified = true;
                    entryT.Property(e => e.TransNumber).IsModified = true;
                    entryT.Property(e => e.BatchNumber).IsModified = true;
                    entryT.Property(e => e.SeqNumber).IsModified = true;
                    entryT.Property(e => e.DisplayReceipt).IsModified = true;
                    entryT.Property(e => e.DisplayTerminal).IsModified = true;
                    entryT.Property(e => e.DateReceived).IsModified = true;
                    entryT.Property(e => e.DepositDate).IsModified = true;
                    entryT.Property(e => e.Notes).IsModified = true;
                    entryT.Property(e => e.TransactionTypeId).IsModified = true;
                    entryT.Property(e => e.TransactionSignatureId).IsModified = true;
                    entryT.Property(e => e.Amount).IsModified = true;

                    context.SaveChanges();

                    return entryT.Entity;
               }
          }

          public List<TransactionAttemptCash> GetCashTransactionByMobileAppId(int mobileAppId) {
               using (DataContext context = new DataContext()) {
                    try {
                         var trans = context.TransactionAttemptCash.Include("TransactionCash")
                                 .Include("TransactionCash.MerchantPOS.MerchantBranch")
                                 .Include("TransactionCash.MerchantPOS")
                                 .Include("TransactionCash.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("TransactionCash.TransactionEntryType")
                                 .Include("TransactionCash.Currency")
                                 .Include("TransactionSignature")
                                 .Where(t => t.MobileAppId == mobileAppId && t.ReturnCode != null && t.DateReceived >= DateTime.Today).OrderByDescending(t => t.DateReceived).Take(10).ToList();

                         return trans.Where(x => x.Amount.ToString() != "0.00").ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttemptCash> GetCashTransactionAttemptByRefNumApp(string appRefNum) {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionAttemptCash.Include("TransactionCash")
                                 .Include("TransactionCash.MerchantPOS.MerchantBranch")
                                 .Include("TransactionCash.MerchantPOS")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("TransactionCash.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("TransactionCash.TransactionEntryType")
                                 .Include("TransactionCash.Mid.Switch")
                                 .Include("TransactionSignature")
                                 .Where(t => t.TransactionCash.RefNumApp == appRefNum).ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttemptCash> GetCashTransactionAttemptByTransNumber(int transId, int transAttId) {
               using (DataContext context = new DataContext()) {
                    try {
                         return context.TransactionAttemptCash
                                 .Include("TransactionCash")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("TransactionCash.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("TransactionCash.TransactionEntryType")
                                 .Include("TransactionSignature")
                                 .Include("TransactionCash.Currency")
                                 .Where(t => t.TransactionCashId == transId && t.TransactionAttemptCashId == transAttId && t.ReturnCode != null).ToList();
                    } catch (Exception ex) {
                         return null;
                    }
               }
          }

          public List<TransactionAttemptCash> GetCashTransactionAllByMobileAppId(int mobileAppId, string searchTrans, out int totalrecords) {
               using (DataContext context = new DataContext()) {
                    try {
                         var qry = context.TransactionAttemptCash
                                 .Include("TransactionCash")
                                 .Include("MobileApp.MerchantBranchPOS.MerchantBranch.Merchant")
                                 .Include("TransactionCash.MerchantPOS.MerchantBranch.Merchant")
                                 .Include("TransactionCash.TransactionEntryType")
                                 .Include("TransactionCash.Currency")
                                 .Include("TransactionSignature")
                                 .Where(t => (t.ReturnCode != null && t.MobileAppId == mobileAppId) && (t.TransactionCashId.ToString().Contains(searchTrans) || t.TransactionAttemptCashId.ToString().Contains(searchTrans))).ToList();

                         totalrecords = qry.Count();

                         return qry.Where(x => x.Amount.ToString() != "0.00").ToList();
                    } catch (Exception ex) {
                         throw ex;
                    }
               }
          }

          public TransactionCash GetCashTransaction(int transactionId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionCash
                                  .Include("TransactionAttemptCash")
                                  .Include("MerchantPOS")
                                  .Include("MerchantPOS.MerchantBranch")
                                  .Include("MerchantPOS.MerchantBranch.Merchant")
                                  .Include("Mid")
                                  .Include("Currency")
                                  .SingleOrDefault(t => t.TransactionCashId == transactionId);
               }
          }

          public TransactionAttemptCash GetTransactionAttemptCash(int transactionAttemptId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttemptCash
                                  .Include("TransactionCash")
                                  .Include("TransactionSignature")
                                  .SingleOrDefault(t => t.TransactionAttemptCashId == transactionAttemptId);
               }
          }

          public bool IsCashTransactionAlreadyVoid(int transId) {
               using (DataContext context = new DataContext()) {
                    return context.TransactionAttemptCash.SingleOrDefault(t => t.TransactionCashId == transId && t.TransactionTypeId == 4 && t.ReturnCode == "00") == null;
               }
          }
          #endregion


          public async Task<List<TransactionAttempt>> ReportCentral(int? pId, int? rId, int? mId, int? posId, int? bId, int? cardTypeId, int? transTypeId, int? actionId, string currencyName, DateTime? startDate, DateTime? endDate, string sSearch, int iDisplayStart, int iDisplayLength, string v, string sSortDir_0) {
               return await Task.Run(() => {
                    using (DataContext context = new DataContext()) {
                         List<TransactionAttempt> results = new List<TransactionAttempt>();
                         try {
                              string sqlCommand = string.Format("exec {0}", "GetKeys");
                              var keys = context.Database.SqlQuery<_Key>(sqlCommand, new List<dynamic>().ToArray()).ToList();
                              results = context.TransactionAttempts
                                               .SqlQuery(ReportCentralQueryBuilder(pId, rId, mId, posId, bId, cardTypeId, transTypeId, actionId, currencyName, startDate, endDate))
                                               .ToList();
                              if (results.Count > 0) {
                                   results.ForEach(a => {
                                        a.Transaction = context.Transactions.Where(b => b.TransactionId == a.TransactionId).FirstOrDefault();
                                        a.Transaction.CardType = context.CardTypes.Where(b => b.CardTypeId == a.Transaction.CardTypeId).FirstOrDefault();
                                        a.Transaction.Currency = context.Currencies.Where(b => b.CurrencyId == a.Transaction.CurrencyId).FirstOrDefault();
                                        if (!string.IsNullOrEmpty(currencyName)) {
                                             a.Transaction.MerchantPOS = context.MerchantPOSs.Where(b => b.MerchantPOSId == a.Transaction.MerchantPOSId).FirstOrDefault();
                                             a.Transaction.MerchantPOS.MerchantBranch = context.MerchantBranches.Where(b => b.MerchantBranchId == a.Transaction.MerchantPOS.MerchantBranchId).FirstOrDefault();
                                             a.Transaction.MerchantPOS.MerchantBranch.Merchant = context.Merchants.Where(b => b.MerchantId == a.Transaction.MerchantPOS.MerchantBranch.MerchantId).FirstOrDefault();
                                             a.Transaction.Key = keys.Where(b => b.KeyId == a.Transaction.KeyId).FirstOrDefault();
                                             if (a.TransactionSignatureId.HasValue) {
                                                  a.TransactionSignature = context.TransactionSignature.Where(b => b.TransactionSignatureId == a.TransactionSignatureId.Value).FirstOrDefault();
                                             }
                                             if (a.TransactionVoidReasonId.HasValue) {
                                                  a.TransactionVoidReason = context.TransactionVoidReason.Where(b => b.TransactionVoidReasonId == a.TransactionVoidReasonId.Value).FirstOrDefault();
                                             }
                                        }
                                   });
                              }
                         } catch (Exception ex) {
                              throw ex;
                         }
                         return results;
                    }
               });
          }
          public string ReportCentralQueryBuilder(int? pId, int? rId, int? mId, int? posId, int? bId, int? cardTypeId, int? transTypeId, int? actionId, string currencyName, DateTime? startDate, DateTime? endDate) {
               string sql = "select transactionattempts.*"
                         + " from transactionattempts"
                         + " inner join transactions on transactionattempts.TransactionId = transactions.TransactionId"
                         + " left join Keys on transactions.KeyId = Keys.KeyId"
                         + " left outer join MerchantBranchPOSs on transactions.MerchantPOSId = MerchantBranchPOSs.MerchantPOSId"
                         + " left join MerchantBranches on MerchantBranchPOSs.MerchantBranchId = MerchantBranches.MerchantBranchId"
                         + " left join Merchants on MerchantBranches.MerchantId = Merchants.MerchantId"
                         + " left join Resellers on Merchants.ResellerId = Resellers.ResellerId"
                         + " left outer join Currencies on transactions.CurrencyId = Currencies.CurrencyId";
               sql += " AND transactionattempts.ReturnCode != null AND transactionattempts.ReturnCode = '00'";
               if (startDate.Value != DateTime.MinValue) {
                    sql += " WHERE transactionattempts.DateReceived >= CAST('" + startDate.Value.ToString("yyyy-MM-dd") + " 00:00:00.000' AS Datetime)";
               } else {
                    sql += " WHERE transactionattempts.DateReceived >= CAST('" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000' AS Datetime)";
               }
               if (endDate.Value != DateTime.MinValue) {
                    sql += " AND transactionattempts.DateReceived <= CAST('" + endDate.Value.ToString("yyyy-MM-dd") + " 23:59:59.999' AS Datetime)";
               } else {
                    sql += " AND transactionattempts.DateReceived <= CAST('" + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59.999' AS Datetime)";
               }
               if (pId.HasValue && pId.Value != 0 && pId.Value > 0) {
                    sql += " AND Resellers.PartnerId = CAST('" + pId.Value + "' AS Int)";
               }
               if (rId.HasValue && rId.Value != 0 && rId.Value > 0) {
                    sql += " AND Resellers.ResellerId = CAST('" + rId.Value + "' AS Int)";
               }
               if (mId.HasValue && mId.Value != 0 && mId.Value > 0) {
                    sql += " AND Merchants.MerchantId = CAST('" + mId.Value + "' AS Int)";
               }
               if (bId.HasValue && bId.Value != 0 && bId.Value > 0) {
                    sql += " AND MerchantBranchPOSs.MerchantBranchId = CAST('" + bId.Value + "' AS Int)";
               }
               if (posId.HasValue && posId.Value != 0 && posId.Value > 0) {
                    sql += " AND Transactions.MerchantPOSId = CAST('" + posId.Value + "' AS Int)";
               }
               if (transTypeId.HasValue && transTypeId.Value != 0 && transTypeId.Value > 0) {
                    sql += " AND transactions.TransactionEntryTypeId = CAST('" + transTypeId.Value + "' AS Int)";
               }
               if (actionId.HasValue && actionId.Value != 0 && actionId.Value > 0) {
                    sql += " AND transactionattempts.TransactionTypeId = CAST('" + actionId.Value + "' AS Int)";
               }
               if (cardTypeId.HasValue && cardTypeId.Value != 0 && cardTypeId.Value > 0) {
                    sql += " AND transactions.CardTypeId = CAST('" + cardTypeId.Value + "' AS Int)";
               }
               if (!string.IsNullOrEmpty(currencyName)) {
                    sql += " AND Currencies.CurrencyName = '" + currencyName + "'";
               }
               return sql;
          }
     }
     public class StoredProcedureParam {
          public string Param { get; set; }
          public string Value { get; set; }
     }
}