using SDGAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CTPaymentUtilities;
using CTPayment;
using System.Diagnostics;
using GatewayProcessor;
using SDGUtil;
using SDGDAL.Entities;
using SDGDAL;
using SDGDAL.Repositories;

namespace SDGAdmin.Controllers
{
    public class CTPaymentUserController : Controller
    {
        ApiCTPayment _apiCTAddUser;
        GatewayProcessor.Gateways _gateway;
        ReferenceRepository _refRepo;

        string action = String.Empty;

        public CTPaymentUserController()
        {
            _apiCTAddUser = new ApiCTPayment();
            _gateway = new Gateways();
            _refRepo = new ReferenceRepository();
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(CTPaymentUser model)
        {
            RecurAddUserInput user = new RecurAddUserInput();

            try
            {
                user.companyNumberField = "00101";
                user.merchantNumberField = "53400100"; //"47200001";
                user.customerNumberField = "00000000";
                user.nameField = model.Name;
                user.emailField = model.Email;
                user.cardNumberField = String.Empty;
                user.expirationDateField = String.Empty;
                user.languageCodeField = "E";
                user.operatorIDField = "REDIRDMO";

                var output = _apiCTAddUser.ProcessCTPaymentAddUser(user);

                if (output.Result != null)
                {
                    var resAck = _apiCTAddUser.AcceptCTAcknowledge(output.Result.idField);

                    if (resAck.Result.returnCodeField == "true")
                    {
                        RecurRedirSessionInput session = new RecurRedirSessionInput();

                        session.companyNumberField = user.companyNumberField;
                        session.customerNumberField = user.customerNumberField;
                        session.merchantNumberField = user.merchantNumberField;
                        session.operatorIDField = user.operatorIDField;
                        session.tokenField = output.Result.tokenField;
                        session.successUrlField = "http://localhost:37367/CTPaymentUser/Success";
                        session.failureUrlField = "http://localhost:37367/CTPaymentUser/Failed";

                        var response = _apiCTAddUser.ProcessCTPaymentRedirectUser(session);

                        if (response.Result.recurReturnCodeField.Replace(" ", "") == "00")
                        {
                            string addUserURL = System.Configuration.ConfigurationManager.AppSettings["CTPaymentAddUserDemoURL"].ToString();

                            TempData["SecureID"] = response.Result.secureIDField;
                            Process.Start(String.Format(addUserURL + "?secureID={0}", response.Result.secureIDField));
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Add user cannot be completed.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Acknowledgement cannot confirm.");
                    }
                }
            }
            catch(Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "CtPaymentUserController", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Index";
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
        
        public ActionResult Success()
        {
            TransactionModel model = new TransactionModel();

            try
            {
                action = "processing ct payment adding user";

                RecurRedirSessionResponseInput resInput = new RecurRedirSessionResponseInput();

                resInput.secureIDField = TempData["SecureID"].ToString();

                var response = _apiCTAddUser.ProcessCTPaymentResponseRedirectUser(resInput);

                if (response.Result.recurReturnCodeField.Replace(" ", "") != "00")
                {
                    model.ErrNumber = response.Result.recurReturnCodeField;
                    model.ErrMessage = response.Result.statusField;

                    return View(model);
                }
            }
            catch(Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "CtPaymentUserController", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Success";
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

        public ActionResult Failed()
        {
            TransactionModel model = new TransactionModel();

            try
            {
                action = "processing ct payment adding user";

                RecurRedirSessionResponseInput resInput = new RecurRedirSessionResponseInput();

                resInput.secureIDField = TempData["SecureID"].ToString();

                var response = _apiCTAddUser.ProcessCTPaymentResponseRedirectUser(resInput);

                if (response.Result.recurReturnCodeField.Replace(" ", "") != "00")
                {
                    model.ErrNumber = response.Result.recurReturnCodeField;
                    model.ErrMessage = response.Result.statusField;

                    return View(model);
                }
            }
            catch(Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "CtPaymentUserController", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Success";
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
    }
}
