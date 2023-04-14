namespace FlexGridShowcaseDemo;

partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        this._ribbon = new C1.Win.Ribbon.C1Ribbon();
        this._ribbonApplicationMenu = new C1.Win.Ribbon.RibbonApplicationMenu();
        this._ribbonBottomToolBar = new C1.Win.Ribbon.RibbonBottomToolBar();
        this._ribbonConfigToolBar = new C1.Win.Ribbon.RibbonConfigToolBar();
        this._ribbonQat = new C1.Win.Ribbon.RibbonQat();
        this._ribbonTabData = new C1.Win.Ribbon.RibbonTab();
        this._ribbonGroupData = new C1.Win.Ribbon.RibbonGroup();
        this._ribbonComboBoxDataSize = new C1.Win.Ribbon.RibbonComboBox();
        this._ribbonSeparatorData = new C1.Win.Ribbon.RibbonSeparator();
        this._ribbonToolBarSearch = new C1.Win.Ribbon.RibbonToolBar();
        this._ribbonTextBoxSearch = new C1.Win.Ribbon.RibbonTextBox();
        this._ribbonButtonFilter = new C1.Win.Ribbon.RibbonButton();
        this._ribbonSeparatorGroup = new C1.Win.Ribbon.RibbonSeparator();
        this._ribbonMenuFormatting = new C1.Win.Ribbon.RibbonMenu();
        this._ribbonMenuColumns = new C1.Win.Ribbon.RibbonMenu();
        this._ribbonGroupGroup = new C1.Win.Ribbon.RibbonGroup();
        this._ribbonCheckBoxGroupByCountry = new C1.Win.Ribbon.RibbonCheckBox();
        this._ribbonCheckBoxGroupByProduct = new C1.Win.Ribbon.RibbonCheckBox();
        this._ribbonTopToolBar = new C1.Win.Ribbon.RibbonTopToolBar();
        this._ribbonComboBoxThemes = new C1.Win.Ribbon.RibbonComboBox();
        this._themeController = new C1.Win.Themes.C1ThemeController();
        this._flexGrid = new C1.Win.FlexGrid.C1FlexGrid();
        this._tooltip = new C1.Win.SuperTooltip.C1SuperTooltip(this.components);
        this._errorProvider = new C1.Win.SuperTooltip.C1SuperErrorProvider(this.components);
        ((System.ComponentModel.ISupportInitialize)(this._ribbon)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this._themeController)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this._flexGrid)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
        this.SuspendLayout();
        // 
        // _ribbon
        // 
        this._ribbon.ApplicationMenuHolder = this._ribbonApplicationMenu;
        this._ribbon.BottomToolBarHolder = this._ribbonBottomToolBar;
        this._ribbon.ConfigToolBarHolder = this._ribbonConfigToolBar;
        this._ribbon.Location = new System.Drawing.Point(0, 0);
        this._ribbon.Margin = new System.Windows.Forms.Padding(4);
        this._ribbon.Name = "_ribbon";
        this._ribbon.QatHolder = this._ribbonQat;
        this._ribbon.Size = new System.Drawing.Size(1121, 105);
        this._ribbon.Tabs.Add(this._ribbonTabData);
        this._themeController.SetTheme(this._ribbon, "(default)");
        this._ribbon.ToolTipSettings.BackColor = System.Drawing.Color.White;
        this._ribbon.ToolTipSettings.BackgroundGradient = C1.Win.Ribbon.ToolTipGradient.None;
        this._ribbon.ToolTipSettings.Border = true;
        this._ribbon.ToolTipSettings.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
        this._ribbon.ToolTipSettings.Font = new System.Drawing.Font("Segoe UI", 9.75F);
        this._ribbon.ToolTipSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
        this._ribbon.ToolTipSettings.InitialDelay = 900;
        this._ribbon.ToolTipSettings.ReshowDelay = 180;
        this._ribbon.ToolTipSettings.Shadow = false;
        this._ribbon.TopToolBarHolder = this._ribbonTopToolBar;
        this._ribbon.ViewMode = C1.Win.Ribbon.ViewMode.Simplified;
        // 
        // _ribbonApplicationMenu
        // 
        this._ribbonApplicationMenu.Name = "_ribbonApplicationMenu";
        this._ribbonApplicationMenu.Visible = false;
        // 
        // _ribbonBottomToolBar
        // 
        this._ribbonBottomToolBar.Name = "_ribbonBottomToolBar";
        this._ribbonBottomToolBar.Visible = false;
        // 
        // _ribbonConfigToolBar
        // 
        this._ribbonConfigToolBar.Name = "_ribbonConfigToolBar";
        this._ribbonConfigToolBar.Visible = false;
        // 
        // _ribbonQat
        // 
        this._ribbonQat.Name = "_ribbonQat";
        // 
        // _ribbonTabData
        // 
        this._ribbonTabData.Groups.Add(this._ribbonGroupData);
        this._ribbonTabData.Groups.Add(this._ribbonGroupGroup);
        this._ribbonTabData.Name = "_ribbonTabData";
        this._ribbonTabData.Text = "Data";
        // 
        // _ribbonGroupData
        // 
        this._ribbonGroupData.Items.Add(this._ribbonComboBoxDataSize);
        this._ribbonGroupData.Items.Add(this._ribbonSeparatorData);
        this._ribbonGroupData.Items.Add(this._ribbonToolBarSearch);
        this._ribbonGroupData.Items.Add(this._ribbonSeparatorGroup);
        this._ribbonGroupData.Items.Add(this._ribbonMenuFormatting);
        this._ribbonGroupData.Items.Add(this._ribbonMenuColumns);
        this._ribbonGroupData.Name = "_ribbonGroupData";
        this._ribbonGroupData.Text = "Data";
        // 
        // _ribbonComboBoxDataSize
        // 
        this._ribbonComboBoxDataSize.DropDownStyle = C1.Win.Ribbon.RibbonComboBoxStyle.DropDownList;
        this._ribbonComboBoxDataSize.Label = "Data Size:";
        this._ribbonComboBoxDataSize.MaxLength = 300;
        this._ribbonComboBoxDataSize.Name = "_ribbonComboBoxDataSize";
        this._ribbonComboBoxDataSize.TextAreaWidth = 120;
        this._ribbonComboBoxDataSize.SelectedIndexChanged += new System.EventHandler(this._ribbonComboBoxDataSize_SelectedIndexChanged);
        // 
        // _ribbonSeparatorData
        // 
        this._ribbonSeparatorData.Name = "_ribbonSeparatorData";
        // 
        // _ribbonToolBarSearch
        // 
        this._ribbonToolBarSearch.Items.Add(this._ribbonTextBoxSearch);
        this._ribbonToolBarSearch.Items.Add(this._ribbonButtonFilter);
        this._ribbonToolBarSearch.Name = "_ribbonToolBarSearch";
        // 
        // _ribbonTextBoxSearch
        // 
        this._ribbonTextBoxSearch.Label = "Search:";
        this._ribbonTextBoxSearch.Name = "_ribbonTextBoxSearch";
        this._ribbonTextBoxSearch.TextChanged += new System.EventHandler(this._ribbonTextBoxSearch_TextChanged);
        this._ribbonTextBoxSearch.ChangeCommitted += new System.EventHandler(this._ribbonTextBoxSearch_ChangeCommitted);
        // 
        // _ribbonButtonFilter
        // 
        this._ribbonButtonFilter.Name = "_ribbonButtonFilter";
        this._ribbonButtonFilter.Click += new System.EventHandler(this._ribbonButtonFilter_Click);
        // 
        // _ribbonSeparatorGroup
        // 
        this._ribbonSeparatorGroup.Name = "_ribbonSeparatorGroup";
        // 
        // _ribbonMenuFormatting
        // 
        this._ribbonMenuFormatting.Name = "_ribbonMenuFormatting";
        this._ribbonMenuFormatting.Text = "Conditional Formatting";
        // 
        // _ribbonMenuColumns
        // 
        this._ribbonMenuColumns.Name = "_ribbonMenuColumns";
        this._ribbonMenuColumns.Text = "Columns";
        // 
        // _ribbonGroupGroup
        // 
        this._ribbonGroupGroup.Items.Add(this._ribbonCheckBoxGroupByCountry);
        this._ribbonGroupGroup.Items.Add(this._ribbonCheckBoxGroupByProduct);
        this._ribbonGroupGroup.Name = "_ribbonGroupGroup";
        this._ribbonGroupGroup.Text = "Group";
        // 
        // _ribbonCheckBoxGroupByCountry
        // 
        this._ribbonCheckBoxGroupByCountry.Name = "_ribbonCheckBoxGroupByCountry";
        this._ribbonCheckBoxGroupByCountry.Text = "Group by Country";
        this._ribbonCheckBoxGroupByCountry.CheckedChanged += new System.EventHandler(this._ribbonCheckBoxGroupByCountry_CheckedChanged);
        // 
        // _ribbonCheckBoxGroupByProduct
        // 
        this._ribbonCheckBoxGroupByProduct.Name = "_ribbonCheckBoxGroupByProduct";
        this._ribbonCheckBoxGroupByProduct.Text = "Group by Product";
        this._ribbonCheckBoxGroupByProduct.CheckedChanged += new System.EventHandler(this._ribbonCheckBoxGroupByProduct_CheckedChanged);
        // 
        // _ribbonTopToolBar
        // 
        this._ribbonTopToolBar.Items.Add(this._ribbonComboBoxThemes);
        this._ribbonTopToolBar.Name = "_ribbonTopToolBar";
        // 
        // _ribbonComboBoxThemes
        // 
        this._ribbonComboBoxThemes.DropDownStyle = C1.Win.Ribbon.RibbonComboBoxStyle.DropDownList;
        this._ribbonComboBoxThemes.Label = "Theme:";
        this._ribbonComboBoxThemes.Name = "_lstThemes";
        this._ribbonComboBoxThemes.TextAreaWidth = 120;
        this._ribbonComboBoxThemes.SelectedIndexChanged += new System.EventHandler(this._ribbonComboBoxThemes_SelectedIndexChanged);
        // 
        // _themeController
        // 
        this._themeController.ObjectThemeApplied += new C1.Win.Themes.ObjectThemeEventHandler(this._themeController_ObjectThemeApplied);
        // 
        // _flexGrid
        // 
        this._flexGrid.AllowFiltering = true;
        this._flexGrid.BackColor = System.Drawing.Color.White;
        this._flexGrid.ColumnInfo = "10,1,0,0,0,-1,Columns:";
        this._flexGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        this._flexGrid.DrawMode = C1.Win.FlexGrid.DrawModeEnum.OwnerDraw;
        this._flexGrid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
        this._flexGrid.Location = new System.Drawing.Point(0, 105);
        this._flexGrid.Margin = new System.Windows.Forms.Padding(4);
        this._flexGrid.Name = "_flexGrid";
        this._flexGrid.ShowErrors = true;
        this._flexGrid.ShowThemedHeaders = C1.Win.FlexGrid.ShowThemedHeadersEnum.None;
        this._flexGrid.Size = new System.Drawing.Size(1121, 708);
        this._flexGrid.StyleInfo = resources.GetString("_flexGrid.StyleInfo");
        this._flexGrid.TabIndex = 1;
        this._themeController.SetTheme(this._flexGrid, "(default)");
        this._flexGrid.Tree.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
        this._flexGrid.GridChanged += new C1.Win.FlexGrid.GridChangedEventHandler(this._flexGrid_GridChanged);
        this._flexGrid.OwnerDrawCell += new C1.Win.FlexGrid.OwnerDrawCellEventHandler(this._flexGrid_OwnerDrawCell);
        // 
        // _tooltip
        // 
        this._tooltip.BackgroundGradient = C1.Win.SuperTooltip.BackgroundGradient.None;
        this._tooltip.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
        this._tooltip.Font = new System.Drawing.Font("Tahoma", 8F);
        this._tooltip.RightToLeft = System.Windows.Forms.RightToLeft.Inherit;
        this._themeController.SetTheme(this._tooltip, "(default)");
        // 
        // _errorProvider
        // 
        this._errorProvider.ContainerControl = this;
        this._themeController.SetTheme(this._errorProvider, "(default)");
        this._errorProvider.ToolTip = this._tooltip;
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1121, 813);
        this.Controls.Add(this._flexGrid);
        this.Controls.Add(this._ribbon);
        this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
        this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
        this.Margin = new System.Windows.Forms.Padding(4);
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "C1FlexGrid: Showcase";
        this._themeController.SetTheme(this, "Office2016Green");
        ((System.ComponentModel.ISupportInitialize)(this._ribbon)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this._themeController)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this._flexGrid)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private C1.Win.Ribbon.C1Ribbon _ribbon;
    private C1.Win.Ribbon.RibbonApplicationMenu _ribbonApplicationMenu;
    private C1.Win.Ribbon.RibbonBottomToolBar _ribbonBottomToolBar;
    private C1.Win.Ribbon.RibbonConfigToolBar _ribbonConfigToolBar;
    private C1.Win.Ribbon.RibbonQat _ribbonQat;
    private C1.Win.Ribbon.RibbonTab _ribbonTabData;
    private C1.Win.Ribbon.RibbonGroup _ribbonGroupData;
    private C1.Win.Ribbon.RibbonTopToolBar _ribbonTopToolBar;
    private C1.Win.Ribbon.RibbonComboBox _ribbonComboBoxDataSize;
    private C1.Win.Ribbon.RibbonSeparator _ribbonSeparatorData;
    private C1.Win.Ribbon.RibbonMenu _ribbonMenuFormatting;
    private C1.Win.Ribbon.RibbonMenu _ribbonMenuColumns;
    private C1.Win.Ribbon.RibbonCheckBox _ribbonCheckBoxGroupByCountry;
    private C1.Win.Ribbon.RibbonCheckBox _ribbonCheckBoxGroupByProduct;
    private C1.Win.Themes.C1ThemeController _themeController;
    private C1.Win.Ribbon.RibbonComboBox _ribbonComboBoxThemes;
    private C1.Win.FlexGrid.C1FlexGrid _flexGrid;
    private C1.Win.Ribbon.RibbonButton _ribbonButtonFilter;
    private C1.Win.Ribbon.RibbonGroup _ribbonGroupGroup;
    private C1.Win.Ribbon.RibbonTextBox _ribbonTextBoxSearch;
    private C1.Win.Ribbon.RibbonToolBar _ribbonToolBarSearch;
    private C1.Win.Ribbon.RibbonSeparator _ribbonSeparatorGroup;
    private C1.Win.SuperTooltip.C1SuperTooltip _tooltip;
    private C1.Win.SuperTooltip.C1SuperErrorProvider _errorProvider;
}

