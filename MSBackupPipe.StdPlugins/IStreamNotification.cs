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

namespace MSBackupPipe.StdPlugins
{
    public interface IStreamNotification
    {
        /// <summary>
        /// The estimated size of all streams combined.  It does not need to be 100% accurate,
        /// as this is used for UI notification only, though users would like accuracy.
        /// </summary>
        long EstimatedBytes { get; set; }

        /// <summary>
        /// Gets the thread ID to use in the UpdateBytesProcessed method. 
        /// </summary>
        int GetThreadId();

        /// <summary>
        /// Whenever *any* stream reads or writes data, you must call this method
        /// so that the engine can keep track of the progress.
        /// </summary>
        /// <param name="additionalBytesProcessed"></param>
        /// <returns>The *suggested* duration to be notified again</returns>
        TimeSpan UpdateBytesProcessed(long totalBytesProcessedByThread, int threadId);

    }
}
