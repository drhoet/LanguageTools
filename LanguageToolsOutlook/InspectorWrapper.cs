using LanguageTools.Common;
using Microsoft.Office.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSOutlook = Microsoft.Office.Interop.Outlook;

namespace LanguageTools.Outlook
{
    public class InspectorWrapper
    {
        private MSOutlook.Inspector inspector;
        private CustomTaskPane taskPane;

        public InspectorWrapper(MSOutlook.Inspector Inspector)
        {
            inspector = Inspector;
            ((MSOutlook.InspectorEvents_Event)inspector).Close += InspectorWrapper_Close;

            taskPane = Globals.ThisAddIn.CustomTaskPanes.Add(new LookupPane(), "German Grammar", inspector);
            taskPane.VisibleChanged += TaskPane_VisibleChanged;
        }

        private void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            //Globals.Ribbons[inspector].LanguageToolsRibbon.btnToggleLookupPane.Checked = taskPane.Visible;
        }

        private void InspectorWrapper_Close()
        {
            if (taskPane != null)
            {
                Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
            }

            taskPane = null;
            Globals.ThisAddIn.InspectorWrappers.Remove(inspector);
            ((MSOutlook.InspectorEvents_Event)inspector).Close -= InspectorWrapper_Close;
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
