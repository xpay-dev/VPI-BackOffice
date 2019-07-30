using SDGDAL.Entities;
using System;
using System.Linq;

namespace SDGDAL.Repositories
{
    public class DebitSystemTraceNumRepository
    {
        public string GenerateSystemTraceNumber()
        {
            TempSystemTraceNumber tempSystemtraceNumber = new TempSystemTraceNumber();
            string traceNum;
            int i = 1;

            using (DataContext context = new DataContext())
            {
                try
                {
                    var data = context.TempSystemTraceNumber.ToList();

                    if (data.Count() == 0 || data == null)
                    {
                        traceNum = Convert.ToString(i).PadLeft(6, '0');

                        tempSystemtraceNumber.TraceNumber = traceNum;
                        tempSystemtraceNumber.DateCreated = DateTime.Now;

                        var saveTraceNum = context.TempSystemTraceNumber.Add(tempSystemtraceNumber);

                        context.SaveChanges();

                        return traceNum;
                    }
                    else
                    {
                        var q = context.TempSystemTraceNumber.OrderByDescending(x => x.Id).First();

                        if (q.DateCreated.ToString("YYYYMMdd") == DateTime.Now.ToString("YYYYMMdd"))
                        {
                            traceNum = Convert.ToString(Convert.ToInt32(q.TraceNumber) + 1).PadLeft(6, '0');

                            tempSystemtraceNumber.TraceNumber = traceNum;
                            tempSystemtraceNumber.DateCreated = DateTime.Now;

                            var saveTraceNum = context.TempSystemTraceNumber.Add(tempSystemtraceNumber);

                            context.SaveChanges();

                            return traceNum;
                        }
                        else
                        {
                            //Delete old data
                            var oldData = context.TempSystemTraceNumber.ToList();

                            foreach (var t in oldData)
                            {
                                context.TempSystemTraceNumber.Remove(t);
                            }

                            context.SaveChanges();

                            traceNum = Convert.ToString(i).PadLeft(6, '0');

                            tempSystemtraceNumber.TraceNumber = traceNum;
                            tempSystemtraceNumber.DateCreated = DateTime.Now;

                            var saveTraceNum = context.TempSystemTraceNumber.Add(tempSystemtraceNumber);

                            context.SaveChanges();

                            return traceNum;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return null;
        }
    }
}