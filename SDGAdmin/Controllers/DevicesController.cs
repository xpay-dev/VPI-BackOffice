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
    public class DevicesController : Controller
    {
        string action = string.Empty;

        DeviceRepository _deviceRepo;
        ReferenceRepository _refRepo;

        public DevicesController()
        {
            _refRepo = new ReferenceRepository();
            _deviceRepo = new DeviceRepository();
        }

        public ActionResult Index()
        {
            var userActivity = "Entered Device Management Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            return View();
        }

        public ActionResult CreateMasterDevice()
        {
            var userActivity = "Entered Create Master Device Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "CreateMasterDevice", "");

            GenerateDataForDropDown();

            return View();
        }

        [HttpPost]
        public ActionResult CreateMasterDevice(MasterDeviceModel device)
        {
            try
            {
                action = "creating master device.";

                if (!_deviceRepo.IsDeviceNameAvailable(device.DeviceName))
                {
                    ModelState.AddModelError(string.Empty, "Device name already exists.");
                    GenerateDataForDropDown();
                }

                if (ModelState.IsValid && device != null)
                {
                    var d = new SDGDAL.Entities.MasterDevice();

                    d.DeviceName = device.DeviceName;
                    d.Manufacturer = device.Manufacturer;
                    d.Warranty = device.Warranty;
                    d.ExternalData = device.ExternalData;
                    d.DeviceFlowTypeId = device.DeviceFlowTypeId;
                    d.DeviceTypeId = device.DeviceTypeId;
                    d.IsActive = true;
                    d.IsDeleted = false;

                    var newDevice = _deviceRepo.CreateMasterDevice(d);

                    if (newDevice.MasterDeviceId > 0)
                    {
                        var userActivity = "Registered a Master Device";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "CreateMasterDevice", "");

                        Session["Success"] = "Master Device created.";

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    GenerateDataForDropDown();
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "CreateMasterDevice", ex.StackTrace);

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

            return View(device);
        }

        public ActionResult UpdateMasterDeviceInfo(string id)
        {
            var userActivity = "Entered Update Master Device Info";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateMasterDeviceInfo", "");

            MasterDeviceModel md = new MasterDeviceModel();

            try
            {
                action = "fetching the data of the master device.";

                if (id == string.Empty)
                {
                    return HttpNotFound();
                }

                var device = _deviceRepo.GetDetailsByMasterDeviceId(Convert.ToInt32(Utility.Decrypt(id)));

                var flowtype = _refRepo.GetDeviceFlowType();

                ViewBag.DeviceFlowType = flowtype.Select(c => new SelectListItem()
                {
                    Value = c.DeviceFlowTypeId.ToString(),
                    Text = c.FlowType,
                    Selected = c.DeviceFlowTypeId == device.DeviceFlowTypeId
                }).ToList();

                var devicetype = _refRepo.GetDeviceType();

                ViewBag.DeviceType = devicetype.Select(c => new SelectListItem()
                {
                    Value = c.DeviceTypeId.ToString(),
                    Text = c.TypeName,
                    Selected = c.DeviceTypeId == device.DeviceTypeId
                }).ToList();

                TempData["MasterDeviceId"] = device.MasterDeviceId;
                md.DeviceName = device.DeviceName;
                md.Manufacturer = device.Manufacturer;
                md.Warranty = device.Warranty;
                md.ExternalData = device.ExternalData;
                md.DeviceFlowTypeId = device.DeviceFlowTypeId;
                md.DeviceTypeId = device.DeviceTypeId;

                return View(md);

            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateMasterDeviceInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update Master Device";
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

            return View(md);
        }

        [HttpPost]
        public ActionResult UpdateMasterDeviceInfo(MasterDeviceModel device)
        {
            MasterDevice md = new MasterDevice();
            try
            {
                action = "updating master device info.";

                md.MasterDeviceId = Convert.ToInt32(TempData["MasterDeviceId"]);
                md.DeviceName = device.DeviceName;
                md.Manufacturer = device.Manufacturer;
                md.Warranty = device.Warranty;
                md.ExternalData = device.ExternalData;
                md.DeviceTypeId = device.DeviceTypeId;
                md.DeviceFlowTypeId = device.DeviceFlowTypeId;

                var result = _deviceRepo.UpdateMasterDevice(md);

                if (result != null)
                {
                    var userActivity = "Updates Master Device Info";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateMasterDeviceInfo", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateMasterDeviceInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update Master Device";
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

            return View(device);
        }

        public ActionResult ViewMasterDeviceInfo(string id)
        {
            var userActivity = "Entered View Master Device Info";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewMasterDeviceInfo", "");

            try
            {
                action = "fetching the data for master device";

                if (id == string.Empty)
                {
                    return HttpNotFound();
                }

                var device = _deviceRepo.GetDetailsByMasterDeviceId(Convert.ToInt32(Utility.Decrypt(id)));

                var flowtype = _refRepo.GetDeviceFlowType();

                ViewBag.DeviceFlowType = flowtype.Select(c => new SelectListItem()
                {
                    Value = c.DeviceFlowTypeId.ToString(),
                    Text = c.FlowType,
                    Selected = c.DeviceFlowTypeId == device.DeviceFlowTypeId
                }).ToList();


                var devicetype = _refRepo.GetDeviceType();

                ViewBag.DeviceType = devicetype.Select(c => new SelectListItem()
                {
                    Value = c.DeviceTypeId.ToString(),
                    Text = c.TypeName,
                    Selected = c.DeviceTypeId == device.DeviceTypeId
                }).ToList();

                MasterDeviceModel md = new MasterDeviceModel();

                md.DeviceName = device.DeviceName;
                md.Manufacturer = device.Manufacturer;
                md.Warranty = device.Warranty;
                md.ExternalData = device.ExternalData;
                md.DeviceFlowTypeId = device.DeviceFlowTypeId;
                md.DeviceTypeId = device.DeviceTypeId;

                return View(md);

            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "ViewMasterDeviceInfo", ex.StackTrace);

                return HttpNotFound();
            }
        }

        public ActionResult AddSerialNumber(string id)
        {
            var userActivity = "Entered Add Serial Number Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "AddSerialNumber", "");


            if (id != string.Empty)
            {
                GenerateDataForDropDown();
            }
            else
            {
                var masterDevice = _deviceRepo.GetAllMasterDevice();

                var ddlmasterDevice = new List<SelectListItem>();

                ddlmasterDevice.AddRange(masterDevice.Select(d => new SelectListItem()
                {
                    Value = d.MasterDeviceId.ToString(),
                    Text = d.DeviceName,
                    Selected = d.MasterDeviceId == Convert.ToInt32(Utility.Decrypt(id))
                }).ToList());

                ViewBag.MasterDevice = ddlmasterDevice;

                DeviceModel dm = new DeviceModel();
                dm.MasterDeviceId = Convert.ToInt32(Utility.Decrypt(id));

                return View(dm);
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddSerialNumber(DeviceModel device, string SaveAddMore)
        {
            GenerateDataForDropDown();

            DeviceModel dm = new DeviceModel();
            dm.MasterDeviceId = Convert.ToInt32(device.MasterDeviceId);
            dm.DeviceId = device.DeviceId;

            try
            {
                action = "error adding serial number to the device";

                if (!_deviceRepo.IsSerialNumberAvailable(device.SerialNumber) && device.SerialNumber != "0")
                {
                    ModelState.AddModelError(string.Empty, "Serial Number already exists.");
                }

                if (ModelState.IsValid && device != null)
                {
                    var d = new SDGDAL.Entities.Device();

                    d.SerialNumber = device.SerialNumber;
                    d.KeyInjectedId = device.IsKeyInjected == true ? 1 : 0;
                    d.MasterDeviceId = device.MasterDeviceId;

                    var result = _deviceRepo.CreateSerialNumber(d);

                    if (result != null && String.IsNullOrWhiteSpace(SaveAddMore))
                    {
                        var userActivity = "Registered a Serial Number";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "AddSerialNumber", "");

                        Session["Success"] = "Serial Number Created.";

                        return RedirectToAction("Index");
                    }
                }

            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);

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

            return View(dm);
        }

        private void GenerateDataForDropDown()
        {
            var flowtype = _refRepo.GetDeviceFlowType();

            var ddlflowtype = new List<SelectListItem>();

            ddlflowtype.AddRange(flowtype.Select(d => new SelectListItem()
            {
                Value = d.DeviceFlowTypeId.ToString(),
                Text = d.FlowType
            }).ToList());

            ViewBag.DeviceFlowType = ddlflowtype;

            var devicetype = _refRepo.GetDeviceType();

            var ddldevicetype = new List<SelectListItem>();

            ddldevicetype.AddRange(devicetype.Select(d => new SelectListItem()
            {
                Value = d.DeviceTypeId.ToString(),
                Text = d.TypeName
            }).ToList());

            ViewBag.DeviceType = ddldevicetype;

            var masterDevice = _deviceRepo.GetAllMasterDevice();

            var ddlmasterDevice = new List<SelectListItem>();

            ddlmasterDevice.AddRange(masterDevice.Select(d => new SelectListItem()
            {
                Value = d.MasterDeviceId.ToString(),
                Text = d.DeviceName
            }).ToList());

            ViewBag.MasterDevice = ddlmasterDevice;
        }

        public ActionResult ViewSerialNumber(string id)
        {
            var userActivity = "Entered View Serial Number Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewSerialNumber", "");

            MasterDeviceModel md = new MasterDeviceModel();

            try
            {
                action = "fetching the data for the device serial number.";

                #region
                //if (id != string.Empty)
                //{
                //    return HttpNotFound();
                //}
                #endregion

                var masterDevice = _deviceRepo.GetDetailsByMasterDeviceId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)));

                md.MasterDeviceId = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));
                md.DeviceName = masterDevice.DeviceName;

                return View(md);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "View Serial Numbers";
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

            return View();
        }

        public ActionResult UpdateSerialNumber(string id)
        {
            var userActivity = "Entered Update Serial Number Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateSerialNumber", "");

            DeviceModel device = new DeviceModel();
            try
            {
                action = "fetching data for the device serial number.";

                if (id == null)
                {
                    return HttpNotFound();
                }

                var deviceResult = _deviceRepo.GetDetailsByDeviceId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)));

                device.MasterDeviceId = deviceResult.MasterDeviceId;
                device.SerialNumber = deviceResult.SerialNumber;
                device.DeviceId = deviceResult.DeviceId;

                return View(device);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update Serial Number";
                err.Method = "Save";
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

        [HttpPost]
        public ActionResult UpdateSerialNumber(DeviceModel device)
        {
            try
            {
                action = "updating the serial number for the device.";

                Device result = new Device();
                var sn = new SDGDAL.Entities.Device();

                var deviceResult = _deviceRepo.GetDetailsByDeviceId(device.DeviceId);

                if (deviceResult.SerialNumber == device.SerialNumber)
                {
                    sn.DeviceId = device.DeviceId;
                    sn.SerialNumber = device.SerialNumber;

                    result = _deviceRepo.UpdateDevice(sn);
                }
                else
                {
                    if (!_deviceRepo.IsSerialNumberAvailable(device.SerialNumber))
                    {
                        ModelState.AddModelError(string.Empty, "Serial Number already exists.");
                    }
                    else
                    {
                        sn.DeviceId = device.DeviceId;
                        sn.SerialNumber = device.SerialNumber;
                        result = _deviceRepo.UpdateDevice(sn);
                    }
                }

                if (result.DeviceId != 0)
                {
                    var userActivity = "Entered Update Serial Number Page";

                    var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateSerialNumber", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);

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

            return View(device);
        }

        public ActionResult AddToBlackList(string id)
        {
            var userActivity = "Entered Update Serial Number Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "AddToBlackList", "");

            DeviceModel device = new DeviceModel();
            try
            {
                action = "fetching data for the device serial number.";

                if (id == null)
                {
                    return HttpNotFound();
                }

                Device result = new Device();
                var sn = new SDGDAL.Entities.Device();

                var deviceResult = _deviceRepo.GetDetailsByDeviceId(device.DeviceId);

                if (deviceResult.SerialNumber == device.SerialNumber)
                {
                    sn.DeviceId = device.DeviceId;
                    sn.SerialNumber = device.SerialNumber;
                    sn.IsBlackListed = true;

                    result = _deviceRepo.UpdateDevice(sn);

                    if (result.DeviceId != 0)
                    {
                        var userAct = "Entered Update Serial Number Page";

                        var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userAct, "AddToBlackList", "");

                        Session["Success"] = "Added to Black Listed Devices";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch(Exception ex)
            {   
                //Log error
            }

            return View();
        }
    }
}
