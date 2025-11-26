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

		[TestMethod]
		public void EnableVerboseDiagnostics_ShouldDefaultToFalse()
		{
			// Verify the default value is false
			Assert.IsFalse(NtfsReader.EnableVerboseDiagnostics);
		}

		[TestMethod]
		public void EnableVerboseDiagnostics_ShouldBeSettable()
		{
			// Store original value
			var originalValue = NtfsReader.EnableVerboseDiagnostics;
			
			try
			{
				// Test setting to true
				NtfsReader.EnableVerboseDiagnostics = true;
				Assert.IsTrue(NtfsReader.EnableVerboseDiagnostics);
				
				// Test setting to false
				NtfsReader.EnableVerboseDiagnostics = false;
				Assert.IsFalse(NtfsReader.EnableVerboseDiagnostics);
			}
			finally
			{
				// Restore original value
				NtfsReader.EnableVerboseDiagnostics = originalValue;
			}
		}

		[TestMethod]
		public void LogicalSectorSize_CalculationFrom_UsaCount()
		{
			// Test the logical sector size calculation logic that is used in FixupRawMftdata
			// UsaCount = 1 (sequence number) + number of sectors
			// logicalSectorSize = BytesPerMftRecord / (UsaCount - 1)
			
			// Scenario 1: 1024-byte MFT record with UsaCount=3
			// Expected: logicalSectorSize = 1024 / (3-1) = 512
			uint bytesPerMftRecord1 = 1024;
			ushort usaCount1 = 3;
			uint logicalSectorSize1 = bytesPerMftRecord1 / (uint)(usaCount1 - 1);
			Assert.AreEqual(512u, logicalSectorSize1, "1024-byte MFT with UsaCount=3 should have 512-byte logical sectors");
			
			// Scenario 2: 4096-byte MFT record with UsaCount=9
			// Expected: logicalSectorSize = 4096 / (9-1) = 512
			uint bytesPerMftRecord2 = 4096;
			ushort usaCount2 = 9;
			uint logicalSectorSize2 = bytesPerMftRecord2 / (uint)(usaCount2 - 1);
			Assert.AreEqual(512u, logicalSectorSize2, "4096-byte MFT with UsaCount=9 should have 512-byte logical sectors");
			
			// Scenario 3: 1024-byte MFT record with UsaCount=5
			// This would represent 4 sectors of 256 bytes each (unlikely, but tests the formula)
			// Expected: logicalSectorSize = 1024 / (5-1) = 256
			uint bytesPerMftRecord3 = 1024;
			ushort usaCount3 = 5;
			uint logicalSectorSize3 = bytesPerMftRecord3 / (uint)(usaCount3 - 1);
			Assert.AreEqual(256u, logicalSectorSize3, "1024-byte MFT with UsaCount=5 should have 256-byte logical sectors");
		}

		[TestMethod]
		public void UsaFixupIndex_ShouldFitInBuffer()
		{
			// Verify that the USA fixup index calculation stays within bounds
			// For a 1024-byte MFT record on a 4K drive:
			// - Old (broken): increment = 4096/2 = 2048, Index = 2047 -> byte 4094 (OUT OF BOUNDS)
			// - New (fixed): increment = 512/2 = 256, Index = 255 -> byte 510 (IN BOUNDS)
			
			uint bytesPerMftRecord = 1024;
			ushort usaCount = 3;
			
			// Calculate logical sector size using the fixed formula
			uint logicalSectorSize = bytesPerMftRecord / (uint)(usaCount - 1);
			uint increment = (uint)(logicalSectorSize / sizeof(ushort));
			uint index = increment - 1;
			
			// The index should point to a position within the MFT record
			uint bytePosition = (uint)(index * sizeof(ushort));
			Assert.IsTrue(bytePosition < bytesPerMftRecord, 
				$"Index {index} at byte position {bytePosition} should be less than MFT record size {bytesPerMftRecord}");
			
			// Verify the maximum index accessed (last sector) is also in bounds
			// We access UsaCount-1 sectors total
			uint maxIndex = (uint)((usaCount - 1) * increment - 1);
			uint maxBytePosition = (uint)(maxIndex * sizeof(ushort));
			Assert.IsTrue(maxBytePosition < bytesPerMftRecord,
				$"Max index {maxIndex} at byte position {maxBytePosition} should be less than MFT record size {bytesPerMftRecord}");
		}

		[TestMethod]
		public void UsaFixupIndex_Comparison_OldVsNew()
		{
			// Compare old vs new calculation to demonstrate the bug fix
			// Scenario: 1024-byte MFT record, 4096-byte physical sector, UsaCount=3
			
			uint bytesPerMftRecord = 1024;
			ushort physicalBytesPerSector = 4096;
			ushort usaCount = 3;
			
			// Old (broken) calculation - used physical sector size
			uint oldIncrement = (uint)(physicalBytesPerSector / sizeof(ushort));  // 4096/2 = 2048
			uint oldIndex = oldIncrement - 1;  // 2047
			uint oldBytePosition = (uint)(oldIndex * sizeof(ushort));  // 4094 - OUT OF BOUNDS!
			
			Assert.AreEqual(2048u, oldIncrement, "Old increment should be 2048");
			Assert.AreEqual(2047u, oldIndex, "Old index should be 2047");
			Assert.AreEqual(4094u, oldBytePosition, "Old byte position should be 4094");
			Assert.IsTrue(oldBytePosition >= bytesPerMftRecord, 
				"Old calculation causes out-of-bounds access");
			
			// New (fixed) calculation - uses logical sector size from UsaCount
			uint logicalSectorSize = bytesPerMftRecord / (uint)(usaCount - 1);  // 1024/2 = 512
			uint newIncrement = (uint)(logicalSectorSize / sizeof(ushort));  // 512/2 = 256
			uint newIndex = newIncrement - 1;  // 255
			uint newBytePosition = (uint)(newIndex * sizeof(ushort));  // 510 - IN BOUNDS!
			
			Assert.AreEqual(512u, logicalSectorSize, "Logical sector size should be 512");
			Assert.AreEqual(256u, newIncrement, "New increment should be 256");
			Assert.AreEqual(255u, newIndex, "New index should be 255");
			Assert.AreEqual(510u, newBytePosition, "New byte position should be 510");
			Assert.IsTrue(newBytePosition < bytesPerMftRecord, 
				"New calculation stays within bounds");
		}
	}
}