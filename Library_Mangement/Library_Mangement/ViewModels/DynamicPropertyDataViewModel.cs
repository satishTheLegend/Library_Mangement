using Library_Mangement.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.ViewModels
{
    public class DynamicPropertyDataViewModel : ValidatableBase
    {
        #region Properties
        private int _fieldId;
        public int FieldId
        {
            get => _fieldId;
            set
            {
                _fieldId = value;
                OnPropertyChanged(nameof(FieldId));
            }
        }

        private string _fieldName;
        public string FieldName
        {
            get => _fieldName;
            set
            {
                _fieldName = value;
                OnPropertyChanged(nameof(FieldName));
            }
        }
        private string _placeHolderName;
        public string PlaceHolderName
        {
            get => _placeHolderName;
            set
            {
                _placeHolderName = value;
                OnPropertyChanged(nameof(PlaceHolderName));
            }
        }
        private List<string> _optValues;
        public List<string> OptValues
        {
            get => _optValues;
            set
            {
                _optValues = value;
                OnPropertyChanged(nameof(OptValues));
            }
        }

        private string _controlType;
        public string ControlType
        {
            get => _controlType;
            set
            {
                _controlType = value;
                OnPropertyChanged(nameof(ControlType));
            }
        }

        private string _pageName;
        public string PageName
        {
            get => _pageName;
            set
            {
                _pageName = value;
                OnPropertyChanged(nameof(PageName));
            }
        }

        private string _listValues;
        public string ListValues
        {
            get => _listValues;
            set
            {
                _listValues = value;
                OnPropertyChanged(nameof(ListValues));
            }
        }

        private bool _required;
        public bool Required
        {
            get => _required;
            set
            {
                _required = value;
                OnPropertyChanged(nameof(Required));
            }
        }

        private string _validation;
        public string Validation
        {
            get => _validation;
            set
            {
                _validation = value;
                OnPropertyChanged(nameof(Validation));
            }
        }

        private string _validationMsg;
        public string ValidationMsg
        {
            get => _validationMsg;
            set
            {
                _validationMsg = value;
                OnPropertyChanged(nameof(ValidationMsg));
            }
        }

        private string _fieldValue;
        public string FieldValue
        {
            get => _fieldValue;
            set
            {
                _fieldValue = value;
                OnPropertyChanged(nameof(FieldValue));
            }
        }

        private int _sequence;
        public int Sequence
        {
            get => _sequence;
            set
            {
                _sequence = value;
                OnPropertyChanged(nameof(Sequence));
            }
        }

        private bool _isValidationFaild;
        public bool IsValidationFaild
        {
            get => _isValidationFaild;
            set
            {
                _isValidationFaild = value;
                OnPropertyChanged(nameof(IsValidationFaild));
            }
        }
        private bool _isFieldValidationFaild;
        public bool IsFieldValidationFailed
        {
            get => _isFieldValidationFaild;
            set
            {
                _isFieldValidationFaild = value;
                OnPropertyChanged(nameof(IsFieldValidationFailed));
            }
        }



        #endregion
    }
}
