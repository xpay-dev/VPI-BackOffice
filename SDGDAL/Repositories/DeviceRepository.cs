using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class DeviceRepository
    {
        public MasterDevice CreateMasterDevice(MasterDevice device)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        device.DateCreated = DateTime.Now;
                        var savedMasterDevice = context.MasterDevices.Add(device);
                        context.SaveChanges();

                        trans.Commit();

                        return savedMasterDevice;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public bool IsDeviceNameAvailable(string DeviceName)
        {
            using (DataContext context = new DataContext())
            {
                return context.MasterDevices.SingleOrDefault(dev => dev.DeviceName == DeviceName) == null;
            }
        }

        public List<MasterDevice> GetAllMasterDevice()
        {
            List<MasterDevice> partners = new List<Entities.MasterDevice>();
            using (DataContext context = new DataContext())
            {
                return context.MasterDevices.Include("DeviceType").OrderByDescending(d => d.DeviceName).Where(d => d.IsActive == true && d.IsDeleted == false).ToList();
            }
        }

        public List<MasterDevice> GetAllMasterDevice(string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<MasterDevice> devices = new List<Entities.MasterDevice>();
            using (DataContext context = new DataContext())
            {
                var q = context.MasterDevices.Include("DeviceType")
                    .Where(d => d.IsActive == true && d.IsDeleted == false);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                devices = q.ToList();
            }

            return devices;
        }

        public MasterDevice GetDetailsByMasterDeviceId(int? mdId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var device = context.MasterDevices
                                        .Include("FlowType")
                                        .Include("DeviceType")
                                        .SingleOrDefault(d => d.MasterDeviceId == mdId);
                    return device;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public MasterDevice UpdateMasterDevice(MasterDevice device)
        {
            using (DataContext context = new DataContext())
            {
                MasterDevice md = context.MasterDevices.Single(i => i.MasterDeviceId == device.MasterDeviceId);
                md.DeviceName = device.DeviceName;
                md.Manufacturer = device.Manufacturer;
                md.Warranty = device.Warranty;
                md.ExternalData = device.ExternalData;
                md.DeviceFlowTypeId = device.DeviceFlowTypeId;
                md.DeviceTypeId = device.DeviceTypeId;

                context.SaveChanges();

                return md;
            }
        }

        public bool IsSerialNumberAvailable(string sn)
        {
            using (DataContext context = new DataContext())
            {
                return context.Device.SingleOrDefault(d => d.SerialNumber == sn) == null;
            }
        }

        public Device CreateSerialNumber(Device device)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        device.DateCreated = DateTime.Now;
                        device.IsActive = true;
                        device.IsDeleted = false;
                        var savedDevice = context.Device.Add(device);
                        context.SaveChanges();

                        trans.Commit();

                        return savedDevice;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public List<Device> GetAllDevicesAvailableByMasterDeviceId(int id)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var device = from d in context.Device
                                 where (d.MasterDeviceId == id) && !context.DeviceMerchantLink.Any(i => i.DeviceId == d.DeviceId)
                                 select d;

                    return device.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<Device> GetAllDevicesByMasterDeviceId(int id)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var device = from d in context.Device
                                 where (d.MasterDeviceId == id)
                                 select d;

                    return device.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<DevicePOSLink> GetAssignedPosDevice(int? posId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var device = context.DevicePOSLink.Include("MasterDevice").Where(i => i.MerchantPOSId == posId);

                    return device.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<MasterDevice> GetAllNotAssignMasterDevice(int posId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var availMasterDevice = from md in context.MasterDevices
                                            where !context.DevicePOSLink.Any(d => d.MasterDeviceId == md.MasterDeviceId && d.MerchantPOSId == posId)
                                            select md;

                    return availMasterDevice.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<DeviceMerchantLink> GetDevicesByMerchantId(int id)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var sn = context.DeviceMerchantLink.Include("Device")
                             .Include("Device.MasterDevice")
                             .Include("Device.MasterDevice.FlowType")
                             .Include("Device.MasterDevice.DeviceType")
                             .Where(i => i.MerchantId == id);

                    return sn.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<Device> GetAllSerialNumberByMasterDeviceId(int id)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var sn = context.Device.Where(i => i.MasterDeviceId == id);

                    return sn.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Device GetDetailsByDeviceId(int deviceId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var device = context.Device.SingleOrDefault(d => d.DeviceId == deviceId);

                    return device;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Device UpdateDevice(Device sn)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        Device device = context.Device.Single(i => i.DeviceId == sn.DeviceId);
                        device.SerialNumber = sn.SerialNumber;
                        context.SaveChanges();

                        trans.Commit();

                        return device;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public bool HasDeviceByMasterDeviceId(int masterDeviceId)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var device = context.Device.Where(d => d.MasterDeviceId == masterDeviceId).ToList();

                    if (device.Count() != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}