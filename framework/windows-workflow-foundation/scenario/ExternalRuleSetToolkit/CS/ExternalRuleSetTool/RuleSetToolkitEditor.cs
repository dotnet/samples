//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Workflow.Activities.Rules;
using System.Workflow.Activities.Rules.Design;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;
using Microsoft.Samples.Rules.ExternalRuleSetLibrary;

namespace Microsoft.Samples.Rules.ExternalRuleSetToolkit
{
    public partial class RuleSetToolkitEditor : Form
    {

        #region Variables and constructor

        private readonly int maxMinorVersions = 100;
        private readonly int maxMajorVersions = 1000;
        private readonly WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
        private string connectionString;

        private RuleSetData selectedRuleSetData;
        private List<RuleSetData> deletedRuleSetDataCollection = new List<RuleSetData>();
        private Dictionary<TreeNode,RuleSetData> ruleSetDataDictionary = new Dictionary<TreeNode,RuleSetData>();
        private bool dirty; //indicates if any RuleSetData has been modified
        private string assemblyPath; //used to resolve assembly errors

        public RuleSetToolkitEditor()
        {
            InitializeComponent();
        }

        #endregion

        #region Form level

        private void Form1_Load(object sender, EventArgs e)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(ruleSetEditor_AssemblyResolve);
            this.FormClosing += new FormClosingEventHandler(RuleSetEditor_FormClosing);

            this.menuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(menuStrip1_ItemClicked);

            treeView1.TreeViewNodeSorter = new TreeSortClass() as IComparer;
            treeView1.HideSelection = false;

            this.ruleSetNameBox.Validating += new System.ComponentModel.CancelEventHandler(ruleSetNameBox_Validating);
            this.minorVersionBox.Validating += new CancelEventHandler(minorVersionBox_Validating);
            this.majorVersionBox.Validating += new CancelEventHandler(majorVersionBox_Validating);

            majorVersionBox.Maximum = maxMajorVersions;
            minorVersionBox.Maximum = maxMinorVersions;

            ConnectionStringSettingsCollection connectionStringSettingsCollection = ConfigurationManager.ConnectionStrings;
            foreach (ConnectionStringSettings connectionStringSettings in connectionStringSettingsCollection)
            {
                if (string.CompareOrdinal(connectionStringSettings.Name, "RuleSetStoreConnectionString") == 0)
                    connectionString = connectionStringSettings.ConnectionString;
            }

            if (connectionString != null)
                this.openToolStripMenuItem_Click(this, EventArgs.Empty);
            else
                MessageBox.Show("SQL connection string not available (should be provided in the config file).", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void RuleSetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !ContinueRuleDefinitionsChange();
        }

        //If an assembly referenced by the selected assembly cannot be found try loading it from the same directory as the referenced assembly
        //should only occur when loading a RuleSetData with existing activity information
        Assembly ruleSetEditor_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (selectedRuleSetData != null && !String.IsNullOrEmpty(assemblyPath))  
            {
                return ResolveAssembly(assemblyPath, args.Name);
            }
            else
            {
                return null;
            }
        }

