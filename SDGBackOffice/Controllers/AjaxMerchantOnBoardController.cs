using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGBackOffice.Models;
using SDGDAL;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using System.Globalization;
using CTPayment;

namespace SDGBackOffice.Controllers
{
    public class AjaxMerchantOnBoardController : Controller
    {
        MidsRepository _midsRepo;
        MerchantOnBoardRepository _merchantOnBoardRepo;
        ApiCTPayment _sendCTRequest;
        MerchantRepository _merchantRepo;

        public AjaxMerchantOnBoardController()
        {
            _merchantOnBoardRepo = new MerchantOnBoardRepository();
            _sendCTRequest = new ApiCTPayment();
            _midsRepo = new MidsRepository();
            _merchantRepo = new MerchantRepository();
        }

        [HttpPost]
        public ActionResult MerchantOnBoardResponseFilenameList()
        {
            try
            {
                List<string> apiFileNames = new List<string>();
                DataTableModel data = new DataTableModel(Request);
                int totalRecords = 0;

                var results = _merchantOnBoardRepo.GetAllMerchantOnBoardResponse(data.sSearch, data.iDisplayStart, data.iDisplayLength, "DateReceived", data.sSortDir_0, out totalRecords);

                var resultsTable = results.Select(m => new
                {
                    FileId = m.MerchantOnBoardResponseId,
                    FileName = m.ResponseFileName,
                    DateReceived = m.DateReceived.ToString("F")
                }).OrderByDescending(x => x.DateReceived);

                return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = resultsTable });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetMerchantMidsTobeAdded(int? id)
        { 
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int resellerId = 0;

            if (id.Value != 0)
            {
                resellerId = id.Value;
            }
            else
            {
                if (CurrentUser.ParentType != Enums.ParentType.Reseller)
                {
                    int.TryParse(Request["ResellerId"], out resellerId);
                }
                else
                {
                    resellerId = CurrentUser.ParentId;
                }
            }

            var merchants = new List<Mid>();

            //get all merchant with CT Mids
            var listMidsCTMerchant = _midsRepo.GetMerchantMidAddedByResellerId(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords).GroupBy(i => i.MerchantId,  (key, group) => group.First()).ToArray(); ;

            //loop all MerchantID
            MerchantBulkModel model = new MerchantBulkModel();
            List<MerchantBulkModel> merchantResult = new List<MerchantBulkModel>();

            for (int i = 0; i < listMidsCTMerchant.Count(); i++)
            {
                try
                {
                    int merchantId = Convert.ToInt32(listMidsCTMerchant[i].MerchantId);
                    string[] noData = { "None" };
                    string command = string.Empty;
                    string MID = string.Empty;

                    //get merchant Info from Mids
                    var merchantInfo = _merchantRepo.GetDetailsbyMerchantId(merchantId);

                    var midsMerchant = _midsRepo.GetAllListMidsToBeAdded(merchantId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords);

                    List<string> creditCardMid = new List<string>();
                    List<string> debitCardMid = new List<string>();

                    for (int j = 0; j < midsMerchant.Count(); j++)
                    {
                        if (midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.MasterCard)
                            || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Visa)
                            || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.JCB)
                            || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Discover)
                            || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.AmericanExpress)
                            || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Diners))
                        {
                            creditCardMid.Add(midsMerchant[j].CardType.TypeName + "(" + midsMerchant[j].Param_6 + ")");
                        }
                        else if (midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Debit))
                        {
                            debitCardMid.Add(midsMerchant[j].CardType.TypeName + "(" + midsMerchant[j].Param_6 + ")");
                        }

                        if (merchantInfo.ContactInformation.NeedsUpdate == true)
                        {
                            command = "Need to update first the information of this Merchant.";
                        }
                        else
                        {
                            if (merchantInfo.NeedAddToCT)
                            {
                                command += "- ADD MERCHANT TO CTPayment. <br/>";
                            }
                            if (midsMerchant[j].NeedAddBulk)
                            {
                                command += "- ADD Credit/Debit MID to this Merchant " + midsMerchant[j].Param_2 + "<br/>";
                            }
                            if (midsMerchant[j].NeedAddTerminal)
                            {
                                command += "- ADD TERMINAL to this Merchant " + midsMerchant[j].Param_2 + "<br/>";
                            }
                        }

                        MID = midsMerchant[j].Param_2;
                    }

                    merchantResult.Add(new MerchantBulkModel()
                    {
                        MerchantId = Convert.ToString(merchantId),
                        MerchantName = merchantInfo.MerchantName,
                        ResellerName = merchantInfo.Reseller.ResellerName,
                        PartnerName = merchantInfo.Partner.CompanyName,
                        CreditCard = creditCardMid.Count() == 0 ? noData : creditCardMid.ToArray(),
                        DebitCard = debitCardMid.Count() == 0 ? noData : debitCardMid.ToArray(),
                        MerchantInfo = merchantInfo.ContactInformation.Address + " "
                                     + merchantInfo.ContactInformation.City + " "
                                     + merchantInfo.ContactInformation.StateProvince + " "
                                     + merchantInfo.ContactInformation.Country.CountryName + " "
                                     + merchantInfo.ContactInformation.ZipCode,
                        PhoneNumber = merchantInfo.ContactInformation.MobileNumber,
                        NeedUpdateMerchant = merchantInfo.ContactInformation.NeedsUpdate,
                        MID = MID,
                        Command = command
                    });
                }
                catch (Exception ex)
                {

                }
            }

            totalRecords = merchantResult.Count();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchantResult });

        }

        [HttpPost]
        public ActionResult ViewAddBulkData(string ids)
        {
            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            var merchId = ids.Split(",".ToArray());

            MerchantBulkModel model = new MerchantBulkModel();
            List<MerchantBulkModel> merchantResult = new List<MerchantBulkModel>();

            for (int i = 0; i < merchId.Count(); i++)
            {
                int merchantId = Convert.ToInt32(merchId[i]);
                string[] noData = { "None" };
                //get merchant Info
                var merchantInfo = _merchantRepo.GetDetailsbyMerchantId(merchantId);

                var midsMerchant = _midsRepo.GetAllListMidsToBeAdded(merchantId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords);

                List<string> creditCardMid = new List<string>();
                List<string> debitCardMid = new List<string>();

                for (int j = 0; j < midsMerchant.Count(); j++)
                {
                    if (midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.MasterCard)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Visa)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.JCB)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Discover)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.AmericanExpress)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Diners))
                    {
                        creditCardMid.Add(midsMerchant[j].CardType.TypeName);
                    }
                    else if (midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Debit))
                    {
                        debitCardMid.Add(midsMerchant[j].CardType.TypeName);
                    }
                }

                merchantResult.Add(new MerchantBulkModel()
                {
                    MerchantId = Convert.ToString(merchantId),
                    MerchantName = merchantInfo.MerchantName,
                    ResellerName = merchantInfo.Reseller.ResellerName,
                    PartnerName = merchantInfo.Partner.CompanyName,
                    CreditCard = creditCardMid.Count() == 0 ? noData : creditCardMid.ToArray(),
                    DebitCard = debitCardMid.Count() == 0 ? noData : debitCardMid.ToArray(),
                    Command = "ADD"
                });
            }

            totalRecords = merchantResult.Count();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchantResult });
        }

        [HttpPost]
        public ActionResult GetMerchantMidsTobeUpdated(int? id, int? actionId)
        {
            string noData = "Data is not available.";
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int resellerId = 0;
            int actId = 0;

            if (id.Value != 0)
            {
                resellerId = id.Value;
                actId = Convert.ToInt32(actionId);
            }
            else
            {
                if (CurrentUser.ParentType != Enums.ParentType.Reseller)
                {
                    int.TryParse(Request["ResellerId"], out resellerId);
                }
                else
                {
                    resellerId = CurrentUser.ParentId;
                }
            }

            var merchants = new List<Mid>();

            if (actId == 1)
            {
                merchants = _midsRepo.GetMerchantMidUpdatedByResellerIdAndMerchant(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords).GroupBy(i => i.MerchantId, (key, group) => group.First()).ToList();
            }
            else if (actId == 2)
            {
                merchants = _midsRepo.GetMerchantMidUpdatedByResellerIdAndTerminal(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords);
            }

            var merchantModel = merchants.Select(m => new MerchantOnBoardModel()
            {
                MidId = m.MidId,
                MerchantName = m.Merchant.MerchantName,
                MerchantId = m.MerchantId,
                IsActive = m.IsActive,
                ResellerName = m.Merchant.Reseller.ResellerName,
                PartnerName = m.Merchant.Reseller.Partner.CompanyName,
                MerchantMids = m.Param_2,
                TerminalId = m.Param_6,
                SetLikeTerminalId = m.SetLikeTerminalId,
                NeedMidsUpdateToCT = m.NeedUpdateBulk,
                MidCardType = m.CardType.TypeName,
                MerchantAddress = m.Merchant.ContactInformation.Address + " " + m.Merchant.ContactInformation.City + " " + m.Merchant.ContactInformation.StateProvince + " " + m.Merchant.ContactInformation.ProvIsoCode + " " +
                                  m.Merchant.ContactInformation.Country.CountryName + " " + m.Merchant.ContactInformation.ZipCode,
                Command = "Update",
                ActionId = actionId
            }).ToList();

            totalRecords = merchantModel.Count();
            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchantModel });
        }

        [HttpPost]
        public ActionResult GetMerchantMidsTobeDeleted(int? id, int? actionId)
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int resellerId = 0;
            int actId = 0;

            if (id.Value != 0)
            {
                resellerId = id.Value;
                actId = Convert.ToInt32(actionId);
            }
            else
            {
                if (CurrentUser.ParentType != Enums.ParentType.Reseller)
                {
                    int.TryParse(Request["ResellerId"], out resellerId);
                }
                else
                {
                    resellerId = CurrentUser.ParentId;
                }
            }

            var merchants = new List<Mid>();

            if (actionId == 1)
            {
                merchants = _midsRepo.GetMerchantMidDeletedByResellerIdAndCreditMerchant(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords);
            }
            else if (actionId == 2)
            {
                merchants = _midsRepo.GetMerchantMidDeletedByResellerIdAndTerminal(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords);
            }
            else if (actionId == 3)
            {
                merchants = _midsRepo.GetMerchantMidDeletedByResellerIdAndMerchant(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords).GroupBy(i => i.MerchantId, (key, group) => group.First()).ToList();
            }

            var merchantModel = merchants.Select(m => new MerchantOnBoardModel()
            {
                MidId = m.MidId,
                MerchantName = m.Merchant.MerchantName,
                MerchantId = m.MerchantId,
                IsActive = m.IsActive,
                ResellerName = m.Merchant.Reseller.ResellerName,
                PartnerName = m.Merchant.Reseller.Partner.CompanyName,
                MerchantMids = m.Param_2,
                TerminalId = m.Param_6,
                SetLikeTerminalId = m.SetLikeTerminalId,
                NeedMidsUpdateToCT = m.NeedUpdateBulk,
                MidCardType = m.CardType.TypeName,
                MerchantAddress = m.Merchant.ContactInformation.Address + " " + m.Merchant.ContactInformation.City + " " + m.Merchant.ContactInformation.StateProvince + " " + m.Merchant.ContactInformation.ProvIsoCode + " " +
                                   m.Merchant.ContactInformation.Country.CountryName + " " + m.Merchant.ContactInformation.ZipCode,
                Command = "Delete",
                ActionId = actionId
            }).ToList();

            totalRecords = merchantModel.Count();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchantModel });
        }
    }
}
