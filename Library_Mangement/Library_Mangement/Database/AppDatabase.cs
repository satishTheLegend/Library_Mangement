﻿using Library_Mangement.Database.Repositories;
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
        readonly SettingsRepository _settingsRepository;
        readonly CodesMasterRepository _codesMaster;
        readonly LibraryDynamicFieldsRepository _libraryDynamicFields;
        readonly BorrowBookRepository _borrowBook;
        readonly AddToCartRepository _addToCart;
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
        public SettingsRepository Settings
        {
            get { return _settingsRepository; }
        }
        public CodesMasterRepository CodesMaster
        {
            get { return _codesMaster; }
        }
        public LibraryDynamicFieldsRepository LibraryDynamicFields
        {
            get { return _libraryDynamicFields; }
        }
        public BorrowBookRepository BorrowBook
        {
            get { return _borrowBook; }
        }
         public AddToCartRepository AddToCart
        {
            get { return _addToCart; }
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
                _settingsRepository = new SettingsRepository(dbPath);
                _codesMaster = new CodesMasterRepository(dbPath);
                _libraryDynamicFields = new LibraryDynamicFieldsRepository(dbPath);
                _borrowBook = new BorrowBookRepository(dbPath);
                _addToCart = new AddToCartRepository(dbPath);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }

   
}
