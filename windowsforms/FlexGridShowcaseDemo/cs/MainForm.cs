using C1.Win.FlexGrid;
using C1.Win.Themes;
using C1.Win.Ribbon;
using C1.Win.RulesManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FlexGridShowcaseDemo
{

    public partial class MainForm : C1RibbonForm 
    {
        private const string _customThemeName = "Greenwich";
        private const int _footerTextPadding = 4;
        private DataSet _ds = new DataSet();
        private C1RulesManager _rulesManager = new C1RulesManager();
        private IEnumerable<IRule> _rules = Enumerable.Empty<IRule>();
        private Random _rnd = new Random();

        #region Initialization

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillData();

            InitImages();
            InitFlexGrid();
            InitRules();
            InitThemes();

            // Data sizes
            var dataSizes = new List<string>() { "10 Rows, 12 Columns", "50 Rows, 12 Columns", "100 Rows, 12 Columns", "1000 Rows, 12 Columns", "5000 Rows, 12 Columns" };
            foreach (var item in dataSizes)
                _ribbonComboBoxDataSize.Items.Add(item);

            // Columns visible
            _ribbonMenuColumns.Items.Clear();
            (from s in _flexGrid.Cols.Cast<Column>() select s)
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => new RibbonToggleButton()
                {
                    Text = x.Caption,
                    Pressed = x.Visible
                })
                .ToList()
                .ForEach(x =>
                {
                    x.PressedButtonChanged += new EventHandler(_lstColumns_PressedChanged);
                    _ribbonMenuColumns.Items.Add(x);
                });


            _ribbonComboBoxDataSize.SelectedIndex = 2;

            ActiveControl = _flexGrid;
        }

        #endregion

        #region Fill Data

        private enum Countries
        {
            Germany,
            UK,
            US,
            Japan
        };

        private enum DrawColors
        {
            Black,
            White,
            Green,
            Red
        };

        private void BuildRows(int countRows, DataTable dt)
        {
            var startPeriod = new DateTime(2020, 01, 25, 8, 29, 0);
            _flexGrid.BeginUpdate();

            // Related data
            var products = (from s in _ds.Tables["Products"].Rows.Cast<DataRow>() select s)
                    .Select(x => x["Name"].ToString())
                    .ToArray();

            var countries = Enum.GetValues(typeof(Countries))
                    .Cast<Countries>()
                    .ToArray();

            var colors = Enum.GetValues(typeof(DrawColors))
                    .Cast<DrawColors>()
                    .ToArray();

            // Creating rows
            var data = Enumerable.Range(1, countRows)
                    .Select(x => new object[]
                    { 
                    // ID
                    x,
                    // Product
                    products[_rnd.Next(products.Length)],
                    // Country
                    countries[_rnd.Next(countries.Length)],
                    // Color
                    colors[_rnd.Next(colors.Length)],
                    // Price
                    _rnd.NextDouble() * 100 * _rnd.Next(100),
                    // Change
                    _rnd.NextDouble() * 10 * _rnd.Next(100) * (_rnd.Next(1,3) == 1 ? (-1) : 1),
                    // History
                    Enumerable.Range(0, 12).Select(y => _rnd.Next(0,50)).ToArray(),
                    // Discount
                    _rnd.NextDouble(),
                    // Raiting
                    _rnd.Next(0,5),
                    // Active
                    (_rnd.Next(1,3) == 1 ? false : true),
                    // Date
                    startPeriod.AddDays(_rnd.Next(60))
                        .AddHours(_rnd.Next(60))
                        .AddMinutes(_rnd.Next(60))
                    });

            dt.Clear();
            data.ToList().ForEach(x => dt.Rows.Add(x));

            _ds.AcceptChanges();
            _flexGrid.EndUpdate();
        }

        private void FillData()
        {
            var descriptions = new List<string>()
            {
                "Across all our software products and services, our focus is on helping our customers achieve their goals. Our key principles – thoroughly understanding our customers' business objectives, maintaining a strong emphasis on quality, and adhering to the highest ethical standards – serve as the foundation for everything we do."
            };

            // Products table
            var dt = new DataTable("Products");
            var columns = new List<string>() { "Name", "Size", "Weight", "Quantity", "Description" }
                        .Select(x => new DataColumn() { ColumnName = x, DataType = typeof(string) });
            dt.Columns.AddRange(columns.ToArray());

            // Add rows
            dt.Rows.Add(new object[] { "Gadget", "120", "900", "2", descriptions[_rnd.Next(descriptions.Count - 1)] });
            dt.Rows.Add(new object[] { "Widget", "20", "20", "25", descriptions[_rnd.Next(descriptions.Count - 1)] });
            dt.Rows.Add(new object[] { "Doohickey", "74", "90", "100", descriptions[_rnd.Next(descriptions.Count - 1)] });
            _ds.Tables.Add(dt);

            // Data table
            dt = new DataTable("Data");
            dt.Columns.Add("ID", typeof(int));

            dt.Columns.Add("Product", typeof(string));
            dt.Columns.Add("Country", typeof(Countries));
            dt.Columns.Add("Color", typeof(DrawColors));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("Change", typeof(decimal));
            dt.Columns.Add("History", typeof(object));
            dt.Columns.Add("Discount", typeof(decimal));
            dt.Columns.Add("Rating", typeof(int));
            dt.Columns.Add("Active", typeof(bool));
            dt.Columns.Add("Date", typeof(DateTime));

            _ds.Tables.Add(dt);

            // Creating relation between products and data
            _ds.Relations.Add("Producs_Data",
                    _ds.Tables["Products"].Columns["Name"], _ds.Tables["Data"].Columns["Product"]);

            _flexGrid.DataSource = _ds;
            _flexGrid.DataMember = "Data";
        }

        #endregion

        #region Init FlexGrid

        static Image LoadImage(string recourceName)
        {
            // load the picture
            Image img = null;
            try
            {
                var resource = "FlexGridShowcaseDemo.Properties.Resources";
                var assembly = Assembly.GetExecutingAssembly();

                var manager = new System.Resources.ResourceManager(resource, assembly);
                if (manager != null)
                {
                    Bitmap bmp = (Bitmap)manager.GetObject(recourceName, CultureInfo.InvariantCulture);
                    return bmp;
                }
            }
            catch (Exception)
            {
            }

            // return what we got
            return img;
        }

        private void InitImages()
        {
            // ConditionalFormating
            var image = new C1.Framework.C1BitmapIcon("ConditionalFormating", new Size(20, 20), Color.Transparent, LoadImage("ConditionalFormating"));
            _ribbonMenuFormating.IconSet.Add(image);

            // Columns
            image = new C1.Framework.C1BitmapIcon("Columns", new Size(20, 20), Color.Transparent, LoadImage("Columns"));
            _ribbonMenuColumns.IconSet.Add(image);

            // Filter
            image = new C1.Framework.C1BitmapIcon("Filter", new Size(20, 20), Color.Transparent, LoadImage("Filter"));
            _ribbonButtonFilter.IconSet.Add(image);

            var appIcon = Properties.Resources.App;
            Icon = appIcon;
        }
        private void InitGroups()
        {
            var props = new List<string>();
            if (_ribbonCheckBoxGroupByCountry.Checked)
                props.Add("Country");
            if (_ribbonCheckBoxGroupByProduct.Checked)
                props.Add("Product");

            // Clear condition filters
            if (_flexGrid.GroupDescriptions != null && props.Count == 0)
            {
                _flexGrid.GroupDescriptions = null;
                return;
            }

            var groups = props
                    .Select(x => new GroupDescription(x, ListSortDirection.Ascending))
                    .ToList();
            _flexGrid.GroupDescriptions = groups;
        }

        private void InitFlexGrid()
        {
            _flexGrid.AllowFiltering = true;
            _flexGrid.ShowErrors = true;

            _flexGrid.Cols[0].Width = 22;

            // build data map
            var flagsHt = new Hashtable();
            Enum.GetValues(typeof(Countries))
             .Cast<Countries>()
             .ToList()
             .ForEach(x =>
             {
                 flagsHt.Add(x, LoadImage(x.ToString()));
             });

            // assign ImageMap to countries column
            _flexGrid.Cols["Country"].ImageMap = flagsHt;
            _flexGrid.Cols["Country"].ImageAndText = true;

            var colorsHt = new Hashtable();
            Enum.GetValues(typeof(DrawColors))
                .Cast<DrawColors>()
                .ToList()
                .ForEach(x =>
                {
                    colorsHt.Add(x, LoadImage(x.ToString()));
                });

            _flexGrid.Cols["Color"].ImageMap = colorsHt;
            _flexGrid.Cols["Color"].ImageAndText = true;

            // Formatting columns
            _flexGrid.Cols["ID"].Width = 50;
            _flexGrid.Cols["ID"].AllowEditing = false;
            _flexGrid.Cols["Date"].Format = "g";
            _flexGrid.Cols["Price"].Format = "C2";
            _flexGrid.Cols["Price"].TextAlign = TextAlignEnum.RightCenter;

            // Add validator
            _flexGrid.Cols["Price"].EditorValidation.Add(new RequiredRule());
            _flexGrid.Cols["Price"].EditorValidation.Add(new RangeRule()
            {
                Minimum = decimal.Zero,
                Maximum = decimal.MaxValue,
                ErrorMessage = "Price cannot be negative"
            });


            _flexGrid.Cols["Change"].Format = "C2";
            _flexGrid.Cols["Change"].TextAlign = TextAlignEnum.RightCenter;
            _flexGrid.Cols["Discount"].Format = "p0";
            _flexGrid.Cols["Discount"].AllowEditing = false;
            _flexGrid.Cols["Discount"].Width = 80;
            _flexGrid.Cols["Rating"].ImageAndText = false;
            _flexGrid.Cols["Rating"].AllowEditing = false;
            _flexGrid.Cols["Price"].Width = 80;

            // For the history column initialize sparkline
            _flexGrid.Cols["History"].ShowSparkline = true;
            _flexGrid.Cols["History"].Sparkline.ShowLow = true;
            _flexGrid.Cols["History"].Sparkline.ShowHigh = true;
            _flexGrid.Cols["History"].AllowEditing = false;

            _flexGrid.Cols["Active"].Width = 60;

            // Init combobox list
            var productList = (from s in _ds.Tables["Products"].Rows.Cast<DataRow>() select s)
                .Select(x => x["Name"].ToString())
                .ToArray();
            var list = string.Join("|", productList);
            _flexGrid.Cols["Product"].ComboList = list;


            // Creating footers
            var footerDescription = new FooterDescription();

            // Price agg
            var aggFooterPriceAvg = new AggregateDefinition();
            aggFooterPriceAvg.Aggregate = AggregateEnum.Average;
            aggFooterPriceAvg.Caption = "Average price: ${0:N2}";
            aggFooterPriceAvg.PropertyName = "Price";

            // Discount agg
            var aggFooterDiscoutAvg = new AggregateDefinition();
            aggFooterDiscoutAvg.Aggregate = AggregateEnum.Average;
            aggFooterDiscoutAvg.Caption = "Average discount: {0:P}";
            aggFooterDiscoutAvg.PropertyName = "Discount";

            footerDescription.Aggregates.Add(aggFooterPriceAvg);
            footerDescription.Aggregates.Add(aggFooterDiscoutAvg);

            // Add footers
            _flexGrid.Footers.Descriptions.Add(footerDescription);
            _flexGrid.Footers.Fixed = true;

            // Set details
            _flexGrid.RowDetailProvider = (g, r) => new CustomRowDetail();

            // Other properties
            _flexGrid.HideGroupedColumns = true;
            _flexGrid.AllowFiltering = true;
            _flexGrid.AllowMerging = AllowMergingEnum.Nodes;

            // Add styles (Red, Green and Rating)
            var style = _flexGrid.Styles.Add("Red");
            style.ImageAlign = ImageAlignEnum.LeftCenter;
            style.ForeColor = Color.Red;

            style = _flexGrid.Styles.Add("Green");
            style.ImageAlign = ImageAlignEnum.LeftCenter;
            style.ForeColor = Color.Green;

            style = _flexGrid.Styles.Add("Rating");
            style.ImageAlign = ImageAlignEnum.RightCenter;
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

            // Creating rules
            _rules = rulesDict.Keys
                .Select(x => new C1.Win.RulesManager.Rule()
                {
                    Name = x,
                    Expression = rulesDict[x],
                    Style = new ItemStyle()
                    {
                        ForeColor = Color.FromArgb(_rnd.Next(255), _rnd.Next(255), _rnd.Next(255)),
                        BorderColor = Color.DarkBlue,
                        FontStyle = FontStyle.Bold
                    }
                });

            // Add menu items
            rulesDict.Keys
                .Select(x => new RibbonToggleButton()
                {
                    Text = x,
                    Pressed = false
                })
                .ToList()
                .ForEach(x =>
                {
                    x.PressedButtonChanged += new EventHandler(_lstFormating_PressedChanged);
                    _ribbonMenuFormating.Items.Add(x);
                });

        }

        #endregion

        #region Themes

        private void InitThemes()
        {
            // Register custom theme
            var customThemePath = Path.Combine(Directory.GetCurrentDirectory(), _customThemeName + ".c1themez");
            if (File.Exists(customThemePath))
            {
                C1ThemeController.RegisterTheme(customThemePath);
            }

            // Load themes into ribbon combo box
            var themes = C1ThemeController.GetThemes().Where(x => x == _customThemeName || x.Contains("Office2016") || x.Contains("Material"));
            foreach (var theme in themes)
            {
                _ribbonComboBoxThemes.Items.Add(theme);
            }

            // Set default theme
            var customThemeIndex = _ribbonComboBoxThemes.Items.IndexOf(_customThemeName);
            if (customThemeIndex > -1)
            {
                _ribbonComboBoxThemes.SelectedIndex = customThemeIndex;
            }
        }

        #endregion

        #region FlexGrid Events

        private void _flex_GridChanged(object sender, GridChangedEventArgs e)
        {
            var lastRowIndex = _flexGrid.Rows.Count - 1;
            if (e.GridChangedType == GridChangedTypeEnum.CellChanged && e.r1 == lastRowIndex)
            {
                UpdateFooterColumnWidth(e.c1);
            }
        }

        private void _flex_OwnerDrawCell(object sender, OwnerDrawCellEventArgs e)
        {
            var columnName = _flexGrid.Cols[e.Col].Name;

            // custom paint cells for change column
            if (_flexGrid[e.Row, e.Col] is decimal && columnName == "Change")
            {
                var value = (decimal)_flexGrid[e.Row, e.Col];
                if (value >= 0)
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

            // custom paint cells for raiting
            if (_flexGrid[e.Row, e.Col] is int && columnName == "Rating")
            {
                var value = (int)_flexGrid[e.Row, e.Col];
                if (value > 0)
                {
                    e.Style = _flexGrid.Styles["Rating"];
                    e.Image = LoadImage($"star{value}");
                }
            }
        }

        private void UpdateFooterColumnWidth(int columnIndex)
        {
            var lastRowIndex = _flexGrid.Rows.Count - 1;
            if (_flexGrid.Footers.Descriptions.Count > 0)
            {
                var footerCellValue = _flexGrid[lastRowIndex, columnIndex];
                if (footerCellValue == null)
                {
                    return;
                }

                var footerCellTextWidth = TextRenderer.MeasureText(footerCellValue.ToString(), _flexGrid.Styles.Footer.Font).Width;
                _flexGrid.Cols[columnIndex].Width = Math.Max(footerCellTextWidth + _footerTextPadding, _flexGrid.Cols[columnIndex].Width);
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
            var dt = _ds.Tables["Data"];
            switch (_ribbonComboBoxDataSize.SelectedIndex)
            {
                case 0:
                    {
                        BuildRows(10, dt);
                        break;
                    }
                case 1:
                    {
                        BuildRows(50, dt);
                        break;
                    }
                case 2:
                    {
                        BuildRows(100, dt);
                        break;
                    }
                case 3:
                    {
                        BuildRows(1000, dt);
                        break;
                    }
                case 4:
                    {
                        BuildRows(5000, dt);
                        break;
                    }
            }
        }

        private void _ribbonComboBoxThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Change theme
            var themeName = _ribbonComboBoxThemes.Text;
            _themeController.Theme = themeName;
        }

        private void _ribbonTextBoxSearch_ChangeCommitted(object sender, EventArgs e)
        {
            _flexGrid.ApplySearch(_ribbonTextBoxSearch.Text, true, true);
        }

        private void _lstColumns_PressedChanged(object sender, EventArgs e)
        {
            var preseedColumn = sender as RibbonToggleButton;
            if (preseedColumn != null)
                _flexGrid.Cols[preseedColumn.Text].Visible = preseedColumn.Pressed;
        }

        private void _lstFormating_PressedChanged(object sender, EventArgs e)
        {
            var pressedRule = sender as RibbonToggleButton;
            if (pressedRule != null)
            {
                var ruleName = pressedRule.Text;
                var pressed = pressedRule.Pressed;

                var newRule = _rules.Where(x => x.Name == ruleName).FirstOrDefault();
                newRule.AppliesTo.Add(new FieldRange(new string[] { "Product", "Country", "Color", "Discount" }));

                var existsRule = _rulesManager.Rules.Where(x => x.Name == ruleName).FirstOrDefault();

                if (!pressed && _rulesManager.Rules.Contains(existsRule))
                    _rulesManager.Rules.Remove(existsRule);
                if (pressed && existsRule == null)
                    _rulesManager.Rules.Add(newRule);
            }
        }

        private void _ribbonButtonFilter_Click(object sender, EventArgs e)
        {
            _flexGrid.ApplySearch(_ribbonTextBoxSearch.Text, true, true);
        }

        private void _ribbonTextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            _flexGrid.ApplySearch(_ribbonTextBoxSearch.Text, true, false);
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

            var cols = _flexGrid.Cols;
            if (cols == null)
            {
                return;
            }

            if (cols.Contains("Price"))
            {
                UpdateFooterColumnWidth(cols["Price"].Index);
            }
            if (cols.Contains("Discount"))
            {
                UpdateFooterColumnWidth(cols["Discount"].Index);
            }
        }

        #endregion

    }
}
