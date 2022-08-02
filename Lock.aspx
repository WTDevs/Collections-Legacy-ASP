<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lock.aspx.cs" Inherits="CollectionsPortal.Admin.Lock"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
            .button {
  background-color: darkgray;  /* #4CAF50;  Green */
  border: none;
  color: white;
  padding: 8px 8px;
  text-align: center;
  text-decoration: none;
  display: inline-block;
  font-size: 12px; font-weight: 500;

border-radius: 8px;
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <!-- Adding Button Control to perform 'Global Lock' Function (Lock/Unlock all reports at once) -->
    <asp:Panel runat="server" ID="pnlButton" GroupingText="Global Lock">
        <asp:Button ID="btnLock" runat="server" OnClick="btnLock_Click" Text="Lock" CssClass="button" />
        <asp:Label runat="server" Text-="***Enabling the global lock will restrict the ability to work all new tickets for all reports in all categories in all regions UNTIL IT IS DISABLED. It will override any scheduled unlocks.***" ForeColor="Red" />

    </asp:Panel>
    &nbsp;

    <!-- Adding Controls to take input for scheduled locks. Schedules are based on a Day of the Week, Start Time, and End Time. -->
    <asp:Panel runat="server" ID="pnlSchedule" GroupingText="Lock Schedule">
        <div runat="server" id="divScheduleForm">

            <!-- Creating DropDownList Control to Select Day of Week -->
            <label>Day of Week</label>
            <asp:DropDownList runat="server" ID="ddlDayOfWeek">
                <asp:ListItem Value="0" Text="Sunday" />
                <asp:ListItem Value="1" Text="Monday" />
                <asp:ListItem Value="2" Text="Tuesday" />
                <asp:ListItem Value="3" Text="Wednesday" />
                <asp:ListItem Value="4" Text="Thursday" />
                <asp:ListItem Value="5" Text="Friday" />
                <asp:ListItem Value="6" Text="Saturday" />
            </asp:DropDownList>

            <!-- Start Time Input with AM/PM Functionality -->
            <label class="fieldLabel">Start Time:</label>

            <asp:TextBox runat="server" ID="tbStartTime" Width="90px" />
            <asp:MaskedEditExtender TargetControlID="tbStartTime" MaskType="Time" runat="server" ID="ftbeStartTime" ClearTextOnInvalid="true" Mask="99:99" AcceptAMPM="true" />
            <asp:RequiredFieldValidator runat="server" ID="rfvStartTime" ControlToValidate="tbStartTime" CssClass="failureNotification"
                Display="Dynamic" ErrorMessage="Start Time is required" ValidationGroup="valUpdate" />

            <!-- End Time Input with AM/PM Functionality -->
            <label class="fieldLabel">End Time:</label>

            <asp:TextBox runat="server" ID="tbEndTime" Width="90px" />
            <asp:MaskedEditExtender TargetControlID="tbEndTime" MaskType="Time" runat="server" ID="MaskedEditExtender1" ClearTextOnInvalid="true" Mask="99:99" AcceptAMPM="true" />
            <asp:RequiredFieldValidator runat="server" ID="rfvEndTime" ControlToValidate="tbEndTime" CssClass="failureNotification"
                Display="Dynamic" ErrorMessage="End Time is required" ValidationGroup="valUpdate" />
                        
            <!-- Button Control to Save Schedule -->
            <asp:Button runat="server" ID="btnSave" Text="Create Schedule" OnClick="btnSave_Click" ValidationGroup="valUpdate" CssClass="button" />
            <asp:CustomValidator runat="server" ID="custValDates" ControlToValidate="tbStartTime" CssClass="failureNotification"
                Display="Dynamic" ErrorMessage="Error: Invalid Times. Please check times and try again." ValidationGroup="valUpdate" />
            <span id="spanSuccessMsg">
                <asp:Label runat="server" ID="lblSuccess" Visible="false" ForeColor="Green" Font-Size="Large" >Schedule Successfully Created</asp:Label>
            </span>
        </div>
        <br />
        <!-- Controls to filter between schedules that are on/off -->
        Show On: <asp:CheckBox runat="server" ID="cb_ScheduleOn" Checked="false" AutoPostBack="true" OnCheckedChanged="cb_ScheduleOn_CheckedChanged" />        
        Show Off: <asp:CheckBox runat="server" ID="cb_ScheduleOff" Checked="false" AutoPostBack="true" OnCheckedChanged="cb_ScheduleOff_CheckedChanged" />
        
        <!-- Gridview showing all locking schedules, as well as their on/off status and controls -->
        <asp:GridView runat="server" ID="gvReportList" DataKey="ID" AutoGenerateColumns="false"
            DataSourceID="DS_Reports" DataKeyNames="ID" OnRowDataBound="gvReportList_RowDataBound"
            AllowSorting="true" CellPadding="5">
            <Columns>                
                <asp:BoundField Visible="false" DataField="Id" HeaderText="Id" SortExpression="Id" />
                <asp:TemplateField ShowHeader="false">
                    <ItemTemplate>
                        <asp:Button ID="scheduleOn" runat="server" Text="On" OnClick="scheduleOn_Click" CssClass="button" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:Button ID="scheduleOff" runat="server" Text="Off" OnClick="scheduleOff_Click" CssClass="button" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="NameOfDay" HeaderText="Day of Week" SortExpression="DayOfWeek" />
                <asp:BoundField DataField="StartTime" HeaderText="StartTime" SortExpression="StartTime" Visible="false" />
               <asp:TemplateField HeaderText="Shutdown Start" SortExpression="StartTime" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblStartTime" Text='<%# FormatTime((TimeSpan)(Eval("StartTime"))) %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EndTime" HeaderText="EndTime" SortExpression="EndTime" Visible="false" />
                <asp:TemplateField HeaderText="Shutdown End" SortExpression="EndTime" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblEndTime" Text='<%# FormatTime((TimeSpan)(Eval("EndTime"))) %>' />
                    </ItemTemplate>
                </asp:TemplateField> <asp:BoundField DataField="UpdatedOn" HeaderText="Updated On" DataFormatString="{0:d}" SortExpression="UpdatedOn" />
                <asp:BoundField DataField="UpdatedBy" HeaderText="Updated By" SortExpression="UpdatedBy" />
                <asp:BoundField DataField="IsActive" Visible="false" />
                <asp:CommandField ButtonType="Link" SelectText="Edit" ShowSelectButton="true" Visible="false" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:ImageButton ID="DeleteButton" runat="server" ImageUrl="~/Images/trash.png"
                            CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this schedule?');"
                            AlternateText="Delete" />
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>

    </asp:Panel>

    <asp:Panel runat="server" ID="pnlReports" GroupingText="Reports">
        <asp:Label runat="server" Text="***A locked report will restrict the ability to work all new tickets for that report even if the portal is unlocked.
            An unlocked report will support the ability to work all new tickets for that report when the portal is unlocked.***" ForeColor="Red" />
        <%--<div style="width: 500px; height: 400px; overflow: scroll; overflow-x: hidden">  --%>          
        <div>            
            <!-- Controls to filter between reports that are locked/unlocked -->
            Show Unlocked: <asp:CheckBox runat="server" ID="cbUnlocked" Checked="false" AutoPostBack="true" OnCheckedChanged="cbUnlocked_CheckedChanged" />
            Show Locked: <asp:CheckBox runat="server" ID="cbIsLocked" Checked="false" AutoPostBack="true" OnCheckedChanged="cbIsLocked_CheckedChanged" />

            <!-- Gridview showing all reports, as well as their lock status and controls to lock/unlock -->
            <asp:GridView runat="server" ID="gvLockByReport" DataKey="ReportCode" AutoGenerateColumns="false"
                DataSourceID="DS_LockByReport" DataKeyNames="ReportCode" OnRowDataBound="gvLockByReport_RowDataBound"
                AllowSorting="true"  CellPadding="5">
                <Columns>
                    <asp:TemplateField ShowHeader="false">
                        <ItemTemplate>
                            <asp:Button runat="server" ID="btnUnlockByReport" Text="Unlock" CommandName="setInitBtn" CommandArgument="setInitBtn" OnClick="btnUnlockByReport_Click" Enabled="false" Width="100%" CssClass="button" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="false">
                        <ItemTemplate>
                            <asp:Button runat="server" ID="btnLockByReport" Text="Lock" CommandName="Lock" CommandArgument="ReportUnlock" OnClick="btnLockByReport_Click" Enabled="false" Width="100%"  CssClass="button" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ReportCode" HeaderText="Report Code" SortExpression="ReportCode" />
                    <asp:BoundField DataField="ReportName" HeaderText="Report Name" SortExpression="ReportName" ControlStyle-BorderWidth="200" />                    
                    <asp:BoundField DataField="isLocked" HeaderText="isLocked" SortExpression="isLocked" Visible="false"/>
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>


    <asp:SqlDataSource ID="DS_Reports" runat="server" ConnectionString="<%$ ConnectionStrings:connApp %>"
        DeleteCommand="delete ReportLockSchedule WHERE ID = @ID"
        SelectCommand="select [Id], [DayOfWeek], [StartTime], [EndTime], [UpdatedOn], [UpdatedBy], [IsActive]

,CASE WHEN [DayOfWeek] = 0 THEN 'Sunday'
WHEN [DayOfWeek] = 1 THEN 'Monday'
WHEN [DayOfWeek] = 2 THEN 'Tuesday'
WHEN [DayOfWeek] = 3 THEN 'Wednesday'
WHEN [DayOfWeek] = 4 THEN 'Thursday'
WHEN [DayOfWeek] = 5 THEN 'Friday'

ELSE 'Saturday'

End AS NameOfDay

FROM ReportLockSchedule ORDER BY DayOfWeek, StartTime"
        OnSelecting="DS_Reports_Selecting">
        
        <FilterParameters>
            <asp:ControlParameter ControlID="cb_ScheduleOn" DefaultValue="1" Type="Int32" Name="IsActive" />
            <asp:ControlParameter ControlID="cb_ScheduleOff" DefaultValue="0" Type="Int32" Name="IsActive" />
        </FilterParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="DS_LockByReport" runat="server" ConnectionString="<%$ ConnectionStrings:connApp %>"
        DeleteCommand="delete FROM ReportLock WHERE ReportCode=ID"
        SelectCommand="select r.ReportCode, r.ReportName, rL.UpdatedOn, rL.UpdatedBy, CASE WHEN rL.ReportCode IS NULL THEN 0 ELSE 1 END AS isLocked

FROM ReportTbl AS r LEFT JOIN ReportLock AS rL ON r.ReportCode=rL.ReportCode WHERE GETDATE() BETWEEN r.EffDate AND r.ExpDate
ORDER BY ReportCode, ReportName"
        OnSelecting="DS_LockByReport_Selecting">
        
        <FilterParameters>
            <asp:ControlParameter ControlID="cbIsLocked" DefaultValue="1" Type="Int32" Name="IsLocked" />
            <asp:ControlParameter ControlID="cbUnlocked" DefaultValue="0" Type="Int32" Name="Unlocked" />
        </FilterParameters>

    </asp:SqlDataSource>

<script type="text/javascript">
    setTimeout(function () {
        $('#spanSuccessMsg').fadeOut('fast');
    }, 3000); // <-- time in milliseconds
</script>


</asp:Content>
