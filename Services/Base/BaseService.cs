using Data.Models.Enums;
using Data.Repository;
using Data.Repository.Interface;
using General.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Base
{
    public abstract class ServiceBase
    {
        IAuditTrailManager _auditTrailMgr;
        string currentUserId=null;

        protected ServiceBase(IAuditTrailManager auditTrailManager, IHttpContextAccessor contextAccessor)
        {
            _auditTrailMgr = auditTrailManager;
            var user = contextAccessor. HttpContext.User;
            currentUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private string errorText = null;

        /// <summary>
        /// Execute <paramref name="executor"/> 
        /// </summary>
        /// <typeparam name="TResult">result type</typeparam>
        /// <param name="useTransaction">If set to false, the operation is done out of transaction context and cannot be rollback</param>
        /// <param name="operation">The audit trail action</param>
        /// <param name="unitOfWork"></param>
        /// <param name="executor"></param>
        /// <param name="operationDescription"></param>
        /// <returns></returns>
        protected TResult Execute<TResult>(bool useTransaction, AuditTrailOperations operation, IUnitOfWork unitOfWork, Func<TResult> executor, string operationDescription = null)
        {
            TResult tResult;
            try
            {
                try
                {
                    if (useTransaction)
                    {
                        unitOfWork.BeginTransaction();
                    }
                    TResult tResult1 = executor();
                    unitOfWork.SaveChanges();
                    if (useTransaction)
                    {
                        unitOfWork.Commit();
                    }
                    tResult = tResult1;
                }
                catch (Exception ex)
                {
                    if (useTransaction && unitOfWork.GetCurrentTransaction() != null)
                    {
                        unitOfWork.Rollback();
                    }
                    errorText = ex.Message;
                    throw new ApplicationException(ex.Message);
                }
            }
            finally
            {
                try
                {
                    WriteAuditRecord(operation, operationDescription, errorText);
                }
                catch
                {
                }
            }
            return tResult;
        }

        /// <summary>
        /// Execute <paramref name="executor"/> 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="useTransaction">Use transaction to execute the operation. It will automatically rollback if error occurs</param>
        /// <param name="operation">The audit trail action</param>
        /// <param name="unitOfWork"></param>
        /// <param name="executor"></param>
        /// <param name="operationDescription"></param>
        /// <returns></returns>
        protected async Task<TResult> ExecuteAsync<TResult>(bool useTransaction,  AuditTrailOperations operation, IUnitOfWork unitOfWork, Func<Task<TResult>> executor, string operationDescription = null)
        {
            TResult tResult;
            try
            {
                try
                {
                    if (useTransaction)
                    {
                        unitOfWork.BeginTransaction();
                    }
                    TResult tResult1 = await executor();
                    await unitOfWork.SaveChangesAsync();
                    if (useTransaction)
                    {
                        unitOfWork.Commit();
                    }
                    tResult = tResult1;
                }
                catch (Exception ex)
                {
                    if (useTransaction && unitOfWork.GetCurrentTransaction() != null)
                    {
                        unitOfWork.Rollback();
                    }
                    errorText = ex.Message;
                    throw new ApplicationException(ex.Message);
                }
            }
            finally
            {
                try
                {
                    WriteAuditRecord(operation, operationDescription, errorText);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Console.Write(ex.Message);
                    Trace.WriteLine(ex.Message);
                }
            }
            return tResult;
        }

        /// <summary>
        /// Execute <paramref name="executor"/> synchronously.          
        /// </summary>
        /// <param name="useTransaction">Use transaction to execute the operation. It will automatically rollback if error occurs</param>
        /// <param name="operation">The audit trail action</param>
        /// <param name="unitOfWork"></param>
        /// <param name="executor"></param>
        /// <param name="operationDescription"></param>
        /// <returns></returns>
        protected void ExecuteVoid(bool useTransaction,  AuditTrailOperations operation, IUnitOfWork unitOfWork, Func<Task> executor, string operationDescription = null)
        {
            Execute<object>(useTransaction, operation, unitOfWork, () =>
            {
                executor();
                return null;
            }, operationDescription);
        }
      
        private void WriteAuditRecord(AuditTrailOperations action,  string description, string error = null)
        {
            if (!string.IsNullOrWhiteSpace(currentUserId))
                _auditTrailMgr.CreateAuditRecord(action, description ?? "Execute", DateTime.Now, error, currentUserId);
        }
    }
}
