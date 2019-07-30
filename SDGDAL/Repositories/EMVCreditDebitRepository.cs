using SDGDAL.Entities;
using System;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class EMVCreditDebitRepository
    {
        public bool SaveDownloadHostTerminal(HeaderResponse headerResponse, EmvHostParameter hostParam, EmvTerminalParameter terminalParam,
                                                       EmvMastercardParameter emvMcParam, EmvVisaParameter emvVisaParam, EmvAmexParameter emvAmexParam,
                                                       EmvInteracParameter emvInteracParam, EmvJcbParameter emvJcbParam, EmvDiscoverParameter emvDiscoverParam)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        hostParam.DateCreated = DateTime.Now;
                        hostParam.IsActive = true;
                        var savedHostParam = context.EmvHostParameters.Add(hostParam);
                        context.SaveChanges();

                        terminalParam.DateCreated = DateTime.Now;
                        terminalParam.IsActive = true;
                        var savedTerminalParam = context.EmvTerminalParameters.Add(terminalParam);
                        context.SaveChanges();

                        emvMcParam.DateCreated = DateTime.Now;
                        emvMcParam.IsActive = true;
                        var savedEmvMcParam = context.EmvMastercardParameters.Add(emvMcParam);
                        context.SaveChanges();

                        emvVisaParam.DateCreated = DateTime.Now;
                        emvVisaParam.IsActive = true;
                        var savedEmvVisaParam = context.EmvVisaParameters.Add(emvVisaParam);
                        context.SaveChanges();

                        emvAmexParam.DateCreated = DateTime.Now;
                        emvAmexParam.IsActive = true;
                        var savedEmvAmexParam = context.EmvAmexParameters.Add(emvAmexParam);
                        context.SaveChanges();

                        emvInteracParam.DateCreated = DateTime.Now;
                        emvInteracParam.IsActive = true;
                        var savedEmvInteracParam = context.EmvInteracParameters.Add(emvInteracParam);
                        context.SaveChanges();

                        emvJcbParam.DateCreated = DateTime.Now;
                        emvJcbParam.IsActive = true;
                        var savedEmvJcbParam = context.EmvJcbParameters.Add(emvJcbParam);
                        context.SaveChanges();

                        emvDiscoverParam.DateCreated = DateTime.Now;
                        emvDiscoverParam.IsActive = true;
                        var savedEmvDiscoverParam = context.EmvDiscoverParameters.Add(emvDiscoverParam);
                        context.SaveChanges();

                        headerResponse.EmvHostParameterId = savedHostParam.EmvHostParameterId;
                        headerResponse.EmvTerminalParameterId = savedTerminalParam.EmvTerminalParameterId;
                        headerResponse.DateCreated = DateTime.Now;
                        headerResponse.IsActive = true;
                        headerResponse.EmvAmexParameterId = savedEmvAmexParam.EmvAmexParameterId;
                        headerResponse.EmvDiscoverParameterId = savedEmvDiscoverParam.EmvDiscoverParameterId;
                        headerResponse.EmvInteracParameterId = savedEmvInteracParam.EmvInteracParameterId;
                        headerResponse.EmvJcbParametersId = savedEmvJcbParam.EmvJcbParameterId;
                        headerResponse.EmvMastercardParameterId = savedEmvMcParam.EmvMastercardParameterId;
                        headerResponse.EmvVisaParameterId = savedEmvVisaParam.EmvVisaParameterId;

                        var savedHeaderResponse = context.HeaderResponse.Add(headerResponse);
                        context.SaveChanges();

                        trans.Commit();

                        return true;
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        trans.Rollback();
                        Exception raise = dbEx;

                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string msg = string.Format("{0} : {1}", validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                raise = new InvalidOperationException(msg, raise);
                            }
                        }
                        throw raise;
                    }
                }
            }
        }

        public HeaderResponse SaveHeaderResponse(HeaderResponse response)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        response.DateCreated = DateTime.Now;
                        var savedHeaderResponse = context.HeaderResponse.Add(response);

                        return savedHeaderResponse;
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        trans.Rollback();
                        Exception raise = dbEx;

                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string msg = string.Format("{0} : {1}", validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                raise = new InvalidOperationException(msg, raise);
                            }
                        }
                        throw raise;
                    }
                }
            }
        }

        public HeaderResponse GetDownloadEMVParameters(string terminalId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var qry = context.HeaderResponse.Include("EmvHostParameter")
                              .Include("EmvTerminalParameter")
                              .Include("EmvMastercardParameter")
                              .Include("EmvVisaParameter")
                              .Include("EmvAmexParameter")
                              .Include("EmvInteracParameter")
                              .Include("EmvJcbParameter")
                              .Include("EmvDiscoverParameter")
                              .SingleOrDefault(e => e.TerminalId == terminalId);

                    return qry;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}