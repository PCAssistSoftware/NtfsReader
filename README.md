# NtfsReader
Modifications made in this fork:

- targeting netstandard2.0 instead of netstandard2.1 so can use with .NET 4.8
  
- fixed LastAccessTime and FileChangeTime being swapped - from https://github.com/michaelkc/NtfsReader/pull/3/files
  
- fixed unable to read volume information issue which occurred on large 3TB drive and threw exception when calling ntfsReader.GetNodes - as per PR#1 and PR#3 - this error initially occurred on a 4K drive e.g. one using Advanced Format standard, where each sector is 4096 bytes instead of the traditional 512 bytes. Reference: https://en.wikipedia.org/wiki/Advanced_Format
  
- fixed "no bitmap data" error as per https://github.com/Timothyoverton/NtfsReader and https://github.com/michaelkc/NtfsReader/commit/a66e9992a2a2d71236f551e1176ba16694f8ab5b#diff-a908e1f95ee94ddeaa5e5129e1f21b22960788c8dda68ac6cca43360873bf7e1 - slightly amended with error handling for invalid MFT records
  
- added diagnostic logging to assist with diagnosing problems reading from some drives - instructions below to enable this in your own app using this library


```
Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
    ' ...  existing code ...
    
    ' Subscribe to NtfsReader diagnostic messages
    AddHandler NtfsReader.DiagnosticMessage, AddressOf NtfsReader_DiagnosticMessage

    ' Enable verbose diagnostics (optional - only when troubleshooting - will make logs large!)
    NtfsReader.EnableVerboseDiagnostics = True  ' ✅ Set to False for normal use
    
    ' ...  rest of your code ...
End Sub

Private Sub NtfsReader_DiagnosticMessage(sender As Object, e As DiagnosticEventArgs)
    ' Forward NtfsReader diagnostics to Serilog
    Select Case e.Level.ToLower()
        Case "error"
            Log.Error(e.Message)
        Case "warning"
            Log.Warning(e.Message)
        Case "debug"
            Log.Debug(e.Message)
        Case Else
            Log. Information(e.Message)
    End Select
End Sub
```

***

Original description from https://sourceforge.net/projects/ntfsreader/ :

The NtfsReader library.

Copyright (C) 2008 Danny Couture

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

For the full text of the license see the "License.txt" file.

This library is based on the work of Jeroen Kessels, Author of JkDefrag.
http://www.kessels.com/Jkdefrag/

Special thanks goes to him.

Danny Couture
Software Architect

