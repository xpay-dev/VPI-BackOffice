using SDGBackOffice.Models;
using SDGDAL;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SDGBackOffice.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class AjaxController : Controller
    {
        MerchantRepository _merchantRepo;
        UserRepository _userRepo;
        ResellerRepository _resellerRepo;
        MerchantBranchRepository _branchRepo;
        PartnerRepository _partnerRepo;
        ReferenceRepository _refRepo;
        MerchantBranchPOSRepository _posRepo;
        MidsRepository _midsRepo;
        MidsMerchantBranchPOSsRepository _midsmerchantbranchposRepository;
        DeviceRepository _deviceRepo;
        TransactionRepository _transactionRepo;
        AccountRepository _accountRepo;
        RequestedMerchantRepository _requestedMerchantRepository;

        string con;

        public AjaxController()
        {
            _merchantRepo = new MerchantRepository();
            _userRepo = new UserRepository();
            _resellerRepo = new ResellerRepository();
            _branchRepo = new MerchantBranchRepository();
            _partnerRepo = new PartnerRepository();
            _refRepo = new ReferenceRepository();
            _posRepo = new MerchantBranchPOSRepository();
            _midsRepo = new MidsRepository();
            _midsmerchantbranchposRepository = new MidsMerchantBranchPOSsRepository();
            _deviceRepo = new DeviceRepository();
            _transactionRepo = new TransactionRepository();
            _accountRepo = new AccountRepository();
            _requestedMerchantRepository = new RequestedMerchantRepository();

            con = ConfigurationManager.ConnectionStrings["SDGroupConnString"].ConnectionString.ToString();
        }

        [HttpPost]
        public ActionResult PartnerList()
        {
            if (CurrentUser.ParentType != Enums.ParentType.Partner)
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            var parentId = CurrentUser.ParentId;

            int tryParentId = 0;

            if (int.TryParse(Request["PartnerId"], out tryParentId))
            {
                parentId = tryParentId;
            }

            //var partners = _partnerRepo.GetAllPartnersByParent(parentId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "PartnerId", data.sSortDir_0, out totalRecords);
            var partners = _partnerRepo.GetAllPartnersByParent(parentId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "PartnerId", data.sSortDir_0, out totalRecords);

            var partnersModel = partners.Select(p => new PartnerModel()
            {
                CompanyName = p.CompanyName,
                EncryptedPartnerId = Utility.Encrypt(Convert.ToString(p.PartnerId)),
                City = p.ContactInformation.City + ((p.ContactInformation.NeedsUpdate) ? "" : ", " + p.ContactInformation.Country.CountryName),
                IsActive = p.IsActive,
                LogoUrl = p.LogoUrl,
                PrimaryContactNumber = p.ContactInformation.PrimaryContactNumber,
                ParentPartner = new PartnerModel()
                {
                    CompanyName = !p.ParentPartnerId.HasValue ? "-" : p.ParentPartner.CompanyName,
                    PartnerId = !p.ParentPartnerId.HasValue ? -1 : p.ParentPartnerId.Value
                }
            }).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = partnersModel });
        }

        [HttpPost]
        public ActionResult ResellerList()
        {
            if (CurrentUser.ParentType != Enums.ParentType.Reseller && CurrentUser.ParentType != Enums.ParentType.Partner)
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int partnerId = 0;

            //TODO: Validate if user can view Resellers

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                partnerId = CurrentUser.ParentId;
            }

            var resellers = _resellerRepo.GetAllResellersByPartner(partnerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "ResellerId", data.sSortDir_0, out totalRecords);

            var resellerModel = resellers.Select(p => new ResellerModel()
            {
                ResellerName = p.ResellerName,
                EncryptedResellerId = Utility.Encrypt(Convert.ToString(p.ResellerId)),
                City = p.ContactInformation.City + ((p.ContactInformation.NeedsUpdate) ? "" : ", " + p.ContactInformation.Country.CountryName),
                IsActive = p.IsActive,
                PrimaryContactNumber = p.ContactInformation.PrimaryContactNumber,
                Partner = new PartnerModel()
                {
                    CompanyName = p.Partner.CompanyName,
                    PartnerId = p.Partner.PartnerId
                }, ResellerId = p.ResellerId
            }).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = resellerModel });
        }

        [HttpPost]
        public ActionResult GetReseller(int pId)
        {
            var resellers = _resellerRepo.GetAllResellersByPartner(pId, "");

            var ddlreseller = new List<SelectListItem>();

            var val = 0;

            if (resellers.Count > 0)
            {
                if (resellers.Count > 1)
                {
                    ddlreseller.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "All Resellers",
                        Selected = val == 0
                    });

                    ddlreseller.AddRange(resellers.Select(p => new SelectListItem()
                    {
                        Value = p.ResellerId.ToString(),
                        Text = p.ResellerName,
                        Selected = val == 0
                    }).ToList());
                }
                else
                {
                    ddlreseller.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "Select a Reseller",
                        Selected = val == 0
                    });

                    ddlreseller.AddRange(resellers.Select(p => new SelectListItem()
                    {
                        Value = p.ResellerId.ToString(),
                        Text = p.ResellerName,
                        Selected = true
                    }).ToList());
                }
            }
            else
            {
                ddlreseller.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Resellers available",
                    Selected = val == 0
                });
            }

            return Json(ddlreseller);
        }

        [HttpPost]
        public ActionResult GetResellerforUser(int pId)
        {
            var resellers = _resellerRepo.GetAllResellersByPartner(pId, "");

            var ddlreseller = new List<SelectListItem>();

            var val = 0;

            if (resellers.Count > 0)
            {
                ddlreseller.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select a Reseller",
                    Selected = val == 0
                });

                ddlreseller.AddRange(resellers.Select(p => new SelectListItem()
                {
                    Value = p.ResellerId.ToString(),
                    Text = p.ResellerName,
                    Selected = true
                }).ToList());
            }
            else
            {
                ddlreseller.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Resellers available",
                    Selected = val == 0
                });
            }

            return Json(ddlreseller);
        }

        [HttpPost]
        public ActionResult GetMerchantsByReseller(int rId)
        {
            var merchants = _merchantRepo.GetAllMerchantsByReseller(rId, "");

            var ddlmerchants = new List<SelectListItem>();

            var val = 0;

            if (merchants.Count > 0)
            {
                if (merchants.Count == 1)
                {
                    ddlmerchants.AddRange(merchants.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantId.ToString(),
                        Text = p.MerchantName,
                        Selected = true
                    }).ToList());
                }
                else
                {
                    ddlmerchants.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "Select a Merchant",
                        Selected = val == 0
                    });

                    ddlmerchants.AddRange(merchants.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantId.ToString(),
                        Text = p.MerchantName,
                        Selected = true
                    }).ToList());
                }
            }
            else
            {
                ddlmerchants.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Merchants available",
                    Selected = val == 0
                });
            }

            return Json(ddlmerchants);
        }

        [HttpPost]
        public ActionResult GetMerchantsByResellerforUser(int rId)
        {
            var merchants = _merchantRepo.GetAllMerchantsByReseller(rId, "");

            var ddlmerchants = new List<SelectListItem>();

            var val = 0;

            if (merchants.Count > 0)
            {
                ddlmerchants.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select a Merchant",
                    Selected = val == 0
                });

                ddlmerchants.AddRange(merchants.Select(p => new SelectListItem()
                {
                    Value = p.MerchantId.ToString(),
                    Text = p.MerchantName,
                    Selected = true
                }).ToList());

            }
            else
            {
                ddlmerchants.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Merchants available",
                    Selected = val == 0
                });
            }

            return Json(ddlmerchants);
        }

        [HttpPost]
        public ActionResult MerchantList(int? id)
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int resellerId = 0;

            //TODO: Validate if user can view Merchants
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

            var merchants = _merchantRepo.GetAllMerchantsByReseller(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords);

            var merchantModel = merchants.Select(p => new
            {
                MerchantName = p.MerchantName,
                MerchantId = p.MerchantId,//Utility.Encrypt(Convert.ToString(p.MerchantId)),//p.MerchantId,
                City = p.ContactInformation.City + ", " + p.ContactInformation.Country.CountryName,
                IsActive = p.IsActive,
                PrimaryContactNumber = p.ContactInformation.PrimaryContactNumber,
                ResellerName = p.Reseller.ResellerName,
                PartnerName = p.Reseller.Partner.CompanyName
            }).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchantModel });
        }

        [HttpPost]
        public ActionResult MerchantforFeatureList()
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int resellerId = 0;

            //TODO: Validate if user can view Merchants
            var merchants = _merchantRepo.GetAllMerchantsByReseller(CurrentUser.ParentId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantId", data.sSortDir_0, out totalRecords);

            var merchantModel = merchants.Select(p => new
            {
                MerchantName = p.MerchantName,
                MerchantId = p.MerchantId,
                City = p.ContactInformation.City + ", " + p.ContactInformation.Country.CountryName,
                IsActive = p.IsActive,
                PrimaryContactNumber = p.ContactInformation.PrimaryContactNumber,
                ResellerName = p.Reseller.ResellerName,
                PartnerName = p.Reseller.Partner.CompanyName
            }).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchantModel });
        }

        [HttpPost]
        public ActionResult BranchList()
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller, Enums.ParentType.Merchant };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int merchantId = 0;
            int resellerId = 0;

            //TODO: Validate if user can view Merchants
            if (CurrentUser.ParentType != Enums.ParentType.Merchant)
            {
                int.TryParse(Request["MerchantId"], out merchantId);
                int.TryParse(Request["ResellerId"], out resellerId);
            }
            else
            {
                merchantId = CurrentUser.ParentId;
            }

            List<MerchantBranch> merchantBranches = new List<MerchantBranch>();

            if (merchantId > 0)
            {
                merchantBranches = _branchRepo.GetAllBranchesByMerchant(merchantId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);
            }
            else
            {
                if (resellerId > 0)
                {
                    merchantBranches = _branchRepo.GetAllBranchesByReseller(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);
                }
                else
                {
                    merchantBranches = _branchRepo.GetAllBranchesByPartner(CurrentUser.ParentId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);
                }
            }

            var merchantModel = merchantBranches.Select(p => new
            {
                MerchantBranchId = Utility.Encrypt(Convert.ToString(p.MerchantBranchId)),
                BranchName = p.MerchantBranchName,
                //City = p.ContactInformation.City + ", " + p.ContactInformation.Country.CountryName,
                IsActive = p.IsActive,
                PrimaryContactNumber = p.ContactInformation.PrimaryContactNumber,
                POSs = p.MerchantPOSs.Count,
                MerchantName = p.Merchant.MerchantName,
                ResellerName = p.Merchant.Reseller.ResellerName
            }).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchantModel });
        }

        [HttpPost]
        public JsonResult POSList()
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller, Enums.ParentType.Merchant };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int branchId = 0;
            int merchantId = 0;
            int resellerId = 0;

            if (CurrentUser.ParentType != Enums.ParentType.Merchant)
            {
                int.TryParse(Request["MerchantId"], out merchantId);
                int.TryParse(Request["ResellerId"], out resellerId);
                int.TryParse(Request["BranchId"], out branchId);
            }
            else
            {
                merchantId = CurrentUser.ParentId;
            }

            List<MerchantBranchPOS> merchantBranchPOSs = new List<MerchantBranchPOS>();

            if (branchId > 0)
            {
                merchantBranchPOSs = _posRepo.GetAllPOSsByBranch(branchId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);
            }
            else if (merchantId > 0)
            {
                merchantBranchPOSs = _posRepo.GetAllPOSsByMerchant(merchantId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);
            }
            else
            {
                if (resellerId > 0)
                {
                    merchantBranchPOSs = _posRepo.GetAllPOSsByReseller(resellerId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);
                }
                else
                {
                    merchantBranchPOSs = _posRepo.GetAllPOSsByPartner(CurrentUser.ParentId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);
                }
            }

            var poss = merchantBranchPOSs.Select(p => new
            {
                POSId = Utility.Encrypt(Convert.ToString(p.MerchantPOSId)),//p.MerchantPOSId,
                POSName = p.MerchantPOSName,
                ActivationCode = p.MobileApp.Last().ActivationCode,
                Status = !p.IsActive ? "Inactive" : p.MobileApp.Last().IsActive ? "Active" : "Not yet activated",
                DateActivated = p.MobileApp.Last().DateActivated.HasValue ? p.MobileApp.Last().DateActivated.Value.Date.ToString("d") : null,
                BranchName = p.MerchantBranch.MerchantBranchName,
                MerchantName = p.MerchantBranch.Merchant.MerchantName,
                ResellerName = p.MerchantBranch.Merchant.Reseller.ResellerName,
            });

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = poss });
        }

        [HttpPost]
        public JsonResult POSForMidsList(int? id)
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller, Enums.ParentType.Merchant };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            if (id == null)
            {
                id = 0;
            }

            List<MerchantBranchPOS> merchantBranchPOSs = new List<MerchantBranchPOS>();

            merchantBranchPOSs = _posRepo.GetAllPOSsByBranch(id.Value, data.sSearch, data.iDisplayStart, data.iDisplayLength, "POSId", data.sSortDir_0, out totalRecords);

            var poss = merchantBranchPOSs.Select(p => new
            {
                POSId = p.MerchantPOSId,
                POSName = p.MerchantPOSName,
                ActivationCode = p.MobileApp.Last().ActivationCode,
                Status = !p.IsActive ? "Inactive" : p.MobileApp.Last().IsActive ? "Active" : "Not yet activated",
                DateActivated = p.MobileApp.Last().DateActivated,
                BranchName = p.MerchantBranch.MerchantBranchName,
                MerchantName = p.MerchantBranch.Merchant.MerchantName,
                ResellerName = p.MerchantBranch.Merchant.Reseller.ResellerName,
            });

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = poss });
        }

        [HttpPost]
        public JsonResult GetPOSByMerchantBranch(int bId)
        {
            var merchantBranchPOSs = _posRepo.GetAllPOSsByBranch(bId, "");

            var ddlpos = new List<SelectListItem>();
            if (merchantBranchPOSs.Count > 0)
            {
                if (merchantBranchPOSs.Count > 1)
                {
                    ddlpos.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "All POS devices",
                        Selected = true
                    });

                    ddlpos.AddRange(merchantBranchPOSs.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantPOSId.ToString(),
                        Text = p.MerchantPOSName
                    }).ToList());
                }

                else
                {
                    ddlpos.AddRange(merchantBranchPOSs.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantPOSId.ToString(),
                        Text = p.MerchantPOSName
                    }).ToList());

                }
            }
            else
            {
                ddlpos.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No POS device Available",
                    Selected = true
                });
            }

            return Json(ddlpos);
        }

        [HttpPost]
        public JsonResult GetPOSByBranch(int bId)
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller, Enums.ParentType.Merchant };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            List<MerchantBranchPOS> merchantBranchPOSs = new List<MerchantBranchPOS>();

            merchantBranchPOSs = _posRepo.GetAllPOSsByBranch(bId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);


            var poss = merchantBranchPOSs.Select(p => new
            {
                POSId = p.MerchantPOSId, //Utility.Encrypt(Convert.ToString(p.MerchantPOSId)),//p.MerchantPOSId,
                POSName = p.MerchantPOSName,
                ActivationCode = p.MobileApp.Last().ActivationCode,
                Status = p.MobileApp.Last().IsActive ? "Active" : "Not yet activated",
                DateActivated = p.MobileApp.Last().DateActivated.HasValue ? p.MobileApp.Last().DateActivated.Value.Date.ToString("d") : null,
                BranchName = p.MerchantBranch.MerchantBranchName,
                MerchantName = p.MerchantBranch.Merchant.MerchantName,
                ResellerName = p.MerchantBranch.Merchant.Reseller.ResellerName,
                IsActive = p.IsActive
            });

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = poss });
        }

        [HttpPost]
        public ActionResult GetMerchants()
        {
            List<Merchant> merchants = new List<Merchant>();

            int id = Convert.ToInt32(Request["ResellerId"]);

            if (id > 0)
            {
                merchants = _merchantRepo.GetAllMerchantsByReseller(id, "");
            }
            else
            {
                merchants = _merchantRepo.GetAllMerchantsByPartner(CurrentUser.ParentId, "");
            }

            var ddlmerchant = new List<SelectListItem>();

            if (merchants.Count > 0)
            {
                if (merchants.Count > 1)
                {
                    ddlmerchant.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "All Merchants",
                        Selected = true
                    });

                    ddlmerchant.AddRange(merchants.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantId.ToString(),
                        Text = p.MerchantName
                        //Selected = r.Merchant.MerchantId == p.MerchantId
                    }).ToList());
                }
                else
                {
                    ddlmerchant.AddRange(merchants.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantId.ToString(),
                        Text = p.MerchantName,
                        Selected = true
                    }).ToList());
                }
            }
            else
            {
                ddlmerchant.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Merchants"
                    //Selected = r.PartnerId == parentPartner.PartnerId
                });
            }

            return Json(new[] { new { data = ddlmerchant } }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMerchantsforPoS()
        {
            List<Merchant> merchants = new List<Merchant>();

            var merchant = _accountRepo.GetTypeIdByAccountId(CurrentUser.UserId);

            var id = merchant.ParentId;

            ViewBag.User = id;

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                merchants = _merchantRepo.GetAllMerchantsByPartner(id, "");
            }
            else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
            {
                merchants = _merchantRepo.GetAllMerchantsByReseller(id, "");
            }

            var ddlmerchant = new List<SelectListItem>();

            var val = 0;

            if (merchants.Count > 0)
            {
                if (merchants.Count > 1)
                {
                    ddlmerchant.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "All Merchants",
                        Selected = val == 0
                    });

                    ddlmerchant.AddRange(merchants.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantId.ToString(),
                        Text = p.MerchantName,
                        Selected = val == 0
                    }).ToList());
                }
                else
                {
                    ddlmerchant.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "Select a Merchant",
                        Selected = val == 0
                    });

                    ddlmerchant.AddRange(merchants.Select(p => new SelectListItem()
                    {
                        Value = p.MerchantId.ToString(),
                        Text = p.MerchantName,
                        Selected = true
                    }).ToList());
                }
            }
            else
            {
                ddlmerchant.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Merchants",
                    Selected = val == 0
                });
            }

            return Json(ddlmerchant);
        }

        [HttpPost]
        public ActionResult GetBranches(int id)
        {
            var branch = _branchRepo.GetAllBranchesByMerchant(id, "");

            var ddlbranch = new List<SelectListItem>();

            var val = 0;

            if (branch.Count > 0)
            {
                if (branch.Count == 1)
                {
                    ddlbranch.AddRange(branch.Select(b => new SelectListItem()
                    {
                        Text = b.MerchantBranchName,
                        Value = Convert.ToString(b.MerchantBranchId)
                    }).ToList());
                }
                else
                {
                    ddlbranch.Add(new SelectListItem()
                    {
                        Text = "Select Branch",
                        Value = "0",
                        Selected = val == 0
                    });

                    ddlbranch.AddRange(branch.Select(b => new SelectListItem()
                    {
                        Text = b.MerchantBranchName,
                        Value = Convert.ToString(b.MerchantBranchId)
                    }).ToList());

                }
            }
            else
            {
                ddlbranch.Add(new SelectListItem()
                {
                    Text = "No Merchant Branches available",
                    Value = "0",
                    Selected = val == 0
                });
            }

            return Json(ddlbranch);
        }

        [HttpPost]
        public ActionResult GetBranchesforUser(int id)
        {
            var branch = _branchRepo.GetAllBranchesByMerchant(id, "");

            var ddlbranch = new List<SelectListItem>();

            var val = 0;

            if (branch.Count > 0)
            {
                ddlbranch.Add(new SelectListItem()
                {
                    Text = "Select Branch",
                    Value = "0",
                    Selected = val == 0
                });

                ddlbranch.AddRange(branch.Select(b => new SelectListItem()
                {
                    Text = b.MerchantBranchName,
                    Value = Convert.ToString(b.MerchantBranchId)
                }).ToList());
            }
            else
            {
                ddlbranch.Add(new SelectListItem()
                {
                    Text = "No Merchant Branches available",
                    Value = "0",
                    Selected = val == 0
                });
            }

            return Json(ddlbranch);
        }

        [HttpPost]
        public ActionResult GetBranchesByMerchant(int mId)
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller, Enums.ParentType.Merchant };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            List<MerchantBranch> merchantBranches = new List<MerchantBranch>();

            merchantBranches = _branchRepo.GetAllBranchesByMerchant(mId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MerchantBranchId", data.sSortDir_0, out totalRecords);


            var merchantModel = merchantBranches.Select(p => new
            {
                MerchantBranchId = Utility.Encrypt(Convert.ToString(p.MerchantBranchId)),//p.MerchantBranchId,
                BranchName = p.MerchantBranchName,
                IsActive = p.IsActive,
                PrimaryContactNumber = p.ContactInformation.PrimaryContactNumber,
                POSs = p.MerchantPOSs.Count,
                MerchantName = p.Merchant.MerchantName,
                ResellerName = p.Merchant.Reseller.ResellerName
            }).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchantModel });
        }

        [HttpPost]
        public ActionResult BanchPOS(int id)
        {
            TempData["BranchID"] = id;

            int md = Convert.ToInt32(TempData["MIDId"]);
            TempData.Keep();

            List<MerchantBranchPOS> merchantBranchPOSs = new List<MerchantBranchPOS>();
            List<MidsMerchantBranchPOSs> midsBranchPOS = new List<MidsMerchantBranchPOSs>();

            merchantBranchPOSs = _posRepo.GetAllPOSsByBranch(id, "");

            midsBranchPOS = _midsmerchantbranchposRepository.GetAllMidsMerchantBranchPOSs();

            var mbPOSId = midsBranchPOS.Where(mbp => mbp.MidId == md).Select(m => m.MerchantBranchPOSId);

            //var mmbPOSsIsDeleted = midsBranchPOS.Select(mb => mb.IsDeleted).ToList();

            var posforDisplay = merchantBranchPOSs.Where(x => !mbPOSId.Contains(x.MerchantPOSId)).Select(p => new
            {
                MerchantPOSId = p.MerchantPOSId,
                MerchantPOSName = p.MerchantPOSName
            }).ToList();

            return Json(posforDisplay);
        }

        [HttpPost]
        public ActionResult BanchPOSForMid(int id)
        {
            TempData["BranchID"] = id;

            List<MerchantBranchPOS> merchantBranchPOSs = new List<MerchantBranchPOS>();
            List<MidsMerchantBranchPOSs> midsBranchPOS = new List<MidsMerchantBranchPOSs>();

            merchantBranchPOSs = _posRepo.GetAllPOSsByBranch(id, "");

            var ddlpos = new List<SelectListItem>();

            if (merchantBranchPOSs.Count != 0)
            {
                if (merchantBranchPOSs.Count == 1)
                {
                    ddlpos.AddRange(merchantBranchPOSs.Select(p => new SelectListItem()
                    {
                        Value = Convert.ToString(p.MerchantPOSId),
                        Text = p.MerchantPOSName
                    }).ToList());
                }
                else
                {
                    ddlpos.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "Select a POS"
                    });

                    ddlpos.AddRange(merchantBranchPOSs.Select(p => new SelectListItem()
                    {
                        Value = Convert.ToString(p.MerchantPOSId),
                        Text = p.MerchantPOSName
                    }).ToList());
                }
            }
            else
            {
                ddlpos.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No POS device available"
                });
            }

            ViewBag.POS = ddlpos;

            return Json(ddlpos);
        }

        [HttpPost]
        public ActionResult MIDList(string id)
        {
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            int merchantId = 0;

            //TODO: Validate if user can view Merchants

            if (id != "0")
            {
                merchantId = Convert.ToInt32(id);
            }

            var mids =
                merchantId > 0 ?
                _midsRepo.GetAllMids(merchantId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MidId", data.sSortDir_0, out totalRecords)
                :
                _midsRepo.GetAllMids(merchantId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MidId", data.sSortDir_0, out totalRecords);

            var midModel = mids.Select(p => new MerchantMidModel()
            {
                EncryptedMIDId = Utility.Encrypt(Convert.ToString(p.MidId)),//p.MidId,
                MIDId = p.MidId,
                IsActive = p.IsActive,
                MIDName = p.MidName,
                EncryptedCardTypeId = Utility.Encrypt(Convert.ToString(p.CardTypeId)),
                CardType = p.CardType.TypeName,
                CardTypeId = p.CardTypeId,
                SwitchId = p.SwitchId,
                Switch = p.Switch.SwitchName
            }).ToList();


            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = midModel });
        }

        [HttpPost]
        public ActionResult POSAssignedMID(int? id)
        {
            int pId = 0;

            var mbp = _midsmerchantbranchposRepository.GetMidsMerchantBranchesPossByPOSId(id.Value, "");

            if (mbp != null)
            {
                //pId = mbp.MidId;
            }
            var parentTypes = new Enums.ParentType[] { Enums.ParentType.Partner, Enums.ParentType.Reseller };

            if (!parentTypes.Contains(CurrentUser.ParentType))
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            //var mids =
            //    mId > 0 ?
            //    _midsRepo.GetAllMidsByMidID(mId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MidId", data.sSortDir_0, out totalRecords)
            //    :
            //    _midsRepo.GetAllMidsByMidID(mId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MidId", data.sSortDir_0, out totalRecords);

            var mids =
                pId > 0 ?
                _midsmerchantbranchposRepository.GetAllMidsMerchantBranchPOSs(id.Value, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MidId", data.sSortDir_0, out totalRecords)
                :
                _midsmerchantbranchposRepository.GetAllMidsMerchantBranchPOSs(id.Value, data.sSearch, data.iDisplayStart, data.iDisplayLength, "MidId", data.sSortDir_0, out totalRecords);

            var midModel = mids.Select(p => new MidsMerchantBranchesPOSsModel()
            {
                MidId = p.MidId,
                MerchantBranchPOSsId = p.MerchantBranchPOSId,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                MidName = p.Mid.MidName,
                Currency = p.Mid.Currency.CurrencyName
            }).ToList();


            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = midModel });
        }

        [HttpPost]
        public JsonResult MasterDeviceList()
        {
            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            List<MasterDevice> masterDevice = new List<MasterDevice>();

            masterDevice = _deviceRepo.GetAllMasterDevice(data.sSearch, data.iDisplayStart, data.iDisplayLength, "MasterDeviceId", data.sSortDir_0, out totalRecords);

            var devices = masterDevice.Select(p => new
            {
                MasterDeviceId = Utility.Encrypt(Convert.ToString(p.MasterDeviceId)),//(p.MasterDeviceId),
                DeviceName = p.DeviceName,
                Manufacturer = p.Manufacturer,
                IsActive = p.IsActive
            });

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = devices });
        }

        [HttpPost]
        public JsonResult SerialNumbersListBeyMasterDevice(int id)
        {
            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            List<Device> sn = new List<Device>();

            sn = _deviceRepo.GetAllDevicesByMasterDeviceId(id);

            var devices = sn.Select(d => new
            {
                DeviceId = Utility.Encrypt(Convert.ToString(d.DeviceId)),//d.DeviceId,
                SerialNumber = d.SerialNumber,
                IsActive = d.IsActive
            });

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = devices });
        }

        [HttpPost]
        public ActionResult DeviceList(int id)
        {
            TempData["MasterDeviceId"] = id;
            int merchantId = Convert.ToInt32(TempData["MerchantId"]);

            List<Device> device = new List<Device>();

            device = _deviceRepo.GetAllDevicesAvailableByMasterDeviceId(id);
            TempData["MerchantId"] = merchantId;
            var sn = new List<SelectListItem>();

            sn.AddRange(device.Select(p => new SelectListItem()
            {
                Text = p.SerialNumber,
                Value = Convert.ToString(p.DeviceId)
            }).ToList());

            return Json(sn);
        }

        [HttpPost]
        public ActionResult ViewMerchantDevice(int id)
        {
            List<DeviceMerchantLink> device = new List<DeviceMerchantLink>();

            device = _deviceRepo.GetDevicesByMerchantId(id);
            TempData["MerchantId"] = Convert.ToInt32(id);

            var sn = new List<SelectListItem>();

            sn.AddRange(device.Select(p => new SelectListItem()
            {
                Text = p.Device.MasterDevice.DeviceName + ": " + p.Device.SerialNumber,
                Value = Convert.ToString(p.DeviceId)
            }).ToList());

            return Json(sn);
        }

        public ActionResult DeviceToPos(int id)
        {
            List<MasterDevice> device = new List<MasterDevice>();

            device = _deviceRepo.GetAllNotAssignMasterDevice(id);

            var md = new List<SelectListItem>();

            md.AddRange(device.Select(p => new SelectListItem()
            {
                Text = p.DeviceName,
                Value = Convert.ToString(p.MasterDeviceId)
            }).ToList());

            return Json(md);
        }

        public ActionResult GetPosDevice(int? id)
        {
            List<DevicePOSLink> posDevice = new List<DevicePOSLink>();

            posDevice = _deviceRepo.GetAssignedPosDevice(id);

            var assigned = new List<SelectListItem>();

            assigned.AddRange(posDevice.Select(p => new SelectListItem()
            {
                Text = p.MasterDevice.DeviceName,
                Value = Convert.ToString(p.MasterDeviceId)
            }).ToList());

            return Json(assigned);
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult UsersList(int parentId, int parentTypeId)
        {
            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            List<Account> user = new List<Account>();

            user =
                parentId > 0 ?
                _userRepo.GetUserDetailsUserType(parentId, parentTypeId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "AccountId", data.sSortDir_0, out totalRecords)
                :
                _userRepo.GetUserDetailsUserType(parentId, parentTypeId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "AccountId", data.sSortDir_0, out totalRecords);

            var eserver = user.Select(p => new
            {
                AccountId = Utility.Encrypt(Convert.ToString(p.AccountId)),//p.AccountId,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                User = new User()
                {
                    FirstName = p.User.FirstName + " " + p.User.MiddleName + " " + p.User.LastName,
                    //MiddleName = p.User.MiddleName,
                    //LastName = p.User.LastName,
                    ContactInformation = new ContactInformation()
                    {
                        City = p.User.ContactInformation.City,
                        StateProvince = p.User.ContactInformation.StateProvince,
                        PrimaryContactNumber = p.User.ContactInformation.PrimaryContactNumber,
                        Country = new Country()
                        {
                            CountryName = p.User.ContactInformation.Country.CountryName
                        }
                    }
                }


            });

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = eserver });
        }

        #region Transaction Credit
        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TransactionAttemptList(int? pId, int? rId, int? mId, int? posId, int? bId, int transTypeId, int actionId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                DataTableModel data = new DataTableModel(Request);

                int totalRecords = 0;

                List<TransactionAttempt> transactionAttempts = new List<TransactionAttempt>();
                if (posId.HasValue && posId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllTransactionAttemptsbyPosId(transTypeId, actionId, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else if (bId.HasValue && bId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllTransactionAttemptsbyBranchId(transTypeId, actionId, bId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else if (mId.HasValue && mId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllTransactionAttemptsbyMerchantId(transTypeId, actionId, mId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else if (rId.HasValue && rId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllTransactionAttemptsbyResellerId(transTypeId, actionId, rId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else if (pId.HasValue && pId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllTransactionAttemptsbyPartnerId(transTypeId, actionId, pId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else
                {
                    transactionAttempts = _transactionRepo.GetAllTransactionAttemptsbyPosId(0, actionId, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }

                var gTransactionAttempts = transactionAttempts.Where(t => t.Amount > 0).GroupBy(item => new { CardTypeId = item.Transaction.CardTypeId, CardType = item.Transaction.CardType.TypeName, TransactionTypeId = item.TransactionTypeId, CurrencyName = item.Transaction.Currency.CurrencyName }, item => item,
                    (key, items) => new { CardTypeId = key.CardTypeId, CardType = key.CardType, TransactionType = key.TransactionTypeId, TransactionAttempts = items, Currency = key.CurrencyName });

                var reportsModel = gTransactionAttempts.Select(t => new
                {
                    CardTypeId = t.CardTypeId,
                    CardType = t.CardType,
                    Currency = t.Currency,
                    TransactionTypeId = t.TransactionType,
                    TotalAmount = t.TransactionAttempts.Sum(a => a.Amount),
                    TotalCount = t.TransactionAttempts.Count()
                }).Where(a => a.TotalAmount > 0);

                return Json(new { draw = data.sEcho, recordsTotal = reportsModel.Count(), recordsFiltered = reportsModel.Count(), data = reportsModel });
            }
            catch (Exception ex)
            {

            }
            return Json(null);
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TransactionList(int? pId, int? rId, int? mId, int? posId, int? bId, int cardTypeId, int transTypeId, int actionId, string currenyName, DateTime? startDate, DateTime? endDate)
        {
            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            List<TransactionAttempt> transactionAttempts = new List<TransactionAttempt>();

            if (posId.HasValue && posId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyPosId(cardTypeId, transTypeId, actionId, currenyName, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else if (bId.HasValue && bId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyBranchId(cardTypeId, transTypeId, actionId, currenyName, bId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else if (mId.HasValue && mId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyMerchantId(cardTypeId, transTypeId, actionId, currenyName, mId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else if (rId.HasValue && rId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyResellerId(cardTypeId, transTypeId, actionId, currenyName, rId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else if (pId.HasValue && pId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyPartnerId(cardTypeId, transTypeId, actionId, currenyName, pId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyPosId(0, 0, actionId, currenyName, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }

            var reportsModel = transactionAttempts.Select(t => new
            {
                TransactionId = t.Transaction.TransactionId,
                CardType = t.Transaction.CardType.TypeName,
                TransactionEntryTypeId = t.Transaction.TransactionEntryTypeId,
                CardNumber = Utility.DecryptEncDataWithKeyAndIV(t.Transaction.CardNumber, t.Transaction.Key.Key, t.Transaction.Key.IV).Remove(0, 4),
                NameOnCard = t.Transaction.NameOnCard,
                TransactionTypeId = t.TransactionTypeId,
                TotalTransaction = transactionAttempts.Count,
                Currency = t.Transaction.Currency.CurrencyName,
                TotalAmount = t.Transaction.Currency.CurrencyCode + " " + t.Amount.ToString("N2"),
                Amount = t.Amount,
                AuthNumber = t.AuthNumber,
                InvoiceNumber = t.Reference,
                TransNumber = t.TransactionId + "-" + t.TransactionAttemptId,
                BatchNumber = t.BatchNumber,
                Reference = t.Reference,
                POS = t.Transaction.MerchantPOS.MerchantPOSName,
                Location = t.Transaction.MerchantPOS.MerchantBranch.MerchantBranchName,
                MerchantName = t.Transaction.MerchantPOS.MerchantBranch.Merchant.MerchantName,
                DateReceived = t.DateReceived.Date.ToString("d"),
                TransactionTime = t.DateReceived.ToString("hh:mm tt"),
                LatLng = t.GPSLat + ", " + t.GPSLong,
                POSEntryMode = t.PosEntryMode,
                TraceNumber = t.TransNumber,
                ImageSource = t.TransactionSignature == null ?
                null : t.TransactionSignature.FileData == null ?
                ConvertImagetoBase64(t.TransactionSignature.Path + @"\" + t.TransactionSignature.FileName) : ToByteArray(t.TransactionSignature.FileData)

            }).Where(ta => ta.Amount > 0 && ta.TransactionTypeId == transTypeId && ta.Currency == currenyName).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = reportsModel.Count, recordsFiltered = totalRecords, data = reportsModel });
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TransactionList2(int? pId, int? rId, int? mId, int? posId, int? bId, int cardTypeId, int transTypeId, int actionId, string currenyName, DateTime? startDate, DateTime? endDate)
        {
            DataTableModel data = new DataTableModel(Request);

            data.sSortDir_0 = "desc";

            data.iDisplayStart = 0;

            data.iDisplayLength = int.MaxValue;

            int totalRecords = 0;

            List<TransactionAttempt> transactionAttempts = new List<TransactionAttempt>();

            if (posId.HasValue && posId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyPosId2(cardTypeId, transTypeId, actionId, currenyName, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else if (bId.HasValue && bId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyBranchId2(cardTypeId, transTypeId, actionId, currenyName, bId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else if (mId.HasValue && mId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyMerchantId2(cardTypeId, transTypeId, actionId, currenyName, mId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else if (rId.HasValue && rId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyResellerId2(cardTypeId, transTypeId, actionId, currenyName, rId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else if (pId.HasValue && pId.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyPartnerId2(cardTypeId, transTypeId, actionId, currenyName, pId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }
            else
            {
                transactionAttempts = _transactionRepo.GetAllTransactionbyPosId2(0, 0, actionId, currenyName, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
            }

            var reportsModel = transactionAttempts.Where(ta => ta.TransactionTypeId == transTypeId
                && ta.Transaction.Currency.CurrencyName == currenyName).Select(t => new
                {
                    NameOnCard = t.Transaction.NameOnCard,
                    TotalAmount = t.Transaction.Currency.CurrencyCode + " " + t.Amount.ToString("N2"),
                    Amount = t.Amount,
                    TransNumber = t.TransactionId + "-" + t.TransactionAttemptId,
                    Reference = t.Reference,
                    POS = t.Transaction.MerchantPOS.MerchantPOSName,
                    Location = t.Transaction.MerchantPOS.MerchantBranch.MerchantBranchName,
                    MerchantName = t.Transaction.MerchantPOS.MerchantBranch.Merchant.MerchantName,
                    DateReceived = t.DateReceived.ToString("D"),
                    POSEntryMode = t.PosEntryMode,
                    AuthNumber = t.AuthNumber
                }).Where(t => t.Amount > 0).ToList();


            return Json(reportsModel);
        }
        #endregion

        #region Transaction Debit
        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TransactionAttemptDebitList(int? pId, int? rId, int? mId, int? posId, int? bId, int transTypeId, int actionId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                DataTableModel data = new DataTableModel(Request);

                int totalRecords = 0;

                List<TransactionAttemptDebit> transactionAttempts = new List<TransactionAttemptDebit>();
                if (posId.HasValue && posId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsbyPosId(transTypeId, actionId, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else if (bId.HasValue && bId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsbyBranchId(transTypeId, actionId, bId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else if (mId.HasValue && mId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsbyMerchantId(transTypeId, actionId, mId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else if (rId.HasValue && rId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsbyResellerId(transTypeId, actionId, rId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else if (pId.HasValue && pId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsbyPartnerId(transTypeId, actionId, pId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }
                else
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsbyPosId(0, actionId, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }

                var gTransactionAttempts = transactionAttempts.GroupBy(item => new { TransactionTypeId = item.TransactionTypeId, CurrencyName = item.TransactionDebit.Currency.CurrencyName }, item => item,
                    (key, items) => new { TransactionType = key.TransactionTypeId, TransactionAttempts = items, Currency = key.CurrencyName });

                var reportsModel = gTransactionAttempts.Select(t => new
                {
                    Currency = t.Currency,
                    TransactionTypeId = t.TransactionType,
                    TotalAmount = t.TransactionAttempts.Sum(a => a.Amount),
                    TotalCount = t.TransactionAttempts.Count()
                }).Where(a => a.TotalAmount > 0 && transTypeId != 7);

                return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = reportsModel });
            }
            catch (Exception ex)
            {

            }
            return Json(null);
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TransactionDebitList(int? pId, int? rId, int? mId, int? posId, int? bId, int transTypeId, int actionId, string currenyName, DateTime? startDate, DateTime? endDate)
        {
            try
            {

                DataTableModel data = new DataTableModel(Request);

                int totalRecords = 0;

                List<TransactionAttemptDebit> transactionAttempts = new List<TransactionAttemptDebit>();

                if (posId.HasValue && posId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyPosId(transTypeId, actionId, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else if (bId.HasValue && bId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyBranchId(transTypeId, actionId, bId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else if (mId.HasValue && mId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyMerchantId(transTypeId, actionId, mId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else if (rId.HasValue && rId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyResellerId(transTypeId, actionId, rId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else if (pId.HasValue && pId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyPartnerId(transTypeId, actionId, pId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyPosId(0, actionId, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }

                var reportsModel = transactionAttempts.Select(t => new
                {
                    TransactionId = t.TransactionDebit.TransactionDebitId,
                    TransactionEntryTypeId = t.TransactionDebit.TransactionEntryTypeId,
                    CardNumber = Utility.DecryptEncDataWithKeyAndIV(t.TransactionDebit.CardNumber, t.TransactionDebit.Key.Key, t.TransactionDebit.Key.IV),
                    NameOnCard = t.TransactionDebit.NameOnCard,
                    TransactionTypeId = t.TransactionTypeId,
                    TotalTransaction = transactionAttempts.Count,
                    Currency = t.TransactionDebit.Currency.CurrencyName,
                    TotalAmount = t.TransactionDebit.Currency.CurrencyCode + " " + t.Amount.ToString("N2"),
                    AuthNumber = t.AuthNumber,
                    TransNumber = t.TransactionDebitId + "-" + t.TransactionAttemptDebitId,
                    BatchNumber = t.InvoiceNumber,
                    Reference = t.ReferenceNumber,
                    POS = t.TransactionDebit.MerchantPOS.MerchantPOSName,
                    Location = t.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchName,
                    MerchantName = t.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.MerchantName,
                    DateReceived = t.DateReceived.Date.ToString("d"),
                    LatLng = t.GPSLat + ", " + t.GPSLong
                }).Where(ta => ta.TransactionTypeId == transTypeId && ta.Currency == currenyName).ToList();

                return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = reportsModel });
            }
            catch (Exception ex)
            {

            }

            return Json(null);
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TransactionDebitList2(int? pId, int? rId, int? mId, int? posId, int? bId, int cardTypeId, int transTypeId, int actionId, string currenyName, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (CurrentUser.ParentType != Enums.ParentType.Partner)
                    return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

                DataTableModel data = new DataTableModel(Request);

                int totalRecords = 0;

                List<TransactionAttemptDebit> transactionAttempts = new List<TransactionAttemptDebit>();

                if (posId.HasValue && posId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyPosId2(cardTypeId, transTypeId, actionId, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else if (bId.HasValue && bId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyBranchId2(cardTypeId, transTypeId, actionId, bId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else if (mId.HasValue && mId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyMerchantId2(cardTypeId, transTypeId, actionId, mId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else if (rId.HasValue && rId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyResellerId2(cardTypeId, transTypeId, actionId, rId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else if (pId.HasValue && pId.Value > 0)
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyPartnerId2(cardTypeId, transTypeId, actionId, pId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionDebitId", data.sSortDir_0, out totalRecords);
                }
                else
                {
                    transactionAttempts = _transactionRepo.GetAllDebitTransactionbyPosId2(0, 0, actionId, posId.Value, startDate, endDate, data.sSearch, data.iDisplayStart, data.iDisplayLength, "TransactionId", data.sSortDir_0, out totalRecords);
                }

                var reportsModel = transactionAttempts.Where(ta => ta.TransactionTypeId == transTypeId
                && ta.TransactionDebit.Currency.CurrencyName == currenyName).Select(t => new
                {
                    TransNumber = t.TransactionDebitId + "-" + t.TransactionAttemptDebitId,
                    TotalAmount = t.TransactionDebit.Currency.CurrencyCode + " " + t.Amount.ToString("N2"),
                    Reference = t.ReferenceNumber,
                    Location = t.TransactionDebit.MerchantPOS.MerchantBranch.MerchantBranchName,
                    MerchantName = t.TransactionDebit.MerchantPOS.MerchantBranch.Merchant.MerchantName,
                    POS = t.TransactionDebit.MerchantPOS.MerchantPOSName,
                    DateReceived = t.DateReceived.ToString("D")
                }).ToList();

                return Json(reportsModel);
            }
            catch (Exception ex)
            {

            }

            return Json(null);
        }
        #endregion

        #region Transaction Graph
        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult GetDailySalesTransaction(DateTime date, int? r, int? m, int? b, int? p)
        {
            List<TransactionAttempt> transactionAttempts = new List<TransactionAttempt>();

            if (p.HasValue && p.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllPOSTransactionAttemptsforGraph(date, p.Value);
            }
            else if (b.HasValue && b.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllBranchTransactionAttemptsforGraph(date, b.Value);
            }
            else if (m.HasValue && m.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllMerchantTransactionAttemptsforGraph(date, m.Value);
            }
            else if (r.HasValue && r.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllResellerTransactionAttemptsforGraph(date, r.Value);
            }
            else
            {
                transactionAttempts = _transactionRepo.GetAllTransactionAttemptsforGraph(date, Convert.ToInt32(CurrentUser.ParentType), CurrentUser.ParentId);
            }

            DateTime end = date.AddDays(6);

            var samp = Enumerable.Range(0, 1 + end.Subtract(date).Days)
                    .Select(offset => date.AddDays(offset))
                    .ToArray();

            var tr = transactionAttempts.GroupBy(g => g.DateReceived.Date).Select(gr => new
            {
                Amount = gr.Where(pr => pr.TransactionTypeId == 3).Sum(a => a.Amount),
                Column = gr.Select(d => d.DateReceived.Date.ToString("d"))
            }).ToList();

            var z = samp.Select(t => new
            {
                Count = t.Date.ToString("d")
            }).ToList();

            return Json(new { data1 = z, data2 = tr });
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult GetDailyTransactionCount(DateTime date, int? r, int? m, int? b, int? p)
        {
            List<TransactionAttempt> transactionAttempts = new List<TransactionAttempt>();

            if (p.HasValue && p.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllPOSTransactionAttemptsforGraph(date, p.Value);
            }
            else if (b.HasValue && b.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllBranchTransactionAttemptsforGraph(date, b.Value);
            }
            else if (m.HasValue && m.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllMerchantTransactionAttemptsforGraph(date, m.Value);
            }
            else if (r.HasValue && r.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllResellerTransactionAttemptsforGraph(date, r.Value);
            }
            else
            {
                transactionAttempts = _transactionRepo.GetAllTransactionAttemptsforGraph(date, Convert.ToInt32(CurrentUser.ParentType), CurrentUser.ParentId);
            }

            DateTime end = date.AddDays(6);

            var samp = Enumerable.Range(0, 1 + end.Subtract(date).Days)
                    .Select(offset => date.AddDays(offset))
                    .ToArray();

            var tr = transactionAttempts.GroupBy(g => g.DateReceived.Date).Select(gr => new
            {
                Count = gr.Count(),
                Column = gr.Select(d => d.DateReceived.Date.ToString("d"))
            }).ToList();

            var z = samp.Select(t => new
            {
                Count = t.Date.ToString("d")
            }).ToList();

            return Json(new { data1 = z, data2 = tr });
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult GetDailyDeclinedTransactionCount(DateTime date, int? r, int? m, int? b, int? p)
        {
            List<TransactionAttempt> transactionAttempts = new List<TransactionAttempt>();

            if (p.HasValue && p.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllPOSTransactionAttemptsforGraph(date, p.Value);
            }
            else if (b.HasValue && b.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllBranchTransactionAttemptsforGraph(date, b.Value);
            }
            else if (m.HasValue && m.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllMerchantTransactionAttemptsforGraph(date, m.Value);
            }
            else if (r.HasValue && r.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllResellerTransactionAttemptsforGraph(date, r.Value);
            }
            else
            {
                transactionAttempts = _transactionRepo.GetAllTransactionAttemptsforGraph(date, Convert.ToInt32(CurrentUser.ParentType), CurrentUser.ParentId);
            }

            var rGraph = transactionAttempts.GroupBy(item => new { Date = item.DateReceived.Date }, item => item,
                (key, items) => new { Dates = key.Date });

            DateTime end = date.AddDays(6);

            var samp = Enumerable.Range(0, 1 + end.Subtract(date).Days)
                    .Select(offset => date.AddDays(offset))
                    .ToArray();

            var tr = transactionAttempts.GroupBy(g => g.DateReceived.Date).Select(gr => new
            {
                Count = gr.Where(d => d.TransactionTypeId == 6).Count(),
                Column = gr.Select(d => d.DateReceived.Date.ToString("d"))
            }).ToList();

            var z = samp.Select(t => new
            {
                Count = t.Date.ToString("d")
            }).ToList();

            return Json(new { data1 = z, data2 = tr });
        }
        #endregion

        #region Transaction Graph Debit
        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult GetDailyDebitSalesTransaction(DateTime date, int? r, int? m, int? b, int? p)
        {
            List<TransactionAttemptDebit> transactionAttempts = new List<TransactionAttemptDebit>();

            if (p.HasValue && p.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitPOSTransactionAttemptsforGraph(date, p.Value);
            }
            else if (b.HasValue && b.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitBranchTransactionAttemptsforGraph(date, b.Value);
            }
            else if (m.HasValue && m.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitMerchantTransactionAttemptsforGraph(date, m.Value);
            }
            else if (r.HasValue && r.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitResellerTransactionAttemptsforGraph(date, r.Value);
            }
            else
            {
                transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsforGraph(date, 0);
            }

            DateTime end = date.AddDays(6);

            var samp = Enumerable.Range(0, 1 + end.Subtract(date).Days)
                    .Select(offset => date.AddDays(offset))
                    .ToArray();

            var tr = transactionAttempts.GroupBy(g => g.DateReceived.Date).Select(gr => new
            {
                Amount = gr.Where(pr => pr.TransactionTypeId == 3).Sum(a => a.Amount),
                Column = gr.Select(d => d.DateReceived.Date.ToString("d"))
            }).ToList();

            var z = samp.Select(t => new
            {
                Count = t.Date.ToString("d")
            }).ToList();

            return Json(new { data1 = z, data2 = tr });
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult GetDailyDebitTransactionCount(DateTime date, int? r, int? m, int? b, int? p)
        {
            List<TransactionAttemptDebit> transactionAttempts = new List<TransactionAttemptDebit>();

            if (p.HasValue && p.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitPOSTransactionAttemptsforGraph(date, p.Value);
            }
            else if (b.HasValue && b.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitBranchTransactionAttemptsforGraph(date, b.Value);
            }
            else if (m.HasValue && m.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitMerchantTransactionAttemptsforGraph(date, m.Value);
            }
            else if (r.HasValue && r.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitResellerTransactionAttemptsforGraph(date, r.Value);
            }
            else
            {
                transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsforGraph(date, 0);
            }

            DateTime end = date.AddDays(6);

            var samp = Enumerable.Range(0, 1 + end.Subtract(date).Days)
                    .Select(offset => date.AddDays(offset))
                    .ToArray();

            var tr = transactionAttempts.GroupBy(g => g.DateReceived.Date).Select(gr => new
            {
                Count = gr.Count(),
                Column = gr.Select(d => d.DateReceived.Date.ToString("d"))
            }).ToList();

            var z = samp.Select(t => new
            {
                Count = t.Date.ToString("d")
            }).ToList();

            return Json(new { data1 = z, data2 = tr });
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult GetDailyDebitDeclinedTransactionCount(DateTime date, int? r, int? m, int? b, int? p)
        {
            List<TransactionAttemptDebit> transactionAttempts = new List<TransactionAttemptDebit>();

            if (p.HasValue && p.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitPOSTransactionAttemptsforGraph(date, p.Value);
            }
            else if (b.HasValue && b.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitBranchTransactionAttemptsforGraph(date, b.Value);
            }
            else if (m.HasValue && m.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitMerchantTransactionAttemptsforGraph(date, m.Value);
            }
            else if (r.HasValue && r.Value > 0)
            {
                transactionAttempts = _transactionRepo.GetAllDebitResellerTransactionAttemptsforGraph(date, r.Value);
            }
            else
            {
                transactionAttempts = _transactionRepo.GetAllDebitTransactionAttemptsforGraph(date, 0);
            }

            var rGraph = transactionAttempts.GroupBy(item => new { Date = item.DateReceived.Date }, item => item,
                (key, items) => new { Dates = key.Date });

            DateTime end = date.AddDays(6);

            var samp = Enumerable.Range(0, 1 + end.Subtract(date).Days)
                    .Select(offset => date.AddDays(offset))
                    .ToArray();

            var tr = transactionAttempts.GroupBy(g => g.DateReceived.Date).Select(gr => new
            {
                Count = gr.Where(d => d.TransactionTypeId == 6).Count(),
                Column = gr.Select(d => d.DateReceived.Date.ToString("d"))
            }).ToList();

            var z = samp.Select(t => new
            {
                Count = t.Date.ToString("d")
            }).ToList();

            return Json(new { data1 = z, data2 = tr });
        }
        #endregion

        #region Top Merchant & Branch Name
        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TopMerchantList(int pId)
        {
            if (CurrentUser.ParentType != Enums.ParentType.Partner)
                return Json(new { draw = 1, recordsTotal = 0, recordsFiltered = 0, data = "" });

            DataTableModel data = new DataTableModel(Request);

            var transaction = _transactionRepo.GetAllTransactionAttemptsForTopMerchantsByPartner(Convert.ToString(con), pId);

            var gTransactionAttempts = transaction.Where(t => t.Amount > 0).GroupBy(item => new { MerchantName = item.Transaction.MerchantPOS.MerchantBranch.Merchant.MerchantName, PartnerName = item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.CompanyName }, item => item,
                (key, items) => new { MerchantName = key.MerchantName, PartnerName = key.PartnerName, TransactionAttempts = items });

            var tMerchant = gTransactionAttempts.Select(t => new
            {
                MerchantName = t.MerchantName,
                PartnerCompany = t.PartnerName,
                TotalTransactionCount = t.TransactionAttempts.Count()
            }).Where(ta => ta.TotalTransactionCount > 0).OrderByDescending(a => a.TotalTransactionCount).ToList().Take(10);

            return Json(new { draw = data.sEcho, recordsTotal = 10, recordsFiltered = 10, data = tMerchant });
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TopMerchantListByReseller(int rId)
        {

            DataTableModel data = new DataTableModel(Request);

            var transaction = _transactionRepo.GetAllTransactionAttemptsForTopMerchantsByReseller(rId);

            var gTransactionAttempts = transaction.Where(t => t.Amount > 0).GroupBy(item => new { MerchantName = item.Transaction.MerchantPOS.MerchantBranch.Merchant.MerchantName, PartnerName = item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.CompanyName }, item => item,
                (key, items) => new { MerchantName = key.MerchantName, PartnerName = key.PartnerName, TransactionAttempts = items }).Take(10);

            var tMerchant = gTransactionAttempts.Select(t => new
            {
                MerchantName = t.MerchantName,
                PartnerCompany = t.PartnerName,
                TotalTransactionCount = t.TransactionAttempts.Count()
            }).Where(ta => ta.TotalTransactionCount > 0).OrderByDescending(a => a.TotalTransactionCount).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = 10, recordsFiltered = 10, data = tMerchant });
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult TopMerchantListByMerchant(int mId)
        {
            DataTableModel data = new DataTableModel(Request);

            var transaction = _transactionRepo.GetAllTransactionAttemptsForTopMerchantsByMerchant(mId);

            var gTransactionAttempts = transaction.Where(t => t.Amount > 0).GroupBy(item => new { MerchantBranchName = item.Transaction.MerchantPOS.MerchantBranch.MerchantBranchName, PartnerName = item.Transaction.MerchantPOS.MerchantBranch.Merchant.Reseller.Partner.CompanyName }, item => item,
                (key, items) => new { MerchantBranchName = key.MerchantBranchName, PartnerName = key.PartnerName, TransactionAttempts = items }).Take(10);

            var tMerchant = gTransactionAttempts.Select(t => new
            {
                MerchantBranchName = t.MerchantBranchName,
                PartnerCompany = t.PartnerName,
                TotalTransactionCount = t.TransactionAttempts.Count()
            }).Where(ta => ta.TotalTransactionCount > 0).OrderByDescending(a => a.TotalTransactionCount).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = 10, recordsFiltered = 10, data = tMerchant });

        }
        #endregion

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult RequestedMerchantList()
        {

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            var rMerchant = _requestedMerchantRepository.RequestedMerchantList(data.sSearch, data.iDisplayStart, data.iDisplayLength, "RequestedMerchantId", data.sSortDir_0, out totalRecords);

            var merchant = rMerchant.Select(rm => new
            {
                RequestedMerchantId = rm.RequestedMerchantId,
                MerchantName = rm.MerchantName,
                DateCreated = rm.DateCreated.Date.ToString("d"),
                IsActive = rm.IsActive,
                Status = rm.IsActive
            }).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchant });
        }

        [HttpPost]
        [CustomAttributes.SessionExpireFilter]
        public JsonResult RequestedMerchantsList(int parentId)
        {

            DataTableModel data = new DataTableModel(Request);

            int totalRecords = 0;

            var rMerchant = _requestedMerchantRepository.RequestedMerchantList(parentId, data.sSearch, data.iDisplayStart, data.iDisplayLength, "RequestedMerchantId", data.sSortDir_0, out totalRecords);

            var merchant = rMerchant.Select(rm => new
            {
                RequestedMerchantId = rm.RequestedMerchantId,
                MerchantName = rm.MerchantName,
                DateCreated = rm.DateCreated.Date.ToString("d"),
                IsActive = rm.IsActive,
                Status = rm.IsActive
            }).ToList();

            return Json(new { draw = data.sEcho, recordsTotal = totalRecords, recordsFiltered = totalRecords, data = merchant });
        }

        private string HashCardNumber(string cardNum)
        {
            try
            {
                int cardlength = cardNum.Length - 16;
                //get 16 char
                string newCC = cardNum.Substring(cardlength);
                int lastfour = newCC.Length - 4;
                string newCard = newCC.Substring(lastfour);
                string dash = "-";
                string hash = "";
                for (int i = 0; i < lastfour; i++)
                {
                    int ctr = i + 1;
                    hash += "X";
                    if (ctr % 4 == 0)
                    {
                        hash += dash;
                    }
                }

                return hash + newCard;
            }
            catch
            {
                return "XXXXXXXXXXXX";
            }
        }

        public string ConvertImagetoBase64(string path)
        {
            try
            {
                using (Image image = Image.FromFile(path))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);
                        return base64String;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ToByteArray(String HexString)
        {
            if (HexString != null)
            {

                byte[] data = ConvertFromStringToHex(HexString);
                string base64 = Convert.ToBase64String(data);

                return base64;
            }

            return null;
        }

        public static byte[] ConvertFromStringToHex(string inputHex)
        {
            inputHex = inputHex.Replace("-", "");

            byte[] resultantArray = new byte[inputHex.Length / 2];
            for (int i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }

        [HttpPost]
        public ActionResult GetProvincesByCountry(int id)
        {
            var listProvinces = _refRepo.GetAllProvincesByCountry(id);

            var ddlprovinces = new List<SelectListItem>();

            var val = 0;

            if (listProvinces.Count > 0)
            {
                ddlprovinces.AddRange(listProvinces.Select(p => new SelectListItem()
                {
                    Value = p.ProvinceId.ToString(),
                    Text = p.ProvinceName,
                    Selected = true
                }).ToList());
            }
            else
            {
                ddlprovinces.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Provinces available",
                    Selected = val == 0
                });
            }

            return Json(ddlprovinces);
        }
    }
}
