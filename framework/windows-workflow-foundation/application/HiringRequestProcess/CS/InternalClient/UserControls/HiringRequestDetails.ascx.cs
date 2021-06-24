//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Contoso.OrgService;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.WebClient
{    
    public partial class HiringRequestDetails : System.Web.UI.UserControl
    {
        public bool Editable { get; set; }

        public HiringRequestInfo HiringRequestInfo { get; set; }

        public string DepartmentId
        {
            get
            {
                if (this.Editable)
                {
                    return this.ddlDepartment.SelectedValue;
                }
                else
                {
                    return this.HiringRequestInfo.DepartmentId;
                }
            }
        }

        public string PositionId
        {
            get
            {
                if (this.Editable)
                {
                    return this.ddlPosition.SelectedValue;
                }
                else
                {
                    return this.HiringRequestInfo.PositionId;
                }
            }
        }

        public string Comments
        {
            get
            {
                return this.txtComments.Text;
            }
        }

        public string Description
        {
            get
            {
                if (this.Editable)
                {
                    return this.txtDescription.Text;
                }
                else
                {
                    return this.HiringRequestInfo.Description;
                }
            }
        }

        public bool ShowCommentsField
        {
            set
            {
                this.trComments.Visible = value;
            }
            get
            {
                return this.trComments.Visible;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PrepareUI();
                this.ShowHiringRequestData();
            }
        }

        public string CreateRequestTitle(Employee employee)
        {
            OrgServiceClient client = new OrgServiceClient();
            string deptName = client.GetDepartment(this.DepartmentId).Name;
            string positionName = client.GetPosition(this.PositionId).Name;
            return string.Format("{0} for {1} created by {2} ({3})", positionName, deptName, employee.Name, DateTime.Now.ToString());
        }

        // set visibility of the controls 
        void PrepareUI()
        {
            this.lblDepartment.Visible = !this.Editable;
            this.ddlDepartment.Visible = this.Editable;

            this.lblPosition.Visible = !this.Editable;
            this.ddlPosition.Visible = this.Editable;

            this.lblDescription.Visible = !this.Editable;
            this.txtDescription.Visible = this.Editable;

            if (this.Editable)
            {
                this.BindDepartments();
                this.BindPositions();
            }
        }

        // show the data of the hiring request
        void ShowHiringRequestData()
        {
            this.lblRequester.Text = GetEmployeeName(this.HiringRequestInfo.RequesterId);
            if (this.Editable)
            {
                if (!string.IsNullOrEmpty(this.HiringRequestInfo.DepartmentId))
                {
                    this.ddlDepartment.Items.FindByValue(this.HiringRequestInfo.DepartmentId).Selected = true;
                }
                if (!string.IsNullOrEmpty(this.HiringRequestInfo.PositionId))
                {
                    this.ddlPosition.Items.FindByValue(this.HiringRequestInfo.PositionId).Selected = true;
                }
                this.txtDescription.Text = this.HiringRequestInfo.Description;
            }
            else
            {
                this.lblPosition.Text = this.GetPositionName(this.HiringRequestInfo.PositionId);
                this.lblDepartment.Text = this.GetDepartmentName(this.HiringRequestInfo.DepartmentId);
                this.lblDescription.Text = this.HiringRequestInfo.Description;
            }
        }

        // bind DropDownList of departments
        void BindDepartments()
        {
            OrgServiceClient client = new OrgServiceClient();
            IList<Department> departments = client.GetAllDepartments().OrderBy(d => d.Name).ToList();
            foreach (Department department in departments)
            {
                this.ddlDepartment.Items.Add(new ListItem(department.Name, department.Id));
            }
            client.Close();
        }

        // bind DropDownList of positions
        void BindPositions()
        {
            OrgServiceClient client = new OrgServiceClient();
            IList<Position> positions = client.GetAllPositions().OrderBy(p => p.Name).ToList();
            foreach (Position position in positions)
            {
                this.ddlPosition.Items.Add(new ListItem(position.Name, position.Id));
            }
            client.Close();
        }

        // get the name and id of the employee
        string GetEmployeeName(string employeeId)
        {
            OrgServiceClient client = new OrgServiceClient();
            Employee emp = client.GetEmployee(employeeId);
            client.Close();
            return string.Format("{0} - {1}", emp.Id, emp.Name);
        }

        // get the name of the department
        string GetDepartmentName(string id)
        {
            OrgServiceClient client = new OrgServiceClient();
            Department dept = client.GetDepartment(id);
            client.Close();
            return dept.Name;
        }

        // get the name of the position
        string GetPositionName(string id)
        {
            OrgServiceClient client = new OrgServiceClient();
            Position position = client.GetPosition(id);
            client.Close();
            return position.Name;
        }
    }
}