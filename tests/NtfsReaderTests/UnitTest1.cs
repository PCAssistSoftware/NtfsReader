using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Filesystem.Ntfs;
using System.IO;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace NtfsReaderTests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void NtfsReader_ShouldRead()
		{
			DriveInfo driveInfo = new DriveInfo("C:\\");

			var ntfsReader = new NtfsReader(driveInfo, RetrieveMode.Minimal);
		}

		[TestMethod]
		public void NtfsReader_NullDrive()
		{
			Action action = delegate
			{
				var ntfsReader = new NtfsReader(null, RetrieveMode.Minimal);
			};

			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void NtfsReader_EnumerateNodes()
		{
			DriveInfo driveInfo = new DriveInfo("C:\\");


			var ntfsReader = new NtfsReader(driveInfo, RetrieveMode.Minimal);

			var initialMemory = System.GC.GetTotalMemory(true);

			foreach (var node in ntfsReader.EnumerateNodes(driveInfo.Name))
            {
				Assert.IsNotNull(node);
            }

			var finalMemory = System.GC.GetTotalMemory(true);

			var consumption = finalMemory - initialMemory;

			Trace.WriteLine(
				string.Format(
					"Memory Used: {0}",
					consumption / 1e+6
				)
			);
		}

		[TestMethod]
		public void NtfsReader_GetNodes()
		{
			DriveInfo driveInfo = new DriveInfo("C:\\");


			var ntfsReader = new NtfsReader(driveInfo, RetrieveMode.Minimal);

			var initialMemory = System.GC.GetTotalMemory(true);

			foreach (var node in ntfsReader.GetNodes(driveInfo.Name))
			{
				Assert.IsNotNull(node);
			}

			var finalMemory = System.GC.GetTotalMemory(true);

			var consumption = finalMemory - initialMemory;

			Trace.WriteLine(
				string.Format(
					"Memory Used: {0}",
					consumption / 1e+6
				)
			);
		}

		[TestMethod]
		public void NtfsReader_EnumerateAllNodes()
		{
			DriveInfo driveInfo = new DriveInfo("C:\\");


			var ntfsReader = new NtfsReader(driveInfo, RetrieveMode.Minimal);

			var initialMemory = System.GC.GetTotalMemory(true);

			foreach (var node in ntfsReader.EnumerateAllNodes())
			{
				Assert.IsNotNull(node);
			}

			var finalMemory = System.GC.GetTotalMemory(true);

			var consumption = finalMemory - initialMemory;

			Trace.WriteLine(
				string.Format(
					"Memory Used: {0}",
					consumption / 1e+6
				)
			);
		}

		[TestMethod]
		public void NtfsReader_GetAllNodes()
		{
			DriveInfo driveInfo = new DriveInfo("C:\\");


			var ntfsReader = new NtfsReader(driveInfo, RetrieveMode.Minimal);

			var initialMemory = System.GC.GetTotalMemory(true);

			foreach (var node in ntfsReader.GetAllNodes())
			{
				Assert.IsNotNull(node);
			}

			var finalMemory = System.GC.GetTotalMemory(true);

			var consumption = finalMemory - initialMemory;

			Trace.WriteLine(
				string.Format(
					"Memory Used: {0}",
					consumption / 1e+6
				)
			);
		}

		[TestMethod]
		public void DiagnosticEventArgs_ShouldStoreProperties()
		{
			// Test that DiagnosticEventArgs stores level and message correctly
			var args = new NtfsReader.DiagnosticEventArgs("Information", "Test message");
			
			Assert.AreEqual("Information", args.Level);
			Assert.AreEqual("Test message", args.Message);
		}

		[TestMethod]
		public void DiagnosticEventArgs_ShouldHandleAllLevels()
		{
			// Test all documented severity levels
			var levels = new[] { "Debug", "Information", "Warning", "Error" };
			
			foreach (var level in levels)
			{
				var args = new NtfsReader.DiagnosticEventArgs(level, $"Test {level} message");
				Assert.AreEqual(level, args.Level);
				Assert.AreEqual($"Test {level} message", args.Message);
			}
		}

		[TestMethod]
		public void DiagnosticMessage_ShouldBeSubscribable()
		{
			// Verify that the event can be subscribed to and unsubscribed from
			var receivedMessages = new List<NtfsReader.DiagnosticEventArgs>();
			
			EventHandler<NtfsReader.DiagnosticEventArgs> handler = (sender, e) =>
			{
				receivedMessages.Add(e);
			};

			// Subscribe
			NtfsReader.DiagnosticMessage += handler;
			
			// Unsubscribe (should not throw)
			NtfsReader.DiagnosticMessage -= handler;
			
			// Should not throw when no handlers are subscribed
			Assert.AreEqual(0, receivedMessages.Count);
		}
	}
}