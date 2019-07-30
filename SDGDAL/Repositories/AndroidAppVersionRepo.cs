using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class AndroidAppVersionRepo
    {
        public List<AndroidAppVersion> GetAllAppVersion(string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<AndroidAppVersion> app = new List<AndroidAppVersion>();

            using (DataContext context = new DataContext())
            {
                var q = from d in context.AndroidAppVersion
                        select d; ;

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                return app = q.ToList();
            }
        }

        public AndroidAppVersion CreateAndroidVersionApp(AndroidAppVersion app)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        app.DateCreated = DateTime.Now;

                        var savedAndroidAppVer = context.AndroidAppVersion.Add(app);
                        context.SaveChanges();

                        trans.Commit();
                        return savedAndroidAppVer;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public AndroidAppVersion UpdateAndroidVersionApp(AndroidAppVersion app)
        {
            using (DataContext context = new DataContext())
            {
                AndroidAppVersion av = context.AndroidAppVersion.Single(i => i.AndroidAppVersionId == app.AndroidAppVersionId);
                av.AppName = app.AppName;
                av.PackageName = app.PackageName;
                av.VersionName = app.VersionName;
                av.VersionCode = app.VersionCode;
                av.VersionBuild = app.VersionBuild;
                av.Description = app.Description;
                av.IsActive = app.IsActive;

                context.SaveChanges();

                return av;
            }
        }

        public AndroidAppVersion GetAndroidVersionAppById(int? id)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var app = context.AndroidAppVersion
                              .SingleOrDefault(d => d.AndroidAppVersionId == id);

                    return app;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public AndroidAppVersion GetAndroidVersionAppByPackageName(string name)
        {
            using (DataContext context = new DataContext())
            {
                try
                {
                    var app = context.AndroidAppVersion
                              .SingleOrDefault(d => d.PackageName == name);

                    return app;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public bool IsPackageNameAlreadyExists(string name)
        {
            using (DataContext context = new DataContext())
            {
                return context.AndroidAppVersion.SingleOrDefault(d => d.PackageName == name) == null;
            }
        }
    }
}