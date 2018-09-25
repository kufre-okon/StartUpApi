using General.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Data.DbScript
{
    /// <summary>
    /// Manages Database Version patch scripts
    /// </summary>
    public class DbPatchManager : IDbPatchManager
    {
        readonly IConfiguration _appConfig;
        readonly IDbScriptProcessor _processor;
        bool isDbOperationOn = false;


        public DbPatchManager(IConfiguration appConfig, IDbScriptProcessor processor)
        {
            _appConfig = appConfig;
            _processor = processor;
        }

        public bool GetStatus()
        {
            return isDbOperationOn;
        }

        private string[] splitSql(string str)
        {
            string[] array = (from s in str.Split(new string[] { "GO" }, StringSplitOptions.None)
                              select s.Trim() into r
                              where !string.IsNullOrEmpty(r)
                              select r).ToArray();
            return array;
        }

        private void ApplyPatch(string patchSQL)
        {
            string[] array = splitSql(patchSQL);
            for (int i = 0; i < array.Length; i++)
            {
                var result = _processor.ExecuteNonQuery(array[i]);
                if (!result.Status)
                    throw new ApplicationException(result.ErrorDetails);
            }
        }

        public int GetCurrentDBVesrion()
        {
            int num = 0;

            var result = _processor.ExecuteScalar<int>("SELECT VersionNumber FROM DbVersion");
            if (result.Status)
                num = result.Data;
            else
                throw new ApplicationException(result.ErrorDetails);
            return num;
        }

        private List<ScriptInfo> GetRequiredScript(int currenVersion, int requiredVersion, string path)
        {
            List<ScriptInfo> scriptInfos = new List<ScriptInfo>();
            for (int i = currenVersion + 1; i <= requiredVersion; i++)
            {
                string str = Path.Combine(path, string.Format("patch{0:d8}.sql", i));
                if (!File.Exists(str))
                {
                    throw new ApplicationException("Automatic Database update failed because not all patches exist");
                }
                scriptInfos.Add(new ScriptInfo()
                {
                    Version = i,
                    ScriptText = File.ReadAllText(str, Encoding.UTF8)
                });
            }
            return scriptInfos;
        }

        /// <summary>
        /// Synchronize database to the latest version
        /// </summary>
        public void Sync()
        {
            int num = Convert.ToInt32(_appConfig.GetSection("AppConfiguration:DBVersion").Value);
            if (num == 0)
            {
                throw new ApplicationException("Required database version is not specified or \"0\" found in \"appsettings\"");
            }
            try
            {
                int currentDBVesrion = GetCurrentDBVesrion();
                if (currentDBVesrion >= num)
                { 
                    return; // do nothing if the currrent database version is newer than that of the application
                }
                isDbOperationOn = true;
                string str = Path.Combine(AppContext.BaseDirectory, "DbPatches");
                List<ScriptInfo> requiredScript = GetRequiredScript(currentDBVesrion, num, str);
                foreach (ScriptInfo scriptInfo in requiredScript)
                {
                    try
                    {
                        _processor.BeginTransaction();
                        ApplyPatch(scriptInfo.ScriptText);
                        _processor.CommitTransaction();
                    }
                    catch
                    {
                        _processor.RollbackTransaction();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Concat("Database syncronization failed: ", Environment.NewLine, ex.Message));
            }
            finally
            {
                isDbOperationOn = false;
            }
        }

        private class ScriptInfo
        {
            public string ScriptText
            {
                get;
                set;
            }

            public int Version
            {
                get;
                set;
            }

            public ScriptInfo()
            {
            }
        }
    }

    public interface IDbPatchManager
    {
        bool GetStatus();
        int GetCurrentDBVesrion();
        void Sync();
    }
}
