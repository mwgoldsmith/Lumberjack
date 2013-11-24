Lumberjack
==========

Lumberjack is a utility providing a suite of tools for processing log files. Core functionality includes:
* Parse logs of any format (all entry patterns are user configurable)
* Merge log entries in chronological order
* Filter log entries or entry fields at level at a very granular level
* Custom log entry fields
* Ability to retreive logs from various sources (local, FTP, SFTP, ...)


**NOTE:**
References the assembly 'protobuf-net'. If not already present, NuGet must be set to automatically download missing packages. Using VS2010, this can be achived by the following:
* Navigate to Options -> Package Manager -> General
* Check 'Allow NuGet to download missing packages during build'
* Click Ok