        internal static Assembly ResolveAssembly(string assemblyPath, string failedAssemblyName)
        {
            try
            {
                string assemblyName;
                if (failedAssemblyName.Contains(",")) //strong name; need to strip off everything but the name
                {
                    assemblyName = failedAssemblyName.Substring(0, failedAssemblyName.IndexOf(",", StringComparison.Ordinal));
                }
                else
                {
                    assemblyName = failedAssemblyName;
                }
                string tempPath = Path.HasExtension(assemblyPath) ? Path.GetDirectoryName(assemblyPath) : assemblyPath;
                string assemblyPathToTry = tempPath + Path.DirectorySeparatorChar + assemblyName + ".dll";

                FileInfo assemblyFileInfo = new FileInfo(assemblyPathToTry);
                if (assemblyFileInfo.Exists)
                {
                    return Assembly.LoadFile(assemblyPathToTry);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception) //no luck in resolving the assembly
            {
                return null;
            }
        }

        #endregion

        #region Menu items

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ContinueRuleDefinitionsChange())
            {
                selectedRuleSetData = null;
                List<RuleSetData> ruleSetDataCollection = this.GetRuleSets();
                this.BuildTree(ruleSetDataCollection);

                this.EnableApplicationFields(true);
                this.EnableRuleSetFields(false);
            }
        }

        private List<RuleSetData> GetRuleSets()
        {
            List<RuleSetData> ruleSetDataCollection = new List<RuleSetData>();
            dirty = false;

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                string commandString = "SELECT Name, MajorVersion, MinorVersion, RuleSet, Status, AssemblyPath, ActivityName, ModifiedDate FROM RuleSet ORDER BY Name,MajorVersion, MinorVersion";
                SqlCommand command = new SqlCommand(commandString, sqlConn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        RuleSetData data = new RuleSetData();
                        data.Name = reader.GetString(0);
                        data.OriginalName = data.Name; // will be used later to see if one of these key values changed                       
                        data.MajorVersion = reader.GetInt32(1);
                        data.OriginalMajorVersion = data.MajorVersion;
                        data.MinorVersion = reader.GetInt32(2);
                        data.OriginalMinorVersion = data.MinorVersion;

                        data.RuleSetDefinition = reader.GetString(3);
                        data.Status = reader.GetInt16(4);
                        data.AssemblyPath = reader.GetString(5);
                        data.ActivityName = reader.GetString(6);
                        data.ModifiedDate = reader.GetDateTime(7);
                        data.Dirty = false;

                        ruleSetDataCollection.Add(data);
                    }
                    catch (InvalidCastException)
                    {
                        MessageBox.Show("Error parsing table row.", "RuleSet Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                sqlConn.Close();
            }

            return ruleSetDataCollection;
        }

        private void BuildTree(List<RuleSetData> ruleSetDataCollection)
        {
            ruleSetDataCollection.Sort();
            ruleSetDataDictionary.Clear();
            treeView1.Nodes.Clear();
            RuleSetData lastData = null;
            TreeNode lastRuleSetNameNode = null;
            foreach (RuleSetData data in ruleSetDataCollection)
            {
                if (lastData == null || lastData.Name != data.Name) //new ruleset name
                {
                    TreeNode newNode = new TreeNode(data.Name);
                    treeView1.Nodes.Add(newNode);
                    lastRuleSetNameNode = newNode;
                }

                TreeNode newVersionNode = new TreeNode(VersionTreeNodeText(data.MajorVersion, data.MinorVersion));
                lastRuleSetNameNode.Nodes.Add(newVersionNode);
                ruleSetDataDictionary.Add(newVersionNode, data);

                lastData = data;
            }
            treeView1.Sort();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveToDB();
        }

        private void SaveToDB()
        {
            if (ruleSetDataDictionary != null)
            {
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();
                    using (SqlTransaction transaction = sqlConn.BeginTransaction())
                    {

                        SqlCommand deleteCommand = new SqlCommand();
                        deleteCommand.Connection = sqlConn;
                        deleteCommand.Transaction = transaction;
                        deleteCommand.Parameters.Add("@name", System.Data.SqlDbType.NVarChar, 128);

                        List<RuleSetData> dirtyRSDs = new List<RuleSetData>();

                        foreach (RuleSetData data in deletedRuleSetDataCollection)
                        {
                            if (!string.IsNullOrEmpty(data.OriginalName)) //delete the ruleset if it exists in the DB; OriginalName will be null if it was created in this session
                            {
                                deleteCommand.CommandText = (string.Format(CultureInfo.InvariantCulture, "DELETE RuleSet WHERE Name = @name AND MajorVersion = {0} AND MinorVersion = {1}", data.OriginalMajorVersion, data.OriginalMinorVersion));
                                deleteCommand.Parameters["@name"].Value = data.OriginalName;
                                try
                                {
                                    deleteCommand.ExecuteNonQuery(); 
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error deleting RuleSets from DB. \r\n\n", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }

                        foreach (RuleSetData data in ruleSetDataDictionary.Values)
                        {
                            if (data.Dirty == true)
                            {
                                dirtyRSDs.Add(data);
                                data.RuleSetDefinition = this.SerializeRuleSet(data.RuleSet);
                                try
                                {
                                    if (!string.IsNullOrEmpty(data.OriginalName)) //delete the ruleset if it exists in the DB; OriginalName will be null if it was created in this session
                                    {
                                        deleteCommand.CommandText = (string.Format(CultureInfo.InvariantCulture, "DELETE RuleSet WHERE Name = @name AND MajorVersion = {0} AND MinorVersion = {1}", data.OriginalMajorVersion, data.OriginalMinorVersion));
                                        deleteCommand.Parameters["@name"].Value = data.OriginalName;

                                        deleteCommand.ExecuteNonQuery();
                                    }

                                    using (SqlCommand insertCommand = new SqlCommand())
                                    {
                                        insertCommand.Connection = sqlConn;
                                        insertCommand.Transaction = transaction;
                                        insertCommand.CommandText = (string.Format(CultureInfo.InvariantCulture, "INSERT RuleSet (Name, MajorVersion, MinorVersion, RuleSet, Status, AssemblyPath, ActivityName, ModifiedDate ) VALUES('{0}',{1},{2},'{3}',{4},'{5}','{6}','{7}')", data.Name, data.MajorVersion, data.MinorVersion, data.RuleSetDefinition, data.Status, data.AssemblyPath, data.ActivityName, DateTime.Now));
                                        insertCommand.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error updating RuleSets in DB. \r\n\n", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }

                        try
                        {
                            transaction.Commit();
                            sqlConn.Close();

                            foreach (RuleSetData data in dirtyRSDs)
                            {
                                // after updates have been stored to the DB, set/reset the "Original" values
                                data.OriginalName = data.Name;
                                data.OriginalMajorVersion = data.MajorVersion;
                                data.OriginalMinorVersion = data.MinorVersion;
                                data.Dirty = false;
                            }

                            deletedRuleSetDataCollection.Clear();

                            dirty = false;
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error saving RuleSets to DB. \r\n\n", ex.Message), "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("RuleSet collection is empty.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private string SerializeRuleSet(RuleSet ruleSet)
        {
            StringBuilder ruleDefinition = new StringBuilder();

            if (ruleSet != null)
            {
                try
                {
                    StringWriter stringWriter = new StringWriter(ruleDefinition, CultureInfo.InvariantCulture);
                    XmlTextWriter writer = new XmlTextWriter(stringWriter);
                    serializer.Serialize(writer, ruleSet);
                    writer.Flush();
                    writer.Close();
                    stringWriter.Flush();
                    stringWriter.Close();
                }
                catch (Exception ex)
                {
                    if (selectedRuleSetData != null)
                        MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error serializing RuleSet: '{0}'. \r\n\n{1}", selectedRuleSetData.Name, ex.Message), "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error serializing RuleSet. \r\n\n{0}", ex.Message), "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (selectedRuleSetData != null)
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Error serializing RuleSet: '{0}'.", selectedRuleSetData.Name), "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Error serializing RuleSet.", "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ruleDefinition.ToString();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ContinueRuleDefinitionsChange()
        {
            bool continueResult = true;
            
            if (dirty)
            {
                DialogResult result = MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Do you want to save the changes?"), "RuleSet Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(null, EventArgs.Empty);
                }
                else if (result == DialogResult.No)
                {
                }
                else //Cancel
                {
                    continueResult = false;
                }
            }
            return continueResult;
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Rules files (*.rules)|*.rules";
            DialogResult fileResult = fileDialog.ShowDialog();

            if (fileResult == DialogResult.OK && !String.IsNullOrEmpty(fileDialog.FileName))
            {
                XmlTextReader reader = new XmlTextReader(fileDialog.FileName);
                object objectRuleSet = null; ;

                try
                {
                    objectRuleSet = serializer.Deserialize(reader);
                }
                catch  (Exception ex)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Error loading file.  \r\n\n{0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                reader.Close();

                RuleDefinitions ruleDefinitions = objectRuleSet as RuleDefinitions;

                if (ruleDefinitions != null)
                {
                    if (ruleDefinitions.RuleSets.Count > 0)
                    {
                        RuleSetData ruleSetData = null;
                        List<RuleSetData> ruleSetDataCollection = new List<RuleSetData>();
                        foreach (RuleSet ruleSet in ruleDefinitions.RuleSets)
                        {
                            ruleSetData = this.CreateRuleSetData(ruleSet); 
                            
                            //find the next available major version
                            ruleSetData.MajorVersion = this.GetNextMajorVersion(ruleSet.Name);
                            ruleSetDataCollection.Add(ruleSetData);
                        }

                        RuleSetSelector selectorForm = new RuleSetSelector();
                        selectorForm.RuleSetDataCollection.AddRange(ruleSetDataCollection);
                        selectorForm.SelectAll = true;
                        selectorForm.Instructions = "Select the RuleSets you would like to import.  Each RuleSet has been assigned the next available Major Version number.";
                        DialogResult selectorResult = selectorForm.ShowDialog();

                        if (selectorResult == DialogResult.OK && selectorForm.SelectedRuleSetDataCollection != null)
                        {
                            foreach (RuleSetData data in selectorForm.SelectedRuleSetDataCollection)
                            {
                                this.AddRuleSetData(data);
                                this.GetThisType(Path.GetDirectoryName(fileDialog.FileName), Path.GetFileName(fileDialog.FileName));
                            }
                            this.treeView1_AfterSelect(this, new TreeViewEventArgs(treeView1.SelectedNode));  //force this call so that assembly and activity information if populated on form
                        }
                    }
                    else
                    {
                        MessageBox.Show("File does not contain any RuleSets.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("File is not a valid .rules file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RuleSetSelector selectorForm = new RuleSetSelector();
            selectorForm.RuleSetDataCollection.AddRange(ruleSetDataDictionary.Values);
            selectorForm.Instructions = "Select the RuleSets you would like to export.  Note that version numbers are not included in the output file, and only a single RuleSet with a given Name can be exported to the same file.";
            DialogResult selectorResult = selectorForm.ShowDialog();

            if (selectorResult == DialogResult.OK && selectorForm.SelectedRuleSetDataCollection.Count > 0)
            {
                RuleDefinitions ruleDefinitions = new RuleDefinitions();
                foreach (RuleSetData data in selectorForm.SelectedRuleSetDataCollection)
                {
                    ruleDefinitions.RuleSets.Add(data.RuleSet);
                }

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Rules files (*.rules)|*.rules";
                dialog.AddExtension = true;
                dialog.DefaultExt = "rules";
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !String.IsNullOrEmpty(dialog.FileName))
                {
                    XmlTextWriter writer = new XmlTextWriter(dialog.FileName, null);
                    serializer.Serialize(writer, ruleDefinitions);
                    writer.Flush();
                    writer.Close();
                }
            }
            else
            {
                MessageBox.Show("No RuleSets selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void validateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedRuleSetData != null)
            {
                if (ActivitySelector.ValidateRuleSet(selectedRuleSetData.RuleSet, selectedRuleSetData.Activity, false))
                    MessageBox.Show("RuleSet is valid.", "RuleSet Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        #endregion

        #region TreeView

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RuleSetData data;
            if (e.Node != null && ruleSetDataDictionary.TryGetValue(e.Node, out data))
            {
                selectedRuleSetData = data;
                assemblyPath = selectedRuleSetData.AssemblyPath;
                ruleSetNameBox.Text = selectedRuleSetData.Name;
                majorVersionBox.Value = selectedRuleSetData.MajorVersion;
                minorVersionBox.Value = selectedRuleSetData.MinorVersion;
                activityBox.Text = selectedRuleSetData.ActivityName;

                if (selectedRuleSetData.Activity == null)
                    this.LoadAssemblyAndActivity();

                this.PopulateMembers();

                this.EnableRuleSetFields(true);
            }
            else
            {
                selectedRuleSetData = null;
                assemblyPath = null;
                this.EnableRuleSetFields(false);
            }
        }

        private void SetSelectedNode(TreeNode node)
        {
            if (node != null)
            {
                treeView1.SelectedNode = node;
                this.treeView1_AfterSelect(this, new TreeViewEventArgs(node));
            }
            else
            {
                treeView1.SelectedNode = null;
                this.treeView1_AfterSelect(this, new TreeViewEventArgs(null));
            }
        }

        private TreeNode FindParentNode(RuleSetData data)
        {
            if (data != null)
            {
                foreach (TreeNode node in treeView1.Nodes)
                {
                    if (String.CompareOrdinal(node.Text, data.Name) == 0)
                        return node;
                }
            }
            return null;
        }

        private void EnableApplicationFields(bool enable)
        {
            newButton.Enabled = enable;
            ruleSetNameCollectionLabel.Enabled = enable;

            if (!enable)
                this.EnableRuleSetFields(enable);
        }

        private void EnableRuleSetFields(bool enable)
        {
            editButton.Enabled = enable;
            deleteButton.Enabled = enable;
            copyButton.Enabled = enable;
            ruleSetNameBox.Enabled = enable;
            ruleSetNameLabel.Enabled = enable;
            majorVersionBox.Enabled = enable;
            majorVersionLabel.Enabled = enable;
            minorVersionBox.Enabled = enable;
            minorVersionLabel.Enabled = enable;
            getActivityButton.Enabled = enable;
            selectedActivityLabel.Enabled = enable;
            membersLabel.Enabled = enable;
            validateToolStripMenuItem.Enabled = enable;

            if (!enable)
                this.ClearRuleSetFields();
        }

        private void ClearRuleSetFields()
        {
            ruleSetNameBox.Text = "";
            majorVersionBox.Value = 0;
            minorVersionBox.Value = 0;
            activityBox.Text = "";
            membersBox.Items.Clear();
        }

        private void LoadAssemblyAndActivity()
        {
            if (selectedRuleSetData != null)
            {
                activityBox.Text = "";
                membersBox.Items.Clear();

                if (!String.IsNullOrEmpty(assemblyPath) && !String.IsNullOrEmpty(selectedRuleSetData.ActivityName)) 
                {
                    Assembly assembly = LoadAssembly(assemblyPath);

                    if (assembly != null)
                    {
                        Type activityType = LoadActivity(assembly, selectedRuleSetData.ActivityName);
                        if (activityType != null)
                        {
                            activityBox.Text = activityType.ToString();
                            selectedRuleSetData.Activity = activityType;
                            this.PopulateMembers();
                        }
                    }
                }
            }
        }

        internal static Assembly LoadAssembly(string assemblyPath)
        {
            Assembly assembly = null;
            if (!String.IsNullOrEmpty(assemblyPath)) 
            {
                try
                {
                    FileInfo assemblyFileInfo = new FileInfo(assemblyPath);
                    if (assemblyFileInfo.Exists)
                    {
                        assembly = Assembly.LoadFile(assemblyPath);
                    }
                    else
                    {
                        //try to load from the application directory
                        AssemblyName assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath));
                        assembly = Assembly.Load(assemblyName);
                    }
                }
                catch (FileLoadException ex)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Error loading assembly for the referenced Type at: \r\n\n'{0}' \r\n\n{1}", assemblyPath, ex.Message), "Assembly Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Error loading assembly for the referenced Type at: \r\n\n'{0}'", assemblyPath), "Assembly Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
            return assembly;
        }

        internal static Type LoadActivity(Assembly assembly, string activityName)
        {
            Type activityType = null;
            if (assembly != null && !String.IsNullOrEmpty(activityName))
            {
                try
                {
                    activityType = assembly.GetType(activityName, false);
                }
                catch (TypeLoadException ex)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Error loading the target Type from the assembly: \r\n\n{0}", ex.Message), "Type Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ReflectionTypeLoadException ex) 
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Error loading the target Type from the assembly: \r\n\n{0}", ex.LoaderExceptions[0].Message), "Type Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return activityType;
        }
     
        #endregion

        #region ActivitySelector

        private void getActivityButton_Click(object sender, EventArgs e)
        {
            ActivitySelector activitySelector = new ActivitySelector();

            if (selectedRuleSetData != null)
            {
                activitySelector.AssemblyPath = selectedRuleSetData.AssemblyPath;
                activitySelector.Activity = selectedRuleSetData.Activity;
                activitySelector.RuleSet = selectedRuleSetData.RuleSet;
            }
            
            activitySelector.ShowDialog();

            if (selectedRuleSetData != null && !String.IsNullOrEmpty(activitySelector.AssemblyPath) && activitySelector.Activity != null)
            {
                if (string.CompareOrdinal(selectedRuleSetData.AssemblyPath, activitySelector.AssemblyPath) != 0 || selectedRuleSetData.Activity != activitySelector.Activity)
                {
                    selectedRuleSetData.AssemblyPath = activitySelector.AssemblyPath;
                    selectedRuleSetData.Activity = activitySelector.Activity;
                    activityBox.Text = activitySelector.Activity.ToString();
                    this.PopulateMembers();
                    this.MarkDirty(selectedRuleSetData);
                }
            }
        }

        private void PopulateMembers()
        {
            if (selectedRuleSetData != null)
            {
                membersBox.Items.Clear();

                Type activityType = selectedRuleSetData.Activity;

                if (activityType != null)
                {
                    membersBox.Items.AddRange(GetMembers(activityType).ToArray());
                }
            }
        }

        internal static List<string> GetMembers(Type targetType)
        {
            List<string> members = new List<string>();

            if (targetType != null)
            {
                try
                {
                    PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (PropertyInfo property in properties)
                    {
                        members.Add(string.Format(CultureInfo.InvariantCulture, "{0}   ({1})", property.Name, property.PropertyType));
                    }

                    FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (FieldInfo field in fields)
                    {
                        members.Add(string.Format(CultureInfo.InvariantCulture, "{0}   ({1})", field.Name, field.FieldType));
                    }

                    members.Sort(); //sort all fields and properties as one, but exclude methods which will all be listed at the end

                    List<string> methodMembers = new List<string>();
                    MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (MethodInfo method in methods)
                    {
                        if (!method.Name.StartsWith("get_", StringComparison.Ordinal) && !method.Name.StartsWith("set_", StringComparison.Ordinal))
                        {
                            methodMembers.Add(method.ToString());
                        }
                    }

                    methodMembers.Sort();
                    members.AddRange(methodMembers);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Error loading members for the target Type: \r\n\n{0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return members;
        }

        #endregion

        #region RuleSet actions

        private void copyButton_Click(object sender, EventArgs e)
        {
            if (selectedRuleSetData != null)
            {
                RuleSetData newData = selectedRuleSetData.Clone();
                int newMajor;
                int newMinor;

                this.GenerateNewVersionInfo(selectedRuleSetData, out newMajor, out newMinor);
                newData.MajorVersion = newMajor;
                newData.MinorVersion = newMinor;

                this.MarkDirty(newData);
                this.AddRuleSetData(newData);
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (selectedRuleSetData != null)
            {
                if (selectedRuleSetData.Activity != null)
                {
                    RuleSetDialog ruleSetDialog = new RuleSetDialog(selectedRuleSetData.Activity, null, selectedRuleSetData.RuleSet);
                    DialogResult result = ruleSetDialog.ShowDialog();

                    if (result == DialogResult.OK) //check if they cancelled
                    {
                        selectedRuleSetData.RuleSet = ruleSetDialog.RuleSet;
                        this.MarkDirty(selectedRuleSetData);
                    }

                }
                else
                {
                    MessageBox.Show("You must first specify a target Activity for the RuleSet.", "RuleSet Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("You must first select a RuleSet.", "RuleSet Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            TreeNode parentNode = selectedNode.Parent;

            if (IsVersionNode(selectedNode) && selectedRuleSetData != null)
            {
                deletedRuleSetDataCollection.Add(selectedRuleSetData);
                this.MarkDirty(selectedRuleSetData);

                ruleSetDataDictionary.Remove(selectedNode);
                parentNode.Nodes.Remove(selectedNode);

                //if this was the only version node, remove the ruleset name node
                if (parentNode.Nodes.Count == 0)
                {
                    treeView1.Nodes.Remove(parentNode);
                }

                //selectedRuleSetData = null;
                //assemblyPath = null;
                this.SetSelectedNode(null);                
            }
        }

        private TreeNode GetTreeNodeForRuleSetData(RuleSetData data)
        {
            if (data != null)
            {
                Dictionary<TreeNode, RuleSetData>.Enumerator enumerator = ruleSetDataDictionary.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    RuleSetData otherData = enumerator.Current.Value;
                    if (String.CompareOrdinal(otherData.Name, data.Name) == 0 && otherData.MajorVersion == data.MajorVersion && otherData.MinorVersion == data.MinorVersion)
                        return enumerator.Current.Key;
                }
            }
            return null;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            RuleSetData newData = this.CreateRuleSetData(null);
            this.AddRuleSetData(newData);
        }

        private RuleSetData CreateRuleSetData(RuleSet ruleSet)
        {
            RuleSetData data = new RuleSetData();
            if (ruleSet != null)
            {
                data.Name = ruleSet.Name;
                data.RuleSet = ruleSet;
            }
            else
            {
                data.Name = this.GenerateRuleSetName();
                data.RuleSet = new RuleSet(data.Name);
            }
            data.MajorVersion = 1;
            this.MarkDirty(data);
            return data;
        }

        #endregion

        #region Event handlers

        // this is needed since the Validating events aren't fired for the fields if a menu item is selected
        // note that after this event fires, the user can still select a menu item, but the field values will have been reset (in this method)
        // could add similar logic for each tool strip item Click, but this is sufficient
        void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (selectedRuleSetData != null)
            {
                CancelEventArgs cancelArgs = new CancelEventArgs(false);
                this.ruleSetNameBox_Validating(this, cancelArgs);
                if (cancelArgs.Cancel)
                {
                    ruleSetNameBox.Text = selectedRuleSetData.Name; // name is invalid so set it back (since they can still navigate the menu)
                }

                cancelArgs.Cancel = false;
                this.majorVersionBox_Validating(this, cancelArgs);
                if (cancelArgs.Cancel)
                {
                    majorVersionBox.Value = selectedRuleSetData.MajorVersion;
                }

                cancelArgs.Cancel = false;
                this.minorVersionBox_Validating(this, cancelArgs);
                if (cancelArgs.Cancel)
                {
                    minorVersionBox.Value = selectedRuleSetData.MinorVersion;
                }
            }
        }

        void ruleSetNameBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            if (selectedRuleSetData != null)
            {
                if (String.IsNullOrEmpty(ruleSetNameBox.Text))
                {
                    MessageBox.Show("RuleSet Name cannot be empty.", "RuleSet Property Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ruleSetNameBox.Text = selectedRuleSetData.Name;
                }
                else if (ruleSetNameBox.Text != selectedRuleSetData.Name)
                {
                    RuleSetData duplicateData;
                    if (!this.IsDuplicateRuleSet(ruleSetNameBox.Text, selectedRuleSetData.MajorVersion, selectedRuleSetData.MinorVersion, out duplicateData)
                        || duplicateData == selectedRuleSetData)
                    {
                        selectedRuleSetData.Name = ruleSetNameBox.Text;
                        this.MarkDirty(selectedRuleSetData);

                        List<RuleSetData> ruleSetDataCollection = new List<RuleSetData>();
                        foreach (RuleSetData data in ruleSetDataDictionary.Values)
                            ruleSetDataCollection.Add(data);

                        this.BuildTree(ruleSetDataCollection);
                        this.SetSelectedNode(this.GetTreeNodeForRuleSetData(selectedRuleSetData));
                    }
                    else
                    {
                        MessageBox.Show("A RuleSet with the same name and version numbers already exists.", "RuleSet Property Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            }
        }

        void majorVersionBox_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            int majorInt = Convert.ToInt32(majorVersionBox.Value);
            if (selectedRuleSetData != null && treeView1.SelectedNode != null && majorInt != selectedRuleSetData.MajorVersion)
            {
                if (majorInt > 0)
                {
                    RuleSetData duplicateData;
                    if (!this.IsDuplicateRuleSet(selectedRuleSetData.Name, Convert.ToInt32(majorVersionBox.Value), selectedRuleSetData.MinorVersion, out duplicateData)
                        || duplicateData == selectedRuleSetData)
                    {
                        selectedRuleSetData.MajorVersion = majorInt;
                        this.MarkDirty(selectedRuleSetData);

                        TreeNode selectedNode = treeView1.SelectedNode;
                        selectedNode.Text = VersionTreeNodeText(selectedRuleSetData.MajorVersion, selectedRuleSetData.MinorVersion);
                        treeView1.Sort();
                        this.SetSelectedNode(selectedNode);
                    }
                    else
                    {
                        MessageBox.Show("A RuleSet with the same name and version numbers already exists.", "RuleSet Property Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                else
                {
                    MessageBox.Show("Major version number must be greater than 0", "RuleSet Property Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                }
            }
        }

        void minorVersionBox_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            int minorInt = Convert.ToInt32(minorVersionBox.Value);
            if (selectedRuleSetData != null && treeView1.SelectedNode != null && minorInt != selectedRuleSetData.MinorVersion)
            {
                RuleSetData duplicateData;
                if (!this.IsDuplicateRuleSet(selectedRuleSetData.Name, selectedRuleSetData.MajorVersion, Convert.ToInt32(minorVersionBox.Value), out duplicateData)
                    || duplicateData == selectedRuleSetData)
                {
                    selectedRuleSetData.MinorVersion = minorInt;
                    this.MarkDirty(selectedRuleSetData);

                    TreeNode selectedNode = treeView1.SelectedNode;
                    selectedNode.Text = VersionTreeNodeText(selectedRuleSetData.MajorVersion, selectedRuleSetData.MinorVersion);
                    this.treeView1.Sort();
                    this.SetSelectedNode(selectedNode);
                }
                else
                {
                    MessageBox.Show("A RuleSet with the same name and version numbers already exists.", "RuleSet Property Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        private static string VersionTreeNodeText(int majorVersion, int minorVersion)
        {
            return String.Format(CultureInfo.InvariantCulture, "Version {0}.{1}", majorVersion, minorVersion);
        }

        #endregion

        #region Other

        private static bool IsVersionNode(TreeNode node)
        {
            if (node != null)
                return node.Text.StartsWith("Version", StringComparison.Ordinal);
            else
                return false;
        }

        private void AddRuleSetData(RuleSetData ruleSetData)
        {
            if (ruleSetData != null)
            {
                TreeNode parentNode = this.FindParentNode(ruleSetData);

                if (parentNode == null)
                {
                    parentNode = new TreeNode(ruleSetData.Name);
                    treeView1.Nodes.Add(parentNode);
                }

                TreeNode newVersionNode = new TreeNode(VersionTreeNodeText(ruleSetData.MajorVersion, ruleSetData.MinorVersion));
                parentNode.Nodes.Add(newVersionNode);
                treeView1.Sort();
                ruleSetDataDictionary.Add(newVersionNode, ruleSetData);
                this.SetSelectedNode(newVersionNode);
            }
        }

        private void MarkDirty(RuleSetData data)
        {
            if (data != null)
                data.Dirty = true;
            
            dirty = true;
        }

        private bool IsDuplicateRuleSet(string name, int majorVersion, int minorVersion, out RuleSetData duplicateRuleSetData)
        {
            foreach (RuleSetData data in ruleSetDataDictionary.Values)
            {
                if (String.CompareOrdinal(data.Name, name) == 0 && data.MajorVersion == majorVersion && data.MinorVersion == minorVersion)
                {
                    duplicateRuleSetData = data;
                    return true;
                }
            }
            duplicateRuleSetData = null;
            return false;
        }

        private string GenerateRuleSetName()
        {
            string namePrefix = "RuleSet";
            string newName = "";
            bool uniqueNameNotFound = true;
            int counter = 0;

            while (uniqueNameNotFound)
            {
                counter++;
                uniqueNameNotFound = false;
                newName = namePrefix + counter.ToString(CultureInfo.InvariantCulture);
                uniqueNameNotFound = this.IsDuplicateRuleSetName(newName);
            }

            return newName;
        }

        private void GenerateNewVersionInfo(RuleSetData currentRuleSetData, out int newMajorVersion, out int newMinorVersion)
        {
            List<RuleSetData> rsdOfInterest = new List<RuleSetData>();
            foreach (RuleSetData data in ruleSetDataDictionary.Values)
            {
                if (data.Name == currentRuleSetData.Name && ((data.MajorVersion > currentRuleSetData.MajorVersion) ||  (data.MajorVersion == currentRuleSetData.MajorVersion && data.MinorVersion > currentRuleSetData.MinorVersion)))
                    rsdOfInterest.Add(data);
            }
            rsdOfInterest.Sort(); 

            bool nextMajorTaken = false;
            int lastMajorUsed = currentRuleSetData.MajorVersion;
            int lastMinorUsed = currentRuleSetData.MinorVersion;
            foreach (RuleSetData data in rsdOfInterest)
            {
                lastMajorUsed = data.MajorVersion;

                if (data.MajorVersion == currentRuleSetData.MajorVersion)
                    lastMinorUsed = data.MinorVersion;

                if (data.MajorVersion == currentRuleSetData.MajorVersion + 1)
                    nextMajorTaken = true;
            }

            if (!nextMajorTaken)
            {
                newMajorVersion = currentRuleSetData.MajorVersion + 1;
                newMinorVersion = 0;
            }
            else if (lastMinorUsed < maxMinorVersions - 1)
            {
                newMajorVersion = currentRuleSetData.MajorVersion;
                newMinorVersion = lastMinorUsed + 1;
            }
            else if (lastMajorUsed < maxMajorVersions - 1)
            {
                newMajorVersion = lastMajorUsed + 1;
                newMinorVersion = 0;
            }
            else
            {
                newMajorVersion = currentRuleSetData.MajorVersion;
                newMinorVersion = currentRuleSetData.MinorVersion;
                MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Only {0} major versions are allowed for a single RuleSet name.  \r\nYou must manually change the version information.", maxMajorVersions), "RuleSet Property Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetNextMajorVersion(string ruleSetName)
        {
            int highestMajorVersionNumber = 0;

            foreach (RuleSetData data in ruleSetDataDictionary.Values)
            {
                if (String.CompareOrdinal(data.Name, ruleSetName) == 0 && data.MajorVersion > highestMajorVersionNumber)
                    highestMajorVersionNumber = data.MajorVersion;
            }

            return highestMajorVersionNumber + 1;
        }

        private bool IsDuplicateRuleSetName(string name)
        {
            foreach (RuleSetData data in ruleSetDataDictionary.Values)
            {
                if (String.CompareOrdinal(data.Name,name) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool GetThisType(string rulesFileDirectoryPath, string rulesFileName)
        {
            bool successfulLoad = false;

            if (!string.IsNullOrEmpty(rulesFileDirectoryPath) && selectedRuleSetData != null)
            {
                string thisTypeAssemblyPath = rulesFileDirectoryPath + Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar + "Debug";
                if (Directory.Exists(thisTypeAssemblyPath))
                {
                    assemblyPath = thisTypeAssemblyPath;
                    string[] fileNames = Directory.GetFiles(thisTypeAssemblyPath);
                    Dictionary<Type,string> candidateThisTypes = new Dictionary<Type,string>();

                    //try and automatically locate the Type referenced by this ruleset
                    foreach (string fileName in fileNames)
                    {
                        if (fileName.EndsWith("dll", StringComparison.Ordinal) || fileName.EndsWith("exe", StringComparison.Ordinal))
                        {
                            Assembly assembly = null;
                            try
                            {
                                assembly = Assembly.LoadFile(fileName); //this will skip the load if it's already been loaded, which is a problem if you point to a different assembly with the same version number
                            }
                            catch (Exception) //ignore this assembly then
                            {
                            }

                            if (assembly != null)
                            {
                                foreach (Type type in assembly.GetTypes())
                                {
                                    try
                                    {
                                        RuleValidation validation = new RuleValidation(type, null);
                                        if (selectedRuleSetData.RuleSet.Validate(validation))
                                            candidateThisTypes.Add(type,fileName); // type matches the ruleset members
                                    }
                                    catch (Exception) //error creating RuleValidation or doing validation so ignore this type
                                    {
                                    }
                                }
                            }
                        }
                    }

                    if (candidateThisTypes.Count == 0) //no matching types found so prompt the user
                    {
                        successfulLoad = this.PromptForThisType(rulesFileDirectoryPath);
                    }
                    else if (candidateThisTypes.Count == 1) //one matching Type in the assemblies in the default path, so just use it
                    {
                        IEnumerator enumerator = candidateThisTypes.Keys.GetEnumerator();
                        enumerator.MoveNext();
                        selectedRuleSetData.Activity = enumerator.Current as Type;
                        selectedRuleSetData.AssemblyPath = candidateThisTypes[selectedRuleSetData.Activity];
                        successfulLoad = true;
                    }
                    else //more than one matching Type
                    {
                        //see if there is a single Type with the same name as the .rules file
                        Dictionary<Type, string> candidateThisTypesMatchingName = new Dictionary<Type, string>();
                        foreach (Type type in candidateThisTypes.Keys)
                        {
                            if (type.Name == Path.GetFileNameWithoutExtension(rulesFileName))
                                candidateThisTypesMatchingName.Add(type, candidateThisTypes[type]);
                        }
                        if (candidateThisTypesMatchingName.Count == 1)
                        {
                            IEnumerator enumerator = candidateThisTypesMatchingName.Keys.GetEnumerator();
                            enumerator.MoveNext();
                            selectedRuleSetData.Activity = enumerator.Current as Type;
                            selectedRuleSetData.AssemblyPath = candidateThisTypesMatchingName[selectedRuleSetData.Activity];
                            successfulLoad = true;
                        }
                        else
                        {
                            successfulLoad = this.PromptForThisType(thisTypeAssemblyPath);
                        }
                    }
                }
                else
                {
                    successfulLoad = this.PromptForThisType(rulesFileDirectoryPath);
                }
            }
            return successfulLoad;
        }

        private bool PromptForThisType(string startingDirectory)
        {
            bool successfulLoad = false;
           
            if (selectedRuleSetData != null)
            {
                ActivitySelector activitySelector = new ActivitySelector();
                activitySelector.RuleSet = selectedRuleSetData.RuleSet;
                if (!String.IsNullOrEmpty(startingDirectory))
                    activitySelector.InitialDirectory = startingDirectory;

                activitySelector.ShowDialog();
                if (!string.IsNullOrEmpty(activitySelector.AssemblyPath))
                    selectedRuleSetData.AssemblyPath = activitySelector.AssemblyPath;
                if (activitySelector.Activity != null)
                {
                    selectedRuleSetData.Activity = activitySelector.Activity;
                    successfulLoad = true;
                }
            }

            return successfulLoad;
        }

        #endregion

    }

    internal class TreeSortClass : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            string versionNodePrefix = "Version";
            TreeNode xNode = x as TreeNode;
            TreeNode yNode = y as TreeNode;

            if (xNode.Text.StartsWith(versionNodePrefix, StringComparison.Ordinal))
            {
                string xVersionString = xNode.Text.Substring(versionNodePrefix.Length);
                string yVersionString = yNode.Text.Substring(versionNodePrefix.Length);

                int xMajor = Int32.Parse(xVersionString.Substring(0,xVersionString.IndexOf(".", StringComparison.Ordinal)), CultureInfo.InvariantCulture);
                int xMinor = Int32.Parse(xVersionString.Substring(xVersionString.IndexOf(".", StringComparison.Ordinal) + 1), CultureInfo.InvariantCulture);
                int yMajor = Int32.Parse(yVersionString.Substring(0, yVersionString.IndexOf(".", StringComparison.Ordinal)), CultureInfo.InvariantCulture);
                int yMinor = Int32.Parse(yVersionString.Substring(yVersionString.IndexOf(".", StringComparison.Ordinal) + 1), CultureInfo.InvariantCulture);

                if (xMajor != yMajor)
                {
                    return xMajor - yMajor;
                }
                else if (xMinor != yMinor)
                {
                    return xMinor - yMinor;
                }
                else
                {
                    MessageBox.Show("Two RuleSets exist with the same name and version numbers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
            else
            {
                return String.CompareOrdinal(xNode.Text, yNode.Text);
            }
        }
    }
}
