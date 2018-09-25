using Data.DbScript;
using Data.Models;
using Data.Models.Enums;
using System;

namespace Data.Repository
{
    public class AuditTrailManager : IAuditTrailManager
    {
        IDbScriptProcessor _dbProcessor;
        public AuditTrailManager(IDbScriptProcessor dbScriptProcessor)
        {
            _dbProcessor = dbScriptProcessor;
        }

        public void CreateAuditRecord(AuditTrailOperations operation, string operationDescription, DateTime operationDateTime, string error, string userId)
        {
            AuditTrail auditRecord = new AuditTrail()
            {
                Error = error,
                Operation = operation.ToString(),
                OperationDate = operationDateTime,
                OperationDescription = operationDescription,
                UserId = userId
            };

            InsertAuditRecord(auditRecord);
        }

        void InsertAuditRecord(AuditTrail record)
        {
            string commandText = "P_AUDIT_CREATE";
            _dbProcessor.AddParameter("userId", record.UserId);
            _dbProcessor.AddParameter("operation", record.Operation);
            _dbProcessor.AddParameter("OperationDescription", record.OperationDescription);
            _dbProcessor.AddParameter("operationDate", record.OperationDate);
            _dbProcessor.AddParameter("error", record.Error);           
            var result = _dbProcessor.ExecuteNonQuery(commandText, System.Data.CommandType.StoredProcedure);
            if (!result.Status)
                throw new Exception(result.ErrorDetails);
        }
    }

    public interface IAuditTrailManager
    {
        void CreateAuditRecord(AuditTrailOperations operation, string operationDescription, DateTime operationDateTime, string error, string userId);
    }
}
