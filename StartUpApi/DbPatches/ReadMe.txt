*************** this readme file explain how to handle db patches *****************

To create new db patch sql file,
	i.  open an existing patch file, copy the first line to the new file
		i.e. copy "EXEC P_CHECKVERSION 1 GO"
	ii. set the file name of the new file to "patch{serilNumber}", the "serilNumber" is the version increment padded with zeroes to 8 characters e.g. 00000001, 00000002,...,00012345, etc
	iii. update "DBVersion" in appsettings.json in the app root directory, to match the version number assigned in step ii above,
			e.g. if the filename in step ii above is 00000123, then DBVersion in appSetting will be '123'
	iv. update "EXEC CheckVersion 1 GO" in the new file to "EXEC P_CHECKVERSION {version Number} GO" where "version Number" is same as the one updated in AppSetting above
	v. cheers

Note: the version number must be incremented serially, no skipping, else the automatic updater will fail during login, and must match
	  Do not update the version number in the database directly, it should be updated by the automatic updater