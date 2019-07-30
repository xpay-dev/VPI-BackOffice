using SDGDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class SwitchRepository
    {
        public List<Switch> GetAllSwitches(string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<Switch> switches = new List<Entities.Switch>();
            using (DataContext context = new DataContext())
            {
                //var q = context.Switches.Select(s => new { s.SwitchId, s.SwitchName });
                var q = context.Switches.Where(s => s.SwitchId != 0);
                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                switches = q.ToList();
            }
            return switches;
        }

        public List<Switch> GetAllSwitches()
        {
            using (DataContext context = new DataContext())
            {
                return context.Switches.OrderByDescending(u => u.SwitchName).ToList();
            }
        }

        public List<Switch> GetAllSwitchPartnerLink(int partnerId)
        {
            using (DataContext context = new DataContext())
            {
                var qry = context.Switches.GroupJoin(context.SwitchPartnerLinks,
                    sw => sw.SwitchId,
                    spl => spl.SwitchId,
                    (x, y) => new { Switches = x, SwitchPartnerLinks = y })
                    .SelectMany(x => x.SwitchPartnerLinks.DefaultIfEmpty(),
                    (x, y) => new { Switches = x.Switches, SwitchPartnerLinks = y });

                return context.Switches.OrderByDescending(u => u.SwitchName).ToList();
            }
        }

        public List<SwitchPartnerLink> GetAllSwitchesByPartner(int partnerId, string search,
           int pageNumber, int numberOfRecords, string sortString, string sortOrder, out int totalRecords)
        {
            List<SwitchPartnerLink> link = new List<SwitchPartnerLink>();
            using (DataContext context = new DataContext())
            {
                var q = context.SwitchPartnerLinks
                              .Include("Switch")
                              .Where(spl => spl.PartnerId == partnerId);

                totalRecords = q.Count();

                if (sortOrder.ToUpper() == "DESC")
                {
                    q = q.OrderByDescending(sortString).Skip(pageNumber).Take(numberOfRecords);
                }
                else
                {
                    q = q.OrderBy(sortString).Skip(pageNumber).Take(numberOfRecords);
                }

                link = q.ToList();
            }

            return link;
        }

        public List<SwitchPartnerLink> GetAllSwitchPartnerLink()
        {
            using (DataContext context = new DataContext())
            {
                return context.SwitchPartnerLinks
                              .Include("Switch")
                              .OrderByDescending(spl => spl.SwitchPartnerLinkId).ToList();
            }
        }

        public Switch GetSwitchById(int switchId)
        {
            Switch switchess = new Switch();
            using (DataContext context = new DataContext())
            {
                var s = context.Switches.SingleOrDefault(p => p.SwitchId == switchId);

                return s;
            }
        }

        public Switch CreateSwitch(Switch switches)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        switches.DateCreated = DateTime.Now;
                        var savedSwitches = context.Switches.Add(switches);
                        context.SaveChanges();

                        trans.Commit();

                        return savedSwitches;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public SwitchPartnerLink SavePartnerSwitch(SwitchPartnerLink switches)
        {
            using (DataContext context = new DataContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        var savedSwitches = context.SwitchPartnerLinks.Add(switches);
                        context.SaveChanges();

                        trans.Commit();

                        return savedSwitches;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public SwitchPartnerLink UpdatePartnerSwitch(SwitchPartnerLink switches)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(switches).State = EntityState.Modified;
                context.SaveChanges();
                return switches;
            }
        }

        public Switch UpdateSwitchStatus(Switch switches)
        {
            using (DataContext context = new DataContext())
            {
                context.Entry(switches).State = EntityState.Modified;
                context.SaveChanges();
                return switches;
            }
        }

        public bool IsSwitchNameAvailable(string switchname)
        {
            using (DataContext context = new DataContext())
            {
                return context.Switches.SingleOrDefault(sw => sw.SwitchName == switchname) == null;
            }
        }
    }
}