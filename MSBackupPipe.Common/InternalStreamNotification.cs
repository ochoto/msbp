﻿/*
	Copyright 2009 Clay Lenhart <clay@lenharts.net>


	This file is part of MSSQL Compressed Backup.

    MSSQL Compressed Backup is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using MSBackupPipe.StdPlugins;

namespace MSBackupPipe.Common
{
    internal class InternalStreamNotification : IStreamNotification
    {
        private readonly IUpdateNotification mExternalNotification;
        private long mEstimatedBytes = 1;
        private readonly IList<long> mBytesProcessed = new List<long>();
        private int mLastThreadIdUsed = -1;

        public InternalStreamNotification(IUpdateNotification notification)
        {
            mExternalNotification = notification;
        }

        public long EstimatedBytes
        {
            get
            {
                lock (this)
                {
                    return mEstimatedBytes;
                }
            }
            set
            {
                lock (this)
                {
                    mEstimatedBytes = Math.Max(1, value);
                }
            }
        }

        public int GetThreadId()
        {
            lock (this)
            {
                mLastThreadIdUsed++;
                mBytesProcessed.Add(0);
                return mLastThreadIdUsed;
            }
        }

        public TimeSpan UpdateBytesProcessed(long totalBytesProcessedByThread, int threadId)
        {
            if (totalBytesProcessedByThread < 0)
            {
                throw new ArgumentException(string.Format("totalBytesProcessedByThread must be non-negative. value={0}", totalBytesProcessedByThread));
            }

            
            float bytesProcessed;
            float size;
            lock (this)
            {
                mBytesProcessed[threadId] = totalBytesProcessedByThread;
                long bytesProcessedSum = 0;
                foreach (long bytes in mBytesProcessed)
                {
                    bytesProcessedSum += bytes;
                }
                bytesProcessed = bytesProcessedSum;
                size = mEstimatedBytes;
            }

            TimeSpan suggestedWait = mExternalNotification.OnStatusUpdate(Math.Max(0f, Math.Min(1f, bytesProcessed / size)));
            return suggestedWait;
            //return TimeSpan.FromMilliseconds(suggestedWait.TotalMilliseconds / 2);
        }


    }
}
