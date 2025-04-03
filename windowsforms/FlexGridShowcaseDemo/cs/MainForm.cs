using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using C1.Framework;
using C1.Win.FlexGrid;
using C1.Win.Ribbon;
using C1.Win.RulesManager;
using C1.Win.RulesManager.Model;
using C1.Win.Themes;

namespace FlexGridShowcaseDemo;

public partial class MainForm : C1RibbonForm
{
    private const string CustomThemeName = "Greenwich";
    private const int FooterTextPadding = 4;

    private readonly DataSet _dataSet = new DataSet();
    private readonly C1RulesManager _rulesManager = new C1RulesManager();
    private IEnumerable<IRule> _rules;
    private static readonly Random s_rnd = new Random();

    #region Initialization

    public MainForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        FillData();

        InitImages();
        InitFlexGrid();
        InitRules();
        InitThemes();

        // Data sizes
        var dataSizes = new List<string>()
        {
            "10 Rows, 12 Columns",
            "50 Rows, 12 Columns",
            "100 Rows, 12 Columns",
            "1000 Rows, 12 Columns",
            "5000 Rows, 12 Columns"
        };
        foreach (string item in dataSizes)
        {
            _ribbonComboBoxDataSize.Items.Add(item);
        }

        // Columns visible
        _ribbonMenuColumns.Items.Clear();
        IEnumerable<RibbonToggleButton> columnRibbonToggleButtons = (from s in _flexGrid.Cols.Cast<Column>() select s)
            .Where(x => !string.IsNullOrEmpty(x.Name))
            .Select(x => new RibbonToggleButton()
            {
                Text = x.Caption,
                Pressed = x.Visible
            });

        foreach (RibbonToggleButton columnRibbonToggleButton in columnRibbonToggleButtons)
        {
            columnRibbonToggleButton.PressedButtonChanged += ColumnRibbonToggleButton_PressedButtonChanged;
            _ribbonMenuColumns.Items.Add(columnRibbonToggleButton);
        }

        _ribbonComboBoxDataSize.SelectedIndex = 2;

        UpdateFooterColumnWidths();

