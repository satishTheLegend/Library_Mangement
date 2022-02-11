using Library_Mangement.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database
{
    public class LogDatabase
    {
        #region Properties
        readonly LogRepository _logRepository;
        public LogRepository Log
        {
            get { return _logRepository; }
        }
        #endregion

        #region Constructor
        public LogDatabase(string dbPath)
        {
            _logRepository = new LogRepository(dbPath);
        }
        #endregion

    }
}
