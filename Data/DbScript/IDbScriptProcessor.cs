using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Data.DbScript
{
    public interface IDbScriptProcessor
    {
        void BeginTransaction();
        void CommitTransaction();

        void RollbackTransaction();

        void AddParameter(string name, object value);
        void RemoveParameter(string name);

        ScriptResult<T> ExecuteSingle<T>(string strSqlCommand, CommandType commandType = CommandType.Text);

        ScriptResult<List<T>> Execute<T>(string strSqlCommand, CommandType commandType = CommandType.Text);
        ScriptResult<T> ExecuteScalar<T>(string strSqlCommand, CommandType commandType = CommandType.Text);
        ScriptResult ExecuteNonQuery(string strSqlCommand, CommandType commandType = CommandType.Text);

    }
}
