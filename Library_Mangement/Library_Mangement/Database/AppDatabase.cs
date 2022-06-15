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


        #endregion

        #region Constructor
        public AppDatabase(string dbPath)
        {
            try
            {
                _logRepository = new LogRepository(dbPath);
                _userRepository = new UserRepository(dbPath);
                _booksRepository = new BooksRepository(dbPath);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }

   
}
