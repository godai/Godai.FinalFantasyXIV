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
using System.Linq;
using System.Text;
using log4net;
using NUnit.Framework;

namespace Godai.FinalFantasyXIV.Tests
{
    [TestFixture]
    public class GameFixture
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameFixture));

        [Test]
        public void GetProcess()
        {
            var process = Game.GetProcesses().FirstOrDefault();
            Assert.That(process != null);
        }

        [Test]
        public void ReadMemory()
        {
            var process = Game.GetProcesses().FirstOrDefault();
            Assert.That(process != null);

            if (process.Modules.Count == 0)
            {
                Assert.Fail();
            }

            Log.InfoFormat("BaseAddress: {0}", process.MainModule.BaseAddress.ToInt32().ToString("X8"));

            var data = new byte[32];

            Assert.That(Game.ReadMemory(process, process.MainModule.BaseAddress, 32, data) == 32);

            foreach (var b in data)
            {
                Log.Info(b.ToString("X2"));
            }
        }
    }
}
