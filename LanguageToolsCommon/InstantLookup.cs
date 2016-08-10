using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace LanguageTools.Common
{
    public class InstantLookup<T>
    {
        public ActiveTextStrategy<T> ActiveTextStrategy { get; private set; }
        public LemmaRepository LemmaRepository { get; private set; }

        private bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                lookupTimer.Enabled = value;
            }
        }

        public delegate void LemmaFoundEventHandler(object sender,  List<Lemma> found, T document);
        public event LemmaFoundEventHandler OnLemmaFound;

        private Timer lookupTimer;
        private string lastLookup = null;

        public InstantLookup(ActiveTextStrategy<T> strategy, int interval, LemmaRepository repo)
        {
            ActiveTextStrategy = strategy;
            enabled = false;

            lookupTimer = new Timer();
            lookupTimer.Interval = interval;
            lookupTimer.Tick += LookupActiveText;

            LemmaRepository = repo;
        }

        public void LookupActiveText(object sender, EventArgs eventArgs)
        {
            bool timerRunning = lookupTimer.Enabled;
            lookupTimer.Stop();

            string searchFor = ActiveTextStrategy.FindActiveWord();
            if (searchFor.Length > 0)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += bgw_DoWork;
                worker.RunWorkerCompleted += bgw_WorkCompleted;
                SearchParams args = new SearchParams();
                args.SearchLemma = searchFor;
                args.TargetWindow = ActiveTextStrategy.FindActiveDocument();
                worker.RunWorkerAsync(args);
            }

            if (timerRunning)
                lookupTimer.Start();
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            SearchParams args = (SearchParams)e.Argument;
            string value = args.SearchLemma;
            if (value != null && value != lastLookup)
            {
                lastLookup = value;
                List<Lemma> found = LemmaRepository.FindAll(new GermanBaseLemmaSpecification(value));
                if (found.Count == 0)
                {
                    found.AddRange(LemmaRepository.FindAll(new GermanCompositionEndLemmaSpecification(value)));
                }
                args.Found = found;
            }
            e.Result = args;
        }

        private void bgw_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchParams args = (SearchParams)e.Result;
            List<Lemma> found = args.Found;
            if (found != null && found.Count > 0)
            {
                OnLemmaFound?.Invoke(this, found, args.TargetWindow);
            }
        }

        private struct SearchParams
        {
            public T TargetWindow { get; set; }
            public string SearchLemma { get; set; }
            public List<Lemma> Found { get; set; }
        }
    }
}
