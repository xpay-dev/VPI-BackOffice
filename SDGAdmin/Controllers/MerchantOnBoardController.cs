using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGAdmin.Models;
using SDGDAL;
using SDGUtil;
using SDGDAL.Repositories;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SDGDAL.Entities;
using CTPayment;
using CTPayment.Classes;

namespace SDGAdmin.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class MerchantOnBoardController : Controller
    {
        PartnerRepository _partnerRepo;
        AccountRepository _accountRepo;
        UserRepository _userRepo;
        MerchantRepository _merchantRepo;
        ResellerRepository _resellerRepo;
        MidsRepository _midsRepo;
        MerchantRequest _merchRequest;
        MerchantOnBoardRepository _merchantOnBoard;
        ReferenceRepository _refRepo;
        ApiCTPayment _sendCTRequest;
        MerchRequestandMiscTextRequest merchMiscRequest;
        Dictionary<string, MerchantRequest> _merchantRequest;
        List<string> listMids;
        Guid guid;

        string action = String.Empty;
        string year = String.Empty;
        string month = String.Empty;
        string date = String.Empty;
        string hour = String.Empty;
        string min = String.Empty;
        string sec = String.Empty;
        string filedate = String.Empty;

        bool isSuccess = false;

        public MerchantOnBoardController()
        {
            _partnerRepo = new PartnerRepository();
            _accountRepo = new AccountRepository();
            _userRepo = new UserRepository();
            _merchantRepo = new MerchantRepository();
            _resellerRepo = new ResellerRepository();
            _midsRepo = new MidsRepository();
            _merchRequest = new MerchantRequest();
            _merchantOnBoard = new MerchantOnBoardRepository();
            _sendCTRequest = new ApiCTPayment();
            merchMiscRequest = new MerchRequestandMiscTextRequest();
            _merchantRequest = new Dictionary<string, MerchantRequest>();
            _refRepo = new ReferenceRepository();
            guid = Guid.NewGuid();
            listMids = new List<string>();
        }

        public void GetResellers()
        {
            try
            {
                action = "fetching the resller list";

                var resellers = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

                var ddlresellers = new List<SelectListItem>();

                ddlresellers.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select a Reseller",
                    Selected = 0 == 0
                });

                ddlresellers.AddRange(resellers.Select(r => new SelectListItem()
                {
                    Value = r.ResellerId.ToString(),
                    Text = r.ResellerName
                }).ToList());

                ViewBag.Resellers = ddlresellers;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGbackOffice", errorOnAction + "\n" + ex.Message, "GetResellers", ex.StackTrace);
            }
        }

        public void GetPartners()
        {
            try
            {
                action = "fetching the partner list";

                var partners = _partnerRepo.GetAllPartners();

                var ddlPartners = new List<SelectListItem>();

                ddlPartners.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select a Partner",
                    Selected = 0 == 0
                });

                ddlPartners.AddRange(partners.Select(p => new SelectListItem()
                {
                    Value = p.PartnerId.ToString(),
                    Text = p.CompanyName
                }).ToList());

                ViewBag.Partners = ddlPartners;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
            }
        }

        public ActionResult Add()
        {
            var userActivity = "Entered Merchant Boarding Add";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            if (Session["Error"] != null)
            {
                ViewBag.Error = Session["Error"];
            }

            Session["Success"] = null;
            Session["Error"] = null;

            GetPartners();

            return View();
        }

        public ActionResult Update()
        {
            var userActivity = "Entered Update Merchant Boarding";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            if (Session["Error"] != null)
            {
                ViewBag.Error = Session["Error"];
            }

            Session["Success"] = null;
            Session["Error"] = null;

            GetPartners();

            #region Actions To be Updated

            List<SelectListItem> actions = new List<SelectListItem>();
            actions.Add(new SelectListItem()
            {
                Text = "----------Select Data To Be Updated------------",
                Value = "0"
            });
            actions.Add(new SelectListItem()
            {
                Text = "Update an Existing Merchant.",
                Value = "1"
            });
            actions.Add(new SelectListItem()
            {
                Text = "Update an Existing Terminal",
                Value = "2"
            });

            ViewBag.Actions = actions;

            #endregion

            return View();
        }

        public ActionResult Delete()
        {
            var userActivity = "Entered Merchant Boarding Delete";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            if (Session["Error"] != null)
            {
                ViewBag.Error = Session["Error"];
            }

            Session["Success"] = null;
            Session["Error"] = null;

            GetPartners();

            #region Actions To be Deleted

            List<SelectListItem> actions = new List<SelectListItem>();
            actions.Add(new SelectListItem()
            {
                Text = "----------Select Data To Be Deleted------------",
                Value = "0"
            });
            actions.Add(new SelectListItem()
            {
                Text = "Delete Credit Card from Existing Merchant.",
                Value = "1"
            });
            actions.Add(new SelectListItem()
            {
                Text = "Delete an Existing Terminal",
                Value = "2"
            });
            actions.Add(new SelectListItem()
            {
                Text = "Delete an Existing Merchant",
                Value = "3"
            });

            ViewBag.Actions = actions;

            #endregion

            return View();
        }

        public ActionResult Result()
        {
            return View();
        }

        public ActionResult ViewAddBulk(string id)
        {
            ViewBag.MerchantIds = id;
            return View();
        }

        public ActionResult GetAllMerchantsToBeAdded(string id)
        {
            DataTableModel data = new DataTableModel(Request);
            string setLikeMid = string.Empty;
            string mid = string.Empty;
            string miscText = guid.ToString("N");
            int totalRecords = 0;
            data.sSearch = "";
            data.sSortDir_0 = "asc";
            data.iDisplayLength = 10;

            var merchId = id.Split(",".ToArray());

            for (int i = 0; i < merchId.Count(); i++)
            {
                List<CreditCard> cCard = new List<CreditCard>();
                List<DebitCard> dCard = new List<DebitCard>();
                List<Terminal> terminal = new List<Terminal>();
                List<PinPad> pinpad = new List<PinPad>();

                string merchantId = merchId[i];

                var merchantInfo = _merchantRepo.GetDetailsbyMerchantId(Convert.ToInt32(merchantId));

                var accountInfo = _userRepo.GetUserByParentIdAndparentTypeId(Convert.ToInt32(SDGDAL.Enums.ParentType.Merchant), Convert.ToInt32(merchantId));

                var midsMerchant = _midsRepo.GetAllListMidsToBeAdded(Convert.ToInt32(merchId[i]), data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords);

                for (int j = 0; j < midsMerchant.Count(); j++)
                {
                    setLikeMid = midsMerchant[j].SetLikeMerchantId;
                    mid = midsMerchant[j].Param_2;

                    listMids.Add(midsMerchant[j].Param_2.ToString());

                    //Check if need to add cc on existing Merchant
                    if ((midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.MasterCard)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Visa)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.JCB)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Discover)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.AmericanExpress)
                        || midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Diners)) && midsMerchant[j].NeedAddBulk)
                    {
                        cCard.Add(new CreditCard
                        {
                            SequenceNumber = Convert.ToString(midsMerchant[j].MidId),
                            Command = Convert.ToString(SDGDAL.Enums.CommandType.A),
                            MerchantId = midsMerchant[j].Param_2,
                            CardType = midsMerchant[j].CardType.IsoCode,
                            OriginatorId = midsMerchant[j].Param_10,
                            DepositMerchantId = midsMerchant[j].Param_12,
                            MerchantBranchTransitNumber = midsMerchant[j].Param_7,
                            MerchantBankNumber = midsMerchant[j].Param_8,
                            MerchantAccountNumber = midsMerchant[j].Param_9,
                            AuthorizationMerchantId = midsMerchant[j].Param_12,
                            MerchantCategoryCode = midsMerchant[j].Param_11
                        });
                    }
                    else if (midsMerchant[j].CardType.CardTypeId == Convert.ToInt32(SDGDAL.Enums.CardTypes.Debit) && midsMerchant[j].NeedAddBulk)
                    {
                        dCard.Add(new DebitCard
                        {
                            SequenceNumber = Convert.ToString(midsMerchant[j].MidId),
                            Command = Convert.ToString(SDGDAL.Enums.CommandType.A),
                            MerchantId = midsMerchant[j].Param_2,
                            CardType = midsMerchant[j].CardType.IsoCode,
                            MerchantBranchTransitNumber = midsMerchant[j].Param_7,
                            MerchantBankNumber = midsMerchant[j].Param_8, //3
                            MerchantAccountNumber = midsMerchant[j].Param_9, //"666666661",
                            MerchantCategoryCode = midsMerchant[j].Param_11,
                        });
                    }

                    //Check if need to add Terminal on existing Merchant
                    if (midsMerchant[j].NeedAddTerminal)
                    {
                        terminal.Add(new Terminal
                        {
                            SequenceNumber = Convert.ToString(midsMerchant[j].MidId),
                            Command = Convert.ToString(SDGDAL.Enums.CommandType.A),
                            MerchantId = midsMerchant[j].Param_2,
                            TerminalID = midsMerchant[j].Param_6,
                            SetLikeTerminalID = midsMerchant[j].SetLikeTerminalId,
                            AllowPurchase = "true",
                            AllowVoid = "true",
                            AllowPreauths = "true",
                            AllowReturns = "false",
                            AllowSettle = "true",
                            AllowTotalsandDetails = "true"
                        });
                    }
                }

                //Check if need to add Merchant Only
                if (!merchantInfo.NeedAddToCT)
                {
                    _merchantRequest.Add(merchantId, new MerchantRequest
                    {
                        SequenceNumber = null,
                        MerchantId = null,
                        MerchantName = null,
                        Street = null,
                        City = null,
                        Province = null,
                        PostalCode = null,
                        PhoneNumber = null,
                        EmailAddress = null,
                        ContactName = null,
                        CurrencyCode = null,
                        Command = null,
                        CreditCard = cCard,
                        DebitCard = dCard,
                        Terminal = terminal,
                        PinPad = pinpad
                    });
                }
                else
                {
                    _merchantRequest.Add(merchantId, new MerchantRequest
                    {
                        SequenceNumber = Convert.ToString(merchantInfo.MerchantId),
                        SetLikeMerchantID = setLikeMid,
                        MerchantId = mid,
                        MerchantName = merchantInfo.MerchantName,
                        Street = merchantInfo.ContactInformation.Address,
                        City = merchantInfo.ContactInformation.City,
                        Province = merchantInfo.ContactInformation.ProvIsoCode,
                        PostalCode = merchantInfo.ContactInformation.ZipCode,
                        PhoneNumber = merchantInfo.ContactInformation.PrimaryContactNumber,
                        EmailAddress = merchantInfo.MerchantEmail,
                        ContactName = accountInfo.User.FirstName + " " + accountInfo.User.LastName,
                        CurrencyCode = merchantInfo.Currency.IsoCode,
                        Command = Convert.ToString(SDGDAL.Enums.CommandType.A),
                        CreditCard = cCard,
                        DebitCard = dCard,
                        Terminal = terminal,
                        PinPad = pinpad
                    });
                }
            }

            merchMiscRequest.merchRequest = _merchantRequest;
            merchMiscRequest.MiscText = miscText;

            ProcessRequestToCT(merchMiscRequest);

            if (isSuccess)
            {
                return RedirectToAction("Result", "MerchantOnBoard");
            }
            else
            {
                return RedirectToAction("Add");
            }
        }

        public ActionResult GetAllCTMerchantDetailsToBeUpdated()
        {
            string ids = Request.QueryString["ids"];
            string ActionId = Request.QueryString["ActionId"];

            DataTableModel data = new DataTableModel(Request);
            List<Terminal> terminal = new List<Terminal>();

            string miscText = guid.ToString("N");
            int totalRecords = 0;
            data.sSearch = "";
            data.sSortDir_0 = "asc";
            data.iDisplayLength = 10;

            var listMidIds = ids.Split(",".ToArray());

            for (int i = 0; i < listMidIds.Count(); i++)
            {
                string midId = listMidIds[i];

                if (Convert.ToInt32(ActionId) == 1)
                {
                    var merchantInfo = _midsRepo.GetAllMerchantUpdateDetailsToCT(Convert.ToInt32(midId));

                    listMids.Add(merchantInfo.Param_2.ToString()); 

                    _merchantRequest.Add(midId.ToString(), new MerchantRequest
                    {
                        SequenceNumber = Convert.ToString(merchantInfo.MerchantId),
                        MerchantId = merchantInfo.Param_2,
                        MerchantName = merchantInfo.Merchant.MerchantName,
                        Street = merchantInfo.Merchant.ContactInformation.Address,
                        City = merchantInfo.Merchant.ContactInformation.City,
                        Province = merchantInfo.Merchant.ContactInformation.ProvIsoCode,
                        PostalCode = merchantInfo.Merchant.ContactInformation.ZipCode,
                        PhoneNumber = merchantInfo.Merchant.ContactInformation.MobileNumber,
                        EmailAddress = merchantInfo.Merchant.MerchantEmail,
                        Command = Convert.ToString(SDGDAL.Enums.CommandType.U),
                        CreditCard = null,
                        DebitCard = null,
                        Terminal = null,
                        PinPad = null
                    });
                }
                else if (Convert.ToInt32(ActionId) == 2)
                {
                    var midsTerminal = _midsRepo.GetAllMerchantTerminalUpdateToCT(Convert.ToInt32(midId));

                    listMids.Add(midsTerminal.Param_2.ToString());

                    terminal.Add(new Terminal
                    {
                        SequenceNumber = midsTerminal.Param_2,
                        Command = Convert.ToString(SDGDAL.Enums.CommandType.U),
                        MerchantId = midsTerminal.Param_2,
                        TerminalID = midsTerminal.Param_6,
                        SetLikeTerminalID = null,
                        AllowPurchase = null,
                        AllowVoid = null,
                        AllowPreauths = null,
                        AllowReturns = "false", //do not allow refunds
                        AllowSettle = null,
                        AllowTotalsandDetails = null
                    });
                }

            }

            if(Convert.ToInt32(ActionId) == 2)
            {
                _merchantRequest.Add(ActionId, new MerchantRequest
                {
                    CreditCard = null,
                    DebitCard = null,
                    Terminal = terminal,
                    PinPad = null
                });
            }



            merchMiscRequest.merchRequest = _merchantRequest;
            merchMiscRequest.MiscText = miscText;

            ProcessRequestToCT(merchMiscRequest);

            if (isSuccess)
            {
                return RedirectToAction("Result");
            }
            else
            {
                Session["Error"] = "Established connection failed for Merchant Bulk Boarding.";
                return RedirectToAction("Update");
            }
        }

        public ActionResult GetAllMerchantsToBeDeleted()
        {
            string ids = Request.QueryString["ids"];
            string ActionId = Request.QueryString["ActionId"];

            DataTableModel data = new DataTableModel(Request);

            List<CreditCard> cCard = new List<CreditCard>();
            List <Terminal> terminal = new List<Terminal>();

            string miscText = guid.ToString("N");
            int totalRecords = 0;
            data.sSearch = "";
            data.sSortDir_0 = "asc";
            data.iDisplayLength = 10;

            var listMidIds = ids.Split(",".ToArray());

            for (int i = 0; i < listMidIds.Count(); i++)
            {
                string midId = listMidIds[i];

                if (Convert.ToInt32(ActionId) == 1) //Delete a CC from Existing Merchant
                {
                    terminal = null;
                    var midsCC = _midsRepo.GetAllCreditCardsToBeDeletedToCT(Convert.ToInt32(midId));

                    listMids.Add(midsCC.Param_2.ToString());

                    cCard.Add(new CreditCard
                    {
                        SequenceNumber = midId,
                        Command = Convert.ToString(SDGDAL.Enums.CommandType.D),
                        MerchantId = midsCC.Param_2,
                    });
                }
                else if (Convert.ToInt32(ActionId) == 2) //Delete an Existing Terminal
                {
                    cCard = null;
                    var midsTerminal = _midsRepo.GetAllTerminalsToBeDeletedToCT(Convert.ToInt32(midId));

                    listMids.Add(midsTerminal.Param_2.ToString());

                    terminal.Add(new Terminal
                    {
                        SequenceNumber = midId,
                        Command = Convert.ToString(SDGDAL.Enums.CommandType.D),
                        MerchantId = midsTerminal.Param_2,
                        TerminalID = midsTerminal.Param_6,
                        SetLikeTerminalID = null,
                        AllowPurchase = null,
                        AllowVoid = null,
                        AllowPreauths = null,
                        AllowReturns = "false", //do not allow refunds
                        AllowSettle = null,
                        AllowTotalsandDetails = null
                    });
                }
                else //Delete Existing Merchant
                {
                    var merchantInfo = _midsRepo.GetAllMerchantsToBeDeletedToCT(Convert.ToInt32(midId));

                    listMids.Add(merchantInfo.Param_2.ToString());

                    _merchantRequest.Add(midId, new MerchantRequest
                    {
                        SequenceNumber = Convert.ToString(merchantInfo.MerchantId),
                        MerchantId = merchantInfo.Param_2,
                        MerchantName = merchantInfo.Merchant.MerchantName,
                        Command = Convert.ToString(SDGDAL.Enums.CommandType.D),
                        CreditCard = null,
                        DebitCard = null,
                        Terminal = null,
                        PinPad = null
                    });
                }
            }

            if(Convert.ToInt32(ActionId) == 1 || Convert.ToInt32(ActionId) == 2)
            {
                _merchantRequest.Add(ActionId, new MerchantRequest
                {
                    CreditCard = cCard,
                    DebitCard = null,
                    Terminal = terminal,
                    PinPad = null
                });
            }

            merchMiscRequest.merchRequest = _merchantRequest;
            merchMiscRequest.MiscText = miscText;

            ProcessRequestToCT(merchMiscRequest);

            if (isSuccess)
            {
                return RedirectToAction("Result");
            }
            else
            {
                return RedirectToAction("Delete");
            }
        }

        public ActionResult GetFileNames(int? id)
        {
            try
            {
                var userActivity = "Entered Merchant Boarding Delete";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

                if (Session["Success"] != null)
                {
                    ViewBag.Success = Session["Success"];
                }

                if (Session["Error"] != null)
                {
                    ViewBag.Error = Session["Error"];
                }

                if (Session["Success"] != null)
                {
                    ViewBag.Success = Session["Success"];
                }

                if (Session["Error"] != null)
                {
                    ViewBag.Error = Session["Error"];
                }

                Session["Success"] = null;
                Session["Error"] = null;
                List<string> apiFileNames = new List<string>();

                var apiGetFileNames = _sendCTRequest.GetFileNames();

                if (apiGetFileNames.Result != null)
                {
                    var resFilenames = new SDGDAL.Entities.MerchantOnBoardResponse();

                    for (int i = 0; i < apiGetFileNames.Result.Count(); i++)
                    {
                        resFilenames.ResponseFileName = apiGetFileNames.Result[i].ToString().Substring(0, apiGetFileNames.Result[i].ToString().Length - 4);
                        filedate = parseDate(resFilenames.ResponseFileName);
                        resFilenames.DateReceived = Convert.ToDateTime(filedate);

                        if (_merchantOnBoard.IsFilenameExists(resFilenames.ResponseFileName))
                        {
                            var result = _merchantOnBoard.SaveResponseFile(resFilenames);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Established connection failed for Merchant Bulk Boarding.";
                return View();
            }

            Session["Success"] = null;
            Session["Error"] = null;

            return View();
        }

        [HttpPost]
        public ActionResult GetFileNames()
        {
            return View();
        }

        public ActionResult GetFileDetails(int id)
        {
            MerchantOnBoardResponseModel apiCTResponse = new MerchantOnBoardResponseModel();

            try
            {
                if (id != null)
                {
                    ViewBag.ResponseId = id;
                    TempData["XMLResponseId"] = id;

                    var response = _merchantOnBoard.GetResponseFileById(id);

                    if (response != null)
                    {
                        var apiFileDetails = _sendCTRequest.GetFileDetails(response.ResponseFileName);

                        if (apiFileDetails.Result.HEADER != null)
                        {
                            string sequenceNumber = apiFileDetails.Result.HEADER.SequenceNumber;

                            apiCTResponse.SequenceNumber = apiFileDetails.Result.HEADER.SequenceNumber;
                            apiCTResponse.Version = apiFileDetails.Result.HEADER.Version;
                            apiCTResponse.InstitutionNumber = apiFileDetails.Result.HEADER.InstitutionNumber;
                            apiCTResponse.LoadDate = apiFileDetails.Result.HEADER.LoadDate;
                            apiCTResponse.LoadTime = apiFileDetails.Result.HEADER.LoadTime;
                            apiCTResponse.MiscText = apiFileDetails.Result.HEADER.MiscText;
                            apiCTResponse.ResultText = apiFileDetails.Result.HEADER.ResultText;

                            if (apiFileDetails.Result.HEADER.ResultText == null || apiFileDetails.Result.HEADER.ResultText == String.Empty)
                            {
                                apiCTResponse.STAT = apiFileDetails.Result.STAT;
                                apiCTResponse.MERCHANT = apiFileDetails.Result.MERCHANT;
                                apiCTResponse.CCARD = apiFileDetails.Result.CCARD;
                                apiCTResponse.DCARD = apiFileDetails.Result.DCARD;
                                apiCTResponse.TERMINAL = apiFileDetails.Result.TERMINAL;
                                apiCTResponse.PINPAD = apiFileDetails.Result.PINPAD;
                            }
                            else
                            {
                                //TODO
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(apiCTResponse);
        }

        [HttpPost]
        public ActionResult GetFileDetails(MerchantOnBoardResponseModel CTResponse)
        {
            try
            {
                if (CTResponse.MERCHANT != null)
                {
                    for (int i = 0; i < CTResponse.MERCHANT.Count; i++)
                    {
                        if (CTResponse.MERCHANT[i].ResultText == null || CTResponse.MERCHANT[i].ResultText == "")
                        {
                            var result = _midsRepo.GetDetailsbyParam2AndSwitchCode(CTResponse.MERCHANT[i].MerchantId);

                            UpdateMerchant(CTResponse.MERCHANT[i].Command, Convert.ToString(result.MerchantId));
                        }
                        else
                        {
                            //TODO: show error message from result. Error detected 
                        }
                    }
                }

                if (CTResponse.CCARD != null)
                {
                    foreach (var ccard in CTResponse.CCARD)
                    {
                        var result = _midsRepo.GetDetailsbyParam2AndSwitchCode(ccard.MerchantId);

                        if (ccard.ResultText == null)
                        {
                            UpdateMidbyCardType(ccard.Command, result.MerchantId, ccard.CardType);
                        }
                        else
                        {
                            //TODO: show error message from result. Error detected 
                        }
                    }
                }

                if (CTResponse.DCARD != null)
                {
                    foreach (var dcard in CTResponse.DCARD)
                    {
                        var result = _midsRepo.GetDetailsbyParam2AndSwitchCode(dcard.MerchantId);

                        if (dcard.ResultText == null)
                        {
                            UpdateMidbyCardType(dcard.Command, result.MerchantId, dcard.CardType);
                        }
                        else
                        {
                            //TODO: show error message from result. Error detected 
                        }
                    }
                }

                if (CTResponse.TERMINAL != null)
                {
                    foreach (var tid in CTResponse.TERMINAL)
                    {
                        var result = _midsRepo.GetDetailsbyParam2AndTerminalId(tid.MerchantId, tid.TerminalId);

                        if (tid.ResultText == null)
                        {
                            UpdateTerminalId(tid.Command, result.MidId, tid.TerminalId);
                        }
                        else
                        {
                            //TODO: show error message from result. Error detected 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //log error
            }

            #region Update Merchant On Board Response from CTPayment

            MerchantOnBoardResponse fileResponse = new MerchantOnBoardResponse();
            fileResponse.MerchantOnBoardResponseId = Convert.ToInt32(TempData["XMLResponseId"]);
            fileResponse.IsActive = false;

            var response = _merchantOnBoard.UpdateMerchantOnBoardResponseStatus(fileResponse);

            if (response.MerchantOnBoardResponseId > 0)
            {
                Session["Success"] = "The data from CTPayment successfully updated. The data with error will not modify the status.";
            }
            else
            {
                Session["Error"] = "Error occurs while updating merchant on board response.";
            }

            return RedirectToAction("GetFilenames");

            #endregion
        }

        public ActionResult DeleteFileResponse(int id)
        {
            try
            {
                if (id != null)
                {
                    MerchantOnBoardResponse req = new MerchantOnBoardResponse();
                    req.IsActive = false;
                    req.MerchantOnBoardResponseId = id;

                    var result = _merchantOnBoard.UpdateMerchantOnBoardResponseStatus(req);

                    if (result.MerchantOnBoardResponseId > 0)
                    {
                        Session["Success"] = "Response filename has been successfully deleted.";
                        return RedirectToAction("Add");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateMerchantOnBoardResponse", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Delete FileResponse";
                err.Method = "Submit";
                err.ErrText = ex.Message;
                err.StackTrace = ex.StackTrace;
                err.DateCreated = DateTime.Now;

                if (ex.InnerException != null)
                {
                    err.InnerException = ex.InnerException.Message;
                    err.InnerExceptionStackTrace = ex.InnerException.StackTrace;
                }

                err = _refRepo.LogError(err);
            }

            return View();
        }

        public ActionResult ProcessRequestToCT(MerchRequestandMiscTextRequest merchMiscRequest)
        {
            try
            {
                var reqFileName = _sendCTRequest.ProcessMerchantBulk(merchMiscRequest);

                if (reqFileName != null && reqFileName.Contains(".xml"))
                {
                    var saveFile = new SDGDAL.Entities.MerchantOnBoardRequest();

                    saveFile.RequestFileName = reqFileName.ToString().Substring(2, reqFileName.ToString().Length - 6);
                    saveFile.MiscText = merchMiscRequest.MiscText;

                    var res = _merchantOnBoard.SaveRequestFile(saveFile);

                    if (res.MerchantOnBoardRequestId > 0)
                    {
                        if (listMids.Count > 0)
                        {
                            foreach (var midId in listMids)
                            {
                                var dataLink = new SDGDAL.Entities.MerchantOnBoardResponseLink();

                                dataLink.MerchantId = midId;
                                dataLink.MerchantOnBoardRequestId = res.MerchantOnBoardRequestId;

                                var result = _merchantOnBoard.CreateMerchantOnBoardLink(dataLink);
                                if (result.MerchantOnBoardResponseLinkId > 0)
                                {
                                    isSuccess = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                    Session["Error"] = "Established connection failed for Merchant Bulk Boarding.";
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                string exTrace = ex.StackTrace;
                string source = ex.Source;

                isSuccess = false;
                Session["Error"] = "Established connection failed for Merchant Bulk Boarding.";
            }

            return View();
        }

        private void UpdateMerchant(string command, string merchantId)
        {
            try
            {
                if (command == "A")
                {
                    var merchantRes = _merchantRepo.UpdateMerchantNeedAddToCT(false, Convert.ToInt32(merchantId));
                }
                else if (command == "U")
                {
                    var merchantRes = _merchantRepo.UpdateMerchantNeedUpdateToCT(false, Convert.ToInt32(merchantId));
                }
                else if (command == "D")
                {
                    //Delete data from CT
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateMidbyCardType(string command, int merchantId, string cardType)
        {
            try
            {
                if (command == "A")
                {
                    var midsRes = _midsRepo.UpdateMidNeedAddBulk(false, merchantId, cardType);
                }
                else if (command == "U")
                {
                    var midsRes = _midsRepo.UpdateMidNeedAddBulk(false, merchantId, cardType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateTerminalId(string command, int midId, string tid)
        {
            try
            {
                if (command == "A")
                {
                    var midsRes = _midsRepo.UpdateMidNeedAddTerminal(false, midId, tid);
                }
                else if (command == "U")
                {
                    var midsRes = _midsRepo.UpdateMidNeedUpdateBulk(false, midId, tid);
                }
                else if (command == "D")
                {
                    var midsRes = _midsRepo.UpdateMidNeedAddTerminal(true, midId, tid);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string parseDate(string filename)
        {
            string finaldate = null;
            try
            {
                string fileDate = filename.Substring(filename.Length - 14, 14);
                year = fileDate.Substring(0, 4);
                month = fileDate.Substring(4, 2);
                date = fileDate.Substring(6, 2);
                hour = fileDate.Substring(fileDate.Length - 6, 2);
                min = fileDate.Substring(fileDate.Length - 4, 2);
                sec = fileDate.Substring(fileDate.Length - 2);

                finaldate = year + "-" + month + "-" + date + " " + hour + ":" + min + ":" + sec;
            }
            catch (Exception ex)
            {
                return finaldate;
            }

            return finaldate;
        }

        public ActionResult SaveResponse(int id)
        {
            //Update 
            return View();
        }
    }
}

