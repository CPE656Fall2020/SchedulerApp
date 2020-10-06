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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace TimelineLibrary
{
    /// <summary>
    /// This class aggregates resize events of TimelineBand controls and loadcomplete of datasources, so
    /// that we know when all controls are resized and all xml files downloaded.</summary>
    public class DataControlNotifier
    {
        List<TimelineBand>                              m_elements;
        private int                                     m_sizeCount;
        private bool                                    m_started;
        
        public event EventHandler                       LoadComplete;

        public DataControlNotifier()
        {
            m_elements = new List<TimelineBand>();
        }

        public DataControlNotifier(List<TimelineBand> bands)
        {
            m_elements = bands;

            foreach (FrameworkElement e in m_elements)
            {
                if (e.ActualWidth != 0)
                {
                    m_sizeCount++;
                    CheckCompleted();
                }
            }
        }

        public void AddElement(TimelineBand band)
        {
            Debug.Assert(band != null);

            band.SizeChanged += OnSizeChanged;
            m_elements.Add(band);
        }

        public void Start()
        {
            Debug.Assert(LoadComplete != null);
            Utilities.Trace(this);

            m_started = true;
        }

        /// <summary>
        /// Checks that all controls resized and all data received</summary>
        public void CheckCompleted()
        {
            if (m_started && m_sizeCount == m_elements.Count && LoadComplete != null)
            {
                Utilities.Trace(this, "All data collected and all controls resized.");
                LoadComplete(this, new EventArgs());
                m_started = false;
                m_sizeCount = 0;
            }
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement) sender).SizeChanged -= OnSizeChanged;
            ++m_sizeCount;

            CheckCompleted();
        }
    }
}
