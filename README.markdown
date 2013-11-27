Lumberjack
==========

At its most fundamental level, Lumberjack is a suite of tools for processing log files in various ways. Core functionality to be implemented includes:

 1. Parse logs of any format (all entry patterns are user configurable)
 2. Merge log entries in chronological order
 3. Filter log entries or entry fields at level at a very granular level
 4. Custom log entry fields
 5. Ability to retrieve logs from various sources (local, FTP, SFTP, ...)
 6. Ability to save state and include the parsed data structures for the logs in a given session.


----------


> **DEPENDENCIES:**
Lumberjack is dependent on the external assemblies Renci SSH.NET and Google's Protocol Buffers. These dependencies can be easily resolved withthe package manager NuGet using one of the two options below.

Using the Package Manager Console:

 1. Navigate to: Tools -> Library Package Manager -> Package Manager Console
 2. From the console, enter the following commands:
     - `Install-Package protobuf-net`
     - `Install-Package SSH.NET`

  
Enable NuGet to automatically download missing packages:

 1. Navigate to: Options -> Package Manager -> General
 2. Check '*Allow NuGet to download missing packages during build*'
 3. Click *Ok*