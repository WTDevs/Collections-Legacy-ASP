using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using CollectionsPortal.Classes;

namespace CollectionsPortal.Admin
{
    public partial class Lock : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetBtnText();
                HidePanels();  
            }
        }

        private void HidePanels()
        {
            //Check user, and hide Admin panels if not and Admin.  Assume that DataLoad and Admin are the only ones that can get here
            //Get User
            string ntLogin = ClsUtil.GetNtUser(Page.User.Identity.Name);

            //Check for ADMIN role
            var access = new ClsAccess();
            var isAdmin = access.GetUserAccess(ntLogin).Any(z=>z.RoleKey == "ADMIN");

            //Set Visibility
            pnlButton.Visible = isAdmin;
            pnlSchedule.Visible = isAdmin;
        }

        private void SetBtnText()
        {
            bool lockStatus = Core.TicketMgr.GetLockStatus("ALL_REPORTS");

            if (lockStatus)
            {

                btnLock.Text = "Disable Global Lock";

            }

            else
            {

                btnLock.Text = "Enable Global Lock";

            }

        }

        protected void btnLock_Click(object sender, EventArgs e)
        {

            //Get User
            string ntLogin = ClsUtil.GetNtUser(Page.User.Identity.Name);

            Core.TicketMgr.SetLockStatus("ALL_REPORTS", ntLogin);

            SetBtnText();

            Session["ForceRefresh"] = "TRUE";

        }

        //Click Event For Button to Create a New Scheduled Lock
        protected void btnSave_Click(object sender, EventArgs e)
        {

            DateTime dt1;

            DateTime dt2;

            //Formatting Start and End Time Variables to build locking schedule
            DateTime.TryParseExact(tbStartTime.Text, "h:mm tt", CultureInfo.InvariantCulture,
                                              DateTimeStyles.None, out dt1);

            DateTime.TryParseExact(tbEndTime.Text, "h:mm tt", CultureInfo.InvariantCulture,
                                              DateTimeStyles.None, out dt2);
            ValidateDates(dt1, dt2);


            if (Page.IsValid)
            {
                string ntLogin = ClsUtil.GetNtUser(Page.User.Identity.Name);

                Core.TicketMgr.SaveLockSchedule(ddlDayOfWeek.SelectedValue, dt1, dt2, ntLogin);

                gvReportList.DataBind();
            }
        }

        //Validating that Start Time is Before End Time to prevent errors
        private void ValidateDates(DateTime startTime, DateTime endTime)
        {
            custValDates.ErrorMessage = "";

            lblSuccess.Visible = false;

            if (startTime < Convert.ToDateTime("1/1/2000"))
            {
                custValDates.IsValid = false;
                custValDates.ErrorMessage = "Start Time is Invalid";                
            }

            if(endTime < Convert.ToDateTime("1/1/2000"))
            {
                custValDates.IsValid = false;
                custValDates.ErrorMessage = "End Time is Invalid";                
            }

            if (startTime < Convert.ToDateTime("1/1/2000") && endTime < Convert.ToDateTime("1/1/2000"))
            {
                custValDates.IsValid = false;
                custValDates.ErrorMessage = "Start and End Times are Invalid";                
            }

            if (IsValid)
            {
                if (startTime >= endTime)
                {
                    custValDates.IsValid = false;
                    custValDates.ErrorMessage += "Start Time must be before End Time";
                }
                else
                {                    
                    lblSuccess.Visible = true;
                    tbStartTime.Text = "";
                    tbEndTime.Text = "";
                }
            }
        }

        //Click event to lock an individual report
        protected void btnLockByReport_Click(object sender, EventArgs e)
        {

            Button btnSend = sender as Button;

            GridViewRow row = btnSend.NamingContainer as GridViewRow;

            lockByReport(row);

        }

        //Click event to unlock an individual report
        protected void btnUnlockByReport_Click(object sender, EventArgs e)
        {

            Button btnSend = sender as Button;

            GridViewRow rowConvert = btnSend.NamingContainer as GridViewRow;

            lockByReport(rowConvert);

        }

        //Method to lock reports and validate user's authorization through NtLogin
        protected void lockByReport(GridViewRow row)
        {

            string ntLogin = ClsUtil.GetNtUser(Page.User.Identity.Name);

            string reportCode = gvLockByReport.DataKeys[row.RowIndex].Value.ToString();


            Core.TicketMgr.SetLockStatus(reportCode, ntLogin);


            bool reportLockStatus = Core.TicketMgr.GetLockStatus(reportCode);

            gvLockByReport.DataBind();

        }

        protected void gvLockByReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Get the Row
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                //Get the button objeccts, both should be disabled
                Button btnLock = (Button)e.Row.FindControl("btnLockByReport");
                Button btnUnLock = (Button)e.Row.FindControl("btnUnlockByReport");

                btnUnLock.ForeColor = System.Drawing.Color.Black;
                btnUnLock.BackColor = System.Drawing.Color.LightGray;

                btnLock.ForeColor = System.Drawing.Color.Black;
                btnLock.BackColor = System.Drawing.Color.LightGray;

                //Check the IsActive for 1
                if (Convert.ToInt32(rowView["isLocked"].ToString()) > 0)
                {
                    //Set the UnLock to Enabled
                    btnUnLock.Enabled = true;

                    btnLock.Text = "Locked";
                    btnLock.BackColor = System.Drawing.Color.Salmon;
                    btnUnLock.BackColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    //else, set the Lock to Enabled
                    btnLock.Enabled = true;

                    btnUnLock.Text = "Unlocked";
                    btnLock.BackColor = System.Drawing.Color.LightGray;
                    btnUnLock.BackColor = System.Drawing.Color.LightGreen;
                }
            }
        }

        protected void scheduleOn_Click(object sender, EventArgs e)
        {

            Button btnSend = sender as Button;

            GridViewRow row = btnSend.NamingContainer as GridViewRow;

            bool onOrOff = true;

            TurnScheduleOnOff(row, onOrOff);

            gvReportList.DataBind();

        }

        protected void scheduleOff_Click(object sender, EventArgs e)
        {

            Button btnSend = sender as Button;

            GridViewRow row = btnSend.NamingContainer as GridViewRow;

            bool onOrOff = false;

            TurnScheduleOnOff(row, onOrOff);

            gvReportList.DataBind();

        }

        //Logic to flip a locking schedule's on/off status
        protected void TurnScheduleOnOff(GridViewRow row, bool turnOnOrOff)
        {
            var id = Convert.ToInt32(gvReportList.DataKeys[row.RowIndex].Value.ToString());

            using (CollectionsEntities entity = new CollectionsEntities())
            {
                var rec = entity.ReportLockSchedules.Find(id);
                string ntLogin = ClsUtil.GetNtUser(Page.User.Identity.Name);

                rec.IsActive = turnOnOrOff;
                rec.UpdatedOn = DateTime.Now;
                rec.UpdatedBy = ntLogin;
                entity.SaveChanges();

            }
        }

        protected void gvReportList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                //Get the button objeccts, both should be disabled
                Button btnOn = (Button)e.Row.FindControl("scheduleOn");
                Button btnOff = (Button)e.Row.FindControl("scheduleOff");

                btnOn.ForeColor = System.Drawing.Color.Black;
                btnOn.BackColor = System.Drawing.Color.LightGray;

                btnOff.ForeColor = System.Drawing.Color.Black;
                btnOff.BackColor = System.Drawing.Color.LightGray;

                btnOn.Enabled = false;
                btnOff.Enabled = false;

                //Check the IsActive for 1
                if (Convert.ToBoolean(rowView["IsActive"].ToString()))
                {
                    //Set the UnLock to Enabled
                    btnOff.Enabled = true;

                    btnOff.BackColor = System.Drawing.Color.LightGray;
                    btnOn.BackColor = System.Drawing.Color.LightGreen;
                }
                else
                {
                    //else, set the Lock to Enabled
                    btnOn.Enabled = true;

                    btnOff.BackColor = System.Drawing.Color.Salmon;
                    btnOn.BackColor = System.Drawing.Color.LightGray;
                }
            }
        }

        public string FormatTime(TimeSpan timeSpan)
        {
            //Convert TimeSpan to a formatted Date
            string hh, mm, ap;
            hh = timeSpan.Hours.ToString();
            mm = timeSpan.Minutes.ToString();
            ap = "AM";

            if (timeSpan.Hours > 12)
            {
                hh =( timeSpan.Hours - 12).ToString();
                ap = "PM";
            }
            else if (timeSpan.Hours == 12)
            {
                hh = timeSpan.Hours.ToString();
                ap = "PM";
            }
            else if (timeSpan.Hours < 1)
            {
                hh = (timeSpan.Hours + 12).ToString();
            }

            return hh + ":" + mm.PadLeft(2,'0') + " " + ap ;
        }

        //Changing checked status for Locked/Unlocked Checkboxes to ensure only one is checked at a time
        protected void cbIsLocked_CheckedChanged(object sender, EventArgs e)
        {
            cbUnlocked.Checked = false;
        }

        protected void cbUnlocked_CheckedChanged(object sender, EventArgs e)
        {
            cbIsLocked.Checked = false;
        }


        //Filter Reports Table to show only locked/unlocked reports
        protected void DS_LockByReport_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            DS_LockByReport.FilterExpression = string.Empty;

            if (cbIsLocked.Checked)
            {
                DS_LockByReport.FilterExpression += "(IsLocked=1)";
            }

            if (cbUnlocked.Checked)
            {
                DS_LockByReport.FilterExpression += "(IsLocked=0)";
            }
        }

        //Changing checked status for On/Off Checkboxes to ensure only one is checked at a time
        protected void cb_ScheduleOn_CheckedChanged(object sender, EventArgs e)
        {
            cb_ScheduleOff.Checked = false;
        }

        protected void cb_ScheduleOff_CheckedChanged(object sender, EventArgs e)
        {
            cb_ScheduleOn.Checked = false;
        }


        //Filter Schedule Table to only show active or inactive Schedules
        protected void DS_Reports_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
            DS_Reports.FilterExpression = string.Empty;

            if (cb_ScheduleOn.Checked)
            {
                DS_Reports.FilterExpression += "IsActive=1";
            }

            if (cb_ScheduleOff.Checked)
            {
                DS_Reports.FilterExpression += "IsActive=0";
            }
        }
    }
}