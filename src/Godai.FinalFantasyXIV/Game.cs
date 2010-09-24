/**

    Godai.FinalFantasyXIV is a .NET library for interacting with Final Fantasy XIV

    Copyright (c) 2010 五大 (godai)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

**/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using log4net;

namespace Godai.FinalFantasyXIV
{
    public static class Game
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Game));

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr address, IntPtr outputBuffer, UIntPtr bufferSize, out UIntPtr numberOfBytesRead);

        /// <summary>
        /// Gets a collection of FFXIV processes
        /// </summary>
        /// <returns>IEnumerable containing any running FFXIV processes</returns>
        public static IEnumerable<Process> GetProcesses()
        {
            return
                (from process in Process.GetProcessesByName("ffxivgame") // TODO: get name from config file
                 select process);
        }

        /// <summary>
        /// Reads a block of memory from the FFXIV process
        /// </summary>
        /// <param name="process">FFXIV process to read from</param>
        /// <param name="address">Starting address to read from</param>
        /// <param name="numBytesToRead">Number of bytes to read</param>
        /// <param name="data">Byte array to copy the memory into; the size of the array should be >= numBytesToRead</param>
        /// <returns>Number of bytes actually read</returns>
        public static int ReadMemory(Process process, IntPtr address, int numBytesToRead, byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (process == null)
            {
                throw new ArgumentNullException("process");
            }

            if (address == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid address", "address");
            }

            if (numBytesToRead == 0)
            {
                return 0;
            }

            if (data.Length < numBytesToRead)
            {
                throw new ArgumentException("data.Length should be >= numBytesToRead", "data");
            }

            var buffer = Marshal.AllocHGlobal(numBytesToRead);

            try
            {
                UIntPtr bytesRead;
                var bytesToRead = (UIntPtr)((uint)numBytesToRead);

                if (!ReadProcessMemory(process.Handle, address, buffer, bytesToRead, out bytesRead))
                {
                    var error = Marshal.GetLastWin32Error();

                    if (Log.IsErrorEnabled)
                    {
                        Log.ErrorFormat("Error reading process memory. Win32 Error is: {0}", error);
                    }

                    if (error != 299) // 299 = ERROR_PARTIAL_COPY, return data even if it's partial
                    {
                        return 0;
                    }
                }

                var read = (int)bytesRead.ToUInt32();
                Marshal.Copy(buffer, data, 0, read);

                return read;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
