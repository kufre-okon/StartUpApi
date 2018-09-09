IF EXISTS (SELECT 1 FROM sys.objects WHERE type='P' AND name='P_AUDIT_CREATE') BEGIN DROP PROCEDURE dbo.P_AUDIT_CREATE END
GO
CREATE PROCEDURE dbo.P_AUDIT_CREATE 
	@userId NVARCHAR(128),@operation NVARCHAR(50),@OperationDescription NVARCHAR(200),@operationDate DATETIME,@error NVARCHAR(600) 
    AS 
	BEGIN 
		INSERT INTO AuditTrail(Error,Operation,OperationDate,OperationDescription,UserId) VALUES (@error,@operation,@operationDate,@OperationDescription,@userId) 
	END
GO
IF EXISTS (SELECT 1 FROM sys.objects WHERE type='P' AND name='P_AUDIT_CLEAR') BEGIN DROP PROCEDURE dbo.P_AUDIT_CLEAR END
GO
CREATE PROCEDURE dbo.P_AUDIT_CLEAR 
	AS
    BEGIN
		TRUNCATE TABLE AuditTrail 
    END
GO
IF EXISTS (SELECT 1 FROM sys.objects WHERE type='P' AND name='P_CHECKVERSION') BEGIN DROP PROCEDURE dbo.P_CHECKVERSION END
GO
CREATE PROCEDURE dbo.P_CHECKVERSION
	@patchVersion INT
AS
BEGIN
DECLARE	
	@errorMessage NVARCHAR(128),
	@requiredVersion INT;
	
	SELECT TOP 1 @requiredVersion = VersionNumber + 1 FROM [DbVersion]
	IF @patchVersion <> @requiredVersion
		BEGIN
			SET @errorMessage = 'This patch is not applicable for this database. You need to run patch number ' + 
									COALESCE(RIGHT('0000000'+ CONVERT(NVARCHAR,@requiredVersion),8), '')
			RAISERROR(@errorMessage, 20, 1) WITH LOG
			RETURN;
		END;
	ELSE
		BEGIN
			UPDATE [DbVersion] SET VersionNumber = @patchVersion;				
		END;
END

