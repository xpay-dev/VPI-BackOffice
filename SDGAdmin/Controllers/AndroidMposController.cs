using SDGAdmin.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGDAL;

namespace SDGAdmin.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class AndroidMposController : Controller
    {
        string action = string.Empty;

        AndroidAppVersionRepo _androidAppVersionRepo;
        ReferenceRepository _refRepo;

        public AndroidMposController()
        {
            _androidAppVersionRepo = new AndroidAppVersionRepo();
            _refRepo = new ReferenceRepository();
        }

        public ActionResult Index()
        {
            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            return View();
        }

        public ActionResult AddInfo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddInfo(AndroidAppVersionModel model)
        {
            try
            {
                action = "adding new mobile app version for android.";

                if(!_androidAppVersionRepo.IsPackageNameAlreadyExists(model.PackageName))
                {
                    ModelState.AddModelError(string.Empty, "Package name already exists.");
                }

                if(ModelState.IsValid && model != null)
                {
                    var app = new SDGDAL.Entities.AndroidAppVersion();

                    app.AppName = model.AppName;
                    app.PackageName = model.PackageName;
                    app.VersionName = model.VersionName;
                    app.VersionCode = model.VersionCode;
                    app.VersionBuild = model.VersionBuild;
                    app.Description = model.Description;
                    app.IsActive = model.IsActive;

                    var response = _androidAppVersionRepo.CreateAndroidVersionApp(app);

                    if(response.AndroidAppVersionId > 0)
                    {
                        var userActivity = "Registered a Mobile App Version";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "CreateMasterDevice", "");

                        Session["Success"] = "Mobile App Version for Android successfully created.";

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error occurs while saving the data.");
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "AddInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Master Device Creation";
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

            return View(model);
        }

        public ActionResult UpdateInfo(string id)
        {
            AndroidAppVersionModel model = new AndroidAppVersionModel();

            try
            {
                action = "updating mobile app version.";

                if (id == string.Empty)
                {
                    HttpNotFound();
                }

                var data = _androidAppVersionRepo.GetAndroidVersionAppById(Convert.ToInt32(Utility.Decrypt(id)));

                TempData["AndroidAppVersionId"] = data.AndroidAppVersionId;
                model.AndroidAppVersionId = data.AndroidAppVersionId;
                model.AppName = data.AppName;
                model.Description = data.Description;
                model.IsActive = data.IsActive;
                model.PackageName = data.PackageName;
                model.VersionBuild = data.VersionBuild;
                model.VersionCode = data.VersionCode;
                model.VersionName = data.VersionName;
            }
            catch(Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update MobileApp Version for Android";
                err.Method = "Update-Get";
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
           
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateInfo(AndroidAppVersionModel model)
        {
            try
            {
                action = "updating mobile app version for android.";
                AndroidAppVersion app = new AndroidAppVersion();

                app.AndroidAppVersionId = Convert.ToInt32(TempData["AndroidAppVersionId"]);
                app.AppName = model.AppName;
                app.DateCreated = DateTime.Now;
                app.Description = model.Description;
                app.IsActive = model.IsActive;
                app.PackageName = model.PackageName;
                app.VersionBuild = model.VersionBuild;
                app.VersionCode = model.VersionCode;
                app.VersionName = model.VersionName;

                var result = _androidAppVersionRepo.UpdateAndroidVersionApp(app);

                if(result != null)
                {
                    Session["Success"] = "Mobile App Version for Android successfully updated.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurs while updating the mobile app package version.");
                }
            }
            catch(Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update MobileApp Version for Android";
                err.Method = "Update";
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

            return View(model);
        }

        public ActionResult ViewInfo(string id)
        {
            AndroidAppVersionModel model = new AndroidAppVersionModel();

            try
            {
                action = "updating mobile app version.";

                if (id == string.Empty)
                {
                    HttpNotFound();
                }

                var data = _androidAppVersionRepo.GetAndroidVersionAppById(Convert.ToInt32(Utility.Decrypt(id)));

                model.AndroidAppVersionId = data.AndroidAppVersionId;
                model.AppName = data.AppName;
                model.Description = data.Description;
                model.IsActive = data.IsActive;
                model.PackageName = data.PackageName;
                model.VersionBuild = data.VersionBuild;
                model.VersionCode = data.VersionCode;
                model.VersionName = data.VersionName;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "ViewInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update MobileApp Version for Android";
                err.Method = "View";
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

            return View(model);
        }
    }
}
