using LanguageTools.Common;
using Microsoft.Office.Tools;
using System;
using MSOutlook = Microsoft.Office.Interop.Outlook;

namespace LanguageTools.Outlook
{
    public class InspectorWrapper
    {
        private MSOutlook.Inspector inspector;
        private InstantLookup<MSOutlook.Inspector> instantLookup;
        private CustomTaskPane taskPane;

        public InspectorWrapper(MSOutlook.Inspector Inspector, InstantLookup<MSOutlook.Inspector> InstantLookup)
        {
            inspector = Inspector;
            ((MSOutlook.InspectorEvents_Event)inspector).Activate += InspectorWrapper_Activate;
            ((MSOutlook.InspectorEvents_Event)inspector).Deactivate += InspectorWrapper_Deactivate;
            ((MSOutlook.InspectorEvents_Event)inspector).Close += InspectorWrapper_Close;

            instantLookup = InstantLookup;
            instantLookup.OnLemmaFound += InstantLookup_OnLemmaFound;

            taskPane = Globals.ThisAddIn.CustomTaskPanes.Add(new LookupPane(), "German Grammar", inspector);
            taskPane.Visible = Properties.Settings.Default.LookupPaneVisible;
            taskPane.Width = Properties.Settings.Default.LookupPaneWidth;
            taskPane.Control.Tag = taskPane;
            taskPane.Control.SizeChanged += Control_SizeChanged;
        }

        private void InstantLookup_OnLemmaFound(object sender, System.Collections.Generic.List<Backend.Noun> found, MSOutlook.Inspector document)
        {
            if (document == inspector)
            {
                LookupPane lookupPane = (LookupPane)taskPane.Control;
                lookupPane.Invoke((Action)delegate
                {
                    lookupPane.Item = found[0];
                    lookupPane.lbxResults.DataSource = found;
                });
            }
        }

        private void InspectorWrapper_Deactivate()
        {
            instantLookup.Paused = true;
        }

        private void InspectorWrapper_Activate()
        {
            instantLookup.Paused = false;

            // when opening the window the first time, this will be null...
            if (Globals.Ribbons[inspector] != null && Globals.Ribbons[inspector].LanguageToolsRibbon != null)
            {
                instantLookup.Enabled = Globals.Ribbons[inspector].LanguageToolsRibbon.btnToggleInstantLookup.Checked;
            }
            else
            {
                instantLookup.Enabled = Properties.Settings.Default.InstantLookupEnabled;
            }
        }

        private void InspectorWrapper_Close()
        {
            taskPane.Control.SizeChanged -= Control_SizeChanged;
            Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
            taskPane = null;

            instantLookup.Paused = true;
            instantLookup.OnLemmaFound -= InstantLookup_OnLemmaFound;
            instantLookup = null;

            Globals.ThisAddIn.InspectorWrappers.Remove(inspector);
            ((MSOutlook.InspectorEvents_Event)inspector).Close -= InspectorWrapper_Close;
            ((MSOutlook.InspectorEvents_Event)inspector).Deactivate -= InspectorWrapper_Deactivate;
            ((MSOutlook.InspectorEvents_Event)inspector).Activate -= InspectorWrapper_Activate;
            inspector = null;

            Properties.Settings.Default.Save(); //need to do this here, as I can't do it in the shutdown of the addon...
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            // the tag was set in AddTaskPane. Ugly hack, I know.
            Properties.Settings.Default.LookupPaneWidth = ((CustomTaskPane)((LookupPane)sender).Tag).Width;
        }

        public CustomTaskPane CustomTaskPane
        {
            get
            {
                return taskPane;
            }
        }
    }
}
