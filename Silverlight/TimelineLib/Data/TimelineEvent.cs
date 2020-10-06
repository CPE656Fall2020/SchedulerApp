/* 
 * Copyright (c) 2010, Andriy Syrov
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided 
 * that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the 
 * following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and 
 * the following disclaimer in the documentation and/or other materials provided with the distribution.
 *
 * Neither the name of Andriy Syrov nor the names of his contributors may be used to endorse or promote 
 * products derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY 
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN 
 * IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 *   
 */
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace TimelineLibrary
{
    /// <summary>
    /// Class that represents event element read from xml file</summary>
    public class TimelineEvent: INotifyPropertyChanged, IComparer<TimelineEvent>
    {
        private int                                     m_row = -1;
        private int                                     m_rowOverride = -1;
        private double                                  m_widthOverride = -1;
        private double                                  m_heightOverride = -1;
        private double                                  m_topOverride = -1;
        private string                                  m_id;
        private string                                  m_title;
        private string                                  m_description;
        private DateTime                                m_start;
        private DateTime                                m_end;
        private bool                                    m_isDuration;
        private string                                  m_eventColor;
		private bool									m_selected;

        public event PropertyChangedEventHandler        PropertyChanged;

        protected void FirePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
                FirePropertyChanged("Id");
            }
        }
        
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
                FirePropertyChanged("Title");
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
                FirePropertyChanged("Description");
            }
        }
        
        public DateTime StartDate
        {
            get
            {
                return m_start;
            }
            set
            {
                m_start = value;
                FirePropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get
            {
                return m_end;
            }
            set
            {
                m_end = value;
                FirePropertyChanged("EndDate");
            }
        }

        public bool IsDuration
        {
            get
            {
                return m_isDuration;
            }
            set
            {
                m_isDuration = value;
                FirePropertyChanged("IsDuration");
            }
        }

        public string EventColor
        {
            get
            {
                return m_eventColor;
            }
            set
            {
                m_eventColor = value;
                FirePropertyChanged("EventColor");
            }
        }

        public int RowOverride
        {
            get
            {
                return m_rowOverride;
            }

            set
            {
                m_rowOverride = value;
				FirePropertyChanged("RowOverride");
            }
        }

        public double WidthOverride
        {
            get
            {
                return m_widthOverride;
            }

            set
            {
                m_widthOverride = value;
				FirePropertyChanged("WidthOverride");
            }
        }

        public double HeightOverride
        {
            get
            {
                return m_heightOverride;
            }

            set
            {
                m_heightOverride = value;
				FirePropertyChanged("HeightOverride");
            }
        }

        public double TopOverride
        {
            get
            {
                return m_topOverride;
            }

            set
            {
                m_topOverride = value;
				FirePropertyChanged("TopOverride");
            }
        }

        public int Row
        {
            get
            {
                return m_row;
            }
            set
            {
                m_row = value;
                FirePropertyChanged("Row");
            }
        }

		public bool Selected
		{
			get
			{
				return m_selected;
			}
			set 
			{
				m_selected = value;
				FirePropertyChanged("Selected");
			}
		}

        public int Compare(TimelineEvent x, TimelineEvent y)
        {
 	        if (x.StartDate == y.EndDate)
            {
                return 0;
            }
            return x.StartDate > y.EndDate ? -1 : 1;
        }

        public bool InRange(
            DateTime                                    from,
            DateTime                                    to
        )
        {
            return !((StartDate < from && EndDate < from) || (StartDate > to && EndDate > to));
        }
    }

    public static class Extensions
    {
        public static void ForEach<T>(
            this IEnumerable<T>                         lst,
	        Action<T>                                   action
        )
        {
            if (lst != null)
            { 
                foreach (T e in lst)
                {
                    action(e);
                }
            }
        }

        public static IEnumerable<TimelineEvent> InRange(
            this IEnumerable<TimelineEvent>             events,
            DateTime                                    from,
            DateTime                                    to
        )
        {
            foreach (TimelineEvent e in events)
            {
                if (e.InRange(from, to))
                {
                    yield return e;
                }
            }
        }

        public static IEnumerable<TimelineEvent> NotInRange(
            this IEnumerable<TimelineEvent>             events,
            DateTime                                    from,
            DateTime                                    to
        )
        {
            foreach (TimelineEvent e in events)
            {
                if (!e.InRange(from, to))
                {
                    yield return e;
                }
            }
        }
    }

    public class TimelineEventStore
    {
        private const int                               INDEX_CREATION_DELAY = 3000;
        private struct DateNode: IComparer<DateNode>
        {
            public DateTime                             Date;
            public TimelineEvent                        Event;

            public int Compare(DateNode x, DateNode y)
            {
                return DateTime.Compare(x.Date, y.Date);
            }
        }

        private DateNode[]                              m_byStart;
        private bool                                    m_initialized;
        private bool                                    m_hasDuration;

        public TimelineEventStore(
            List<TimelineEvent>                         events,
            bool                                        sorted = false
        )
        {
            Initialize(events, sorted);
        }

        public void Initialize(
            List<TimelineEvent>                         events,
            bool                                        sorted = false
        )
        {
            Debug.Assert(events != null);

            if (!sorted)
            { 
                events.Sort(new Comparison<TimelineEvent>((l, r) => DateTime.Compare(l.StartDate, r.StartDate)));
            }
            All = events;

            m_hasDuration = false;
            m_initialized = false;

            if (All.Count > 0)
            {
                SingleDelayedInvoke.Invoke(new Action(() =>
                {
                    InitSearch();
                    m_initialized = true;
                }), INDEX_CREATION_DELAY);
            }
            else
            {
                m_byStart = null;
                m_initialized = false;
            }
        }

        public List<TimelineEvent> All { get; private set; }

        public IEnumerable<TimelineEvent> QuickSearch(
            DateTime                                    from,
            DateTime                                    to,
            bool                                        all = true
        )
        {
            int                                         index;
            DateNode                                    val;
            bool                                        enumAll;

            val.Date = from;
            val.Event = null;

            enumAll = !m_initialized;

            if (enumAll || (all && m_hasDuration))
            {
                foreach (var e in All.InRange(from, to))
                {
                    yield return e;
                }
            }
            else
            {
                val.Date = from;
                val.Event = null;

                index = Array.BinarySearch(m_byStart, val, val);
                if (index < 0)
                {
                    index = ~index;
                }

                for (int i = index; i < m_byStart.Length && m_byStart[i].Date <= to; ++i)
                {
                    yield return m_byStart[i].Event;
                }
            }
        }

        private void InitSearch(
        )
        {
            IEnumerable<TimelineEvent>                  enumeration;
            int                                         i;

            enumeration = All;
            m_byStart = new DateNode[All.Count];

            i = 0;
            foreach (TimelineEvent e in enumeration)
            {
                m_byStart[i].Date = e.StartDate;
                m_byStart[i].Event = e;
                if (e.IsDuration)
                {
                    m_hasDuration = true;
                }
                ++i;
            }
        }
    }
}
    