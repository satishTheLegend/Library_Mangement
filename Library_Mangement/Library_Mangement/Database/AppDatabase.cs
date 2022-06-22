using Library_Mangement.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database
{
    public class AppDatabase
    {
        #region Properties
        readonly LogRepository _logRepository;
        readonly UserRepository _userRepository;
        readonly BooksRepository _booksRepository;
        readonly VerisonMasterRepository _verisonMasterRepository;
        public LogRepository Logs
        {
            get { return _logRepository; }
        }

        public UserRepository User
        {
            get { return _userRepository; }
        }

        public BooksRepository Book
        {
            get { return _booksRepository; }
        }
        public VerisonMasterRepository MasterDataVerison
        {
            get { return _verisonMasterRepository; }
        }


        #endregion

        #region Constructor
        public AppDatabase(string dbPath)
        {
            try
            {
                _logRepository = new LogRepository(dbPath);
                _userRepository = new UserRepository(dbPath);
                _booksRepository = new BooksRepository(dbPath);
                _verisonMasterRepository = new VerisonMasterRepository(dbPath);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }

   
}