        ActiveControl = _flexGrid;
    }

    #endregion

    #region Fill Data

    private enum Country
    {
        Germany,
        UK,
        US,
        Japan
    };

    private enum DrawColor
    {
        Black,
        White,
        Green,
        Red
    };

    private void BuildRows(int rowCount, DataTable dataTable)
    {
        var startPeriod = new DateTime(2020, 01, 25, 8, 29, 0);
        _flexGrid.BeginUpdate();

        // Related data
        string[] products = (from s in _dataSet.Tables["Products"].Rows.Cast<DataRow>() select s)
                .Select(x => x["Name"].ToString())
                .ToArray();

        Country[] countries = Enum.GetValues(typeof(Country))
                .Cast<Country>()
                .ToArray();

        DrawColor[] colors = Enum.GetValues(typeof(DrawColor))
                .Cast<DrawColor>()
                .ToArray();

        dataTable.Clear();

        DataRowCollection dataRows = dataTable.Rows;
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            // Fill history data
            var historyData = new int[12];
            for (var i = 0; i < historyData.Length; i++)
            {
                historyData[i] = s_rnd.Next(0, 50);
            };

            dataRows.Add(
                // ID
                rowIndex + 1,
                // Product
                products[s_rnd.Next(products.Length)],
                // Country
                countries[s_rnd.Next(countries.Length)],
                // Color
                colors[s_rnd.Next(colors.Length)],
                // Price
                s_rnd.NextDouble() * 100 * s_rnd.Next(100),
                // Change
                s_rnd.NextDouble() * 10 * s_rnd.Next(100) * (s_rnd.NextDouble() >= 0.5 ? 1 : -1),
                // History
                historyData,
                // Discount
                s_rnd.NextDouble(),
                // Rating
                s_rnd.Next(0, 5),
                // Active
                (s_rnd.NextDouble() >= 0.5),
                // Date
                startPeriod.AddDays(s_rnd.Next(60))
                    .AddHours(s_rnd.Next(60))
                    .AddMinutes(s_rnd.Next(60))
                );
        }

        _dataSet.AcceptChanges();
        _flexGrid.EndUpdate();
    }

    private void FillData()
    {
        var descriptions = new List<string>()
        {
            "Across all our software products and services, our focus is on helping our customers achieve their goals. Our key principles – thoroughly understanding our customers' business objectives, maintaining a strong emphasis on quality, and adhering to the highest ethical standards – serve as the foundation for everything we do."
        };

        // Products table
        var productsDataTable = new DataTable("Products");

        DataColumnCollection productsColumns = productsDataTable.Columns;

        // Add columns
        productsColumns.Add("Name");
        productsColumns.Add("Size", typeof(float));
        productsColumns.Add("Weight", typeof(float));
        productsColumns.Add("Quantity", typeof(uint));
        productsColumns.Add("Description");

        DataRowCollection productsRows = productsDataTable.Rows;

        // Add rows
        productsRows.Add("Gadget", 120f, 900f, 2, descriptions[s_rnd.Next(descriptions.Count)]);
        productsRows.Add("Widget", 20f, 20f, 25, descriptions[s_rnd.Next(descriptions.Count)]);
        productsRows.Add("Doohickey", 74f, 90f, 100, descriptions[s_rnd.Next(descriptions.Count)]);

        DataTableCollection tables = _dataSet.Tables;
        tables.Add(productsDataTable);

        // Data table
        var dataDataTable = new DataTable("Data");

        DataColumnCollection dataColumns = dataDataTable.Columns;

        // Add columns
        dataColumns.Add("ID", typeof(int));
        dataColumns.Add("Product");
        dataColumns.Add("Country", typeof(Country));
        dataColumns.Add("Color", typeof(DrawColor));
        dataColumns.Add("Price", typeof(decimal));
        dataColumns.Add("Change", typeof(decimal));
        dataColumns.Add("History", typeof(int[]));
        dataColumns.Add("Discount", typeof(decimal));
        dataColumns.Add("Rating", typeof(int));
        dataColumns.Add("Active", typeof(bool));
        dataColumns.Add("Date", typeof(DateTime));

        tables.Add(dataDataTable);

        // Creating relation between products and data
        _dataSet.Relations.Add("Products",
            dataColumns["Product"], productsColumns["Name"], false);

        _flexGrid.DataSource = _dataSet;
        _flexGrid.DataMember = "Data";
        _flexGrid.Cols.Remove("Products");
    }

    #endregion

    #region Init FlexGrid

    static Image LoadImage(string resourceName)
    {
        var resource = "FlexGridShowcaseDemo.Properties.Resources";
        Assembly assembly = Assembly.GetExecutingAssembly();

        var manager = new ResourceManager(resource, assembly);
        if (manager is null)
        {
            return null;
        }

        return manager.GetObject(resourceName, CultureInfo.InvariantCulture) as Image;
    }

    private C1BitmapIcon GetBitmapIcon(string name)
    {
        Image image = LoadImage(name);
        if (image is null)
        {
            return null;
        }

        return new C1BitmapIcon(name, new Size(20, 20), Color.Transparent, image);
    }

    private void InitImages()
    {
        // ConditionalFormatting
        C1BitmapIcon conditionalFormattingBitmapIcon = GetBitmapIcon("ConditionalFormatting");
        if (conditionalFormattingBitmapIcon is not null)
        {
            _ribbonMenuFormatting.IconSet.Add(conditionalFormattingBitmapIcon);
        }

        // Columns
        C1BitmapIcon columnsBitmapIcon = GetBitmapIcon("Columns");
        if (columnsBitmapIcon is not null)
        {
            _ribbonMenuColumns.IconSet.Add(columnsBitmapIcon);
        }

        // Filter
        C1BitmapIcon filterBitmapIcon = GetBitmapIcon("Filter");
        if (filterBitmapIcon is not null)
        {
            _ribbonButtonFilter.IconSet.Add(filterBitmapIcon);
        }

        Icon appIcon = Properties.Resources.App;
        Icon = appIcon;
    }

    private void InitGroups()
    {
        var propertyNames = new List<string>();
        if (_ribbonCheckBoxGroupByCountry.Checked)
        {
            propertyNames.Add("Country");
        }
        if (_ribbonCheckBoxGroupByProduct.Checked)
        {
            propertyNames.Add("Product");
        }

        // Clear condition filters
        if (_flexGrid.GroupDescriptions is not null && propertyNames.Count == 0)
        {
            _flexGrid.GroupDescriptions = null;
            return;
        }

        var groups = propertyNames
                .Select(x => new GroupDescription(x, ListSortDirection.Ascending))
                .ToList();
        _flexGrid.GroupDescriptions = groups;
    }

    private void InitFlexGrid()
    {
        // setup flexgrid
        _flexGrid.AllowFiltering = true;
        _flexGrid.AllowFiltering = true;
        _flexGrid.AllowMerging = AllowMergingEnum.Nodes;
        _flexGrid.HideGroupedColumns = true;
        _flexGrid.ShowErrors = true;

        ColumnCollection columns = _flexGrid.Cols;

        // setup flexgrid columns
        columns[0].Width = 22;

        Column idColumn = columns["ID"];
        idColumn.Width = 50;
        idColumn.AllowEditing = false;

        // setup combo list
        IEnumerable<string> products = (from s in _dataSet.Tables["Products"].Rows.Cast<DataRow>() select s)
            .Select(x => x["Name"].ToString());
        columns["Product"].ComboList = string.Join("|", products);

        // build image map for countries
        var flagImageMap = new Dictionary<Country, Image>();
        foreach (Country country in Enum.GetValues(typeof(Country)))
        {
            flagImageMap.Add(country, LoadImage(country.ToString()));
        }

        Column countryColumn = columns["Country"];

        // assign image map to country column
        countryColumn.ImageMap = flagImageMap;
        countryColumn.ImageAndText = true;

        // build image map for colors
        var colorImageMap = new Dictionary<DrawColor, Image>();
        foreach (DrawColor color in Enum.GetValues(typeof(DrawColor)))
        {
            colorImageMap.Add(color, LoadImage(color.ToString()));
        }

        Column colorColumn = columns["Color"];

        // assign image map to color column
        colorColumn.ImageMap = colorImageMap;
        colorColumn.ImageAndText = true;

        Column priceColumn = columns["Price"];
        priceColumn.Format = "C2";
        priceColumn.TextAlign = TextAlignEnum.RightCenter;
        priceColumn.Width = 80;

        ValidationRuleCollection priceEditorValidation = _flexGrid.Cols["Price"].EditorValidation;

        // add validation rules
        priceEditorValidation.Add(new RequiredRule());
        priceEditorValidation.Add(new RangeRule()
        {
            Minimum = decimal.Zero,
            Maximum = decimal.MaxValue,
            ErrorMessage = "Price cannot be negative"
        });

        Column changeColumn = columns["Change"];
        changeColumn.Format = "C2";
        changeColumn.TextAlign = TextAlignEnum.RightCenter;

        Column historyColumn = columns["History"];
        historyColumn.AllowEditing = false;

        // setup sparkline for history column 
        historyColumn.ShowSparkline = true;

        Sparkline historySparkline = historyColumn.Sparkline;
        historySparkline.ShowLow = true;
        historySparkline.ShowHigh = true;

        Column discountColumn = columns["Discount"];
        discountColumn.Format = "p0";
        discountColumn.AllowEditing = false;
        discountColumn.Width = 80;

        Column ratingColumn = columns["Rating"];
        ratingColumn.ImageAndText = false;
        ratingColumn.AllowEditing = false;

        columns["Active"].Width = 60;

        Column dateColumn = columns["Date"];
        dateColumn.Format = "g";

        // creating footers
        var footerDescription = new FooterDescription();

        // price aggregate
        var priceAggregateDefinition = new AggregateDefinition()
        {
            Aggregate = AggregateEnum.Average,
            Caption = "Average price: {0:C2}",
            PropertyName = "Price"
        };

        // discount aggregate
        var discountAggregateDefinition = new AggregateDefinition
        {
            Aggregate = AggregateEnum.Average,
            Caption = "Average discount: {0:P}",
            PropertyName = "Discount"
        };

        ObservableCollection<AggregateDefinition> aggregates = footerDescription.Aggregates;
        aggregates.Add(priceAggregateDefinition);
        aggregates.Add(discountAggregateDefinition);

        // add footers
        Footers footers = _flexGrid.Footers;
        footers.Descriptions.Add(footerDescription);
        footers.Fixed = true;

        // set details
        _flexGrid.RowDetailProvider = (g, r) => new CustomRowDetail();

        // add red style
        CellStyle redStyle = _flexGrid.Styles.Add("Red");
        redStyle.ImageAlign = ImageAlignEnum.LeftCenter;
        redStyle.ForeColor = Color.Red;

        // add green style
        CellStyle greenStyle = _flexGrid.Styles.Add("Green");
        greenStyle.ImageAlign = ImageAlignEnum.LeftCenter;
        greenStyle.ForeColor = Color.Green;

        // add rating style
        CellStyle ratingStyle = _flexGrid.Styles.Add("Rating");
        ratingStyle.ImageAlign = ImageAlignEnum.RightCenter;
    }

    private void InitRules()
    {
        _rulesManager.SetC1RulesManager(_flexGrid, _rulesManager);
        var rulesDict = new Dictionary<string, string>()
        {
            { "Discount < 10%", "= [Discount] < 0.1" },
            { "Discount < 20%", "= [Discount] < 0.2" },
            { "Discount < 30%", "= [Discount] < 0.3" }
        };

        // creating rules
        _rules = rulesDict.Keys
            .Select(x => new C1.Win.RulesManager.Rule()
            {
                Name = x,
                Expression = rulesDict[x],
                Style = new ItemStyle()
                {
                    ForeColor = Color.FromArgb(s_rnd.Next(255), s_rnd.Next(255), s_rnd.Next(255)),
                    BorderColor = Color.DarkBlue,
                    FontStyle = FontStyle.Bold
                }
            });

        // add menu items
        IEnumerable<RibbonToggleButton> ruleRibbonToggleButtons = rulesDict.Keys
            .Select(x => new RibbonToggleButton()
            {
                Text = x,
                Pressed = false
            });

        foreach (RibbonToggleButton ruleRibbonToggleButton in ruleRibbonToggleButtons)
        {
            ruleRibbonToggleButton.PressedButtonChanged += RuleRibbonToggleButton_PressedButtonChanged;
            _ribbonMenuFormatting.Items.Add(ruleRibbonToggleButton);
        }
    }

    #endregion

    #region Themes

    private void InitThemes()
    {
        // register custom theme
        string customThemePath = Path.Combine(Directory.GetCurrentDirectory(), CustomThemeName + ".c1themez");
        if (File.Exists(customThemePath))
        {
            C1ThemeController.RegisterTheme(customThemePath);
        }

        // load themes into ribbon combo box
        foreach (string theme in C1ThemeController.GetThemes())
        {
            if (theme is not CustomThemeName && !theme.Contains("Office2016") && !theme.Contains("Material"))
            {
                continue;
            }

            _ribbonComboBoxThemes.Items.Add(theme);
        }

        // set default theme
        int customThemeIndex = _ribbonComboBoxThemes.Items.IndexOf(CustomThemeName);
        if (customThemeIndex > -1)
        {
            _ribbonComboBoxThemes.SelectedIndex = customThemeIndex;
        }
    }

    #endregion

    #region FlexGrid Events

    private void _flexGrid_GridChanged(object sender, GridChangedEventArgs e)
    {
        int lastRowIndex = _flexGrid.Rows.Count - 1;
        if (e.GridChangedType == GridChangedTypeEnum.CellChanged && e.r1 == lastRowIndex)
        {
            UpdateFooterColumnWidth(e.c1);
        }
    }

    private void _flexGrid_OwnerDrawCell(object sender, OwnerDrawCellEventArgs e)
    {
        string columnName = _flexGrid.Cols[e.Col].Name;

        // custom paint cells for change column
        if (columnName == "Change" && _flexGrid[e.Row, e.Col] is decimal changeValue)
        {
            if (changeValue >= 0)
            {
                e.Style = _flexGrid.Styles["Green"];
                e.Image = LoadImage("UpGreen");
            }
            else
            {
                e.Style = _flexGrid.Styles["Red"];
                e.Image = LoadImage("DownRed");
            }
        }

        // custom paint cells for rating column
        if (columnName == "Rating" && _flexGrid[e.Row, e.Col] is int ratingValue)
        {
            if (ratingValue > 0)
            {
                e.Style = _flexGrid.Styles["Rating"];
                e.Image = LoadImage($"star{ratingValue}");
            }
        }
    }

    private void UpdateFooterColumnWidths()
    {
        ColumnCollection columns = _flexGrid.Cols;
        if (columns is null)
        {
            return;
        }

        if (columns.Contains("Price"))
        {
            UpdateFooterColumnWidth(columns["Price"].Index);
        }
        if (columns.Contains("Discount"))
        {
            UpdateFooterColumnWidth(columns["Discount"].Index);
        }
    }

    private void UpdateFooterColumnWidth(int columnIndex)
    {
        int lastRowIndex = _flexGrid.Rows.Count - 1;
        if (_flexGrid.Footers.Descriptions.Count > 0)
        {
            object footerCellValue = _flexGrid[lastRowIndex, columnIndex];
            if (footerCellValue is null)
            {
                return;
            }

            int footerCellTextWidth = TextRenderer.MeasureText(footerCellValue.ToString(), _flexGrid.Styles.Footer.Font).Width;

            Column column = _flexGrid.Cols[columnIndex];
            column.Width = Math.Max(footerCellTextWidth + FooterTextPadding, column.Width);
        }
    }

    #endregion

    #region Ribbon Events

    private void _ribbonCheckBoxGroupByCountry_CheckedChanged(object sender, EventArgs e)
    {
        InitGroups();
    }

    private void _ribbonCheckBoxGroupByProduct_CheckedChanged(object sender, EventArgs e)
    {
        InitGroups();
    }

    private void _ribbonComboBoxDataSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dataDataTable = _dataSet.Tables["Data"];
        switch (_ribbonComboBoxDataSize.SelectedIndex)
        {
            case 0:
                {
                    BuildRows(10, dataDataTable);
                    break;
                }
            case 1:
                {
                    BuildRows(50, dataDataTable);
                    break;
                }
            case 2:
                {
                    BuildRows(100, dataDataTable);
                    break;
                }
            case 3:
                {
                    BuildRows(1000, dataDataTable);
                    break;
                }
            case 4:
                {
                    BuildRows(5000, dataDataTable);
                    break;
                }
        }
    }

    private void _ribbonComboBoxThemes_SelectedIndexChanged(object sender, EventArgs e)
    {
        // change theme
        string themeName = _ribbonComboBoxThemes.Text;
        _themeController.Theme = themeName;
    }

    private void _ribbonTextBoxSearch_ChangeCommitted(object sender, EventArgs e)
    {
        _flexGrid.ApplySearch(_ribbonTextBoxSearch.Text, SearchHighlightMode.OnlyFirst, true);
    }

    private void ColumnRibbonToggleButton_PressedButtonChanged(object sender, EventArgs e)
    {
        if (sender is not RibbonToggleButton pressedRibbonToggleButton)
        {
            return;
        }

        _flexGrid.Cols[pressedRibbonToggleButton.Text].Visible = pressedRibbonToggleButton.Pressed;

    }

    private void RuleRibbonToggleButton_PressedButtonChanged(object sender, EventArgs e)
    {
        if (sender is not RibbonToggleButton pressedRule)
        {
            return;
        }

        string ruleName = pressedRule.Text;
        bool pressed = pressedRule.Pressed;

        IRule newRule = _rules.Where(x => x.Name == ruleName).FirstOrDefault();
        newRule.AppliesTo.Add(new FieldRange(new string[] { "Product", "Country", "Color", "Discount" }));

        RuleCollection appliedRules = _rulesManager.Rules;
        IRule existingRule = appliedRules.Where(x => x.Name == ruleName).FirstOrDefault();

        if (!pressed && appliedRules.Contains(existingRule))
        {
            appliedRules.Remove(existingRule);
        }
        if (pressed && existingRule is null)
        {
            appliedRules.Add(newRule);
        }
    }

    private void _ribbonButtonFilter_Click(object sender, EventArgs e)
    {
        _flexGrid.ApplySearch(_ribbonTextBoxSearch.Text, SearchHighlightMode.OnlyFirst, true);
    }

    private void _ribbonTextBoxSearch_TextChanged(object sender, EventArgs e)
    {
        _flexGrid.ApplySearch(_ribbonTextBoxSearch.Text, SearchHighlightMode.OnlyFirst, false);
    }

    #endregion

    #region ThemeController Events

    private void _themeController_ObjectThemeApplied(C1ThemeController sender, ObjectThemeEventArgs e)
    {
        if (e.Object != _flexGrid)
        {
            return;
        }

        _flexGrid.Styles.Footer.TextAlign = TextAlignEnum.RightCenter;

        UpdateFooterColumnWidths();
    }

    #endregion

}
