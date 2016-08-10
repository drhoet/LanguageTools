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
            instantLookup = InstantLookup;
            ((MSOutlook.InspectorEvents_Event)Inspector).Activate += InspectorWrapper_Activate;
            ((MSOutlook.InspectorEvents_Event)Inspector).Deactivate += InspectorWrapper_Deactivate;
            ((MSOutlook.InspectorEvents_Event)inspector).Close += InspectorWrapper_Close;

            instantLookup.OnLemmaFound += InstantLookup_OnLemmaFound;

            taskPane = Globals.ThisAddIn.CustomTaskPanes.Add(new LookupPane(), "German Grammar", inspector);
            taskPane.VisibleChanged += TaskPane_VisibleChanged;
        }

        private void InstantLookup_OnLemmaFound(object sender, System.Collections.Generic.List<Backend.Lemma> found, MSOutlook.Inspector document)
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
        }

        private void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            //Globals.Ribbons[inspector].LanguageToolsRibbon.btnToggleLookupPane.Checked = taskPane.Visible;
        }

        private void InspectorWrapper_Close()
        {
            instantLookup.Paused = true;
            if (taskPane != null)
            {
                Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
            }

            taskPane = null;
            Globals.ThisAddIn.InspectorWrappers.Remove(inspector);
            ((MSOutlook.InspectorEvents_Event)inspector).Close -= InspectorWrapper_Close;
            ((MSOutlook.InspectorEvents_Event)inspector).Deactivate -= InspectorWrapper_Deactivate;
            ((MSOutlook.InspectorEvents_Event)inspector).Activate -= InspectorWrapper_Activate;

            instantLookup = null;
            inspector = null;
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
