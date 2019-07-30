using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class MerchantOnBoardRepository
    {
        public MerchantOnBoardRequest SaveRequestFile(MerchantOnBoardRequest request)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        request.DateCreated = DateTime.Now;
                        request.IsUpdated = false;

                        var savedFile = context.MerchantOnBoardRequest.Add(request);
                        context.SaveChanges();

                        trans.Commit();

                        return savedFile;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public MerchantOnBoardResponse SaveResponseFile(MerchantOnBoardResponse file)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        file.IsActive = true;
                        file.DateReceived = file.DateReceived;
                        var data = context.MerchantOnBoardResponse.Add(file);

                        context.SaveChanges();
                        trans.Commit();

                        return data;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public MerchantOnBoardResponseLink CreateMerchantOnBoardLink(MerchantOnBoardResponseLink data)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        data.IsDeleted = false;
                        var savedLink = context.MerchantOnBoardResponseLink.Add(data);

                        context.SaveChanges();

                        trans.Commit();

                        return savedLink;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public List<MerchantOnBoardResponse> GetAllMerchantOnBoardResponse(string search, int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<MerchantOnBoardResponse> response = new List<Entities.MerchantOnBoardResponse>();

            using (DataContext context = new DataContext())
            {
                var qry = context.MerchantOnBoardResponse
                          .Where(m => m.IsActive == true && m.ResponseFileName.Contains(search));

                totalRecords = qry.Count();

                if (sortOrder.ToUpper() == "ASC")
                {
                    qry = qry.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    qry = qry.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                response = qry.ToList();
            }

            return response;
        }

        public MerchantOnBoardResponse GetResponseFileById(int id)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    var data = context.MerchantOnBoardResponse.SingleOrDefault(m => m.MerchantOnBoardResponseId == id);

                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MerchantOnBoardResponseLink> GetAllMerchantOnBoardResponseLinkByMiscText(string misctext)
        {
            List<MerchantOnBoardResponseLink> result = new List<MerchantOnBoardResponseLink>();

            using (DataContext context = new DataContext())
            {
                var data = context.MerchantOnBoardResponseLink
                           .Include("MerchantOnBoardRequest")
                           .Where(m => m.MerchantOnBoardRequest.MiscText == misctext);

                result = data.ToList();
            }

            return result;
        }

        public bool IsFilenameExists(string filename)
        {
            using (DataContext context = new DataContext())
            {
                return context.MerchantOnBoardResponse.SingleOrDefault(m => m.ResponseFileName == filename) == null;
            }
        }

        public MerchantOnBoardResponse UpdateMerchantOnBoardResponseStatus(MerchantOnBoardResponse res)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        MerchantOnBoardResponse m = context.MerchantOnBoardResponse.Single(i => i.MerchantOnBoardResponseId == res.MerchantOnBoardResponseId);
                        m.IsActive = res.IsActive;

                        context.SaveChanges();

                        trans.Commit();

                        return m;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}