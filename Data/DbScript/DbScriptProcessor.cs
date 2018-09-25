using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Data.DbScript
{
    /// <summary>
    /// Extension that allow executing of custom scripts on the DbContext
    /// </summary>
    public class DbScriptProcessor : BaseProcessor, IDbScriptProcessor
    {
        private DbTransaction transaction;
        private DbConnection _con;


        public DbScriptProcessor(ApplicationContext _context)
        {
            _con = _context.Database.GetDbConnection();
        }

        private DbCommand createCommand()
        {
            var cmd = _con.CreateCommand();
            if (transaction != null)
                cmd.Transaction = transaction;
            return cmd;
        }
        private void checkConnection()
        {
            if (_con.State == ConnectionState.Closed)
                _con.Open();
            else if (_con.State == ConnectionState.Broken)
            {
                _con.Close();
                _con.Open();
            }
        }
        public void BeginTransaction()
        {
            transaction = _con.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (transaction != null)
                transaction.Commit();
            transaction = null;
        }

        public void RollbackTransaction()
        {
            if (transaction != null)
                transaction.Rollback();
            transaction = null;
        }

        #region Paramaters

        private Dictionary<string, object> _parameters;

        public void AddParameter(string name, object value)
        {
            if (_parameters == null)
                _parameters = new Dictionary<string, object>();
            if (_parameters.ContainsKey(name))
                _parameters[name] = value;
            else
                _parameters.Add(name, value);
        }
        public void RemoveParameter(string name)
        {
            if (_parameters != null)
                _parameters.Remove(name);
        }
        private void clearParameters()
        {
            if (_parameters != null)
                _parameters.Clear();
        }
        private void fillParameters(DbCommand dbCommand)
        {
            if (_parameters != null && _parameters.Count > 0)
            {
                foreach (string key in _parameters.Keys)
                {
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    if (dbParameter != null)
                    {
                        dbParameter.ParameterName = string.Format("@{0}", key);
                        object value = _parameters[key];
                        dbParameter.Value = value ?? DBNull.Value;
                        dbCommand.Parameters.Add(dbParameter);
                    }
                }
            }
            clearParameters();
        }

        #endregion

        #region Process      

        /// <summary>
        /// Execute custom sqlcommand and return a single instance of the <typeparamref name="T"/> struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSqlCommand"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public ScriptResult<T> ExecuteSingle<T>(string strSqlCommand, CommandType commandType = CommandType.Text)
        {
            return ExecuteRequest(() =>
            {
                checkConnection();
                var cmd = createCommand();
                cmd.CommandText = strSqlCommand;
                cmd.CommandType = commandType;
                fillParameters(cmd);
                var reader = cmd.ExecuteReader();
                var data = DataReaderMapToList<T>(reader);
                return data.FirstOrDefault();
            });
        }

        /// <summary>
        /// Execute custom sqlcommand and return a collection of the <typeparamref name="T"/> struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSqlCommand"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public ScriptResult<List<T>> Execute<T>(string strSqlCommand, CommandType commandType = CommandType.Text)
        {
            return ExecuteRequest(() =>
            {
                checkConnection();
                var cmd = createCommand();
                cmd.CommandText = strSqlCommand;
                cmd.CommandType = commandType;
                fillParameters(cmd);
                var reader = cmd.ExecuteReader();
                return DataReaderMapToList<T>(reader);
            });
        }

        /// <summary>
        /// Execute custom sqlcommand and return a scalar value e.g. int, string, Guid, DateTime, etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSqlCommand"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public ScriptResult<T> ExecuteScalar<T>(string strSqlCommand, CommandType commandType = CommandType.Text)
        {
            return ExecuteRequest(() =>
            {
                checkConnection();
                var cmd = createCommand();
                cmd.CommandText = strSqlCommand;
                cmd.CommandType = commandType;
                fillParameters(cmd);
                var result = cmd.ExecuteScalar();
                return result == null ? default(T) : (T)result;
            });
        }

        public ScriptResult ExecuteNonQuery(string strSqlCommand, CommandType commandType = CommandType.Text)
        {
            return ExecuteRequest(() =>
            {
                checkConnection();
                var cmd = createCommand();
                cmd.CommandText = strSqlCommand;
                cmd.CommandType = commandType;
                fillParameters(cmd);

                cmd.ExecuteNonQuery();
            });
        }

        private static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        #endregion      
    }

    #region Base

    public abstract class BaseProcessor
    {
        public virtual ScriptResult ExecuteRequest(Action executeMethod)
        {
            ScriptResult scriptResponse = new ScriptResult();
            try
            {
                executeMethod();
                scriptResponse.Status = true;
            }
            catch (Exception ex)
            {
                scriptResponse.ErrorDetails = ex.Message;
            }
            return scriptResponse;
        }

        public virtual ScriptResult<T> ExecuteRequest<T>(Func<T> executeMethod)
        {
            ScriptResult<T> scriptResponse = new ScriptResult<T>();
            try
            {
                scriptResponse.Data = executeMethod();
                scriptResponse.Status = true;
            }
            catch (Exception ex)
            {
                scriptResponse.ErrorDetails = ex.Message;
            }
            return scriptResponse;
        }
    }

    public class ScriptResult<T> : ScriptResult
    {
        public T Data
        {
            get;
            set;
        }
    }

    public class ScriptResult
    {
        public bool Status { get; set; }

        public string ErrorDetails
        {
            get;
            set;
        }
    }

    #endregion
}
