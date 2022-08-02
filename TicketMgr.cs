using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Globalization;

namespace CollectionsPortal.Core
{
    public static class TicketMgr
    {
        internal static bool GetLockStatus(string reportCode)
        {
            using (CollectionsEntities entity = new CollectionsEntities())
            {
                //Set temp variable
                bool lockStatus = false;

                //Check reportLock table for lock status
                lockStatus = entity.ReportLocks.Any(x => x.ReportCode == reportCode);


                return lockStatus;

            }
        }

        internal static bool CheckLockStatus(string reportCode)
        {
            //Set temp variable
            bool lockStatus = false;

            //Check Global Lock
            lockStatus = GetLockStatus("All_REPORTS");

            //Check Schedule Lock
            using (CollectionsEntities entity = new CollectionsEntities())
            {

                DateTime initTime = DateTime.Now;

                if (!lockStatus)
                {

                    lockStatus = entity.ReportLockSchedules.Any(x => x.IsActive
                    && x.StartTime <= initTime.TimeOfDay
                    && x.EndTime >= initTime.TimeOfDay
                    && x.DayOfWeek == (int)initTime.DayOfWeek);

                }

            }

            //Check Report Lock
            if (!lockStatus)
            {

                lockStatus = GetLockStatus(reportCode);

            }

            return lockStatus;

        }

        internal static void SetLockStatus(string reportCode, string ntLogin)
        {
            using (CollectionsEntities entity = new CollectionsEntities())
            {

                //Checking if report is currently locked to determine next logical step
                if (GetLockStatus(reportCode))
                {

                    //Removing ReportLock Record to Unlock Report if currently locked
                    var rec = entity.ReportLocks.Find(reportCode);

                    entity.ReportLocks.Remove(rec);

                    entity.SaveChanges();

                }

                else
                {

                    //Adding ReportLock to Lock Report if Currently Unlocked
                    ReportLock rec = new ReportLock()
                    {
                        ReportCode = reportCode,
                        UpdatedBy = ntLogin,
                        UpdatedOn = DateTime.Now
                    };

                    entity.ReportLocks.Add(rec);

                    entity.SaveChanges();

                }

            }
        }

                
        //Get List of Lock/Unlock Status
        internal static List<ReportLock> GetAllStatus()
        {
            using (CollectionsEntities entity = new CollectionsEntities())
            {

                return entity.ReportLocks.ToList();

            }
        }

        //Saving lock schedule based on UI parameters
        internal static void SaveLockSchedule(string dayOfWeek, DateTime startTime, DateTime endTime, string ntLogin)
        {

            using (CollectionsEntities entity = new CollectionsEntities())
            {


                ReportLockSchedule rec = new ReportLockSchedule()
                {
                    DayOfWeek = Convert.ToInt32(dayOfWeek),
                    StartTime = startTime.TimeOfDay,
                    EndTime = endTime.TimeOfDay,
                    UpdatedBy = ntLogin,
                    UpdatedOn = DateTime.Now,
                    IsActive = true
                };

                entity.ReportLockSchedules.Add(rec);

                entity.SaveChanges();

            }
        }

        //Method to Get a new ticket, and fail back to menu screen if reports have been locked during the session
        internal static int GetNextTicket(string rptCode, string ntLogin, string cycleDay, int regionID, DateTime runDate)
        {

            if (CheckLockStatus(rptCode))
            {

                return -2;

            }

            using (CollectionsEntities entity = new CollectionsEntities())
            {
                var rec = (entity.GetNextTicket_V2(rptCode, ntLogin, cycleDay, regionID, runDate, 1)).FirstOrDefault();

                if (rec == null || rec == 0)
                    return -1;
                else
                    return rec.Value;
            }
        }

    }
}